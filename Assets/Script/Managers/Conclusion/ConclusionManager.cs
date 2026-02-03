using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ConclusionManager : Singleton<ConclusionManager>
{
    [Header("UI References")]
    [SerializeField]
    private GraphicRaycaster graphicraycaster;
    [SerializeField]
    private Transform conclusionList;
    [SerializeField]
    private GameObject selectionPrefab;
    [SerializeField]
    private Transform conclusionTitle;

    private SelectionGroup currentConclusionSelection;

    public SelectionGroup CurrentSelection
    {
        set { currentConclusionSelection = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        SetUp();
    }

    public void SetUp()
    {
        SwitchConclusion(false);
    }

    public void SwitchChapterConclusion(bool enable)
    {
        SwitchConclusion(enable, currentConclusionSelection);
    }

    public void SwitchConclusion(bool enable, SelectionGroup targetSelectionGroup = null)
    {

        conclusionTitle.gameObject.SetActive((targetSelectionGroup!= null && targetSelectionGroup.isConclusion)?enable:false);

        conclusionList.gameObject.SetActive(enable);
        graphicraycaster.enabled = enable;


        ClearList();
        if (targetSelectionGroup != null)
        {
            foreach (var selection in targetSelectionGroup.selections)
            {
                GameObject sel = Instantiate(selectionPrefab);
                sel.transform.SetParent(conclusionList, false);
                sel.transform.localScale = Vector3.one;
                RectTransform rt = sel.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;

                if (sel.TryGetComponent<InteractiveUI>(out InteractiveUI ui))
                {
                    ui.triggerEventStructs = selection.triggerEventStructs;
                }

                Transform textObj = sel.transform.Find("Text");
                if (textObj != null && textObj.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI text))
                {
                    text.text = selection.text;
                }
            }
        }
    }
    
    private void ClearList()
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < conclusionList.childCount; i++)
        {
            children.Add(conclusionList.GetChild(i));
        }

        foreach (var child in children)
        {
            Destroy(child.gameObject);
        }

        children.Clear();
    }
}
