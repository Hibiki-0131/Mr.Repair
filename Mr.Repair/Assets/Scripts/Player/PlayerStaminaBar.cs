using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider; // Sliderコンポーネント参照

    private void Awake()
    {
        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = 1f; // 0...1の割合で管理
            staminaSlider.value = 1f;    // 初期値はフル
        }
    }

    // スタミナ残量をUIに反映
    public void UpdateStamina(float current, float max)
    {
        if (staminaSlider != null)
            staminaSlider.value = Mathf.Clamp01(current / max);
    }
}
