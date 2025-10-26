using UnityEngine;

public class CameraRegister : MonoBehaviour
{
    [SerializeField] private string cameraID = "Camera_A";

    private void Start()
    {
        if (CameraManager.Instance != null)
        {
            Camera cam = GetComponent<Camera>();
            if (cam != null)
            {
                CameraManager.Instance.RegisterCamera(cameraID, cam);
                Debug.Log($"[CameraRegister] Registered camera: {cameraID}");
            }
        }
    }
}
