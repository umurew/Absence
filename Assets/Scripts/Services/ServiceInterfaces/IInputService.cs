public interface IInputService
{
    InputActions.PlayerActions PlayerActions { get; }
    InputActions.UIActions UIActions { get; }
    void SetCursorState(bool isLocked);
    void EnablePlayerControls();
    void DisablePlayerControls();
}