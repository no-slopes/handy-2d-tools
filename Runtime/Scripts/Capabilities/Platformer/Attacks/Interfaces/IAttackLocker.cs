using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any component that wants to handle Simple Melee Attacks
    /// must implement this Interface.
    /// </summary>
    public interface IAttackLocker
    {
        /// <summary>
        /// Whoever wants to request an attack can invoke this event.
        /// </summary>
        UnityEvent<bool> AttackLock { get; }
    }
}
