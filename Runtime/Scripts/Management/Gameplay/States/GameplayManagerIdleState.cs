using System.Collections;
using System.Collections.Generic;
using H2DT.Generics.Transitions;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Gameplay
{
    public class GameplayManagerIdleState : GameplayManagerState
    {
        private UnityEvent<bool> _stateEvent;

        private GameplayManagerPlayingState _playingState;
        private GameplayManagerPausedState _pausedState;
        private GameplayManagerCutsceneState _cutsceneState;

        protected List<GameplayTransitionSubject> _currentTransitionSubjects;

        public void OnLoad()
        {
            _playingState = machine.GetComponent<GameplayManagerPlayingState>();
            _pausedState = machine.GetComponent<GameplayManagerPausedState>();
            _cutsceneState = machine.GetComponent<GameplayManagerCutsceneState>();

            _stateEvent = actor.gameplayHandler.GetStateStatusEvent(GameplayStateType.Idle);
        }

        public void OnEnter()
        {
            if (actor.gameplayHandler.shouldFreezeWhenIdle)
                actor.gameplayHandler.Freeze();

            actor.gameplayHandler.currentStateType = GameplayStateType.Idle;

            _stateEvent.Invoke(true);

            TransitionAndEnter();
        }

        public void OnExit()
        {
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Playing).RemoveListener(OnPlayingRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Paused).RemoveListener(OnPausedRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Cutscene).RemoveListener(OnCutsceneRequest);

            _stateEvent.Invoke(false);
        }

        public async void TransitionAndEnter()
        {
            _currentTransitionSubjects = actor.gameplayHandler.GetCurrentTransitionSubjects(GameplayStateType.Idle);
            await actor.gameplayHandler.gameplayTransitionsCommander.PlayEnterTransitions(_currentTransitionSubjects);

            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Playing).AddListener(OnPlayingRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Paused).AddListener(OnPausedRequest);
            actor.gameplayHandler.GetStateRequestEvent(GameplayStateType.Cutscene).AddListener(OnCutsceneRequest);
        }

        private void OnPlayingRequest()
        {
            EndState(_playingState);
        }

        private void OnPausedRequest()
        {
            EndState(_pausedState);
        }

        private void OnCutsceneRequest()
        {
            EndState(_cutsceneState);
        }

        private async void EndState(GameplayManagerState nextState)
        {
            _currentTransitionSubjects = actor.gameplayHandler.GetCurrentTransitionSubjects(GameplayStateType.Idle);
            await actor.gameplayHandler.gameplayTransitionsCommander.PlayExitTransitions(_currentTransitionSubjects);

            machine.EndState(nextState);
        }
    }
}
