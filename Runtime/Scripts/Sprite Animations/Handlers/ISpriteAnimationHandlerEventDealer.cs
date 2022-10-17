namespace H2DT.SpriteAnimations.Handlers
{
    public interface ISpriteAnimationCycleEndDealer
    {
        void OnAnimationCycleEnd(SpriteAnimation animation, SpriteAnimationCompositeCycleType cycleType);
    }
}