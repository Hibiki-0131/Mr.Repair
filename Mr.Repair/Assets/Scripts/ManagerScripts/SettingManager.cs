using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    [SerializeField] private GameObject settingUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenSettingsUI()
    {
        settingUI.SetActive(true);
    }

    public void CloseSettingsUI()
    {
        settingUI.SetActive(false);
    }
}
