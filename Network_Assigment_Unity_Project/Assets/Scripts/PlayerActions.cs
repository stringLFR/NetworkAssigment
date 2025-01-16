using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] PlayerDamageDealer _damageDealer;
    public PlayerDamageDealer DamageDealer => _damageDealer;
    [SerializeField] Rigidbody _rb;
    public Rigidbody rb => _rb;
    [SerializeField] float _moveSpeedSideToSide = 1f;
    [SerializeField] float _moveSpeedUpAndDown = 1f;
    public PlayerClient PlayerClient;
    Vector3 baseScale = Vector3.one;

    void FixedUpdate()
    {
        Move();
        _damageDealer.StretchTowardsDir(PlayerClient.stretchDir, PlayerClient);
        float xScale = baseScale.x + Mathf.Abs(PlayerClient.moveDir.x) - Mathf.Abs(PlayerClient.moveDir.y) * 0.1f;
        float yScale = baseScale.y + Mathf.Abs(PlayerClient.moveDir.y) - Mathf.Abs(PlayerClient.moveDir.x) * 0.1f;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(xScale, yScale, baseScale.z), 0.5f);
        PlayerClient.Statboard.scale = transform.localScale;
    }

    public void Move(){
        _rb.AddForce(new Vector3(PlayerClient.moveDir.x, 0f, 0f) * Time.fixedDeltaTime * _moveSpeedSideToSide +
        new Vector3(0f, PlayerClient.moveDir.y, 0f) * Time.fixedDeltaTime * _moveSpeedUpAndDown, ForceMode.Impulse);
        PlayerClient.Statboard.position = transform.position;
        PlayerClient.Statboard.velocity = _rb.linearVelocity;
    }


    // New Inputsystem value methods
    private void OnMove(InputValue value) => PlayerClient.moveDir = value.Get<Vector2>();
    private void OnStretch(InputValue value) => PlayerClient.stretchDir = value.Get<Vector2>();
}
