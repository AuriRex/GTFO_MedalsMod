using System;
using System.Diagnostics;
using SNetwork;

namespace MedalsMod.Core;

public class TimedSession
{
    /// <summary>
    /// This property is false if the local player got host at the end of the session, despite not being host at the beginning
    /// </summary>
    public bool WasConsistentMaster => _wasMasterOnStart && _wasMasterOnEnd;
    
    public DateTime StartTime { get; private set; }
    
    public TimeSpan TotalTime { get; private set; }
    
    /// <summary>
    /// The time the game reports on the success screen
    /// </summary>
    public double ReportedGameTime { get; private set; }
    
    public bool WasSuccessful { get; private set; }
    
    private readonly bool _wasMasterOnStart;
    private bool _wasMasterOnEnd;
    
    private readonly Stopwatch _stopwatch;

    public TimedSession()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        
        _wasMasterOnStart = SNet.IsMaster;
        StartTime = DateTime.UtcNow;
    }

    public TimeSpan EndSession(double gameTime, bool success)
    {
        _stopwatch.Stop();
        ReportedGameTime = gameTime;
        WasSuccessful = success;
        _wasMasterOnEnd = SNet.IsMaster;
        TotalTime = _stopwatch.Elapsed;
        return TotalTime;
    }
}