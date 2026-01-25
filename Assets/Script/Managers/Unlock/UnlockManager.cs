using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : Singleton<UnlockManager>
{
    [SerializeField]
    private List<UnlockEventSetting> eventSettings;
    [SerializeField]
    private List<UnlockEventSetting> savePointEventSettings;

    protected override void Awake()
    {
        base.Awake();
        //TEST: Should only be used on demo
        Setup();
    }

    //TEST: Should only be used on demo
    private void Setup()
    {
        foreach (var setting in eventSettings)
        {
            setting.IsUnlocked = false;

            foreach (var conditon in setting.unlockConditions)
            {
                conditon.isUnlocked = false;
            }
        }

        foreach(var setting in savePointEventSettings)
        {
            setting.IsUnlocked = false;

            foreach (var conditon in setting.unlockConditions)
            {
                conditon.isUnlocked = false;
            }
        }
    }

    public void SaveEventSettings()
    {
        foreach(var setting in eventSettings)
        {
            foreach(var saveSetting in savePointEventSettings)
            {
                if(saveSetting.id == setting.id)
                {
                    saveSetting.CopySaveData(setting);
                }
            }
        }
    }

    public void LoadEventSettings()
    {
        foreach(var saveSetting in savePointEventSettings)
        {
            foreach(var setting in eventSettings)
            {
                if(setting.id == saveSetting.id)
                {
                    setting.CopySaveData(saveSetting);
                }
            }
        }
    }

    public void TriggerUnlock(string eventID, string conditionID)
    {
        UnlockEventSetting eventTemp = FindEventByID(eventID);

        if (eventTemp.IsUnlocked) return;

        foreach (var eventCondition in eventTemp.unlockConditions)
        {
            if (eventCondition.id == conditionID)
            {
                eventCondition.isUnlocked = true;
            }
        }
    }
    
    public void TryTriggerEventEffect(string eventID)
    {
        UnlockEventSetting eventTemp = FindEventByID(eventID);

        if (eventTemp.IsUnlocked) return;

        bool allConditionsCompleted = true;

        foreach (var eventCondition in eventTemp.unlockConditions)
        {
            if (!eventCondition.isUnlocked)
            {
                allConditionsCompleted = false;
                break;
            }
        }
        
        if(allConditionsCompleted)
        {
            foreach (var triggerEventStruct in eventTemp.triggerEvents)
            {
                switch (triggerEventStruct.triggerEventType)
                {
                    case TriggerEventType.TriggerDialogue:
                        DialogueSystem.Instance.EnterNewDialgue(triggerEventStruct.targetDialogue);
                        break;
                    case TriggerEventType.SwitchConclusionBtn:
                        ReportManager.Instance.SwitchConclusionBtn(triggerEventStruct.enableConclusion);
                        break;
                    case TriggerEventType.SwitchClue:
                        SceneSystemManager.Instance.SwitchClueState(triggerEventStruct.unlockConditionID,triggerEventStruct.enableConclusion);
                        break;
                }
            }

            eventTemp.IsUnlocked = true;
        }
    }

    private UnlockEventSetting FindEventByID(string eventID)
    {
        UnlockEventSetting eventTemp = null;

        foreach (var setting in eventSettings)
        {
            if(setting.id == eventID)
            {
                eventTemp = setting;
                break;
            }
        }

        return eventTemp;
    }
}
