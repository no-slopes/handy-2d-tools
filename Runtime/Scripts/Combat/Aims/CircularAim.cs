using System.Collections;
using System.Collections.Generic;
using H2DT;
using UnityEngine;

namespace H2DT.Combat.Aims
{
    public class CircularAim : Aim
    {
        #region Mono

        protected void LateUpdate()
        {
            RotateTowardDirection();
        }

        #endregion

        #region  Logic

        protected void RotateTowardDirection()
        {
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        #endregion
    }
}
