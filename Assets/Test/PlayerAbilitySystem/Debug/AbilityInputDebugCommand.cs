using SAS.Utilities.DeveloperConsole;
using System;
using UnityEngine;

namespace SAS.StateMachineCharacterController.DeveloperConsole
{
    [CreateAssetMenu(fileName = "AbilityInputDebugCommand", menuName = "SAS/Utilities/DeveloperConsole/Commands/Ability Input Debug Command")]
    public class AbilityInputDebugCommand : ConsoleCommand
    {
        public override string HelpText => $"Usage: {CommandWord} [true/false].Enable/Disable Ability Input Debug Command";

        public override bool Process(DeveloperConsoleBehaviour developerConsole, string[] args)
        {
#if DEBUG
            string enableAbilityInput = "false";
            var abilityHandler = FindFirstObjectByType<AbilityHandler>();
            bool isEnabled = false;
            if (abilityHandler)
            {
                if (Enum.TryParse(CommandWord, out AbilityType ability))
                {
                    switch (ability)
                    {
                        case AbilityType.Dash:
                            isEnabled = abilityHandler.AbilityData.DashUnlocked;
                            break;
                        case AbilityType.Climb:
                            isEnabled = abilityHandler.AbilityData.WallClimbUnlocked;
                            break;
                        default:
                            Debug.LogWarning($"Ability type '{CommandWord}' is not handled.");
                            break;
                    }
                }
                else
                    Debug.LogWarning($"Invalid ability type: {CommandWord}");

                enableAbilityInput = isEnabled ? "false" : "true";
            }
            if (args == null || args.Length == 0)
                args = new string[] { enableAbilityInput };

            if (bool.TryParse(args[0], out var status))
            {
                if (Enum.TryParse(CommandWord, out AbilityType ability))
                    abilityHandler.UnlockAbility(ability, status);
                else
                    Debug.LogWarning($"Invalid ability type: {CommandWord}");

                return true;
            }
#endif
            return false;
        }
    }
}
