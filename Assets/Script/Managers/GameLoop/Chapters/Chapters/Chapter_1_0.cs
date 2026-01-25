using UnityEngine;
using DG.Tweening;

public class Chapter_1_0 : GameChapter
{
    public DialogueGroup StartDialogue;

    protected override void TriggerOnStart()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(()=>{SceneSystemManager.Instance.EnterTriggerScene("InsideDark");});   
        seq.AppendInterval(ScreenVisualManager.Instance.SceneChangeDelayTime);
        seq.AppendCallback(()=>{DialogueSystem.Instance.EnterNewDialgue(StartDialogue);});   
        
        seq.Play();
    }
}
