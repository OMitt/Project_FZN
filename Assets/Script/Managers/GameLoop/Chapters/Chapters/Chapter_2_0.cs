using System.Collections.Generic;
using UnityEngine;

public class Chapter_2_0 : GameChapter
{
    public DialogueGroup StartDialogue;
    public SelectionGroup ThisChaptersConclusion;

    protected override void TriggerOnStart()
    {
        List<SceneVisualEffectType> VFXs = new List<SceneVisualEffectType>();
        VFXs.Add(SceneVisualEffectType.DisableBlack);

        ScreenVisualManager.Instance.TriggerVisualEffect(VFXs);
        SceneSystemManager.Instance.EnterTriggerScene("Inside_Ch2_0",true);
        DialogueSystem.Instance.EnterNewDialgue(StartDialogue);

        ConclusionManager.Instance.CurrentSelection = ThisChaptersConclusion;
        ReportManager.Instance.SwitchConclusionBtn(true);
    }
}
