using System.Collections.Generic;
using UnityEngine.Events;

namespace H2DT.Management.Gameplay
{
    public class GameplayManagerPausedState : GameplayManagerState
    {
        private UnityEvent<bool> _stateEvent;

        protected GameplayManagerIdleState _idleState;

        protected List<GameplayTransitionSubject> _currentTransitionSubjects;

        public void OnLoad()
        {
            _idleState = machine.GetComponent<GameplayManagerIdleState>();
            _stateEvent = actor.gameplayHandler.GetStateStatusEvent(GameplayStateType.Paused);
        }

        public void OnEnter()
        {
            ApplyPause();
            StartPauseEnterTransitions();
        }

        public void OnExit()
        {
            ApplyUnPause();
        }

        /// <summary>
        /// Start all tasks registered on the gameplayManager -> pauseTaskCommander.
        /// Aftter all tasks are completed, applies pause.
        /// </summary>
        protected async void StartPauseEnterTransitions()
        {
            _currentTransitionSubjects = actor.gameplayHandler.GetCurrentTransitionSubjects(GameplayStateType.Paused);
            await actor.gameplayHandler.gameplayTransitionsCommander.PlayEnterTransitions(_currentTransitionSubjects);

            // Listening for the resume request
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Idle).AddListener(OnRestRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Paused).AddListener(OnPauseRequest);
        }

        /// <summary>
        /// Applies pause for real. This must be called when every task
        /// wich must be performed before really pausing the gameplay
        /// are trully performing.
        /// </summary>
        protected void ApplyPause()
        {
            // Pausing the game
            _stateEvent.Invoke(true);
            actor.gameplayHandler.currentStateType = GameplayStateType.Paused;

        }

        /// <summary>
        /// Unpauses the gameplay real. This must be called when every task
        /// wich must be performed before really pausing the gameplay
        /// are trully stopped.
        /// </summary>
        protected void ApplyUnPause()
        {
            _stateEvent.Invoke(false);
        }


        protected void OnRestRequest()
        {
            Resume(_idleState);
        }

        protected void OnPauseRequest()
        {
            Resume(machine.previousState as GameplayManagerState);
        }

        protected async void Resume(GameplayManagerState nextState)
        {

            // Removing listeners
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Idle).AddListener(OnRestRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Paused).RemoveListener(OnPauseRequest);

            await actor.gameplayHandler.gameplayTransitionsCommander.PlayExitTransitions(_currentTransitionSubjects);

            if (machine.previousState.GetType().Equals(typeof(GameplayManagerPlayingState)))
            {
                actor.gameplayHandler.Unfreeze(actor.gameplayHandler.freezeBeforePauseDuration, () => machine.EndState(machine.previousState));
            }
            else
            {
                machine.EndState(nextState);
            }
        }

    }
}
