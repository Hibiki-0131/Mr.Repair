using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverInput : MonoBehaviour
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
        if (EventSystem.current == null) return;

        Vector2 move = ctx.ReadValue<Vector2>();
        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current == null) return;

        Selectable next = null;

        if (move.y > 0.5f)
            next = current.GetComponent<Selectable>()?.FindSelectableOnUp();
        else if (move.y < -0.5f)
            next = current.GetComponent<Selectable>()?.FindSelectableOnDown();

        if (next != null)
            next.Select();
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        GameObject selected = EventSystem.current?.currentSelectedGameObject;
        if (selected != null)
        {
            Button btn = selected.GetComponent<Button>();
            btn?.onClick.Invoke();
        }
    }
}
