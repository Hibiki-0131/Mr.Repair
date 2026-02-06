using UnityEngine;
using TMPro;
using System.Collections;

public class UIMessagePopup : MonoBehaviour
{
    public static UIMessagePopup Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        Instance = this;

        // é©ï™(this.gameObject)Ç≈ÇÕÇ»Ç≠ÅAíÜêg(root)ÇîÒï\é¶Ç…Ç∑ÇÈ
        if (root != null)
        {
            root.SetActive(false);
        }
    }

    public void Show(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string message)
    {
        root.SetActive(true);
        text.text = message;

        yield return new WaitForSeconds(2f);

        root.SetActive(false);
    }
}
