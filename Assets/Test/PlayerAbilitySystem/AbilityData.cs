public enum AbilityType
{
    Dash,
    DoubleJump,
    WallClimb
}

public class AbilityData
{
    public bool DashUnlocked { get; set; }
    public bool DoubleJumpUnlocked { get; set; }
    public bool WallClimbUnlocked { get; set; }

    public AbilityData()
    {
        DashUnlocked = false;
        DoubleJumpUnlocked = false;
        WallClimbUnlocked = false;
    }
}
