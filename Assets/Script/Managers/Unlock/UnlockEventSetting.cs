using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockEventSetting", menuName = "Scriptable Objects/UnlockEventSetting")]
public class UnlockEventSetting : ScriptableObject
{
    public string id = "";
    public List<UnlockCondition> unlockConditions = new List<UnlockCondition>();
    public List<TriggerEventStruct> triggerEvents = new List<TriggerEventStruct>();

    [SerializeField]
    private bool isUnlocked = false;

    public bool IsUnlocked
    {
        get { return isUnlocked; }
        set { isUnlocked = value; }
    }

    public void CopySaveData(UnlockEventSetting from)
    {
        foreach(var unlockCondition in from.unlockConditions)
        {
            foreach(var thisUnlockCondition in unlockConditions)
            {
                if(thisUnlockCondition.id == unlockCondition.id)
                {
                    thisUnlockCondition.CopySaveDate(unlockCondition);
                }
            }
        }

        IsUnlocked = from.IsUnlocked;
    }
}

[System.Serializable]
public class UnlockCondition
{
    public string id = "";
    public bool isUnlocked = false;

    public void CopySaveDate(UnlockCondition from)
    {
        isUnlocked = from.isUnlocked;
    }
}