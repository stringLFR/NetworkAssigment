using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDamageDealer : MonoBehaviour
{
    [SerializeField] GameObject bodyCenter;
    public GameObject BodyCenter { get { return bodyCenter; } set { bodyCenter = value; } }
    [SerializeField] Collider myPlayer;

    public void StretchTowardsDir(Vector2 dir, PlayerClient client)
    {
        if (dir.magnitude == 0){
            bodyCenter.transform.localScale = Vector3.Lerp(bodyCenter.transform.localScale, Vector3.zero, 0.5f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.5f);
        }
        else{
            bodyCenter.transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.5f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.up, 0.5f);
            Quaternion newRot = Quaternion.Euler(0, 0, dir.x * 10f);
            bodyCenter.transform.localRotation *= newRot;
        }
        client.Statboard.damagePos = transform.localPosition; 
        client.Statboard.damageScale = bodyCenter.transform.localScale;
        client.Statboard.damageRotation = bodyCenter.transform.localRotation.eulerAngles;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == myPlayer) return;
        if (collision.gameObject.CompareTag("Ball") == false) return;
        collision.gameObject.TryGetComponent(out Ball ball);
        ContactPoint contactPoint = collision.GetContact(0);
        int distanceFromCenter = (int)Vector2.Distance(bodyCenter.transform.position, contactPoint.normal);
        ball.BallHit(distanceFromCenter, contactPoint);
    }
}
