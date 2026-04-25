using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstanceList;
    
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance is not null) Debug.LogError("More than one audio manager in the scene");
        instance = this;

        eventInstanceList = new List<EventInstance>();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound,  worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        
        var eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstanceList.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        foreach (var eventInstance in eventInstanceList)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}