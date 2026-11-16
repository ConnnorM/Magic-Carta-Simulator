public interface ICursorInput
{
    CursorInputType InputType { get; }
    string DeviceName { get; }
    void Drive(PlayerCursor cursor, float deltaTime);
    bool SelectPressedThisFrame();
}