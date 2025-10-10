using UnityEngine;
using UnityEngine.UI;

public class GameOverSelected : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button titleButton;

    private void Start()
    {
        continueButton.onClick.AddListener(() => ContinueGame());
        titleButton.onClick.AddListener(() => GoTitle());
    }

    private void ContinueGame()
    {
        SceneController.Instance.LoadMain();
    }

    private void GoTitle()
    {
        SceneController.Instance.LoadTitle();
    }
}
