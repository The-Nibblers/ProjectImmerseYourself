using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private MessageHolder messageHolder;
    [SerializeField] private VideoPlayer videoPlayer;

    public void PlayVideo(string MessageID)
    {
        foreach (MessageHolder.VideoEntry key in messageHolder.videoList)
        {
            if (key.key == MessageID)
            {
                videoPlayer.clip = null;
                videoPlayer.clip = key.clip;
                videoPlayer.Play();
                return;
            }
        }
            Debug.Log("Key not found!");
    }

    public void pauseVideo()
    {
        if (!videoPlayer.isPaused)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }
    
    public void StopVideo()
    {
        videoPlayer.Stop();
        videoPlayer.clip = null;
    }
    
}
