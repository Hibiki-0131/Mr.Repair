using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ItemVisualEffect : MonoBehaviour
{
    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 45f; // 1秒で45度回転

    [Header("発光設定")]
    [SerializeField] private Color glowColor = new Color(0.2f, 1f, 0.2f); // 緑系
    [SerializeField, Range(0f, 2f)] private float emissionIntensity = 0.5f;

    private Material material;
    private Color baseEmissionColor;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material; // インスタンス化されたマテリアルを使用
        baseEmissionColor = glowColor * Mathf.LinearToGammaSpace(emissionIntensity);

        // Emission有効化
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", baseEmissionColor);
    }

    void Update()
    {
        // Y軸に回転
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // （オプション）ゆらぎアニメーションを付けたい場合：
        // float pulse = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f; // 0...1
        // material.SetColor("_EmissionColor", baseEmissionColor * (0.8f + pulse * 0.2f));
    }

    void OnDestroy()
    {
        // 念のためEmissionをリセット（再生時残らないように）
        if (material != null)
        {
            material.SetColor("_EmissionColor", Color.black);
        }
    }
}
