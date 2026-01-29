using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InteractiveUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector2 outlineDistance = new Vector2(8,-8);
    private Color outlineColor = Color.orangeRed;
    private Color imageNormalColor = Color.white;
    private Color imageHighLightColor = Color.indianRed;

    void Setup()
    {
        outlineDistance.x = outlineDistance.x/this.transform.localScale.x;
        outlineDistance.y = outlineDistance.y/this.transform.localScale.y;
        if (this.gameObject.TryGetComponent<Image>(out Image image))
        {
            imageNormalColor = image.color;
        }
        Outline outlineComp = null;
        if(this.gameObject.TryGetComponent<Outline>(out outlineComp))
        {
            outlineComp.effectDistance = outlineDistance;
            outlineComp.effectColor = outlineColor;
        }
    }

    void Start()
    {
        Setup();
        EnableOutlineComp(false);
        EnableImageHighlight(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        EnableOutlineComp(true);
        EnableImageHighlight(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        EnableOutlineComp(false);
        EnableImageHighlight(false);
    }

    private void EnableOutlineComp(bool enable)
    {
        Outline outlineComp = null;
        if(this.gameObject.TryGetComponent<Outline>(out outlineComp))
        {
            outlineComp.enabled = enable;
        }
    }

    private void EnableImageHighlight(bool enable)
    {
        Image image = null;
        if(this.gameObject.TryGetComponent<Image>(out image))
        {
            image.color = enable?imageHighLightColor:imageNormalColor;
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        EnableOutlineComp(false);
        EnableImageHighlight(false);

        foreach(var triggerEventStruct in triggerEventStructs)
        {
            switch (triggerEventStruct.triggerEventType)
            {
                case TriggerEventType.None:
                    break;
                case TriggerEventType.SwitchChapter:
                    GameLoopManager.Instance.EnterTriggerChapter(triggerEventStruct.nextChapterID);
                    break;
                case TriggerEventType.SwitchReport:
                    ReportManager.Instance.SwitchReport(triggerEventStruct.enableReport);
                    break;
                case TriggerEventType.ChangeScene:
                    SceneSystemManager.Instance.EnterTriggerScene(triggerEventStruct.targetSceneID,triggerEventStruct.DontUseBlackEffect);
                    break;
                case TriggerEventType.TriggerDialogue:
                    DialogueSystem.Instance.EnterNewDialgue(triggerEventStruct.targetDialogue,triggerEventStruct.enterDialgueDelay);
                    break;
                case TriggerEventType.SwitchSelection:
                    ConclusionManager.Instance.SwitchConclusion(triggerEventStruct.enableSelection, triggerEventStruct.selections);
                    break;
                case TriggerEventType.SwitchConclusionSelection:
                    ConclusionManager.Instance.SwitchChapterConclusion(triggerEventStruct.enableSelection);
                    break;
                case TriggerEventType.TriggerUnlock:
                    UnlockManager.Instance.TriggerUnlock(triggerEventStruct.unlockEventID,triggerEventStruct.unlockConditionID);
                    break;
                case TriggerEventType.TryTriggerUnlockEffect:
                    UnlockManager.Instance.TryTriggerEventEffect(triggerEventStruct.unlockEventID);
                    break;
                case TriggerEventType.SwitchConclusionBtn:
                    ReportManager.Instance.SwitchConclusionBtn(triggerEventStruct.enableConclusion);
                    break;
                case TriggerEventType.VisualEffect:
                    ScreenVisualManager.Instance.TriggerVisualEffect(triggerEventStruct.VFXs);
                    break;
                case TriggerEventType.OpenLevel:
                    //TEST
                    SceneManager.LoadScene(triggerEventStruct.targetLevelIndex);
                    break;
                case TriggerEventType.QuitGame:
                    //TEST
                    Application.Quit();
                    break;
                case TriggerEventType.EnterSavePointChapter:
                    GameLoopManager.Instance.EnterSavePointChapter();
                    break;
                case TriggerEventType.PlaySFX:
                    SoundManager.Instance.PlaySFX(triggerEventStruct.sfxID);
                    break;
            } 
        }
    }

    [Header("TriggerEventAfterClicking")]
    public List<TriggerEventStruct> triggerEventStructs;
}
