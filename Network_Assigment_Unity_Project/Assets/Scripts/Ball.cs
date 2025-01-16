using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    public Rigidbody Rb => _rb;
    public ballBlackBoard ballStatBoard;
    public BattleManager battleManager;
    public MeshRenderer Renderer;
    public void BallHit(int damageFromHit, ContactPoint pointOfHit){
        Vector3 dir = transform.position - pointOfHit.point;
        battleManager.HanldeBallHit(damageFromHit, dir.normalized);
    }

    public void KickBall(int finalDamage, Vector3 dir){
        _rb.AddForce(new Vector3(dir.x, 0f, 0f) * Time.fixedDeltaTime * 100f +
        new Vector3(0f, dir.y, 0f) * Time.fixedDeltaTime * 100f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
