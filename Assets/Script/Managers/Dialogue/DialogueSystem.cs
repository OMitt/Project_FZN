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
    private GraphicRaycaster graphicraycaster;
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
    private float typeSpeed = 0.03f;

    [Header("Speaker Portraits")]
    [SerializeField]
    private Sprite narratorPortrait;
    [SerializeField]
    private Sprite youPortrait;
    [SerializeField]
    private Sprite assistantPortrait;
    [SerializeField]
    private Sprite broadcastPortrait;

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
        graphicraycaster.enabled = enable;
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
        if (CanClick && graphicraycaster.enabled && Input.GetMouseButtonDown(0))
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
        portrait.sprite = GetPortrait(dialogue.speaker);

        //visual trigger switch image here.

        HandleTrigger(dialogue.triggerEvents);
        SoundManager.Instance.PlayDialogueSFX("TextScrolling_0");
        StartCoroutine(PlayDialogueSounds(dialogue.sounds));

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(dialogue.text));

        Sprite GetPortrait(Speaker s)
        {
            switch (s)
            {
                case Speaker.John: return youPortrait;
                case Speaker.Christina: return assistantPortrait;
                case Speaker.Broadcast: return broadcastPortrait;
                case Speaker.Narrator:
                default: return narratorPortrait;
            }
        }

        IEnumerator TypeText(string text)
        {
            isTyping = true;
            canContinue = false;
            T_Content.text = "";

            foreach (char c in text)
            {
                T_Content.text += c;
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
                //����һ��audiosource������Ϻ��Զ�ɾ����
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

