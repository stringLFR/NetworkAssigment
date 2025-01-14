using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSyncManager : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] NetworkManager _networkManager;
    Dictionary<ulong, PlayerClient> _clients;
    PlayerClient _localClient = null;
    ulong _currentClientsAmount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _clients = new Dictionary<ulong, PlayerClient>();
        _networkManager.OnClientConnectedCallback += NewClientJoined;
    }

    void NewClientJoined(ulong id){
        if (IsHost == false) return;
        AddClientsRpc(id);
        _currentClientsAmount++;
        for(ulong i = 0; i < _currentClientsAmount; i++)
        {
            AddClientsRpc(i);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void AddClientsRpc(ulong id){
        if (_clients.ContainsKey(id)) return;
        GameObject clientOb = Instantiate(_playerPrefab);
        PlayerClient Client = clientOb.GetComponent<PlayerClient>();
        Client.clientId = id;
        _clients.Add(Client.clientId, Client);
        if (_localClient == null) _localClient = Client;
        if (Client.clientId != _localClient.clientId)
        {
            PlayerActions actions = clientOb.GetComponent<PlayerActions>();
            actions.enabled = false;
        }
    }

    public override void OnDestroy() => _networkManager.OnClientConnectedCallback -= NewClientJoined;



    // Update is called once per frame
    void Update()
    {
        
    }
}
