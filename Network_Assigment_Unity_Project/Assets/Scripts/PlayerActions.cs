using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] PlayerDamageDealer _damageDealer;
    [SerializeField] Rigidbody _rb;
    public Rigidbody rb => _rb;
    [SerializeField] float _moveSpeedSideToSide = 1f;
    [SerializeField] float _moveSpeedUpAndDown = 1f;
    public PlayerClient PlayerClient;
    Vector2 moveDir, stretchDir;
    Vector3 baseScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        baseScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        float xScale = baseScale.x + Mathf.Abs(moveDir.x) - Mathf.Abs(moveDir.y)/2;
        float yScale = baseScale.y + Mathf.Abs(moveDir.y) - Mathf.Abs(moveDir.x)/2;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(xScale, yScale, baseScale.z), 0.5f);
        PlayerClient.Statboard.scale = transform.localScale;
    }

    public void Move(){
        _rb.AddForce(new Vector3(moveDir.x, 0f, 0f) * Time.fixedDeltaTime * _moveSpeedSideToSide +
        new Vector3(0f, moveDir.y, 0f) * Time.fixedDeltaTime * _moveSpeedUpAndDown, ForceMode.Impulse);
        PlayerClient.Statboard.position = transform.position;
        PlayerClient.Statboard.velocity = _rb.linearVelocity;
    }


    // New Inputsystem value methods
    private void OnMove(InputValue value) => moveDir = value.Get<Vector2>();
    private void OnStretch(InputValue value) => moveDir = value.Get<Vector2>();
}
