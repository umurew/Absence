using System;
using UnityEngine;

public class InputService : MonoBehaviour, IInputService, IDisposable
{
    private InputActions _inputActions;
    private bool _initialized = false;

    public InputActions.PlayerActions PlayerActions => _inputActions.Player;
    public InputActions.UIActions UIActions => _inputActions.UI;

    public void Initialize()
    {
        if (_initialized)
        {
            Debug.LogWarning($"{nameof(InputService)}: {nameof(Initialize)} can't be called after initialization.");
            return;
        }

        _inputActions = new();

        PlayerActions.Enable();
        UIActions.Enable();

        _initialized = true;
    }

    public void SetCursorState(bool isLocked)
    {
        InitializedCheck();

        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    public void EnablePlayerControls()
    {
        InitializedCheck();

        SetCursorState(true);
        PlayerActions.Enable();
    }

    public void DisablePlayerControls()
    {
        InitializedCheck();

        SetCursorState(false);
        PlayerActions.Disable();
    }

    public void Dispose()
    {
        _inputActions?.Dispose();
        _inputActions = null;
    }

    private void OnDestroy() => Dispose();

    private void InitializedCheck()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(InputService)} must be initialized before use.");
    }
}