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

    public static void Start()
    {
        stamp = DateTime.UtcNow;
    }

    public static Time? FinishRun()
    {
        if (stamp == null) { return null; }

        Time time = new((DateTime)stamp, DateTime.UtcNow);
        stamp = null;
        return time;
    }

    public static void SetInvalid()
    {
        stamp = null;
    }
}
