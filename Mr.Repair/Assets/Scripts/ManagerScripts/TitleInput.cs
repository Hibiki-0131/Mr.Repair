using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleInput : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.UIControl.Enable();

        inputActions.UIControl.Navigate.performed += OnNavigate;
        inputActions.UIControl.Submit.performed += OnSubmit;
    }

    private void OnDisable()
    {
        inputActions.UIControl.Navigate.performed -= OnNavigate;
        inputActions.UIControl.Submit.performed -= OnSubmit;
        inputActions.UIControl.Disable();
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 move = ctx.ReadValue<Vector2>();

        if (EventSystem.current == null)
            return;

        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current == null)
            return;

        // 上下移動で次のUI要素にフォーカスを送る
        Selectable next = null;

        if (move.y > 0.5f)
            next = current.GetComponent<Selectable>()?.FindSelectableOnUp();
        else if (move.y < -0.5f)
            next = current.GetComponent<Selectable>()?.FindSelectableOnDown();

        if (next != null)
        {
            next.Select();
        }
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        GameObject selected = EventSystem.current?.currentSelectedGameObject;
        if (selected != null)
        {
            var button = selected.GetComponent<Button>();
            button?.onClick.Invoke();
        }
    }
}
