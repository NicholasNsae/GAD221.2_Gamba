using System.Collections;
using UnityEngine;

public class NightEnded_Event : EventBase
{
    [SerializeField] BlackjackManager blackjackManager;

    private void Awake()
    {
        blackjackManager = blackjackManager ?? FindFirstObjectByType<BlackjackManager>();
    }

    public override bool ConditionsMet(EventStats nightStats, EventStats overallStats)
    {
        if (overallStats.NightsSpentGambling >= 3)
        {
            return false;
        }

        bool lostTooManyHands = nightStats.HandsLost >= 5 + Mathf.Floor(nightStats.HandsWon / 2f);
        bool notEnoughMoneyForMinimumBet = nightStats.BankBalance < blackjackManager.MinimumBet;
        bool hadTooMuchToDrink = nightStats.DrinksHad >= 5;

        if (lostTooManyHands || notEnoughMoneyForMinimumBet || hadTooMuchToDrink)
        {
            EventInfo.EventTitleText = $"End of Night {overallStats.NightsSpentGambling}";

            if (lostTooManyHands)
            {
                EventInfo.EventBodyText = new string[2];
                EventInfo.EventBodyText[0] = "You've had a bit of a losing streak...";
                EventInfo.EventBodyText[1] = "That means you're due for a winning streak!";
            }
            else if (notEnoughMoneyForMinimumBet)
            {
                EventInfo.EventBodyText = new string[2];
                EventInfo.EventBodyText[0] = "You've run out of money for tonight.";
                EventInfo.EventBodyText[1] = "It's only up from here!";
            }
            else if (hadTooMuchToDrink)
            {
                EventInfo.EventBodyText = new string[1];
                EventInfo.EventBodyText[0] = "You've passed out...";
            }
            
            return true;
        }

        return false;
    }
}
