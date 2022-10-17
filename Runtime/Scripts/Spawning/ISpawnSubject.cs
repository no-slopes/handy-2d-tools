using System.Collections;
using System.Collections.Generic;
using H2DT.Enums;

namespace H2DT.Spawning
{
    public interface ISpawnSubject
    {
        void OnSpawn(SpawnInfo spawnInfo);
    }
}
