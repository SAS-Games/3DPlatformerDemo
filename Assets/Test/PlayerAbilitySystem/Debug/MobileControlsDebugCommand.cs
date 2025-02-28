using SAS.Utilities.DeveloperConsole;
using UnityEngine;

namespace SAS.StateMachineCharacterController.DeveloperConsole
{
    [CreateAssetMenu(fileName = "MobileControlsDebugCommand", menuName = "SAS/Utilities/DeveloperConsole/Commands/MobileControls Command")]
    public class MobileControlsDebugCommand : ConsoleCommand
    {
        public override string HelpText => $"Usage: {CommandWord} [true/false].Enable/Disable Mobile controls UI";

        public override bool Process(DeveloperConsoleBehaviour developerConsole, string[] args)
        {
#if DEBUG
            string enableUI = "false";

            var mobileContolsUI = SceneUtility.FindComponentInScene<MobileControlsUI>("UI");
            if (mobileContolsUI)
                enableUI = mobileContolsUI.gameObject.activeSelf ? "false" : "true";
            
            if (args == null || args.Length == 0)
                args = new string[] { enableUI };

            if (bool.TryParse(args[0], out var status))
            {
                if (mobileContolsUI != null)
                {
                    mobileContolsUI.gameObject.SetActive(status);
                    FlexPrefs.Set<bool>(MobileControlsUI.MobileControls, status);
                }

                return true;
            }
#endif
            return false;
        }
    }
}
