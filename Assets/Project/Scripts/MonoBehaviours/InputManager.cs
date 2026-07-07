using Unity.Cinemachine;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private InputActions InputActions { get; set; }
    public InputActions.PlayerActions PlayerActions { get; private set; }
    public InputActions.UiActions UiActions { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InputActions = new();
        PlayerActions = InputActions.Player;
        UiActions = InputActions.UI;
    }

    private void OnDestroy() => InputActions.Dispose();

    private void OnEnable()
    {
        PlayerActions.Enable();
        UiActions.Enable();
    }

    private void OnDisable()
    {
        PlayerActions.Disable();
        UiActions.Disable();
    }

    public void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    public void SetCinemachineInputProviderState(CinemachineCamera CinemachineCamera, bool isEnabled)
    {
        CinemachineInputAxisController inputProvider = CinemachineCamera.GetComponent<CinemachineInputAxisController>();
        inputProvider.enabled = isEnabled;
    }
}