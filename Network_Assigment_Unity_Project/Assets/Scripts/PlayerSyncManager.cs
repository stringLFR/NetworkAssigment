using NUnit.Framework;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using static PlayerClient;

public class PlayerSyncManager : NetworkBehaviour
{
    [SerializeField] Server _server;
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] NetworkManager _networkManager;
    Dictionary<ulong, PlayerClient> _clients;
    public Dictionary<ulong, PlayerClient> Clients => _clients;
    PlayerClient _localClient = null;
    public PlayerClient LocalClient => _localClient;
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
        _server.HostJoined();
        AddClientsRpc(id);
        AddToServerDictionaryRpc(id);
        _currentClientsAmount++;
        for(ulong i = 0; i < _currentClientsAmount; i++) AddClientsRpc(i);
    }

    #region Rpc server
    [Rpc(SendTo.Server)]
    void ServerLookUpClientRpc(ulong id,ClientCheckUp checkUp) => _server.ServerLookUpClient(id, checkUp);
    [Rpc(SendTo.Server)]
    void AddToServerDictionaryRpc(ulong id) => _server.AddToServerDictionary(id);
    #endregion

    #region Rpc everyone
    #region player Base syncs
    [Rpc(SendTo.Everyone)]
    public void AddClientsRpc(ulong id){
        if (_clients.ContainsKey(id)) return;
        GameObject clientOb = Instantiate(_playerPrefab);
        PlayerClient Client = clientOb.GetComponent<PlayerClient>();
        Client.ClientId = id;
        Client.SyncManager = this;
        _clients.Add(Client.ClientId, Client);
        if (_localClient == null){
            _localClient = Client;
            _localClient.IsLocalClient = true;
        }
        if (Client.ClientId != _localClient.ClientId){
            Client.Input.enabled = false;
            Client.Actions.enabled = false;
        }
        if (!_clients.ContainsKey(0)) return;
        if (_clients[id] == _clients[0]) _server.SetHostColor(_clients[id]);
    }
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsScaleRpc(ulong id, float x, float y, float z){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.SCALE);
            return;
        }
        _clients[id].Statboard.scale.x = x;
        _clients[id].Statboard.scale.y = y;
        _clients[id].Statboard.scale.z = z;
        _clients[id].transform.localScale = _clients[id].Statboard.scale;
    }
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsPositionRpc(ulong id, float x, float y, float z){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.POSITION);
            return;
        }
        _clients[id].Statboard.position.x = x;
        _clients[id].Statboard.position.y = y;
        _clients[id].Statboard.position.z = z;
        _clients[id].transform.position = _clients[id].Statboard.position;
    }
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsVelocityRpc(ulong id, float x, float y, float z){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.VELOCITY);
            return;
        }
        _clients[id].Statboard.velocity.x = x;
        _clients[id].Statboard.velocity.y = y;
        _clients[id].Statboard.velocity.z = z;
        _clients[id].Actions.rb.linearVelocity = _clients[id].Statboard.velocity;
    }
    #endregion
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsSlimeAppendageScaleRpc(ulong id, float sx, float sy, float sz){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.VELOCITY);
            return;
        }
        _clients[id].Statboard.damageScale = new Vector3(sx, sy, sz);
        _clients[id].Actions.DamageDealer.BodyCenter.transform.localScale = _clients[id].Statboard.damageScale;
    }
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsSlimeAppendageRotationRpc(ulong id, float rot){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.VELOCITY);
            return;
        }
        _clients[id].Statboard.damageRotation = new Vector3(0, 0, rot);
        _clients[id].Actions.DamageDealer.BodyCenter.transform.localRotation = Quaternion.Euler(_clients[id].Statboard.damageRotation);
    }
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsSlimeAppendagePositionRpc(ulong id, float px, float py, float pz){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.VELOCITY);
            return;
        }
        _clients[id].Statboard.damagePos = new Vector3(px, py, pz);
        _clients[id].Actions.DamageDealer.transform.localPosition = _clients[id].Statboard.damagePos;
    }
    #endregion
    public override void OnDestroy() => _networkManager.OnClientConnectedCallback -= NewClientJoined;

}
