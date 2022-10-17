
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any component that implements this interface will be able to handle jump
    /// must implement this
    /// </summary>
    public interface IPlatformerJumpHandler
    {
        /// <summary>
        /// Jump performers should listen to this in order to know when request is made
        /// </summary>
        /// <value></value>
        UnityEvent JumpRequest { get; }

        /// <summary>
        /// Jump performers should listen to this in order to know when stop is demanded
        /// </summary>
        /// <value></value>
        UnityEvent JumpStop { get; }
    }
}
