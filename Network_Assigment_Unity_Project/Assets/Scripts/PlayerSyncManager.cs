using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSyncManager : NetworkBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] NetworkManager networkManager;
    List<PlayerClient> clients;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        networkManager.OnClientConnectedCallback += NewClientJoined;
    }

    void NewClientJoined(ulong id){
        if (IsHost == false) return;
        if (clients == null){
            clients = new List<PlayerClient>();
            AddClientsRpc(id);
        }
    }
    [Rpc(SendTo.Everyone)]
    public void AddClientsRpc(ulong id){
        GameObject client = Instantiate(playerPrefab);
        PlayerClient ClientId = client.GetComponent<PlayerClient>();
        ClientId.ClientID = id;
        clients.Add(ClientId);
    }

    private void OnDisable()
    {
        NetworkManager.OnClientConnectedCallback -= NewClientJoined;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
