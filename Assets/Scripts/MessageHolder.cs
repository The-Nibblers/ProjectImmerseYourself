using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


[CreateAssetMenu(fileName = "NewMessageHolder", menuName = "MessageHolder", order = 1)]
public class MessageHolder : ScriptableObject
{
    [Serializable]
    public struct VideoEntry
    {
        public string key;
        public VideoClip clip;
    }

    public List<VideoEntry> videoList = new List<VideoEntry>();
}