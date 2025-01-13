using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] PlayerDamageDealer _damageDealer;
    [SerializeField] Rigidbody _rb;
    [SerializeField] float _moveSpeedSideToSide = 1f;
    [SerializeField] float _moveSpeedUpAndDown = 1f;

    Vector2 moveDir, stretchDir = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move() => _rb.AddForce(new Vector3(moveDir.x, 0f, 0f) * Time.deltaTime * _moveSpeedSideToSide + 
        new Vector3(0f, moveDir.y, 0f) * Time.deltaTime * _moveSpeedUpAndDown, ForceMode.Impulse);


    // New Inputsystem value methods
    private void OnMove(InputValue value) => moveDir = value.Get<Vector2>();
    private void OnStretch(InputValue value) => stretchDir = value.Get<Vector2>();
}
