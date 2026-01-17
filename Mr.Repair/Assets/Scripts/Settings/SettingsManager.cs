using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // ボタン選択に必要

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
    public Slider sfxSlider;

    [Header("First Selected Buttons")]
    public GameObject graphicsTabButton; // GRAPHICSボタンをアサイン
    public GameObject audioTabButton;    // AUDIOボタンをアサイン
    public GameObject backToMainButton;  // BACKボタンをアサイン
    public GameObject startButton;

    // SettingsManager.cs の Start内を修正
    void Start()
    {
        if (SoundManager.Instance == null) return;

        // 1. まずリスナーを登録せずに、スライダーの「見た目」だけを現在の設定に合わせる
        masterSlider.value = SoundManager.Instance.masterVolume;
        bgmSlider.value = SoundManager.Instance.bgmVolume;
        sfxSlider.value = SoundManager.Instance.environmentVolume;

        // 2. 「見た目」が整った後に、リスナーを登録する
        // これにより、起動時の「0から初期値への変化」でイベントが飛ぶのを防げます
        masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetEnvVolume);

        ShowMainMenu();
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsPanel.SetActive(true);

        // 中身は空の状態にする
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(false);

        // 【重要】GRAPHICSボタンを選択状態にする
        SetFocus(graphicsTabButton);
    }

    public void ShowAudioSettings()
    {
        audioPanel.SetActive(true);
        graphicsPanel.SetActive(false);

        // オーディオ設定に入った瞬間、Masterスライダーを選択状態にする
        // これにより UISelectionHighlighter が反応して背景パネルが表示されます
        SetFocus(masterSlider.gameObject);
    }

    public void ShowGraphicsSettings()
    {
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(true);

        // グラフィックス設定の中身ができたら、その最初の項目を選択するようにする
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        settingsPanel.SetActive(false);

        // メインメニューに戻ったら「SETTINGS」ボタンなどを選択状態にする
        // SetFocus(settingsButton); 
        SetFocus(startButton);
    }

    // 指定したオブジェクトにフォーカスを当てるヘルパー関数
    private void SetFocus(GameObject target)
    {
        if (target != null)
        {
            EventSystem.current.SetSelectedGameObject(target);
        }
    }
}