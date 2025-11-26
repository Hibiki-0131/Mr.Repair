using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private Dictionary<string, Camera> cameraDict = new Dictionary<string, Camera>();
    private Camera mainCamera;
    private string currentCameraID = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;
    }

    public void RegisterCamera(string id, Camera cam)
    {
        if (!cameraDict.ContainsKey(id))
            cameraDict.Add(id, cam);
    }

    public void SwitchToCamera(string id)
    {
        // ìØÇ∂IDÇ»ÇÁêÿÇËë÷Ç¶Ç»Ç¢
        if (id == currentCameraID)
        {
            // Debug.Log($"[CameraManager] Camera '{id}' is already active. Ignored.");
            return;
        }

        if (cameraDict.TryGetValue(id, out Camera targetCam))
        {
            mainCamera.transform.position = targetCam.transform.position;
            mainCamera.transform.rotation = targetCam.transform.rotation;

            currentCameraID = id; // åªç›ÇÃÉJÉÅÉâÇçXêV

            Debug.Log($"[CameraManager] Switched to camera: {id} at {targetCam.transform.position}");
        }
        else
        {
            Debug.LogWarning($"[CameraManager] Camera ID '{id}' not found!");
        }
    }

    public Camera GetActiveCamera()
    {
        if (currentCameraID != null && cameraDict.TryGetValue(currentCameraID, out Camera cam))
            return cam;

        return Camera.main;
    }

}
