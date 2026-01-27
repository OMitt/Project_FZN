using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum ScenePortalType { Left = 0, Right = 1, Top = 2, Down = 3}

[System.Serializable]
public struct ScenePortalAndTransform
{
    public ScenePortalType portalType;
    public Transform portalTransform;
}

[System.Serializable]
public struct ScenePortalAndEffect
{
    public ScenePortalType portalType;
    public List<TriggerEventStruct> triggerEvents;
}

[System.Serializable]
public struct SceneAndID
{
    public string id;
    public Transform scene;

    public List<ScenePortalAndEffect> portalAndEffects;
}

[System.Serializable]
public struct UnlockableClueAndID
{
    public string id;
    public Transform clue;
    public bool initialActive;
}

public class SceneSystemManager : Singleton<SceneSystemManager>
{
    [SerializeField]
    private List<ScenePortalAndTransform> portals = new List<ScenePortalAndTransform>();
    [SerializeField]
    private List<SceneAndID> allScenes = new List<SceneAndID>();

    private string currentSceneID = "";

    protected override void Awake()
    {
        base.Awake();

        foreach (var singleScene in allScenes)
        {
            singleScene.scene.gameObject.SetActive(false);
        }
        
        SwitchPortalBtn(false);
    }

    public void EnterTriggerScene(string id, bool DontUseBlackEffect = false)
    {
        Transform triggerScene = null;

        triggerScene = GetTargetScene(id);

        if (triggerScene != null)
        {
            if(DontUseBlackEffect)
            {
                foreach (var singleScene in allScenes)
                {
                    singleScene.scene.gameObject.SetActive(false);
                }
                
                currentSceneID = id;
                triggerScene.gameObject.SetActive(true);
                SwitchPortalBtn(true);           
            }
            else
            {
                Sequence seq = DOTween.Sequence();

                List<SceneVisualEffectType> VFXSceneEnablelack = new List<SceneVisualEffectType>();
                VFXSceneEnablelack.Add(SceneVisualEffectType.SceneChangeEnableBlack);

                List<SceneVisualEffectType> VFXSceneDisableBlack = new List<SceneVisualEffectType>();
                VFXSceneDisableBlack.Add(SceneVisualEffectType.SceneChangeDisableBlack);            

                seq.AppendCallback(()=>{ScreenVisualManager.Instance.TriggerVisualEffect(VFXSceneEnablelack);});   
                seq.AppendInterval(ScreenVisualManager.Instance.SceneChangeDelayTime);
                seq.AppendCallback(()=>
                {
                    foreach (var singleScene in allScenes)
                    {
                        singleScene.scene.gameObject.SetActive(false);
                    }                
                });   
                seq.AppendInterval(ScreenVisualManager.Instance.SceneChangeDelayTime);
                seq.AppendCallback(()=>
                {
                    currentSceneID = id;
                    triggerScene.gameObject.SetActive(true);
                    SwitchPortalBtn(true);             
                });   
                seq.AppendCallback(()=>{ScreenVisualManager.Instance.TriggerVisualEffect(VFXSceneDisableBlack);});
            }
        }
    }

    private Transform GetTargetScene(string id)
    {
        Transform triggerScene = null;

        foreach (var singleScene in allScenes)
        {
            if (singleScene.id == id)
            {
                triggerScene = singleScene.scene;

                return triggerScene;
            }
        }

        return triggerScene;
    }

    public void ChangeSceneBG(Sprite inputSprite)
    {
        Transform currentScene = GetTargetScene(currentSceneID);

        if(currentScene == null) return;

        Transform BGobject = currentScene.Find("BG");

        if(BGobject == null) return;

        if(BGobject.gameObject.TryGetComponent<UnityEngine.UI.Image>(out UnityEngine.UI.Image outImage))
        {
            outImage.sprite = inputSprite; 
        }
    }

    public void SwitchPortalBtn(bool enable)
    {
        foreach (var portal in portals)
        {
            portal.portalTransform.gameObject.SetActive(false);
        }
            
        if(enable)
        {
            foreach (var scene in allScenes)
            {
                if (scene.id == currentSceneID)
                {
                    foreach (var portalAndEffect in scene.portalAndEffects)
                    {
                        Transform portalTemp = null;
                        switch (portalAndEffect.portalType)
                        {
                            case ScenePortalType.Left:
                                portalTemp = GetPortalByType(ScenePortalType.Left);
                                SetupPortal(portalTemp, portalAndEffect.triggerEvents);
                                portalTemp.gameObject.SetActive(enable);
                                break;
                            case ScenePortalType.Right:
                                portalTemp = GetPortalByType(ScenePortalType.Right);
                                SetupPortal(portalTemp, portalAndEffect.triggerEvents);
                                portalTemp.gameObject.SetActive(enable);
                                break;
                            case ScenePortalType.Top:
                                break;
                            case ScenePortalType.Down:
                                break;
                        }
                    }

                    break;
                }
            }
        }
    }

    private void SetupPortal(Transform portal, List<TriggerEventStruct> triggerEvents)
    {
        if(portal.TryGetComponent<InteractiveUI>(out InteractiveUI tempUI))
        {
            tempUI.triggerEventStructs = triggerEvents;
        }
    }

    private Transform GetPortalByType(ScenePortalType type)
    {
        Transform temp = null;

        foreach (var portal in portals)
        {
            if (portal.portalType == type)
            {
                temp = portal.portalTransform;
                break;
            }
        }

        return temp;
    }


    [SerializeField]
    private List<UnlockableClueAndID> unlockableClues;

    #region CluesUnlock
    public void InitialChpaterClues()
    {
        foreach(var clue in unlockableClues)
        {
            clue.clue.gameObject.SetActive(clue.initialActive);
            
            InteractiveUI interactiveUI = null;
            if(clue.clue.gameObject.TryGetComponent<InteractiveUI>(out interactiveUI))
            {
                interactiveUI.enabled = true;
            }
        }
    }

    public void SwitchClueState(string id, bool enable, bool mustHide)
    {
        foreach(var clue in unlockableClues)
        {
            if(clue.id == id)
            {
                if(enable)
                {
                    clue.clue.gameObject.SetActive(enable);
                }
                else
                {
                    InteractiveUI interactiveUI = null;
                    if(clue.clue.gameObject.TryGetComponent<InteractiveUI>(out interactiveUI))
                    {
                        interactiveUI.enabled = enable;
                    }
                }

                if(mustHide)
                {
                    clue.clue.gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion
}
