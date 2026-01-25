using UnityEngine;

public class Chapter_BadEnd : GameChapter
{
    protected override void TriggerOnStart()
    {
        SceneSystemManager.Instance.EnterTriggerScene("BadEnd");
        ReportManager.Instance.SwitchBtn(false);
        ReportManager.Instance.SwitchConclusionBtn(false);
    }
}
