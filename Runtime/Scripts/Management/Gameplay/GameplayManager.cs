using H2DT.Actions;
using H2DT.Debugging;
using H2DT.Management.Booting;
using UnityEngine;

namespace H2DT.Management.Gameplay
{
    [DefaultExecutionOrder(-6000)]
    public abstract class GameplayManager : BooterSubject
    {
        #region Inspector     

        [Header("Generic Actor config")]
        [Tooltip("The state machine wich will handle the actor states. If not provided this component will try finding one among its components or the components of child objects.")]
        [Space]
        [SerializeField]
        private GameplayManagerFSM _stateMachine;

        [Header("Configuration")]
        [Space]
        [SerializeField]
        private GameplayHandler _gameplayHandler;

        #endregion

        #region Properties

        protected GameplayManagerFSM stateMachine => _stateMachine;
        public GameplayHandler gameplayHandler => _gameplayHandler;

        #endregion

        #region Mono

        protected override void Awake()
        {
            SetMachine();
            base.Awake();
        }

        #endregion      

        /// <summary>
        /// Sets the state machine for this Actor. 
        /// </summary>
        protected virtual void SetMachine()
        {
            if (_stateMachine == null)
                FindComponentInChildren<GameplayManagerFSM>(ref _stateMachine);

            if (_stateMachine != null) { _stateMachine.SetUp(this); return; }

            Log.Danger($"State Machine not found for {GetType()}");
        }
    }
}
