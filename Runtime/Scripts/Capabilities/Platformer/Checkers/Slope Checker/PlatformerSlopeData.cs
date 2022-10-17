using UnityEngine;

namespace H2DT.Capabilities.Platforming
{
    public class PlatformerSlopeData
    {
        /// <summary>
        /// Means the object is standing on a slope
        /// </summary>
        public bool onSlope;

        /// <summary>
        /// Means the slope the object is standing on has a higher angle
        /// then what it is allowed to walk on
        /// </summary>
        public bool higherThanMax;

        /// <summary>
        /// The Normal perpendicular to the slope
        /// </summary>
        public Vector2 normalPerpendicular;

        /// <summary>
        /// If the object is ascending
        /// </summary>
        public bool ascending;

        /// <summary>
        /// If the object is descending
        /// </summary>
        public bool descending;

        /// <summary>
        /// If Exiting from above the slope
        /// </summary>
        public bool exitingFromAbove;

        /// <summary>
        /// If exiting from below the slope
        /// </summary>
        public bool exitingFromBelow;
    }
}
