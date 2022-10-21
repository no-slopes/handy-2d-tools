using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace H2DT.Debugging.Console
{
    public class DebugConsole : SingleHandyComponent<DebugConsole>
    {
        #region Static

        private static string ToggleActionName = "Toggle";
        private static string ExecuteActionName = "Execute";

        private static string InputFieldName = "InputField";

        #endregion

        #region Inspector

        [Header("Handling")]
        [Space]
        [SerializeField]
        private DebugConsoleHandler _handler;

        #endregion

        #region Fields

        private DebugConsoleInputActions _consoleInputActions;
        private InputAction _toggleInputAction;
        private InputAction _executeInputAction;

        private bool _isOn;

        private bool _shouldShowCommandsList;
        private Vector2 _commandsListScroll;

        private string _inputText;


        #endregion

        #region Properties

        #endregion

        #region Getters

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            if (!singletonReady) return;

            _consoleInputActions = new DebugConsoleInputActions();

            _toggleInputAction = _consoleInputActions.FindAction(ToggleActionName);
            _executeInputAction = _consoleInputActions.FindAction(ExecuteActionName);

            RegisterDefaultCommands();
        }

        private void OnEnable()
        {
            _consoleInputActions.Enable();
            SubscribeActions();
        }

        private void OnDisable()
        {
            UnsubscribeActions();
            _consoleInputActions.Disable();
        }

        #endregion

        #region Listening to actions

        private void SubscribeActions()
        {
            if (_toggleInputAction != null)
                _toggleInputAction.performed += OnToggleAction;

            if (_executeInputAction != null)
                _executeInputAction.performed += OnExecuteAction;

        }

        private void UnsubscribeActions()
        {
            if (_toggleInputAction != null)
                _toggleInputAction.performed -= OnToggleAction;

            if (_executeInputAction != null)
                _executeInputAction.performed -= OnExecuteAction;
        }

        #endregion

        #region Default Commands

        private void RegisterDefaultCommands()
        {
            DebugConsoleCommand helpCommand = new DebugConsoleCommand("help", "Toggles on and off the list of current available commands", "help", (values) =>
            {
                _shouldShowCommandsList = !_shouldShowCommandsList;
            });

            _handler.AddCommand(helpCommand);
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            if (!_isOn) return;

            Rect boxRect = new Rect(5f, Screen.height - 40f, Screen.width - 10f, 30);

            GUI.Box(boxRect, "");

            Rect textFieldRect = new Rect(10f, Screen.height - 35f, Screen.width - 20f, 20f);

            GUI.SetNextControlName(InputFieldName);
            _inputText = GUI.TextField(textFieldRect, _inputText);
            GUI.FocusControl(InputFieldName);

            if (_shouldShowCommandsList)
            {
                Rect commandsListRect = new Rect(5f, Screen.height - 140f, Screen.width - 10f, 100f);
                GUI.Box(commandsListRect, "");

                Rect viewport = new Rect(commandsListRect.x, commandsListRect.y, commandsListRect.width - 30, 20 * _handler.totalOfCommands);

                Rect scrollViewRect = new Rect(commandsListRect.x, commandsListRect.y + 5f, Screen.width - 10f, 90);

                _commandsListScroll = GUI.BeginScrollView(scrollViewRect, _commandsListScroll, viewport);

                int position = 0;

                foreach (KeyValuePair<string, DebugConsoleCommand> item in _handler.commands)
                {
                    DebugConsoleCommand command = item.Value;
                    string label = $"{command.format} - {command.description}";

                    Rect labelRect = new Rect(5, viewport.y + 20 * position, viewport.width - 100, 20);
                    GUI.Label(labelRect, label);
                    position++;
                }

                GUI.EndScrollView();
            }
        }

        #endregion

        #region Tasks

        private void ExecuteCommand()
        {
            if (string.IsNullOrEmpty(_inputText)) return;

            string[] parts = _inputText.Split(" ");

            string id = parts[0];
            string[] values = parts.Skip(1).ToArray();

            if (_handler.ExecuteCommand(id, values))
            {
                Clear();
            }

        }

        private void Clear()
        {
            _inputText = "";
        }

        #endregion

        #region Input Callbacks

        private void OnToggleAction(InputAction.CallbackContext ctx)
        {
            _isOn = !_isOn;
            _handler.SetActive(_isOn);

            if (!_isOn)
            {
                Clear();
            }
        }

        private void OnExecuteAction(InputAction.CallbackContext ctx)
        {
            ExecuteCommand();
        }

        #endregion
    }
}
