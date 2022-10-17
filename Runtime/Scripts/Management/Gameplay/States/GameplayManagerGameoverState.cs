using System.Collections;
using System.Collections.Generic;
using H2DT.Generics.Transitions;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Gameplay
{
    public class GameplayManagerGameoverState : GameplayManagerState
    {
        private UnityEvent<bool> _stateEvent;

        protected GameplayManagerIdleState _idleState;

        protected List<GameplayTransitionSubject> _currentTransitionSubjects;

        public void OnLoad()
        {
            _idleState = machine.GetComponent<GameplayManagerIdleState>();

            _stateEvent = actor.gameplayHandler.GetStateStatusEvent(GameplayStateType.Gameover);
        }

        public void OnEnter()
        {
            _stateEvent.Invoke(true);
            StartEnterGameoverTransitions();
        }

        public void OnExit()
        {
            _stateEvent.Invoke(false);
        }

        protected async void StartEnterGameoverTransitions()
        {
            _currentTransitionSubjects = actor.gameplayHandler.GetCurrentTransitionSubjects(GameplayStateType.Gameover);
            await actor.gameplayHandler.gameplayTransitionsCommander.PlayEnterTransitions(_currentTransitionSubjects);
            actor.gameplayHandler.currentStateType = GameplayStateType.Gameover;
        }
    }
}
