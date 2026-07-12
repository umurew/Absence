using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class ConsoleService : MonoBehaviour, IConsoleService
{
    private IInputService _inputService;
    private bool _initialized = false;

    private readonly Dictionary<string, ICommand> _commands = new();
    private VisualElement _consoleContainer;
    private ScrollView _consoleScrollView;
    private TextField _consoleInputField;

    private bool _isConsoleEnabled;

    public void Initialize(IInputService inputService, UIDocument consoleDocument, ICommand[] commands)
    {
        if (_initialized)
        {
            Debug.LogWarning($"{nameof(ConsoleService)}: {nameof(Initialize)} can't be called after initialization.");
            return;
        }

        _inputService = inputService;

        foreach (ICommand command in commands)
            RegisterCommand(command);

        VisualElement root = consoleDocument.rootVisualElement;

        _consoleContainer = root.Q<VisualElement>("ConsoleContainer");
        _consoleScrollView = root.Q<ScrollView>("ConsoleScrollView");
        _consoleInputField = root.Q<TextField>("ConsoleInputField");

        _consoleContainer.style.visibility = Visibility.Hidden;
        _consoleInputField.RegisterCallback<KeyDownEvent>(HandleInputKeyDownEvent, TrickleDown.TrickleDown);

        _initialized = true;
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

        command.Initialize(this);
    }

    public void Log(string text)
    {
        Label label = new() { text = $"{text.AppendNewLine()}" };
        label.AddToClassList("console-text");

        _consoleScrollView.Add(label);
        this.ExecuteDelayed(() =>
        {
            VisualElement scrollViewContainer = _consoleScrollView.contentContainer;
            if (scrollViewContainer.childCount > 0)
            {
                VisualElement lastChild = scrollViewContainer[scrollViewContainer.childCount - 1];
                _consoleScrollView.ScrollTo(lastChild);
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
            stringBuilder.Append($"{string.Empty.AppendNewLine()}Available options: {availableOptions}");

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
            stringBuilder.Append($"{string.Empty.AppendNewLine()}Available options: {availableOptions}");

        LogError(stringBuilder.ToString());
        stringBuilder.Clear();
    }

    public void Clear() => _consoleScrollView.Clear();

    public bool TryGetCommand(string commandName, out ICommand command) => _commands.TryGetValue(commandName.ToLower(), out command);

    public IEnumerable<ICommand> GetAllCommands() => _commands.Values.Distinct();

    private void Update()
    {
        if (!_initialized)
            return;

        if (_inputService.UIActions.ToggleConsole.WasPressedThisFrame() || (_inputService.UIActions.Back.WasPressedThisFrame() && _isConsoleEnabled))
            ToggleConsoleState(!_isConsoleEnabled);
    }

    private void ToggleConsoleState(bool isConsoleEnabled)
    {
        _isConsoleEnabled = isConsoleEnabled;
        _consoleContainer.style.visibility = isConsoleEnabled ? Visibility.Visible : Visibility.Hidden;

        if (isConsoleEnabled)
        {
            _inputService.DisablePlayerControls();
            this.ExecuteDelayed(() => _consoleInputField.Focus());
        }
        else
            _inputService.EnablePlayerControls();
    }

    private void HandleInputKeyDownEvent(KeyDownEvent e)
    {
        if (e.keyCode == KeyCode.Return)
        {
            e.StopPropagation();

            if (string.IsNullOrWhiteSpace(_consoleInputField.value))
                return;

            string inputString = _consoleInputField.value;
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

            _consoleInputField.value = string.Empty;
            this.ExecuteDelayed(() => _consoleInputField.Focus());
        }
    }
}