using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] Ball _ball;
    [SerializeField] BallEnabler _ballEnabler; //This will be used by players to enable the ball!
    public Ball Ball => _ball;
    Server _server;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        _server = FindFirstObjectByType<Server>();
        _server.BattleManager = this;
        _ballEnabler.BattleManager = this;
        Ball.battleManager = this;
        Ball.gameObject.SetActive(false);
    }
    public void HanldeBallHit(int damage, Vector3 newDIr) => _server.KickBallRpc(damage, newDIr.x, newDIr.y, newDIr.z);

    public void EnablerActive()
    {
        if (Input.GetKey(KeyCode.E))
        {
            _server.EnableBall();
        }
    }

    public void BallShallBeEnabled()
    {
        _ball.gameObject.SetActive(true);
        _ballEnabler.gameObject.SetActive(false);
    }
}
