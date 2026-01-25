using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectionGroup", menuName = "Scriptable Objects/SelectionGroup")]
public class SelectionGroup : ScriptableObject
{
    public List<Selection> selections;
    public bool isConclusion;
}

[System.Serializable]
public class Selection
{
    [TextArea(3, 3)]
    public string text;

    [Header("TriggerEventAfterCompleting")]
    public List<TriggerEventStruct> triggerEventStructs;
}