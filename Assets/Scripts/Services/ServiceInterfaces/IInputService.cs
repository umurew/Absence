public interface IInputService
{
    InputActions.PlayerActions PlayerActions { get; }
    InputActions.UIActions UIActions { get; }
    string NewLine { get; }
    string Break { get; }
    string Tab { get; }
    void SetCursorState(bool isLocked);
    void EnableControls();
    void DisableControls();
}