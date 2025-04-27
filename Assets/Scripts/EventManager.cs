using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EventStats
{
    public int NightsSpentGambling;
    public int DrinksHad;
    
    public bool TimerRunning;
    public float TimeElapsed;
    
    public int Winnings;
    public int BankBalance;
    public int MoneySpentOnDrinks;
    public int BiggestBet;
    public int BiggestWinnings;
    
    public int HandsPlayed;
    public int HandsWon;
    public int HandsWonWithBlackjack;
    public int HandsLost;
    public int HandsDrawn;
    public int WinningStreak;
    public int BiggestWinningStreak;
    
    public int TextsReceived;
    public int TextsRepliedTo;
    public int TextsIgnored;
    
    public void ResetStats()
    {
        var stats = GetType().GetFields();
        foreach (var stat in stats)
        {
            if (stat.FieldType == typeof(int)) stat.SetValue(this, 0);
            else if (stat.FieldType == typeof(float)) stat.SetValue(this, 0f);
            else if (stat.FieldType == typeof(bool)) stat.SetValue(this, false);
            else if (stat.FieldType == typeof(string)) stat.SetValue(this, "");
        }
    }
}

public class EventManager : MonoBehaviour
{
    public enum TriggerTiming
    {
        Instant,
        AfterHand,
        StartOfNight,
        EndOfNight
    }
    
    [Header("References")]
    [SerializeField] private BlackjackManager blackjackManager;
    [SerializeField] private PhoneManager phoneManager;
    [SerializeField] private WaiterManager waiterManager;
    [SerializeField] private GameObject eventHolder;

    public EventStats NightStats = new();
    public EventStats OverallStats = new();
    private List<EventBase> eventList;
    private NightEnded_Event nightEndedEvent;
    private Coroutine eventsCoroutine;
    
    private Queue<EventBase> instantEventsQueued = new();
    private Queue<EventBase> afterHandEventsQueued = new();
    private Queue<EventBase> startOfNightEventsQueued = new();
    private Queue<EventBase> endOfNightEventsQueued = new();
    
    private Queue<EventBase> instantEventsSimultaneous = new();
    private Queue<EventBase> afterHandEventsSimultaneous = new();
    private Queue<EventBase> startOfNightEventsSimultaneous = new();
    private Queue<EventBase> endOfNightEventsSimultaneous = new();


    private void Awake()
    {
        blackjackManager = blackjackManager ?? FindFirstObjectByType<BlackjackManager>();
        phoneManager = phoneManager ?? FindFirstObjectByType<PhoneManager>();
        waiterManager = waiterManager ?? FindFirstObjectByType<WaiterManager>();
        eventList = GetEventList();
        nightEndedEvent = eventList.OfType<NightEnded_Event>().FirstOrDefault();

        NightStats.TimerRunning = true;
        OverallStats.TimerRunning = true;
    }

    private void Update()
    {
        if (NightStats.TimerRunning)
        {
            NightStats.TimeElapsed += Time.deltaTime;
        }

        if (OverallStats.TimerRunning)
        {
            OverallStats.TimeElapsed += Time.deltaTime;
        }
    }
    
    private void OnEnable()
    {
        waiterManager.DrinkBought += OnDrinkBought;
        blackjackManager.HandStarted += OnHandStarted;
        blackjackManager.DoubledDown += OnDoubleDown;
        blackjackManager.HandEnded += OnHandEnded;
        phoneManager.BankValueChanged += OnBankValueChanged;
        nightEndedEvent.EventStarted += OnNightEnded;
        nightEndedEvent.EventFinished += OnNewNightStarted;
        
        NightStats.TimerRunning = true;
        OverallStats.TimerRunning = true;
    }

    private void OnDisable()
    {
        waiterManager.DrinkBought -= OnDrinkBought;
        blackjackManager.HandStarted -= OnHandStarted;
        blackjackManager.DoubledDown -= OnDoubleDown;
        blackjackManager.HandEnded -= OnHandEnded;
        phoneManager.BankValueChanged -= OnBankValueChanged;
        nightEndedEvent.EventStarted -= OnNightEnded;
        nightEndedEvent.EventFinished -= OnNewNightStarted;
    }

