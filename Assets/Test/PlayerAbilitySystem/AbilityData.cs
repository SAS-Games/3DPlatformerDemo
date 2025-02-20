public enum AbilityType
{
    Dash,
    DoubleJump,
    WallClimb
}

[System.Serializable]
public class AbilityData
{
    public bool DashUnlocked;
    public bool DoubleJumpUnlocked;
    public bool WallClimbUnlocked;

    public AbilityData()
    {
        DashUnlocked = false;
        DoubleJumpUnlocked = false;
        WallClimbUnlocked = false;
    }
}
