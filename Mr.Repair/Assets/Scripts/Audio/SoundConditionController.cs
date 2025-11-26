using UnityEngine;

public class SoundConditionController : MonoBehaviour
{
    [SerializeField] private SoundTrigger soundTrigger; // Inspector設定 optional（nullなら自動探索）

    private PlayerMovement player;
    private bool wasWalking = false;
    private bool wasPartsMode = false;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        // SoundTriggerが未設定なら、DontDestroyOnLoadされたSoundManagerから探す
        if (soundTrigger == null)
        {
            soundTrigger = FindObjectOfType<SoundTrigger>();
            if (soundTrigger == null)
            {
                Debug.LogWarning("SoundTriggerがシーン内に見つかりません。SoundManagerがロードされているか確認してください。");
            }
        }
    }

    private void Update()
    {
        if (player == null || soundTrigger == null) return;

        // 歩行状態の変化を検知して再生
        bool isWalking = player.IsMoving && !player.IsPartsMode;
        if (isWalking && !wasWalking)
        {
            soundTrigger.PlayByKey("walk", transform.position);
        }
        wasWalking = isWalking;

        // 部品化（PartsTransform）状態の変化を検知して再生
        bool isParts = player.IsPartsMode;
        if (isParts && !wasPartsMode)
        {
            soundTrigger.PlayByKey("parts_transform", transform.position);
        }
        wasPartsMode = isParts;
    }
}
