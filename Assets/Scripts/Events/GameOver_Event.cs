using System;
using UnityEngine;

public class GameOver_Event : EventBase
{
    public override bool ConditionsMet(EventStats nightStats, EventStats overallStats)
    {   
        if (overallStats.NightsSpentGambling >= 3)
        {
            EventInfo.EventBodyText = new string[1];

            //Mathf.Floor(overallStats.TimeElapsed / 3600f) + ":" +
            //Mathf.Floor((overallStats.TimeElapsed / 60f) % 60f) + ":" +
            //overallStats.TimeElapsed % 60f

            string bodyA = $"Time Played: {overallStats.TimeElapsed.ToString("mm:ss")}\n" +
                $"Total Hands: {overallStats.HandsPlayed}\n" +
                $"Total Winnings: {overallStats.Winnings}\n" +
                $"Biggest Winning Streak: {overallStats.BiggestWinningStreak}";

            EventInfo.EventBodyText[0] = bodyA;

            return true;
        }

        return false;
    }
}
