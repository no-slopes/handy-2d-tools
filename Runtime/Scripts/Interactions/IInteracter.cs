using UnityEngine;

namespace H2DT.Interactions
{
    public interface IInteracter
    {
        bool CanInteract { get; }
        GameObject gameObject { get; }
    }
}
