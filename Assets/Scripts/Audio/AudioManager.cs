using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstanceList;
    private List<StudioEventEmitter> eventEmitterList;

    private EventInstance ambienceEventInstance;
    
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance is not null) Debug.LogError("More than one audio manager in the scene");
        instance = this;

        eventInstanceList = new List<EventInstance>();
        eventEmitterList = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        InitializeAmbience(FMODEvents.instance.ambience);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound,  worldPos);
    }

    public void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        
        var eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstanceList.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitterList.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        foreach (var eventInstance in eventInstanceList)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        
        foreach (var emitter in eventEmitterList)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}