using UnityEngine;
using DG.Tweening;

public class Chapter_0_1 : GameChapter
{
    public DialogueGroup StartDialogue;

    protected override void TriggerOnStart()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(()=>{SceneSystemManager.Instance.EnterTriggerScene("Outside");});   
        seq.AppendInterval(ScreenVisualManager.Instance.SceneChangeDelayTime);
        seq.AppendCallback(()=>{DialogueSystem.Instance.EnterNewDialgue(StartDialogue);});   
        
        seq.Play();
    }
}
