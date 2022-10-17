using System.Collections;
using System.Collections.Generic;
using H2DT.Actions.FSM;
using H2DT.Management.Gameplay;
using H2DT.Management.Scenes;
using UnityEngine;

namespace H2DT.Management.Gameplay
{
    public abstract class GameplayManagerState : State<GameplayManager>
    {
        protected GameplayManagerFSM machine => GetMachine<GameplayManagerFSM>();
    }
}
