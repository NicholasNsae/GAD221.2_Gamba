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
        bool playedTooManyHands = nightStats.HandsPlayed >= 10 + Mathf.Floor(nightStats.HandsWon / 2f);
        bool notEnoughMoneyForMinimumBet = nightStats.BankBalance < blackjackManager.MinimumBet;
        bool hadTooMuchToDrink = nightStats.DrinksHad >= 5;

        if (playedTooManyHands || notEnoughMoneyForMinimumBet || hadTooMuchToDrink)
        {
            // if (overallStats.NightsSpentGambling == 3)
            // {
            //     EventInfo.EventTitleText = $"End of Night {overallStats.NightsSpentGambling}";
            // }
            // else
            // {
                EventInfo.EventTitleText = $"End of Night {overallStats.NightsSpentGambling}";

                if (playedTooManyHands)
                {
                    EventInfo.EventBodyText = new string[2];
                    EventInfo.EventBodyText[0] = "The night is getting late...";
                    EventInfo.EventBodyText[1] = "Time to finish up";
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
            // }
            
            return true;
        }

        return false;
    }
}
