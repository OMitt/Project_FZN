using UnityEngine;

[System.Serializable]
public class GameChapter : MonoBehaviour
{
    void Awake()
    {
        this.enabled = false;
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnEnable()
    {
        TriggerOnStart();
    }

    protected virtual void TriggerOnStart()
    {
    }

    public void CompletethisChapter()
    {
        this.enabled = false;
    }
}
