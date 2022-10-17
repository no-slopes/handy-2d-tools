using System.Collections;
using System.Collections.Generic;
using H2DT.Generics.Transitions;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Gameplay
{
    public class GameplayManagerCutsceneState : GameplayManagerState
    {
        private UnityEvent<bool> _stateEvent;

        protected GameplayManagerIdleState _idleState;
        protected GameplayManagerPausedState _pausedState;
        protected GameplayManagerPlayingState _playingState;

        public void OnLoad()
        {
            _idleState = machine.GetComponent<GameplayManagerIdleState>();
            _pausedState = machine.GetComponent<GameplayManagerPausedState>();
            _playingState = machine.GetComponent<GameplayManagerPlayingState>();

            _stateEvent = actor.gameplayHandler.GetStateStatusEvent(GameplayStateType.Cutscene);
        }

        public void OnEnter()
        {
            actor.gameplayHandler.currentStateType = GameplayStateType.Cutscene;

            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Idle).AddListener(OnRestRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Paused).AddListener(OnPausedRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Playing).AddListener(OnPlayingRequest);

            _stateEvent.Invoke(true);
        }

        public void OnExit()
        {
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Idle).RemoveListener(OnRestRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Paused).RemoveListener(OnPausedRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Playing).RemoveListener(OnPlayingRequest);

            _stateEvent.Invoke(false);
        }

        protected void OnRestRequest()
        {
            machine.EndState(_idleState);
        }

        protected void OnPausedRequest()
        {
            machine.EndState(_pausedState);
        }

        protected void OnPlayingRequest()
        {
            machine.EndState(_playingState);
        }
    }
}
