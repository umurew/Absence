using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance { get; private set; }

    [SerializeField] private UIDocument consoleDocument;
    [SerializeField] private CinemachineCamera firstPersonCamera;

    private readonly Dictionary<string, ICommand> _commands = new();
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

        consoleContainer.style.visibility = Visibility.Hidden;
        consoleInputTextField.RegisterCallback<KeyDownEvent>(ConsoleTextFieldSubmit, TrickleDown.TrickleDown);

        RegisterCommands();
    }

    private void Update()
    {
        // Handle console toggling
        if (InputManager.Instance.UiActions.ToggleConsole.WasPressedThisFrame())
        {
            consoleState = !consoleState;
            consoleContainer.style.visibility = consoleState ? Visibility.Visible : Visibility.Hidden;

            if (!consoleState)
            {
                InputManager.Instance.PlayerActions.Enable();
                InputManager.Instance.SetCursorState(true);
                InputManager.Instance.SetCinemachineInputProviderState(firstPersonCamera, true);
            }
            else
            {
                InputManager.Instance.PlayerActions.Disable();
                InputManager.Instance.SetCursorState(false);
                InputManager.Instance.SetCinemachineInputProviderState(firstPersonCamera, false);

                StartCoroutine(DelayFocus());
            }
        }
    }

    private void ConsoleTextFieldSubmit(KeyDownEvent e)
    {
        if (e.keyCode != KeyCode.Return)
            return;

        e.StopPropagation();

        if (string.IsNullOrEmpty(consoleInputTextField.value))
            return;

        string inputString = consoleInputTextField.value;
        Log(inputString, true);

        string[] splitInput = inputString.Trim().Split(' ');
        if (splitInput.Length > 0)
        {
            string commandName = splitInput[0];
            string[] arguments = splitInput.Skip(1).ToArray();

            if (_commands.TryGetValue(commandName.ToLower(), out ICommand command))
            {
                try
                {
                    command.Execute(arguments);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Unintended exception occured while executing command '{commandName}': {exception.Message}");
                }
            }
            else
            {
                LogError($"Unknown command: \"{commandName}\". Type \"help\" to see a full list of commands.");
            }
        }

        consoleInputTextField.value = string.Empty;
        StartCoroutine(DelayFocus());
    }

    IEnumerator DelayFocus()
    {
        yield return null;
        consoleInputTextField.Focus();
    }

    IEnumerator DelayAutoScrollToEnd()
    {
        yield return null;

        VisualElement scrollViewContainer = consoleScrollView.contentContainer;
        if (scrollViewContainer.childCount > 0)
        {
            VisualElement lastChild = scrollViewContainer[scrollViewContainer.childCount - 1];
            consoleScrollView.ScrollTo(lastChild);
        }
    }

    public void Log(string message, bool printTimestamp = false)
    {
        var inputManager = InputManager.Instance;

        Label label = new()
        {
            text = printTimestamp ? $"[{DateTime.Now:HH:mm}] {message}{inputManager.Break}" : $"{message}{inputManager.Break}"
        };

        label.AddToClassList("console-text");

        consoleScrollView.Add(label);
        StartCoroutine(DelayAutoScrollToEnd());
    }

    public void LogError(string message, bool printTimestamp = false)
    {
        var uiColors = ColorProvider.UIColors;
        var inputManager = InputManager.Instance;

        Label label = new()
        {
            text = printTimestamp ? $"<color={uiColors.ErrorColor}>[{DateTime.Now:HH:mm}] {message}</color>{inputManager.Break}" : $"<color={uiColors.ErrorColor}>{message}</color>{inputManager.Break}"
        };

        label.AddToClassList("console-text");

        consoleScrollView.Add(label);
        StartCoroutine(DelayAutoScrollToEnd());
    }

    private void RegisterCommands()
    {
        var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => typeof(IRootCommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

        foreach (var type in commandTypes)
        {
            try
            {
                ICommand commandInstance = (ICommand)Activator.CreateInstance(type);
                RegisterCommand(commandInstance);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to auto-instantiate console command '{type.Name}': {exception.Message}");
            }
        }
    }

    private void RegisterCommand(ICommand command)
    {
        if (!_commands.ContainsKey(command.Name.ToLower()))
            _commands.Add(command.Name.ToLower(), command);

        if (command.Aliases != null && command.Aliases.Length != 0)
        {
            foreach (string alias in command.Aliases)
            {
                string cleanAlias = alias.Trim().ToLower();

                if (string.IsNullOrEmpty(cleanAlias))
                    continue;

                if (!_commands.ContainsKey(cleanAlias))
                    _commands.Add(cleanAlias, command);
            }
        }
    }

    public void SimulateCommand(string commandName, string[] args)
    {
        if (_commands.TryGetValue(commandName.ToLower(), out ICommand command))
        {
            try
            {
                command.Execute(args);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Unintended exception occured while simulating command '{commandName}': {exception.Message}");
            }
        }
    }

    public IEnumerable<ICommand> GetAllCommands() => _commands.Values.Distinct();

    public bool TryGetCommand(string commandName, out ICommand command) => _commands.TryGetValue(commandName.ToLower(), out command);
}