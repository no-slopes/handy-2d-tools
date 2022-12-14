//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.2
//     from Assets/Plugins/No Slopes/Handy 2D Tools/Runtime/Debugging/DebugConsoleInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @DebugConsoleInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @DebugConsoleInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DebugConsoleInputActions"",
    ""maps"": [
        {
            ""name"": ""Console"",
            ""id"": ""9dde3106-2aa5-456c-be9c-4a37569fa810"",
            ""actions"": [
                {
                    ""name"": ""Toggle"",
                    ""type"": ""Button"",
                    ""id"": ""e471be9f-d819-4981-8ec2-59e7e21a171f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Execute"",
                    ""type"": ""Button"",
                    ""id"": ""e446b9e7-6068-4937-97cc-8ffdc5f30b06"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9965cbac-f03e-4e39-a29b-ca2ae02fd97c"",
                    ""path"": ""<Keyboard>/f5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Toggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""564011b3-6832-4e19-863e-acb3a7e0b7be"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Execute"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Console
        m_Console = asset.FindActionMap("Console", throwIfNotFound: true);
        m_Console_Toggle = m_Console.FindAction("Toggle", throwIfNotFound: true);
        m_Console_Execute = m_Console.FindAction("Execute", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Console
    private readonly InputActionMap m_Console;
    private IConsoleActions m_ConsoleActionsCallbackInterface;
    private readonly InputAction m_Console_Toggle;
    private readonly InputAction m_Console_Execute;
    public struct ConsoleActions
    {
        private @DebugConsoleInputActions m_Wrapper;
        public ConsoleActions(@DebugConsoleInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Toggle => m_Wrapper.m_Console_Toggle;
        public InputAction @Execute => m_Wrapper.m_Console_Execute;
        public InputActionMap Get() { return m_Wrapper.m_Console; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ConsoleActions set) { return set.Get(); }
        public void SetCallbacks(IConsoleActions instance)
        {
            if (m_Wrapper.m_ConsoleActionsCallbackInterface != null)
            {
                @Toggle.started -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnToggle;
                @Toggle.performed -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnToggle;
                @Toggle.canceled -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnToggle;
                @Execute.started -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnExecute;
                @Execute.performed -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnExecute;
                @Execute.canceled -= m_Wrapper.m_ConsoleActionsCallbackInterface.OnExecute;
            }
            m_Wrapper.m_ConsoleActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Toggle.started += instance.OnToggle;
                @Toggle.performed += instance.OnToggle;
                @Toggle.canceled += instance.OnToggle;
                @Execute.started += instance.OnExecute;
                @Execute.performed += instance.OnExecute;
                @Execute.canceled += instance.OnExecute;
            }
        }
    }
    public ConsoleActions @Console => new ConsoleActions(this);
    public interface IConsoleActions
    {
        void OnToggle(InputAction.CallbackContext context);
        void OnExecute(InputAction.CallbackContext context);
    }
}
