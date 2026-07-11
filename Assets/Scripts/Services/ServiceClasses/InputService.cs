using System;
using UnityEngine;

public class InputService : MonoBehaviour, IInputService, IDisposable
{
    private InputActions _inputActions;

    public InputActions.PlayerActions PlayerActions => _inputActions.Player;
    public InputActions.UIActions UIActions => _inputActions.UI;

    public string NewLine => "\n";
    public string Break => "\n\n";
    public string Tab => "\t";

    public void Initialize()
    {
        _inputActions = new();

        PlayerActions.Enable();
        UIActions.Enable();
    }

    public void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked
            ? CursorLockMode.Locked
            : CursorLockMode.None;

        Cursor.visible = !isLocked;
    }

    public void EnableControls()
    {

    }

    public void DisableControls()
    {

    }

    public void Dispose() => _inputActions?.Dispose();

    private void OnDestroy() => Dispose();
}