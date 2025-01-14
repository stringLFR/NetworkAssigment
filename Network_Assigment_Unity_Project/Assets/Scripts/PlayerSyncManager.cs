using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using static PlayerClient;

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
        for(ulong i = 0; i < _currentClientsAmount; i++) AddClientsRpc(i);
    }

    [Rpc(SendTo.Everyone)]
    public void AddClientsRpc(ulong id){
        if (_clients.ContainsKey(id)) return;
        GameObject clientOb = Instantiate(_playerPrefab);
        PlayerClient Client = clientOb.GetComponent<PlayerClient>();
        Client.ClientId = id;
        Client.SyncManager = this;
        _clients.Add(Client.ClientId, Client);
        if (_localClient == null) _localClient = Client;
        if (Client.ClientId != _localClient.ClientId){
            Client.Input.enabled = false;
            Client.Actions.enabled = false;
        }
    }
    [Rpc(SendTo.Everyone)]
    public void PositionSetRpc(ulong id){
        if (_localClient == _clients[id]) return;
        _clients[id].transform.position = _clients[id].Statboard.position;
        //_clients[id].transform.position = Vector3.Lerp(_clients[id].transform.position, _clients[id].Statboard.position, 0.5f);
    }

    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsScaleRpc(ulong id, float x, float y, float z){
        if (_localClient == _clients[id]) return;
        _clients[id].Statboard.scale.x = x;
        _clients[id].Statboard.scale.y = y;
        _clients[id].Statboard.scale.z = z;
    }

    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsPositionRpc(ulong id, float x, float y, float z){
        if (_localClient == _clients[id]) return;
        _clients[id].Statboard.position.x = x;
        _clients[id].Statboard.position.y = y;
        _clients[id].Statboard.position.z = z;
    }

    public override void OnDestroy() => _networkManager.OnClientConnectedCallback -= NewClientJoined;

}
