using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct ChapterAndID
{
    public string id;
    public GameChapter chapter;
}

public class GameLoopManager : Singleton<GameLoopManager>
{
    [SerializeField]
    private List<ChapterAndID> allChapters = new List<ChapterAndID>();

    [SerializeField]
    private string startChapterID;

    private string savePointChapterID;

    void Start()
    {
        InteractiveUI.clickChirsCount = 0;
        EnterTriggerChapter(startChapterID);
    }

    void Update()
    {
        //TEST
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void EnterTriggerChapter(string id)
    {
        GameChapter triggerChapter = null;

        foreach (var singleChapter in allChapters)
        {
            singleChapter.chapter.CompletethisChapter();

            if (singleChapter.id == id)
            {
                triggerChapter = singleChapter.chapter;
            }
        }

        if (triggerChapter != null)
        {
            triggerChapter.enabled = true;

            if(id != "BadEnd")
            {
                savePointChapterID = id;
                UnlockManager.Instance.SaveEventSettings();
            }
        }
    }

    public void EnterSavePointChapter()
    {
        Debug.Log("EnterSavePointChapter"+savePointChapterID);
        UnlockManager.Instance.LoadEventSettings();
        SceneSystemManager.Instance.InitialChpaterClues();
        EnterTriggerChapter(savePointChapterID);
    }
}
