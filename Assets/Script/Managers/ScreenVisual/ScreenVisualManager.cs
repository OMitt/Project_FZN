using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum SceneVisualEffectType { Flash = 0, EnableBlack = 1, DisableBlack = 2, SceneChangeEnableBlack = 3, SceneChangeDisableBlack = 4 };

public class ScreenVisualManager : Singleton<ScreenVisualManager>
{
    [SerializeField]
    private CanvasGroup blackSceneCG;
    [SerializeField]
    private float eachFlashTime = 0.2f;
    [SerializeField]
    private int FlashLoopCount = 2;
    [SerializeField]
    private float sceneChangeDelayTime = 0.6f;

    [SerializeField]
    private GraphicRaycaster graphicRaycaster;
    [SerializeField]
    private Canvas canvas;

    public float SceneChangeDelayTime
    {
        get {return sceneChangeDelayTime;}
    }

    protected override void Awake()
    {
        base.Awake();

        Setup();
    }

    private void Setup()
    {
        blackSceneCG.alpha = 0.0f;
    }

    public void TriggerVisualEffect(List<SceneVisualEffectType> VFXs)
    {
        Sequence seqs = DOTween.Sequence();

        foreach (var VFX in VFXs)
        {

            Sequence seq = DOTween.Sequence();
            
            float startValue = 0.0f;
            float endValue = 1.0f;

            float tempValue = 0.0f;

            Debug.Log(VFX.ToString());
            switch (VFX)
            {      
                case SceneVisualEffectType.Flash:

                    tempValue = startValue;

                    for (int i = 0; i < FlashLoopCount; i++)
                    {
                        seq.Append(DOTween.To(() => tempValue, x => tempValue = x, endValue, eachFlashTime))
                            .OnUpdate(() => { blackSceneCG.alpha = tempValue;})
                            .OnStart(()=>{
                                SoundManager.Instance.PlaySFX("Light_0");
                                DialogueSystem.Instance.SwitchCanClick(false);});
                        seq.AppendInterval(eachFlashTime);
                        seq.Append(DOTween.To(() => tempValue, x => tempValue = x, startValue, eachFlashTime))
                            .OnUpdate(() => { blackSceneCG.alpha = tempValue;})
                            .OnComplete(()=>{DialogueSystem.Instance.SwitchCanClick(true);});

                    }

                    break;

                case SceneVisualEffectType.EnableBlack:

                    tempValue = startValue;

                    seq.Append(DOTween.To(() => tempValue, x => tempValue = x, endValue, 0.01f))
                            .OnUpdate(() => { blackSceneCG.alpha = tempValue; })
                            .OnStart(()=>{
                                SoundManager.Instance.PlaySFX("Light_1");
                                DialogueSystem.Instance.SwitchCanClick(false);})
                            .OnComplete(()=>{DialogueSystem.Instance.SwitchCanClick(true);});

                    break;

                case SceneVisualEffectType.DisableBlack:

                    tempValue = endValue;

                    seq.Append(DOTween.To(() => tempValue, x => tempValue = x, startValue, 0.01f))
                            .OnUpdate(() => { blackSceneCG.alpha = tempValue; })
                            .OnStart(()=>{
                                SoundManager.Instance.PlaySFX("Light_1");
                                DialogueSystem.Instance.SwitchCanClick(false);})
                            .OnComplete(()=>{DialogueSystem.Instance.SwitchCanClick(true);});
                            
                    break;

                case SceneVisualEffectType.SceneChangeEnableBlack:

                    if(graphicRaycaster!=null)
                    {
                        graphicRaycaster.enabled = true;
                        DialogueSystem.Instance.SwitchCanClick(false);
                    }

                    if(canvas!=null)
                    {
                        //TEST
                        canvas.sortingOrder = 999;
                    }

                    seq.AppendCallback(()=>{blackSceneCG.alpha = endValue;});

                    break;

                case SceneVisualEffectType.SceneChangeDisableBlack:
                    tempValue = endValue;

                    seq.Append(DOTween.To(() => tempValue, x => tempValue = x, startValue, sceneChangeDelayTime))
                            .OnUpdate(() => { blackSceneCG.alpha = tempValue; });
                    seq.AppendInterval(sceneChangeDelayTime);
                    seq.AppendCallback(()=>
                    {
                        if(canvas!=null)
                        {
                            //TEST
                            canvas.sortingOrder = 3;
                        }

                        if(graphicRaycaster!=null)
                        {
                            graphicRaycaster.enabled = false;
                            DialogueSystem.Instance.SwitchCanClick(true);
                        }
                    });

                    break;
            }

            seqs.Append(seq);
        }

        seqs.Play();
    }
}
