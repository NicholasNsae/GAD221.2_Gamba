using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class EventDetails
{
    [Header("Event Data (only fill in what is required)")]
    public string EventTitleText;
    public string[] EventBodyText;

    [Header("FadeToBlackSettings")]
    public float timeBeforeFadeStarts;
    public float imageFadeInAndOutDuration;
    public float textFadeInAndOutDuration;
    public float noTextStayFadedDuration;
    public float textVisibleForDuration;

    [Header("Event Settings")]
    public EventBase.EventType EventType;
    public EventManager.TriggerTiming TriggerTiming;
    public bool TriggerAfterCurrentEvents;
    public bool IsOneTimeEvent;
    
    // Not viewable in inspector
    public EventStats EventStats;
}

public abstract class EventBase : MonoBehaviour
{
    public enum EventType
    {
        FadeToBlackWithText
    }

    public bool InQueue;
    
    [SerializeField] protected FadeToBlack fadeToBlack;
    
    [Header("Event Settings")]
    public EventDetails EventInfo;

    public event Action<EventBase> EventStarted;
    public event Action<EventBase> EventFinished;

    private void Start()
    {
        fadeToBlack = FindFirstObjectByType<FadeToBlack>(FindObjectsInactive.Include);
    }
    
    public virtual IEnumerator TriggerEventCoroutine()
    {
        if (EventInfo.EventType == EventType.FadeToBlackWithText)
        {
            if (!fadeToBlack.CurrentlyFading)
            {
                EventStarted?.Invoke(this);
                yield return StartCoroutine(fadeToBlack.FadeToBlackCoroutine(EventInfo));
                EventFinished?.Invoke(this);
            }
        }
        
        // else if (EventInfo.EventType == EventType.PhoneNotification)
        // {
        //     throw new NotImplementedException();
        // }
    }
    
    public abstract bool ConditionsMet(EventStats nightStats, EventStats overallStats);
}