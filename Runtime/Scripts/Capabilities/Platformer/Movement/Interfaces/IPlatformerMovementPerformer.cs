
namespace H2DT.Capabilities.Platforming
{
    public interface IPlatformerMovementPerformer
    {
        float currentGravityScale { get; }

        void MoveHorizontally(float directionSign);
        void MoveHorizontally(float speed, float directionSign);

        void MoveVertically(float speed, float directionSign);

        void PushHorizontally(float force, float directionSign);
        void PushVertically(float force, float directionSign);

        void StopMovement();

        void ChangeGravityScale(float newScale);
    }
}
