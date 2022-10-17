using UnityEngine;

namespace H2DT.Capabilities.Platforming
{
    public struct WallHitData
    {
        /// <summary>
        /// If hitting any walls
        /// </summary>
        public bool hittingAnyWall => leftHitting || rightHitting;

        /// <summary>
        /// True if hitting any wall to the left
        /// </summary>
        public bool leftHitting => upperLeft || centerLeft || lowerLeft;

        /// <summary>
        /// 
        /// </summary>
        public bool rightHitting => upperRight || centerRight || lowerRight;

        /// <summary>
        /// True if hitting a wall with the top right corner
        /// </summary>
        public RaycastHit2D upperRight;
        /// <summary>
        /// The top right corner hit angle
        /// </summary>
        public float upperRightHitAngle;

        /// <summary>
        /// True if hitting a wall with the lower right corner
        /// </summary>
        public RaycastHit2D lowerRight;
        /// <summary>
        /// The lower right corner hit angle
        /// </summary>
        public float lowerRightHitAngle;

        /// <summary>
        /// True if hitting a wall with the center right checker
        /// </summary>
        public RaycastHit2D centerRight;
        /// <summary>
        /// The center right hit angle
        /// </summary>
        public float centerRightHitAngle;

        /// <summary>
        /// True if hitting a wall with the top left corner
        /// </summary>
        public RaycastHit2D upperLeft;
        /// <summary>
        /// The top left corner hit angle
        /// </summary>
        public float upperLeftHitAngle;

        /// <summary>
        /// True if hitting a wall with the lower left corner
        /// </summary>
        public RaycastHit2D lowerLeft;
        /// <summary>
        /// The lower left corner hit angle
        /// </summary>
        public float lowerLeftHitAngle;

        /// <summary>
        /// True if hitting a wall with the center left checker
        /// </summary>
        public RaycastHit2D centerLeft;
        /// <summary>
        /// The center left hit angle
        /// </summary>
        public float centerLeftHitAngle;
    }
}
