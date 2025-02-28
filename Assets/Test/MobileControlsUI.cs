using UnityEngine;

public class MobileControlsUI : MonoBehaviour
{
    public const string MobileControls = "MobileControls";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(FlexPrefs.Get<bool>(MobileControls));
    }
}
