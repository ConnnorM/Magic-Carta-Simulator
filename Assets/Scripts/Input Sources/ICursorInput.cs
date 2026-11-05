public interface ICursorInput
{
    void Drive(PlayerCursor cursor, float deltaTime);
    bool SelectPressedThisFrame();
}