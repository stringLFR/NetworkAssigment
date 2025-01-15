using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClient : MonoBehaviour 
{
    public ulong ClientId;
    public PlayerActions Actions;
    public PlayerInput Input;
    public PlayerSyncManager SyncManager;
    public clientBlackboard Statboard;
    bool _isLocalClient = false;
    public bool IsLocalClient { get { return _isLocalClient; } set { _isLocalClient = value; } }
    

    private void Awake() => Actions.PlayerClient = this;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate(){
        if (_isLocalClient == false) return;
        SyncManager.SyncBlackboardsPositionRpc(ClientId, Statboard.position.x, Statboard.position.y, Statboard.position.z);
        SyncManager.PositionSetRpc(ClientId);
        SyncManager.SyncBlackboardsVelocityRpc(ClientId, Statboard.velocity.x, Statboard.velocity.y, Statboard.velocity.z);
        SyncManager.VelocitySetRpc(ClientId);
        SyncManager.SyncBlackboardsScaleRpc(ClientId, Statboard.scale.x, Statboard.scale.y, Statboard.scale.z);
        SyncManager.ScaleSetRpc(ClientId);
        SyncManager.SyncBlackboardsSlimeAppendageScaleRpc(ClientId, Statboard.damageScale.x, Statboard.damageScale.y,Statboard.damageScale.x);
        SyncManager.SlimeAppendageScaleSetRpc(ClientId);
        SyncManager.SyncBlackboardsSlimeAppendageRotationRpc(ClientId, Statboard.damageRotation.z);
        SyncManager.SlimeAppendageScaleSetRpc(ClientId);
        SyncManager.SyncBlackboardsSlimeAppendagePositionRpc(ClientId, Statboard.damagePos.x, Statboard.damagePos.y, Statboard.damagePos.x);
        SyncManager.SlimeAppendageScaleSetRpc(ClientId);
    }
}
