using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct AudioResourceAndID
{
    public string id;
    public AudioResource audioResource;
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource bgmAudioSource;
    [SerializeField]
    private AudioSource sfxAudioSource;
    [SerializeField]
    private AudioSource ambientAudioSource;
    [SerializeField]
    private AudioSource dialogueSfxAudioSource;

    [SerializeField]
    private AudioSource clickingSource;

    [SerializeField]
    private List<AudioResourceAndID> bgmResoures;
    [SerializeField]
    private List<AudioResourceAndID> sfxResources;
    [SerializeField]
    private List<AudioResourceAndID> ambientResources;
    [SerializeField]
    private List<AudioClip> mouseSource;

    protected override void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        base.Awake();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PlayClick(0);
    }

    [SerializeField]
    private string startBGM;

    void Start()
    {
        SwitchBGM(startBGM,true);
    }

    public void SwitchBGM(string targetId = "", bool enable = true)
    {
        if(!enable)
        {
            bgmAudioSource.Stop();
        }
        else
        {
            AudioResource temp = (targetId == "")?bgmAudioSource.resource:FindResourceByIDInAssignedList(bgmResoures,targetId);

            if(temp != null)
            {
                bgmAudioSource.Stop();
                bgmAudioSource.resource = temp;
                bgmAudioSource.Play();
            }
        }
    }

    public void SwitchAmbient(string targetId = "", bool enable = true)
    {
        if(!enable)
        {
            ambientAudioSource.Stop();
        }
        else
        {
            AudioResource temp = (targetId == "")?ambientAudioSource.resource:FindResourceByIDInAssignedList(ambientResources,targetId);

            if(temp != null)
            {
                ambientAudioSource.Stop();
                ambientAudioSource.resource = temp;
                ambientAudioSource.Play();
            }
        }
    }

    public void PlaySFX(string targetId)
    {
        AudioResource temp = FindResourceByIDInAssignedList(sfxResources,targetId);

        if(temp != null)
        {
            sfxAudioSource.Stop();
            sfxAudioSource.resource = temp;
            sfxAudioSource.Play();
        }
    }

    public void PlayDialogueSFX(string targetId)
    {
        AudioResource temp = FindResourceByIDInAssignedList(sfxResources,targetId);

        if(temp != null)
        {
            dialogueSfxAudioSource.Stop();
            dialogueSfxAudioSource.resource = temp;
            dialogueSfxAudioSource.Play();
        }
    }
    public void PlayClick(int index)
    {
        Debug.Log("!");
        clickingSource.Stop();
        clickingSource.resource = mouseSource[index];
        clickingSource.Play();
    }

    private AudioResource FindResourceByIDInAssignedList(List<AudioResourceAndID> targetlist, string targetid)
    {
        AudioResource temp = null;

        foreach(var audioSourceAndId in targetlist)
        {
            if(audioSourceAndId.id == targetid)
            {
                temp = audioSourceAndId.audioResource;
                return temp;
            }
        }

        return temp; 
    }
}
