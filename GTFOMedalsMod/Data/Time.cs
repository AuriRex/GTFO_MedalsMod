using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFOMedalsMod.Data;


internal class Time
{

    UInt64 stamp;

    public Time(UInt64 stamp) {  this.stamp = stamp; }
    public Time(DateTime timestamp1, DateTime timestamp2) {
        this.stamp = (ulong)(timestamp2 - timestamp1).TotalMilliseconds;
    }
    public Time(UInt64 minutes, UInt64 seconds) {
        this.stamp = (minutes * 60 + seconds) * 1000;
    }

    public bool CheckValidMedal(Time other) {
        return this.stamp < other.stamp + 1000;
    }

}
