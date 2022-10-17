

namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to flip characters vertically
    /// should implement this interface
    /// </summary>
    public interface IPlatformerVerticalFlipPerformer
    {

        /// <summary>
        /// Flips character vertically
        /// </summary>
        void FlipVertically();

        /// <summary>
        /// This method must evaluate if character should be flipped
        /// and perform accordingly.
        /// </summary>
        void EvaluateAndFlipVertically(float directionSign);

        void Lock(bool shouldLock);
    }
}
