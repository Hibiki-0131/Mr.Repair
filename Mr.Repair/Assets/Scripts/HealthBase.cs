using UnityEngine;

public abstract class HealthBase : MonoBehaviour
{
    [Header("HP設定")]
    [SerializeField] protected int maxHP = 100;
    protected int currentHP;

    protected virtual void Awake()
    {
        currentHP = maxHP;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => maxHP;

    protected abstract void Die(); // 継承先で定義（例：Destroy、自動リスポーンなど）
}
