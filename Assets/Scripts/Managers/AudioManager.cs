using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioClips
{
    public string ID;
    public AudioClip Clip;
    public bool Loop;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClips> audioClips = new();
    private List<AudioSource> activeSources = new();

    public static event Action<string> OnPlayAudio;

    void Start()
    {
        PlayByID("VentLoop");
        PlayByID("Ambience");
        PlayByID("MusicCalm");
    }
    private void OnEnable()
    {
        OnPlayAudio += PlayByID;
    }

    private void OnDisable()
    {
        OnPlayAudio -= PlayByID;
    }

    public static void TriggerAudio(string id)
    {
        OnPlayAudio?.Invoke(id);
    }

    public void PlayByID(string id)
    {
        AudioClips? clipData = audioClips.Find(c => c.ID == id);

        if (clipData == null || clipData.Value.Clip == null)
        {
            return;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clipData.Value.Clip;
        newSource.loop = clipData.Value.Loop;
        newSource.Play();

        activeSources.Add(newSource);

        if (!newSource.loop) // only clean up if not looping
        {
            StartCoroutine(RemoveSourceWhenDone(newSource));
        }
    }

    private System.Collections.IEnumerator RemoveSourceWhenDone(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        activeSources.Remove(source);
        Destroy(source);
    }

    public void StopAudioByID(string id)
    {
        AudioClips? clipData = audioClips.Find(c => c.ID == id);

        if (clipData == null || clipData.Value.Clip == null)
        {
            Debug.LogWarning($"Chatgerpeter say: no clip with ID '{id}' found to stop!");
            return;
        }

        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            AudioSource src = activeSources[i];
            if (src.clip == clipData.Value.Clip)
            {
                src.Stop();
                Destroy(src);
                activeSources.RemoveAt(i);
            }
        }
    }
    
    public void StopAllLoopingAudio()
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            AudioSource src = activeSources[i];
            if (src.loop)
            {
                src.Stop();
                Destroy(src);
                activeSources.RemoveAt(i);
            }
        }
    }

}
