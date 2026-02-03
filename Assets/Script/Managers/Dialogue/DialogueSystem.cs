using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class DialogueSystem : Singleton<DialogueSystem>
{
    [Header("UI References")]
    [SerializeField]
    private Transform blackGound;
    [SerializeField]
    private GameObject chatbox;
    [SerializeField]
    private TMP_Text T_Speaker;
    [SerializeField]
    private TMP_Text T_Content;
    [SerializeField]
    private Image portrait;

    [Header("Typing Settings")]
    [SerializeField]
    private float typeSpeed = 1f;

    [Header("Speaker Portraits")]
    [SerializeField]
    private Sprite narratorPortrait;
    [SerializeField]
    private Sprite[] youPortrait;
    [SerializeField]
    private Sprite[] assistantPortrait;
    [SerializeField]
    private Sprite broadcastPortrait;
    Coroutine blinkRoutine;
    Speaker currentSpeaker;

    private DialogueGroup currentDialogue;
    private int dialogueIndex;
    private bool isTyping = false;
    private bool canContinue = false;
    private Coroutine typingCoroutine;
    private bool CanClick = true;

    [SerializeField]
    private GameObject forbidInputBox;
    [SerializeField]
    private float additionEnterDelay = 0.06f;
    private bool canClickOnDialogue = true;
    protected override void Awake()
    {
        base.Awake();
        SetUp();
    }

    public void SetUp()
    {
        dialogueIndex = 0;
        SwitchForbidInputBox(false);
        SwitchChatbox(false);
        SwitchCanClick(false);
    }

    private void SwitchForbidInputBox(bool enable)
    {
        forbidInputBox.SetActive(enable);
        canClickOnDialogue = enable;
    }

    private void SwitchChatbox(bool enable)
    {
        chatbox.SetActive(enable);
    }

    public void SwitchCanClick(bool enable)
    {
        CanClick = enable;
    }

    private void Update()
    {
        if (CanClick && canClickOnDialogue && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                T_Content.text = currentDialogue.dialogues[dialogueIndex].text;
                isTyping = false;
                canContinue = true;
            }
            else if (canContinue)
            {
                NextDialogue();
            }
        }
    }

    public void EnterNewDialgue(DialogueGroup newDialogue, float enterDelay = 0.0f)
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(()=>{
            dialogueIndex = 0;
            currentDialogue = newDialogue;
            SwitchForbidInputBox(true);
            SwitchCanClick(false);});

        seq.AppendInterval(enterDelay+additionEnterDelay);

        seq.AppendCallback(()=>{        
            SwitchChatbox(true);
            SwitchCanClick(true);
            ShowDialogue();});   
        
        seq.Play();
    }
    private void NextDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex >= currentDialogue.dialogues.Count)
        {
            if (currentDialogue.nextDialogue != null)
            {
                EnterNewDialgue(currentDialogue.nextDialogue);
            }
            else
            {
                SwitchForbidInputBox(false);
                SwitchChatbox(false);
                SwitchCanClick(false);
                CompleteEvent();
            }
        }
        else
        {
            ShowDialogue();
        }
    }


    private void ShowDialogue()
    {
        if (currentDialogue == null || currentDialogue.dialogues.Count == 0)
            return;

        Dialogue dialogue = currentDialogue.dialogues[dialogueIndex];

        T_Speaker.text = dialogue.speaker.ToString();
        SetPortrait(dialogue.speaker);

        Sprite GetPortrait(Speaker s, int index)
        {
            switch (s)
            {
                case Speaker.John:
                    return youPortrait.Length > index ? youPortrait[index] : youPortrait[0];

                case Speaker.Christina:
                    return assistantPortrait.Length > index ? assistantPortrait[index] : assistantPortrait[0];

                case Speaker.Broadcast:
                    return broadcastPortrait;

                case Speaker.Narrator:
                default:
                    return narratorPortrait;
            }
        }
        void SetPortrait(Speaker speaker)
        {
            currentSpeaker = speaker;
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
                blinkRoutine = null;
            }
            portrait.sprite = GetPortrait(speaker, 0);
            if (HasBlink(speaker))
            {
                blinkRoutine = StartCoroutine(BlinkCoroutine(speaker));
            }
        }
        IEnumerator BlinkCoroutine(Speaker speaker)
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                // 防止切人后误触发
                if (speaker != currentSpeaker)
                    yield break;

                // 切到眨眼
                portrait.sprite = GetPortrait(speaker, 1);
                yield return new WaitForSeconds(0.12f);

                // 切回正常
                portrait.sprite = GetPortrait(speaker, 0);
                yield return new WaitForSeconds(1f);
            }
        }
        bool HasBlink(Speaker s)
        {
            return (s == Speaker.John && youPortrait.Length > 1)
                || (s == Speaker.Christina && assistantPortrait.Length > 1);
        }
        HandleTrigger(dialogue.triggerEvents);
        SoundManager.Instance.PlayDialogueSFX("TextScrolling_0");
        StartCoroutine(PlayDialogueSounds(dialogue.sounds));

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(dialogue.text));


        IEnumerator TypeText(string text)
        {
            isTyping = true;
            canContinue = false;
            T_Content.text = "";

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '<')
                {
                    int tagEnd = text.IndexOf('>', i);
                    if (tagEnd != -1)
                    {
                        T_Content.text += text.Substring(i, tagEnd - i + 1);
                        i = tagEnd;
                        continue;
                    }
                }

                T_Content.text += text[i];
                yield return new WaitForSeconds(typeSpeed);
            }

            isTyping = false;
            canContinue = true;
        }

        IEnumerator PlayDialogueSounds(List<DialogueSound> sounds)
        {
            foreach (var s in sounds)
            {
                yield return new WaitForSeconds(s.delay);

            }
        }
    }

    private void HandleTrigger(List<TriggerEventStruct> triggerEvents)
    {
        foreach(var triggerEvent in triggerEvents)
        {
            switch (triggerEvent.triggerEventType)
            {
                case TriggerEventType.None:
                    break;
                case TriggerEventType.VisualEffect:
                    ScreenVisualManager.Instance.TriggerVisualEffect(triggerEvent.VFXs);
                    break;
                case TriggerEventType.SwitchReport:
                    ReportManager.Instance.SwitchReport(triggerEvent.enableReport);
                    break;
                case TriggerEventType.SwitchReportBtn:
                    ReportManager.Instance.SwitchBtn(triggerEvent.enableReport);
                    break;
                case TriggerEventType.ChangeScene:
                    SceneSystemManager.Instance.EnterTriggerScene(triggerEvent.targetSceneID);
                    break;
                case TriggerEventType.ChangeSceneBG:
                    SceneSystemManager.Instance.ChangeSceneBG(triggerEvent.targetSprite);
                    break;
                case TriggerEventType.TriggerUnlock:
                    UnlockManager.Instance.TriggerUnlock(triggerEvent.unlockEventID,triggerEvent.unlockConditionID);
                    break;
                case TriggerEventType.TryTriggerUnlockEffect:
                    UnlockManager.Instance.TryTriggerEventEffect(triggerEvent.unlockEventID);
                    break;
                case TriggerEventType.PlaySFX:
                    SoundManager.Instance.PlaySFX(triggerEvent.sfxID);
                    break;
                case TriggerEventType.SwitchAmbient:
                    SoundManager.Instance.SwitchAmbient(triggerEvent.sfxID,triggerEvent.enableAmbient);
                    break;
                case TriggerEventType.SwitchClue:
                    SceneSystemManager.Instance.SwitchClueState(triggerEvent.unlockConditionID,triggerEvent.enableConclusion,triggerEvent.enableSelection);
                    break;

                case TriggerEventType.SwitchConclusionBtn:
                    ReportManager.Instance.SwitchConclusionBtn(triggerEvent.enableConclusion);
                    break;

                case TriggerEventType.Backup:
                    GameObject.FindGameObjectWithTag("Trigger01").GetComponent<Animator>().enabled = true;
                    break;
            } 
        }
    }

    private void CompleteEvent()
    {
        foreach(var triggerEventStruct in currentDialogue.triggerEventStructs)
        {
            TriggerEventStruct temp = triggerEventStruct;

            switch (temp.triggerEventType)
            {
                case TriggerEventType.None:
                    break;
                case TriggerEventType.SwitchChapter:
                    GameLoopManager.Instance.EnterTriggerChapter(temp.nextChapterID);
                    break;
                case TriggerEventType.SwitchSelection:
                    ConclusionManager.Instance.SwitchConclusion(temp.enableSelection, temp.selections);
                    break;
                case TriggerEventType.SwitchPortal:
                    SceneSystemManager.Instance.SwitchPortalBtn(temp.enablePortals);
                    break;
                case TriggerEventType.TryTriggerUnlockEffect:
                    UnlockManager.Instance.TryTriggerEventEffect(temp.unlockEventID);
                    break;
                case TriggerEventType.SetCurrentConclusionSelection:
                    ConclusionManager.Instance.CurrentSelection = temp.selections;
                    break;
                case TriggerEventType.ChangeScene:
                    SceneSystemManager.Instance.EnterTriggerScene(temp.targetSceneID);
                    break;
                case TriggerEventType.OpenLevel:
                    //TEST
                    Sequence seq = DOTween.Sequence();
                    seq.AppendCallback(()=>{ScreenVisualManager.Instance.TriggerVisualEffect(new List<SceneVisualEffectType>{SceneVisualEffectType.SceneChangeEnableBlack});});   
                    seq.AppendInterval(ScreenVisualManager.Instance.SceneChangeDelayTime);
                    seq.AppendCallback(()=>{SceneManager.LoadScene(temp.targetLevelIndex);});   
                    break;
                case TriggerEventType.PlaySFX:
                    SoundManager.Instance.PlaySFX(temp.sfxID);
                    break;
                case TriggerEventType.SwitchAmbient:
                    SoundManager.Instance.SwitchAmbient(temp.sfxID,temp.enableAmbient);
                    break;
            }  
        }
    }
}

