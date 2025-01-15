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
    }
    [Rpc(SendTo.Everyone)]
    public void ScaleSetRpc(ulong id){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]) return;
        _clients[id].transform.localScale = _clients[id].Statboard.scale;
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
    }
    [Rpc(SendTo.Everyone)]
    public void PositionSetRpc(ulong id){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]) return;
        _clients[id].transform.position = _clients[id].Statboard.position;
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
    }
    [Rpc(SendTo.Everyone)]
    public void VelocitySetRpc(ulong id){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]) return;
        _clients[id].Actions.rb.linearVelocity = _clients[id].Statboard.velocity;
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
    }
    #endregion
    [Rpc(SendTo.Everyone)]
    public void SlimeAppendageScaleSetRpc(ulong id){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]) return;
        _clients[id].Actions.DamageDealer.BodyCenter.transform.localScale = _clients[id].Statboard.damageScale;
    }
    [Rpc(SendTo.Everyone)]
    public void SlimeAppendageRotationSetRpc(ulong id){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]) return;
        _clients[id].Actions.DamageDealer.BodyCenter.transform.localRotation = _clients[id].Statboard.damageRotation;
    }
    [Rpc(SendTo.Everyone)]
    public void SlimeAppendagePositionSetRpc(ulong id){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]) return;
        _clients[id].Actions.DamageDealer.transform.localPosition = _clients[id].Statboard.damagePos;
    }
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsSlimeAppendageScaleRpc(ulong id, float sx, float sy, float sz){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.VELOCITY);
            return;
        }
        _clients[id].Actions.DamageDealer.BodyCenter.transform.localScale = new Vector3(sx, sy, sz);
    }
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsSlimeAppendageRotationRpc(ulong id, float rot){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.VELOCITY);
            return;
        }
        _clients[id].Actions.DamageDealer.BodyCenter.transform.localRotation = Quaternion.Euler(0, 0, rot);
    }
    [Rpc(SendTo.Everyone)]
    public void SyncBlackboardsSlimeAppendagePositionRpc(ulong id, float px, float py, float pz){
        if (!_clients.ContainsKey(id)) return;
        if (_localClient == _clients[id]){
            ServerLookUpClientRpc(id, ClientCheckUp.VELOCITY);
            return;
        }
        _clients[id].Actions.DamageDealer.transform.localPosition = new Vector3(px, py, pz);
    }
    #endregion
    public override void OnDestroy() => _networkManager.OnClientConnectedCallback -= NewClientJoined;

}
