using UnityEngine;

public class EnemyHealth : HealthBase
{
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} ‚ª€–S‚µ‚Ü‚µ‚½B");
        // —áFDestroy(gameObject);
    }
}
