using System;
using UnityEngine;

public struct EventDetails
{
    public string EventTitle;
    public string EventDescription;
    public string[] EventText;
    public EventStats EventStats;
}

public class EventStats
{
    public int DaysSpentGambling;
    public int DrinksHad;
    
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
    [Header("References")]
    [SerializeField] private BlackjackManager blackjackManager;
    [SerializeField] private PhoneManager phoneManager;
    [SerializeField] private WaiterManager waiterManager;

    private EventStats nightStats = new();
    private EventStats overallStats = new();
    
    // Events
    public event Action<EventDetails> FadeOutEvent;
    // public event Action<EventDetails> PhoneTextReceivedEvent;
    // public event Action<EventDetails> PhoneVoicemailLeftEvent;
    // ...
    
    private void Awake()
    {
        blackjackManager = blackjackManager ?? FindFirstObjectByType<BlackjackManager>();
        phoneManager = phoneManager ?? FindFirstObjectByType<PhoneManager>();
        waiterManager = waiterManager ?? FindFirstObjectByType<WaiterManager>();
    }

    private void OnEnable()
    {
        waiterManager.DrinkBought += OnDrinkBought;
        blackjackManager.HandStarted += OnHandStarted;
        blackjackManager.DoubledDown += OnDoubleDown;
        blackjackManager.HandEnded += OnHandEnded;
        phoneManager.BankValueChanged += OnBankValueChanged;
    }

    private void OnDisable()
    {
        waiterManager.DrinkBought -= OnDrinkBought;
        blackjackManager.HandStarted -= OnHandStarted;
        blackjackManager.DoubledDown -= OnDoubleDown;
        blackjackManager.HandEnded -= OnHandEnded;
        phoneManager.BankValueChanged -= OnBankValueChanged;
    }

    // TODO - Need Nick's input for when to trigger events...
    private void CheckForGameEvents()
    {
        throw new NotImplementedException();
        // When to EndNight()?
    }

    // TODO - make a separate event?
    private void EndNight()
    {
        throw new NotImplementedException();
        // Do something
        
        ResetNightStats();
    }

    public void ResetNightStats()
    {
        nightStats.ResetStats();
    }
    
    private void OnDrinkBought(int costOfDrink)
    {
        nightStats.MoneySpentOnDrinks += costOfDrink;
        overallStats.MoneySpentOnDrinks+= costOfDrink;
        
        nightStats.DrinksHad++;
        overallStats.DrinksHad++;
        
        CheckForGameEvents();
    }

    private void OnHandStarted(int betAmount)
    {
        nightStats.BiggestBet = betAmount > nightStats.BiggestBet ? betAmount : nightStats.BiggestBet;
        overallStats.BiggestBet = betAmount > overallStats.BiggestBet ? betAmount : overallStats.BiggestBet;
        
        CheckForGameEvents();
    }
    
    private void OnHandEnded(BlackjackManager.EndState endState, int moneyWon)
    {
        nightStats.HandsPlayed++;
        overallStats.HandsPlayed++;
        
        nightStats.Winnings += moneyWon;
        overallStats.Winnings += moneyWon;
        
        nightStats.BiggestWinnings = moneyWon > nightStats.BiggestWinnings ? moneyWon : nightStats.BiggestWinnings;
        overallStats.BiggestWinnings = moneyWon > overallStats.BiggestWinnings ? moneyWon : overallStats.BiggestWinnings;

        if (endState == BlackjackManager.EndState.Win || endState == BlackjackManager.EndState.NaturalWin)
        {
            nightStats.HandsWon++;
            overallStats.HandsWon++;
            
            nightStats.WinningStreak++;
            nightStats.BiggestWinningStreak = nightStats.WinningStreak > nightStats.BiggestWinningStreak ? nightStats.WinningStreak : nightStats.BiggestWinningStreak;
            overallStats.BiggestWinningStreak = nightStats.WinningStreak > overallStats.BiggestWinningStreak ? nightStats.WinningStreak : overallStats.BiggestWinningStreak;

            if (endState == BlackjackManager.EndState.NaturalWin)
            {
                nightStats.HandsWonWithBlackjack++;
                overallStats.HandsWonWithBlackjack++;
            }
        }
        else if (endState == BlackjackManager.EndState.Draw)
        {
            nightStats.HandsDrawn++;
            overallStats.HandsDrawn++;
        }
        else if (endState == BlackjackManager.EndState.Loss)
        {
            nightStats.HandsLost++;
            overallStats.HandsLost++;
            
            nightStats.WinningStreak = 0;
        }
        
        CheckForGameEvents();
    }
    
    // Note, functionally the same as OnHandStarted, but added in case we wanted to do something different
    private void OnDoubleDown(int betAmount)
    {
        OnHandStarted(betAmount);
    }

    //TODO - Should I keep track of some sort of overall bank balance?
    private void OnBankValueChanged(int oldValue, int newValue)
    {
        nightStats.BankBalance = newValue;
        CheckForGameEvents();
    }
}
