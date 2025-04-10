using System.Collections;

public class NightEnded_Event : EventBase
{
    public override bool ConditionsMet(EventStats nightStats, EventStats overallStats)
    {
        bool conditionsMet = false;
        
        //TODO! - Temporary trigger. At the moment triggers if 3 hands were played.
        if (nightStats.HandsPlayed >= 3)
        {
            conditionsMet = true;
            EventInfo.EventTitleText = $"End of Night {overallStats.NightsSpentGambling}";
        }
        
        return conditionsMet;
    }
}
