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
    [Serializable]
    public struct clientBlackboard{
        public Vector3 position, scale;
        public Quaternion rotation;
    }

    private void Awake() => Actions.PlayerClient = this;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SyncManager.SyncBlackboardsPositionRpc(ClientId, Statboard.position.x, Statboard.position.y, Statboard.position.z);
        SyncManager.PositionSetRpc(ClientId);
        SyncManager.SyncBlackboardsScaleRpc(ClientId, Statboard.scale.x, Statboard.scale.y, Statboard.scale.z);
        SyncManager.ScaleSetRpc(ClientId);
    }
}
