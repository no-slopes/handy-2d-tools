namespace H2DT.Capabilities.Platforming
{
    /// <summary>
    /// Any GameObject that wants to flip characters
    /// should implement this interface
    /// </summary>
    public interface IPlatformerHorizontalFlipPerformer
    {
        /// <summary>
        /// Flips character horizontally
        /// </summary>
        void FlipHorizontally();

        /// <summary>
        /// This method must evaluate if character should be flipped
        /// and perform accordingly.
        /// </summary>
        void EvaluateAndFlipHorizontally(float directionSign);

        void Lock(bool shouldLock);
    }
}
