using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button titleButton;

    private void Start()
    {
        pauseUI.SetActive(false);

        continueButton.onClick.AddListener(() => ResumeGame());
        settingButton.onClick.AddListener(() => OpenSettings());
        titleButton.onClick.AddListener(() => GoTitle());
    }

    public void OpenPauseMenu()
    {
        pauseUI.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    private void ResumeGame()
    {
        pauseUI.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    private void OpenSettings()
    {
        SettingManager.Instance.OpenSettingsUI();
    }

    private void GoTitle()
    {
        pauseUI.SetActive(false);
        GameManager.Instance.ResumeGame();
        SceneController.Instance.LoadTitle();
    }
}
