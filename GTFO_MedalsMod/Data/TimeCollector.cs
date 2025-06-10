#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedalsMod.Data;


internal class TimeCollector
{

    static private DateTime? stamp = null;
    static private bool checkpoint = false;

    public static void Start()
    {
        stamp = DateTime.UtcNow;
    }

    public static Time? FinishRun()
    {
        if (stamp == null || checkpoint == true) {
            checkpoint = false;
            return null; 
        }

        Time time = new((DateTime)stamp, DateTime.UtcNow);
        stamp = null;
        return time;
    }

    public static void SetInvalid()
    {
        stamp = null;
    }

    public static void SetCheckpoint(bool checkpoint_value)
    {
        checkpoint = checkpoint_value;
    }
}
