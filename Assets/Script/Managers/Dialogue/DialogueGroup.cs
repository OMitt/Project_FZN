using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueGroup", menuName = "Scriptable Objects/DialogueGroup")]
public class DialogueGroup : ScriptableObject
{
    public DialogueGroup nextDialogue;
    public List<Dialogue> dialogues;

    [Header("TriggerEventAfterCompleting")]
    public List<TriggerEventStruct> triggerEventStructs;
}
public enum Speaker { Narrator = 0, John = 1, Christina = 2, Broadcast = 3 }
public enum TriggerEventType
{
    None = 0, SwitchChapter = 1, SwitchReport = 2, SwitchSelection = 3,
    ChangeScene = 4, TriggerDialogue = 5, VisualEffect = 6, SwitchPortal = 7,
    TriggerUnlock = 8, TryTriggerUnlockEffect = 9, SwitchConclusionBtn = 10,
    SetCurrentConclusionSelection = 11, SwitchConclusionSelection = 12,
    SwitchReportBtn = 13, OpenLevel = 14, QuitGame = 15, ChangeSceneBG = 16,
    SwitchClue = 17, EnterSavePointChapter = 18, PlaySFX = 19, SwitchAmbient = 20, EnterCredit = 21,
    Backup = 22,
}

[System.Serializable]
public struct TriggerEventStruct
{
    public TriggerEventType triggerEventType;
    public string nextChapterID;
    public bool enableReport;
    public bool enableSelection;
    public SelectionGroup selections;
    public string targetSceneID;
    public DialogueGroup targetDialogue;
    public List<SceneVisualEffectType> VFXs;
    public bool enablePortals;
    public string unlockEventID;
    public string unlockConditionID;
    public bool enableConclusion;
    public int targetLevelIndex;
    public Sprite targetSprite;
    public bool DontUseBlackEffect;
    public string sfxID;
    public bool enableAmbient;
    public float enterDialgueDelay;
}

[System.Serializable]
public class Dialogue
{
    public Speaker speaker;
    [TextArea(3, 3)]
    public string text;
    public List<TriggerEventStruct> triggerEvents;
    public List<DialogueSound> sounds = new List<DialogueSound>();
}
[System.Serializable]
public class DialogueSound
{
    public float delay;
    public AudioClip sound;
}