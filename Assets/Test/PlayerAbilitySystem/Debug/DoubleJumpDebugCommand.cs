using SAS.StateMachineGraph;
using SAS.Utilities.BlackboardSystem;
using SAS.Utilities.DeveloperConsole;
using UnityEngine;

namespace SAS.StateMachineCharacterController.DeveloperConsole
{
    [CreateAssetMenu(fileName = "DoubleJumpDebugCommand", menuName = "SAS/Utilities/DeveloperConsole/Commands/Double Jump Command")]
    public class DoubleJumpDebugCommand : ConsoleCommand
    {
        public override string HelpText => $"Usage: {CommandWord} [true/false].Enable/Disable Double Jump";

        public override bool Process(DeveloperConsoleBehaviour developerConsole, string[] args)
        {
#if DEBUG
            string enableDoubleJump = "false";
            var actor = FindFirstObjectByType<Actor>();
            if (actor)
            {
                actor.TryGet(new BlackboardKey(FSMCharacterBlackboardKey.MaxJumpCount), out int maxJumpCount);
                enableDoubleJump = maxJumpCount > 1 ? "false" : "true";
            }
            if (args == null || args.Length == 0)
                args = new string[] { enableDoubleJump };

            if (bool.TryParse(args[0], out var status))
            {
                if (actor != null)
                    actor.GetComponent<AbilityHandler>().UnlockAbility(AbilityType.DoubleJump,status);

                return true;
            }
#endif
            return false;
        }
    }
}
