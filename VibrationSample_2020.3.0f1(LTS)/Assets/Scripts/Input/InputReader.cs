using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject, GameInputs.IPlayGameActions
{
    public event UnityAction shotEvent = delegate { };
    public event UnityAction<float> moveEvent = delegate { };

    #region UnityEvents
    private GameInputs gameInputs;
    private void OnEnable()
    {
        if(gameInputs == null)
        {
            gameInputs = new GameInputs();
            gameInputs.PlayGame.SetCallbacks(this);
        }
        EnableGamePlayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }
    #endregion

    #region InputEvents
    public void OnMove(InputAction.CallbackContext context)
    {
        moveEvent.Invoke(context.ReadValue<float>());
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            shotEvent.Invoke();
    }
    #endregion

    public void EnableGamePlayInput()
    {
        gameInputs.PlayGame.Enable();
    }

    public void DisableAllInput()
    {
        gameInputs.PlayGame.Disable();
    }
}