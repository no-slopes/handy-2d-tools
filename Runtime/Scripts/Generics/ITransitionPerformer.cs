using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Generics.Transitions
{
    public interface ITransitionPerformer
    {
        Task PlayEnterTransition();
        Task PlayExitTransition();
    }
}
