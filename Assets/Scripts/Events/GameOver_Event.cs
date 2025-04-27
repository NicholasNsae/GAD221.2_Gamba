using System;
using UnityEngine;

public class GameOver_Event : NightEnded_Event
{
    public override bool ConditionsMet(EventStats nightStats, EventStats overallStats)
    {   
        if (overallStats.NightsSpentGambling >= 3)
        {
            //Mathf.Floor(overallStats.TimeElapsed / 3600f) + ":" +
            //Mathf.Floor((overallStats.TimeElapsed / 60f) % 60f) + ":" +
            //overallStats.TimeElapsed % 60f

            string bodyA = $"Time Played: {overallStats.TimeElapsed.ToString("mm:ss")}\n" +
                           $"Total Hands: {overallStats.HandsPlayed}\n" +
                           $"Biggest Winning Streak: {overallStats.BiggestWinningStreak}" +
                           $"Total Winnings: ${overallStats.Winnings}\n" +
                           $"Drinks Had: {overallStats.DrinksHad}";
            
            EventInfo.EventBodyText = new string[1];
            EventInfo.EventBodyText[0] = bodyA;

            return true;
        }

        return false;
    }
}
