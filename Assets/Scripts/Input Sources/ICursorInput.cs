public interface ICursorInput
{
    CursorInputType InputType { get; }
    void Drive(PlayerCursor cursor, float deltaTime);
    bool SelectPressedThisFrame();
}