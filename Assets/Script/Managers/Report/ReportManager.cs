using UnityEngine;
using UnityEngine.UI;

public class ReportManager : Singleton<ReportManager>
{
    [Header("UI References")]
    [SerializeField]
    private Transform reportPanel;
    [SerializeField]
    private Image reportImage;
    [SerializeField]
    private Transform reportBtn;
    [SerializeField]
    private Transform conclusionBtn;

    protected override void Awake()
    {
        base.Awake();
        SetUp();
    }

    public void SetUp()
    {
        SwitchReport(false);
        SwitchBtn(false);
        SwitchConclusionBtn(false);
    }

    public void SetCurrentReport(ReportSetting targetReport)
    {
        reportImage.sprite = targetReport.sprite;
    }

    public void SwitchReport(bool enable)
    {
        reportPanel.gameObject.SetActive(enable);
        SoundManager.Instance.PlaySFX("Paper_0");
    }

    public void SwitchBtn(bool enable)
    {
        reportBtn.gameObject.SetActive(enable);
    }

    public void SwitchConclusionBtn(bool enable)
    {
        conclusionBtn.gameObject.SetActive(enable);
    }
}
