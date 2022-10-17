using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using H2DT.Management.Scenes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Gameplay
{
    public class GameplayManagerPlayingState : GameplayManagerState
    {
        private UnityEvent<bool> _stateEvent;

        private GameplayManagerIdleState _idleState;
        private GameplayManagerPausedState _pausedState;
        private GameplayManagerCutsceneState _cutsceneState;
        private GameplayManagerGameoverState _gameoverState;

        private float _startedAt;

        public void OnLoad()
        {
            _idleState = machine.GetComponent<GameplayManagerIdleState>();
            _pausedState = machine.GetComponent<GameplayManagerPausedState>();
            _cutsceneState = machine.GetComponent<GameplayManagerCutsceneState>();
            _gameoverState = machine.GetComponent<GameplayManagerGameoverState>();

            _stateEvent = actor.gameplayHandler.GetStateStatusEvent(GameplayStateType.Playing);
        }

        public void OnEnter()
        {

            actor.gameplayHandler.Unfreeze(0);

            _stateEvent.Invoke(true);

            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Idle).AddListener(OnRestRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Paused).AddListener(OnPauseRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Cutscene).AddListener(OnCutsceneRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Gameover).AddListener(OnGameOver);

            _startedAt = Time.time;

            actor.gameplayHandler.currentStateType = GameplayStateType.Playing;
        }

        public void OnExit()
        {
            _stateEvent.Invoke(false);

            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Idle).RemoveListener(OnRestRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Paused).RemoveListener(OnPauseRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Cutscene).RemoveListener(OnCutsceneRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Gameover).RemoveListener(OnGameOver);

            int elapsedTime = (int)(Time.time - _startedAt);
            actor.gameplayHandler.AnnouncePlayedTime(elapsedTime);
        }

        protected void OnRestRequest()
        {
            machine.EndState(_idleState);
        }

        protected void OnPauseRequest()
        {
            actor.gameplayHandler.Freeze(actor.gameplayHandler.freezeBeforePauseDuration, () => machine.EndState(_pausedState));
        }

        protected void OnGameOver()
        {
            actor.gameplayHandler.Freeze(0, () => machine.EndState(_gameoverState));
        }

        protected void OnCutsceneRequest()
        {
            machine.EndState(_cutsceneState);
        }
    }
}
