namespace MedalsMod.Core;

public class SessionManager
{
    private static TimedSession Session { get; set; }
    
    public static void OnExpeditionEntered()
    {
        Session = new TimedSession();
        
        Plugin.L.LogInfo($"Expedition started.");
    }

    public static void OnExpeditionCompleted(bool success)
    {
        if (Session == null)
            return;

        var gameTime = Clock.ExpeditionProgressionTime + Clock.ExpeditionCheckpointWastedTime;
        
        var timeSpan = Session.EndSession(gameTime, success);

        var stamp = (ulong) timeSpan.TotalMilliseconds;
        
        Plugin.L.LogInfo($"Expedition Completed; Time: {timeSpan} ({stamp}ms) [{gameTime}] Result: {success}");
    }
}