using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Debugging.Console
{
    public class DebugConsoleCommand
    {
        #region Fields

        private string _id;
        private string _description;
        private string _format;
        private UnityAction<string[]> _action;

        #endregion

        #region Getters        

        public string id => _id;
        public string description => _description;
        public string format => _format;

        #endregion

        #region Constructors

        public DebugConsoleCommand(string commandId, string commandDescription, string commandFormat, UnityAction<string[]> action)
        {
            _id = commandId;
            _description = commandDescription;
            _format = commandFormat;
            _action = action;
        }

        #endregion

        #region Executing

        public void Execute(string[] values = null)
        {
            _action.Invoke(values);
        }

        #endregion
    }
}
