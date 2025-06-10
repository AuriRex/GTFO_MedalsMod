using System;
using System.Text.Json.Serialization;

namespace MedalsMod.Data;


internal class Time
{
    [JsonInclude]
    public UInt64 stamp;

    public Time() { stamp = 0; }
    public Time(UInt64 stamp) {  this.stamp = stamp; }
    public Time(DateTime timestamp1, DateTime timestamp2) {
        this.stamp = (ulong)(timestamp2 - timestamp1).TotalMilliseconds;
    }
    public Time(UInt64 minutes, UInt64 seconds) {
        this.stamp = (minutes * 60 + seconds) * 1000;
    }

    public string GetStringWithMilliseconds()
    {
        UInt64 minutes = stamp / 60000;
        UInt64 seconds = stamp % 60000 / 1000;
        UInt64 milliseconds = stamp % 1000;

        return minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + milliseconds.ToString("D3");
    }

    public string GetString()
    {
        UInt64 minutes = stamp / 60000;
        UInt64 seconds = stamp % 60000 / 1000;

        return minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    public bool CheckValidMedal(Time other) {
        return this.stamp < other.stamp + 1000;
    }

    public static bool operator <(Time left, Time right)
    {
        return left.stamp < right.stamp;
    }

    public static bool operator >(Time left, Time right)
    {
        return left.stamp > right.stamp;
    }

}
