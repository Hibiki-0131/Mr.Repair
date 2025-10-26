using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    private void Awake()
    {
        if (hpSlider == null)
            hpSlider = GetComponent<Slider>();
    }

    public void SetMaxHP(float maxHP)
    {
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
    }

    public void SetHP(float currentHP)
    {
        hpSlider.value = currentHP;
    }
}
