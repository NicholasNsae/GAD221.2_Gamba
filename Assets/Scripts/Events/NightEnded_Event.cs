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

        bool lostTooManyHands = nightStats.HandsLost >= 3 + Mathf.Floor(nightStats.HandsWon / 2f);
        bool notEnoughMoneyForMinimumBet = nightStats.BankBalance < blackjackManager.MinimumBet;

        if (lostTooManyHands || notEnoughMoneyForMinimumBet)
        {
            EventInfo.EventTitleText = $"End of Night {overallStats.NightsSpentGambling}";

            if (lostTooManyHands)
            {
                EventInfo.EventBodyText = new string[2];
                EventInfo.EventBodyText[0] = "You lost one too many games tonight...";
                EventInfo.EventBodyText[1] = "Maybe you'll have better luck next time!";
            }
            else if (notEnoughMoneyForMinimumBet)
            {
                EventInfo.EventBodyText = new string[2];
                EventInfo.EventBodyText[0] = "Tonight wasn't your night...";
                EventInfo.EventBodyText[1] = "That can only mean you're due for a win next time!";
            }
            
            return true;
        }

        return false;
    }
}
