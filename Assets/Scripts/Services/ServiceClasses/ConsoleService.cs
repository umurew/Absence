using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class ConsoleService : MonoBehaviour, IConsoleService
{
    // Injected dependecies
    private IInputService inputService;
    private UIDocument consoleDocument;

    private Dictionary<string, ICommand> _commands => new();
    private VisualElement console_container;
    private ScrollView console_scrollView;
    private TextField console_inputTextField;

    private bool consoleState;

    public void Initialize(IInputService inputService, UIDocument consoleDocument, ICommand[] commands)
    {
        this.inputService = inputService;
        this.consoleDocument = consoleDocument;

        foreach (ICommand command in commands)
            command.Initialize(this);
    }

    private void Start()
    {
        VisualElement root = consoleDocument.rootVisualElement;

        console_container = root.Q<VisualElement>("ConsoleContainer");
        console_scrollView = root.Q<ScrollView>("ConsoleScrollView");
        console_inputTextField = root.Q<TextField>("ConsoleInputTextField");

        console_container.style.visibility = Visibility.Hidden;
        console_inputTextField.RegisterCallback<KeyDownEvent>(HandleInputKeyDownEvent, TrickleDown.TrickleDown);
    }

    private void Update()
    {
        // Handle console toggling
        if (inputService.UIActions.ToggleConsole.WasPressedThisFrame() || (inputService.UIActions.Back.WasPressedThisFrame() && consoleState))
        {
            consoleState = !consoleState;
            console_container.style.visibility = consoleState ? Visibility.Visible : Visibility.Hidden;

            if (!consoleState)
            {
                inputService.PlayerActions.Enable();
                inputService.SetCursorState(true);
                inputService.EnableControls();
            }
            else
            {
                inputService.PlayerActions.Disable();
                inputService.SetCursorState(false);
                inputService.DisableControls();

                this.ExecuteDelayed(() => console_inputTextField.Focus());
            }
        }
    }

    public void RegisterCommand(ICommand command)
    {
        string commandName = command.Name.ToLower();

        if (!_commands.ContainsKey(commandName))
            _commands.Add(commandName, command);

        if (command.Aliases != null)
        {
            foreach (string alias in command.Aliases)
            {
                string cleanAlias = alias.Trim().ToLower();

                if (!string.IsNullOrEmpty(cleanAlias) && !_commands.ContainsKey(cleanAlias))
                    _commands.Add(cleanAlias, command);
            }
        }
    }

    public void Log(string text)
    {
        Label label = new() { text = $"{text}{inputService.NewLine}" };
        label.AddToClassList("console-text");

        console_scrollView.Add(label);
        this.ExecuteDelayed(() =>
        {
            VisualElement scrollViewContainer = console_scrollView.contentContainer;
            if (scrollViewContainer.childCount > 0)
            {
                VisualElement lastChild = scrollViewContainer[scrollViewContainer.childCount - 1];
                console_scrollView.ScrollTo(lastChild);
            }
        });
    }

    public void LogError(string text) => Log($"<color={ColorProvider.UIColors.ErrorColor}>{text}</color>");

    public void LogMissingArgumentError(string commandSyntax, string argumentName, string availableOptions)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append($"Syntax: <i>{commandSyntax}</i>");

        Log(stringBuilder.ToString());
        stringBuilder.Clear();

        stringBuilder.Append($"Missing required argument <{argumentName}>");

        if (!string.IsNullOrEmpty(availableOptions))
            stringBuilder.Append($"{inputService.NewLine}Available options: {availableOptions}");

        LogError(stringBuilder.ToString());
        stringBuilder.Clear();
    }

    public void LogInvalidArgumentError(string commandSyntax, string argumentName, string input, string availableOptions)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append($"Syntax: <i>{commandSyntax}</i>");

        Log(stringBuilder.ToString());
        stringBuilder.Clear();

        stringBuilder.Append($"Invalid argument <{argumentName}> : {input}");

        if (!string.IsNullOrEmpty(availableOptions))
            stringBuilder.Append($"{inputService.NewLine}Available options: {availableOptions}");

        LogError(stringBuilder.ToString());
        stringBuilder.Clear();
    }

    public void Clear() => console_scrollView.Clear();

    public bool TryGetCommand(string commandName, out ICommand command) => _commands.TryGetValue(commandName.ToLower(), out command);

    public IEnumerable<ICommand> GetAllCommands() => _commands.Values.Distinct();

    private void HandleInputKeyDownEvent(KeyDownEvent e)
    {
        if (e.keyCode == KeyCode.Return)
        {
            e.StopPropagation();

            if (string.IsNullOrWhiteSpace(console_inputTextField.value))
                return;

            string inputString = console_inputTextField.value;
            Log(inputString);

            string[] allTokens = Parser.SplitArguments(inputString);

            if (allTokens.Length > 0)
            {
                string commandName = allTokens[0];

                string[] commandArgs = new string[allTokens.Length - 1];
                Array.Copy(allTokens, 1, commandArgs, 0, commandArgs.Length);

                if (_commands.TryGetValue(commandName.ToLower(), out ICommand command))
                {
                    try
                    {
                        command.Execute(commandArgs);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"Exception occured while executing command '{commandName}': {exception}");
                    }
                }
                else
                    LogError($"Unknown command: \"{commandName}\". Type \"help\" to see a full list of commands.");
            }

            console_inputTextField.value = string.Empty;
            this.ExecuteDelayed(() => console_inputTextField.Focus());
        }
    }
}