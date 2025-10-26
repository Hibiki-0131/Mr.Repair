using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private string targetCameraID = "Camera_A";

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CameraManager.Instance != null)
            {
                // åªç›ÇÃÉJÉÅÉâÇ∆à·Ç§èÍçáÇÃÇ›êÿÇËë÷Ç¶ÇÈ
                CameraManager.Instance.SwitchToCamera(targetCameraID);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(transform.position, GetComponent<BoxCollider>().size);
    }
#endif
}
