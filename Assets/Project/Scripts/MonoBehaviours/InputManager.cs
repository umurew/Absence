using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public InputActions inputActions { get; private set; }
    public InputActions.PlayerActions playerActions { get; private set; }
    public InputActions.UIActions uiActions { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new();
        playerActions = inputActions.Player;
        uiActions = inputActions.UI;
    }

    private void OnDestroy() => inputActions.Dispose();

    private void OnEnable()
    {
        playerActions.Enable();
        uiActions.Enable();
    }

    private void OnDisable()
    {
        playerActions.Disable();
        uiActions.Disable();
    }

    public void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}