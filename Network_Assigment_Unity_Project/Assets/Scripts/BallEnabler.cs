using UnityEngine;

public class BallEnabler : MonoBehaviour
{
    public BattleManager BattleManager;
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Material baseMat;
    [SerializeField] Material otherMat;


    private void OnTriggerStay(Collider other)
    {
        renderer.material = otherMat;
        BattleManager.EnablerActive();
    }

    private void OnTriggerExit(Collider other)
    {
        renderer.material = baseMat;
    }
}
