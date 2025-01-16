using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
public enum ClientCheckUp
{
    POSITION, SCALE, VELOCITY, D_POSITION, D_SCALE, D_ROTATION, HEALTH,
}
public struct clientBlackboard{
    public Vector3 position, scale, velocity, damagePos,damageScale, damageRotation;
    public int health;
}
public struct ballBlackBoard
{
    public Vector3 position, scale, velocity;
    public int damage;
}
public class Server : NetworkBehaviour
{
    [SerializeField] PlayerSyncManager _playerManager;
    [SerializeField] Material _hostMat;
    [SerializeField] Material _ballDamageable;
    Material _ballBaseMat;
    public BattleManager BattleManager;
    Dictionary<ulong, PlayerClient> _lastSavedClients;
    Ball _serverBall;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _lastSavedClients = new Dictionary<ulong, PlayerClient>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsHost == false) return;
        if (_serverBall == null) return;
        SyncBall();
    }

    public void AddToServerDictionary(ulong id) {
        if (_lastSavedClients.ContainsKey(id)) return;
        _lastSavedClients.Add(_playerManager.Clients[id].ClientId, _playerManager.Clients[id]);
    }


    public void ServerLookUpClient(ulong id, ClientCheckUp checkUp)
    {
        switch (checkUp)
        {
            case ClientCheckUp.POSITION: break;
            case ClientCheckUp.SCALE: break;

        }


        _lastSavedClients[id].Statboard = _playerManager.Clients[id].Statboard;
    }

    public void HostJoined(){
        if (_serverBall != null) return;
        _serverBall = BattleManager.Ball;
        _ballBaseMat = _serverBall.Renderer.material;
    }

    public void SetHostColor(PlayerClient host){
        MeshRenderer mesh = host.GetComponent<MeshRenderer>();
        mesh.material = _hostMat;
        CapsuleCollider arm = mesh.GetComponentInChildren<CapsuleCollider>();
        arm.GetComponent<MeshRenderer>().material = _hostMat;
    }

    public void SyncBall() => SyncBallRpc(_serverBall.transform.position.x, _serverBall.transform.position.y, _serverBall.transform.position.z,
            _serverBall.Rb.linearVelocity.x, _serverBall.Rb.linearVelocity.y, _serverBall.Rb.linearVelocity.z);

    [Rpc(SendTo.Everyone)]
    public void SyncBallRpc(float px, float py, float pz, float vx, float vy, float vz) {
        BattleManager.Ball.ballStatBoard.position = new Vector3(px, py, pz);
        BattleManager.Ball.ballStatBoard.velocity = new Vector3(vx, vy, vz);
        BattleManager.Ball.transform.position = BattleManager.Ball.ballStatBoard.position;
        BattleManager.Ball.Rb.linearVelocity = BattleManager.Ball.ballStatBoard.velocity;
    }

    [Rpc(SendTo.Server)]
    public void KickBallRpc(int damage, float x, float y, float z){
        Vector3 dir = new Vector3(x, y, z);
        _serverBall.Renderer.material = _ballDamageable;
        _serverBall.KickBall(damage, dir);
    }
}
