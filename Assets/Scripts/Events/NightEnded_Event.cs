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
        bool playedTooManyHands = nightStats.HandsPlayed >= 15 + Mathf.Floor(nightStats.HandsWon / 2f);
        bool notEnoughMoneyForMinimumBet = (nightStats.BankBalance < blackjackManager.MinimumBet) && nightStats.HandsPlayed > 0;
        bool hadTooMuchToDrink = nightStats.DrinksHad >= 5;

        if (playedTooManyHands || notEnoughMoneyForMinimumBet || hadTooMuchToDrink)
        {
            // Shows up on the third night, then goes back to "end of night" so people can keep playing
            if (overallStats.NightsSpentGambling == 3)
            {
                EventInfo.EventTitleText = $"Game Over";

                int minutesPlayed = (int) Mathf.Floor(overallStats.TimeElapsed / 60f);
                int secondsPlayed = (int) Mathf.Floor(overallStats.TimeElapsed % 60f);
                string formattedTime = string.Format("{0:00}:{1:00}", minutesPlayed, secondsPlayed);

                EventInfo.textVisibleForDuration = 5f;
                
                string statsA = $"Time Played: {formattedTime}\n" +
                               $"Total Hands: {overallStats.HandsPlayed}\n" +
                               $"Drinks Had: {overallStats.DrinksHad}";


                string statsB = $"Biggest Streak: {overallStats.BiggestWinningStreak}\n" +
                                $"Total Winnings: ${overallStats.Winnings}";
                
                string keepPlaying = $"You are more than welcome to keep playing!";
                
                EventInfo.EventBodyText = new string[3];
                
                EventInfo.EventBodyText[0] = statsA;
                EventInfo.EventBodyText[1] = statsB;
                EventInfo.EventBodyText[2] = keepPlaying;
            }
            
            else
            {
                EventInfo.EventTitleText = $"End of Night {overallStats.NightsSpentGambling}";
                EventInfo.textVisibleForDuration = 3f;
                
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
            }
            
            return true;
        }

        return false;
    }
}
