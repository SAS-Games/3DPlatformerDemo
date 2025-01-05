using UnityEngine;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
    [SerializeField] Image m_LoadingBar;
    [SerializeField] float m_FillSpeed = 0.5f;
    [SerializeField] Canvas m_LoadingCanvas;

    public void SetLoadProgress(float targetProgress)
    {
        float currentFillAmount = m_LoadingBar.fillAmount;
        float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

        float dynamicFillSpeed = progressDifference * m_FillSpeed;

        m_LoadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void OnEnable()
    {
        if (m_LoadingBar)
            m_LoadingBar.fillAmount = 0f;
    }
    private void OnDisable()
    {
        if (m_LoadingBar)
            m_LoadingBar.fillAmount = 0f;
    }
}
