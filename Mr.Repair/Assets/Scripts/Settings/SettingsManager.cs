using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenu;
    public GameObject settingsPanel;
    public GameObject audioPanel;
    public GameObject graphicsPanel;

    [Header("Audio Sliders")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider envSlider;

    void Start()
    {
        // 初期値をスライダーに反映
        masterSlider.value = SoundManager.Instance.masterVolume;
        bgmSlider.value = SoundManager.Instance.bgmVolume;
        envSlider.value = SoundManager.Instance.environmentVolume;

        // スライダーが動いた時のイベント登録
        masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBgmVolume);
        envSlider.onValueChanged.AddListener(SoundManager.Instance.SetEnvVolume);

        ShowMainMenu();
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsPanel.SetActive(true);
        ShowAudioSettings(); // デフォルトでAudioを表示
    }

    public void ShowAudioSettings()
    {
        audioPanel.SetActive(true);
        graphicsPanel.SetActive(false);
    }

    public void ShowGraphicsSettings()
    {
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        settingsPanel.SetActive(false);
    }
}