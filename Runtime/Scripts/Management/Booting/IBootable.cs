using UnityEngine;
using System.Threading.Tasks;

namespace H2DT.Management.Booting
{
    public interface IBootable
    {
        /// <summary>
        /// This method should be called on the first game scene.
        /// </summary>
        Task BootableBoot();

        /// <summary>
        /// Should be called before quiting game
        /// </summary>
        Task BootableDismiss();
    }
}