    private List<EventBase> GetEventList()
    {
        if (!eventHolder)
        {
            eventHolder = transform.GetChild(0).gameObject;
        }
        
        if (eventHolder)
        {
            return eventHolder.GetComponents<EventBase>().ToList();
        }
        
        return new List<EventBase>();
    }
    
    private void CheckEventsAndAddToQueues()
    {
        if (eventList.Count == 0) return;
        foreach (EventBase gameEvent in eventList)
        {
            // If conditions for game event aren't met, skip this loop.
            if (!gameEvent.ConditionsMet(NightStats, OverallStats)) continue;
            
            EventDetails eventDetails = gameEvent.EventInfo;
            
            // Don't add this event to queue because it's already in there.
            if (gameEvent.InQueue) continue;

            bool eventAddedToQueue = true;

            if (eventDetails.TriggerAfterCurrentEvents)
            {
                switch (eventDetails.TriggerTiming)
                {
                    case TriggerTiming.Instant :
                        instantEventsQueued.Enqueue(gameEvent);
                        break;
                    case TriggerTiming.AfterHand :
                        afterHandEventsQueued.Enqueue(gameEvent);
                        break;
                    case TriggerTiming.StartOfNight :
                        startOfNightEventsQueued.Enqueue(gameEvent);
                        break;
                    case TriggerTiming.EndOfNight :
                        endOfNightEventsQueued.Enqueue(gameEvent);
                        break;
                    default :
                        Debug.LogWarning("Trigger timing queue not implemented!");
                        eventAddedToQueue = false;
                        break;
                }
            }
            else
            {
                switch (eventDetails.TriggerTiming)
                {
                    case TriggerTiming.Instant :
                        instantEventsSimultaneous.Enqueue(gameEvent);
                        break;
                    case TriggerTiming.AfterHand :
                        afterHandEventsSimultaneous.Enqueue(gameEvent);
                        break;
                    case TriggerTiming.StartOfNight :
                        startOfNightEventsSimultaneous.Enqueue(gameEvent);
                        break;
                    case TriggerTiming.EndOfNight :
                        endOfNightEventsSimultaneous.Enqueue(gameEvent);
                        break;
                    default :
                        Debug.LogWarning("Trigger timing queue not implemented!");
                        eventAddedToQueue = false;
                        break;
                }
            }
            
            gameEvent.InQueue = eventAddedToQueue;
            StartCoroutine(TriggerEvents(instantEventsSimultaneous, false));
            StartCoroutine(TriggerEventsAfterCurrent(instantEventsQueued));
        }
    }

    private IEnumerator TriggerEventsAfterCurrent(Queue<EventBase> eventsToTrigger)
    {
        if (eventsCoroutine == null)
        {
            eventsCoroutine = StartCoroutine(TriggerEvents(eventsToTrigger, true));
        }
        else
        {
            yield return eventsCoroutine;
            yield return eventsCoroutine = StartCoroutine(TriggerEvents(eventsToTrigger, true));
            eventsCoroutine = null;
        }
    }
    
    private IEnumerator TriggerEvents(Queue<EventBase> eventsToTrigger, bool triggerOneByOne)
    {
        while (eventsToTrigger.Count > 0)
        {
            EventBase nextEvent = eventsToTrigger.Peek();
            if (triggerOneByOne)
            {
                yield return StartCoroutine(nextEvent.TriggerEventCoroutine());
            }
            else
            {
                StartCoroutine(nextEvent.TriggerEventCoroutine());
            }
            if (nextEvent.EventInfo.IsOneTimeEvent)
            {
                eventList.Remove(nextEvent);
            }
            
            // Remove event from queue and set "InQueue" to false
            eventsToTrigger.Dequeue().InQueue = false;
        }
    }
    
