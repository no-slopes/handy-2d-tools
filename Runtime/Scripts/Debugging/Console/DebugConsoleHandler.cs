using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Management.Booting;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Debugging.Console
{
    [CreateAssetMenu(fileName = "Debug Console Handler", menuName = "Handy 2D Tools/Debugging/Console Handler")]
    public class DebugConsoleHandler : ScriptableObject, IBootable
    {
        #region Inspector

        [Header("Instanciating")]
        [Space]
        [SerializeField]
        private GameObject _consolePrefab;

        [Header("Events")]
        [Space]
        [SerializeField]
        private UnityEvent<bool> _activationUpdate;

        #endregion

        #region Fields

        private bool _isActive = false;

        private Dictionary<string, DebugConsoleCommand> _commands;

        private GameObject _consoleGO;

        #endregion

        #region Properties

        public int totalOfCommands => _commands.Count;

        #endregion

        #region Getters

        public GameObject consoleGO => _consoleGO;
        public Dictionary<string, DebugConsoleCommand> commands => _commands;
        public UnityEvent<bool> activationUpdate => _activationUpdate;

        #endregion

        #region Booting

        public Task BootableBoot()
        {
            _commands = new Dictionary<string, DebugConsoleCommand>();

            return Task.CompletedTask;
        }

        public Task BootableDismiss()
        {
            _commands.Clear();
            _commands = null;
            return Task.CompletedTask;
        }

        #endregion

        #region Instanciation

        public void Enable()
        {
            _consoleGO = Instantiate(_consolePrefab);
        }

        #endregion

        #region Requests

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            _activationUpdate.Invoke(_isActive);
        }

        #endregion

        #region Handling Commands

        public void AddCommand(DebugConsoleCommand command)
        {
            if (_commands.ContainsKey(command.id))
            {
                Debug.LogError($"There already is a registered command with ID '{command.id}'.", this);
                return;
            }

            _commands.Add(command.id, command);
        }

        public DebugConsoleCommand GetCommand(string id)
        {
            if (!_commands.ContainsKey(id)) return null;

            return _commands[id];
        }

        public void RemoveCommand(string id)
        {
            if (!_commands.ContainsKey(id)) return;

            _commands.Remove(id);
        }

        public bool ExecuteCommand(string id, string[] values = null)
        {
            if (_commands.TryGetValue(id, out DebugConsoleCommand command))
            {
                command.Execute(values);
                return true;
            }
            else
            {
                Debug.LogError($"Trying to execute command with ID '{id}' but no command was registered under that ID.", this);
                return false;
            }
        }

        #endregion
    }
}
