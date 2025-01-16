using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] Ball _ball;
    public Ball Ball => _ball;
    Server _server;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        _server = FindFirstObjectByType<Server>();
        _server.BattleManager = this;
        Ball.battleManager = this;
    }
    public void HanldeBallHit(int damage, Vector3 newDIr) => _server.KickBallRpc(damage, newDIr.x, newDIr.y, newDIr.z);
}
