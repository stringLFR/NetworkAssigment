using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
public enum ClientCheckUp
{
    POSITION, SCALE, DAMAGE,VELOCITY,
}
public struct clientBlackboard{
    public Vector3 position, scale, velocity;
    public Quaternion rotation;
    public int health;
}
public class Server : NetworkBehaviour
{
    [SerializeField] PlayerSyncManager _playerManager;
    BattleManager _battleManager;
    Dictionary<ulong, PlayerClient> _lastSavedClients;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _lastSavedClients = new Dictionary<ulong, PlayerClient>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddToServerDictionary(ulong id){
        if (_lastSavedClients.ContainsKey(id)) return;
        _lastSavedClients.Add(_playerManager.Clients[id].ClientId, _playerManager.Clients[id]);
    }

    public void ServerLookUpClient(ulong id, ClientCheckUp checkUp)
    {
        switch (checkUp)
        {
            case ClientCheckUp.POSITION: break;
            case ClientCheckUp.SCALE: break;
            case ClientCheckUp.DAMAGE: break;
        }


        _lastSavedClients[id].Statboard = _playerManager.Clients[id].Statboard;
    }
}