    private void OnNewNightStarted(EventBase nightEvent)
    {
        Debug.Log($"Night {OverallStats.NightsSpentGambling + 1} Started");
        NightStats.ResetStats();
        NightStats.TimerRunning = true;
        OverallStats.TimerRunning = true;
        
        CheckEventsAndAddToQueues();
        StartCoroutine(TriggerEvents(startOfNightEventsSimultaneous, false));
        StartCoroutine(TriggerEventsAfterCurrent(startOfNightEventsQueued));
    }
    
    private void OnNightEnded(EventBase nightEvent)
    {
        Debug.Log($"Night {OverallStats.NightsSpentGambling + 1} Ended");
        NightStats.TimerRunning = false;
        OverallStats.TimerRunning = false;
        OverallStats.NightsSpentGambling++;
        
        CheckEventsAndAddToQueues();
        StartCoroutine(TriggerEvents(endOfNightEventsSimultaneous, false));
        StartCoroutine(TriggerEventsAfterCurrent(endOfNightEventsQueued));
    }
    
    private void OnDrinkBought(int costOfDrink)
    {
        NightStats.MoneySpentOnDrinks += costOfDrink;
        OverallStats.MoneySpentOnDrinks+= costOfDrink;
        
        NightStats.DrinksHad++;
        OverallStats.DrinksHad++;
        
        CheckEventsAndAddToQueues();
    }

    private void OnHandStarted(int betAmount)
    {
        NightStats.BiggestBet = betAmount > NightStats.BiggestBet ? betAmount : NightStats.BiggestBet;
        OverallStats.BiggestBet = betAmount > OverallStats.BiggestBet ? betAmount : OverallStats.BiggestBet;
        
        CheckEventsAndAddToQueues();
    }
    
    private void OnHandEnded(BlackjackManager.EndState endState, int moneyWon)
    {
        NightStats.HandsPlayed++;
        OverallStats.HandsPlayed++;
        
        NightStats.Winnings += moneyWon;
        OverallStats.Winnings += moneyWon;
        
        NightStats.BiggestWinnings = moneyWon > NightStats.BiggestWinnings ? moneyWon : NightStats.BiggestWinnings;
        OverallStats.BiggestWinnings = moneyWon > OverallStats.BiggestWinnings ? moneyWon : OverallStats.BiggestWinnings;

        if (endState == BlackjackManager.EndState.Win || endState == BlackjackManager.EndState.NaturalWin)
        {
            NightStats.HandsWon++;
            OverallStats.HandsWon++;
            
            NightStats.WinningStreak++;
            NightStats.BiggestWinningStreak = NightStats.WinningStreak > NightStats.BiggestWinningStreak ? NightStats.WinningStreak : NightStats.BiggestWinningStreak;
            OverallStats.BiggestWinningStreak = NightStats.WinningStreak > OverallStats.BiggestWinningStreak ? NightStats.WinningStreak : OverallStats.BiggestWinningStreak;

            if (endState == BlackjackManager.EndState.NaturalWin)
            {
                NightStats.HandsWonWithBlackjack++;
                OverallStats.HandsWonWithBlackjack++;
            }
        }
        else if (endState == BlackjackManager.EndState.Draw)
        {
            NightStats.HandsDrawn++;
            OverallStats.HandsDrawn++;
        }
        else if (endState == BlackjackManager.EndState.Loss)
        {
            NightStats.HandsLost++;
            OverallStats.HandsLost++;
            
            NightStats.WinningStreak = 0;
        }
        
        CheckEventsAndAddToQueues();
        StartCoroutine(TriggerEvents(afterHandEventsSimultaneous, false));
        StartCoroutine(TriggerEventsAfterCurrent(afterHandEventsQueued));
    }
    
    // Note, functionally the same as OnHandStarted, but added in case we wanted to do something different
    private void OnDoubleDown(int betAmount)
    {
        OnHandStarted(betAmount);
    }

    //TODO - Should I keep track of some sort of overall bank balance?
    private void OnBankValueChanged(int oldValue, int newValue)
    {
        NightStats.BankBalance = newValue;
        CheckEventsAndAddToQueues();
    }
}
