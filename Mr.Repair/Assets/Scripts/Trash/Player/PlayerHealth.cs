using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHP = 100f;
    private float currentHP;

    private PlayerHPBar hpBar;

    private void Start()
    {
        currentHP = maxHP;
        hpBar = FindObjectOfType<PlayerHPBar>();
        if (hpBar != null)
            hpBar.SetMaxHP(maxHP);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (hpBar != null)
            hpBar.SetHP(currentHP);

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Playerが死亡しました");

        // GameManager経由でゲームオーバー状態を設定
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameOver(true);
        }
        else
        {
            Debug.LogWarning("GameManagerが見つかりません。");
        }
    }
}
