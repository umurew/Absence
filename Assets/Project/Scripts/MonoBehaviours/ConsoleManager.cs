using UnityEngine;
using UnityEngine.UIElements;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance { get; private set; }

    [SerializeField] private UIDocument consoleDocument;

    private VisualElement consoleContainer;
    private ScrollView consoleScrollView;
    private TextField consoleInputTextField;
    private bool consoleState = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        VisualElement root = consoleDocument.rootVisualElement;
        consoleContainer = root.Q<VisualElement>("ConsoleContainer");
        consoleScrollView = root.Q<ScrollView>("ConsoleScrollView");
        consoleInputTextField = root.Q<TextField>("ConsoleInputTextField");

        consoleDocument.enabled = false;
    }

    private void Update()
    {
        if (InputManager.Instance.uiActions.Console.WasPressedThisFrame())
        {
            consoleState = !consoleState;
            consoleDocument.enabled = consoleState;

            if (!consoleState)
                InputManager.Instance.playerActions.Enable();
            else
                InputManager.Instance.playerActions.Disable();
        }

        if (!consoleState)
            return;

        if (InputManager.Instance.uiActions.Enter.WasPressedThisFrame())
        {
            Debug.Log(consoleInputTextField.value);

            Label textLabel = new();
            textLabel.text = consoleInputTextField.value;
            textLabel.AddToClassList("console-text");

            consoleScrollView.Add(textLabel);
            consoleInputTextField.value = string.Empty;
        }
    }
}