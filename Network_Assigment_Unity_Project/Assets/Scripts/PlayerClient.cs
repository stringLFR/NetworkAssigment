using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClient : MonoBehaviour 
{
    public ulong ClientId;
    public GameObject Emote_Wow;
    public GameObject Emote_Happy;
    public GameObject Emote_Angry;
    public PlayerActions Actions;
    public PlayerInput PInput;
    public PlayerSyncManager SyncManager;
    public clientBlackboard Statboard;
    public Vector2 moveDir, stretchDir;
    bool _isLocalClient = false;
    public bool IsLocalClient { get { return _isLocalClient; } set { _isLocalClient = value; } }
    

    private void Awake()
    {
        Actions.PlayerClient = this;
        Emote_Wow.SetActive(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate(){
        if (_isLocalClient == false) return;
        SyncManager.SyncBlackboardsPositionRpc(ClientId, Statboard.position.x, Statboard.position.y, Statboard.position.z);
        SyncManager.SyncBlackboardsVelocityRpc(ClientId, Statboard.velocity.x, Statboard.velocity.y, Statboard.velocity.z);
        SyncManager.SyncBlackboardsScaleRpc(ClientId, Statboard.scale.x, Statboard.scale.y, Statboard.scale.z);
        SyncManager.SyncBlackboardsSlimeAppendagePositionRpc(ClientId, Statboard.damagePos.x, Statboard.damagePos.y, Statboard.damagePos.x);
        SyncManager.SyncBlackboardsSlimeAppendageScaleRpc(ClientId, Statboard.damageScale.x, Statboard.damageScale.y, Statboard.damageScale.x);
        SyncManager.SyncBlackboardsSlimeAppendageRotationRpc(ClientId, Statboard.damageRotation.z);

        if (Input.GetKey(KeyCode.X))
        {
            SyncManager.NewPlayerFace(1, ClientId);
        }
        if (Input.GetKey(KeyCode.C))
        {
            SyncManager.NewPlayerFace(2, ClientId);
        }
        if (Input.GetKey(KeyCode.V))
        {
            SyncManager.NewPlayerFace(3, ClientId);
        }
    }
}
