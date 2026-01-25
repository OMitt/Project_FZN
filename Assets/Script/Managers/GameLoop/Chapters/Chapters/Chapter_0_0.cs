using DG.Tweening;
using UnityEngine;

public class Chapter_0_0 : GameChapter
{
    public DialogueGroup chapter0StartDialogue;

    protected override void TriggerOnStart()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(()=>{SceneSystemManager.Instance.EnterTriggerScene("StartBroadcast");});
        seq.AppendCallback(()=>{SoundManager.Instance.SwitchAmbient("TVBad_0",true);});
        seq.AppendInterval(ScreenVisualManager.Instance.SceneChangeDelayTime);
        seq.AppendCallback(()=>{DialogueSystem.Instance.EnterNewDialgue(chapter0StartDialogue);});   
        
        seq.Play();
    }
}
