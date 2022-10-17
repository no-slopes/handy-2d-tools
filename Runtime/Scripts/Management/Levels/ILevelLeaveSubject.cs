using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace H2DT.Management.Levels
{
    public interface ILevelLeaveSubject
    {
        Task LevelLeaveTask();
    }
}