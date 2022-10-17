using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using H2DT.Environmental;
using H2DT.Management.Gameplay;

namespace H2DT.Editor
{
    public class GameplayManagerEditor
    {
        [MenuItem("GameObject/Handy 2D Tools/Management/Gameplay")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject gameplayManagerGO = new GameObject("Gameplay Manager");
            gameplayManagerGO.AddComponent<GameplayManager>();
            GameObjectUtility.SetParentAndAlign(gameplayManagerGO, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameplayManagerGO, "Create " + gameplayManagerGO.name);
            Selection.activeObject = gameplayManagerGO;


            GameObject stateMachineGO = new GameObject("State Machine");

            stateMachineGO.AddComponent<GameplayManagerFSM>();

            stateMachineGO.AddComponent<GameplayManagerPlayingState>();
            GameplayManagerPlayingState playingState = stateMachineGO.GetComponent<GameplayManagerPlayingState>();
            playingState.SetName("Playing");

            stateMachineGO.AddComponent<GameplayManagerPausedState>();
            GameplayManagerPausedState startState = stateMachineGO.GetComponent<GameplayManagerPausedState>();
            startState.SetName("Paused");

            stateMachineGO.AddComponent<GameplayManagerGameoverState>();
            GameplayManagerGameoverState gameoverState = stateMachineGO.GetComponent<GameplayManagerGameoverState>();
            gameoverState.SetName("Game Over");

            stateMachineGO.AddComponent<GameplayManagerCutsceneState>();
            GameplayManagerCutsceneState cutSceneState = stateMachineGO.GetComponent<GameplayManagerCutsceneState>();
            cutSceneState.SetName("Cutscene");


            GameObjectUtility.SetParentAndAlign(stateMachineGO, gameplayManagerGO);

            GameplayManagerFSM gameplayManagerFSM = stateMachineGO.GetComponent<GameplayManagerFSM>();
            gameplayManagerFSM.RecognizeStates();
        }
    }
}