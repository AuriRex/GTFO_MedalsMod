using System;
using System.Collections.Generic;

namespace MedalsMod.Data;

internal enum Medal
{
    Bronze,
    Silver,
    Gold,
    Champion,
}

internal class MedalColors
{
    public static string GetMedalColor(Medal? medal)
    {
        switch (medal)
        {
            case Medal.Bronze:
                return "#cd7f32";
            case Medal.Silver:
                return "#c0c0c0";
            case Medal.Gold:
                return "#ffd700";
            case Medal.Champion:
                return "#FF2EFF";
            case null:
                return "#ffffff";
        }

        return "#ffffff";
    }
}

internal class MedalNames
{
    public static string GetMedalName(Medal? medal)
    {
        switch (medal)
        {
            case Medal.Bronze:
                return " BRONZE ";
            case Medal.Silver:
                return " SILVER ";
            case Medal.Gold:
                return "  GOLD  ";
            case Medal.Champion:
                return "CHAMPION";
            case null:
                return "NO MEDAL";
        }

        return "#ffffff";
    }
}

internal class MedalTimes
{
#nullable enable
    readonly public Time? bronzeTime;
    readonly public Time? silverTime;
    readonly public Time? goldTime;
    readonly public Time? championTime;

    /// <summary>
    /// Construct the medal times from a string that looks like this:
    /// "2:35 5:10 10:20 20:0"
    /// First medal is champion and so on...
    /// </summary>
    /// <param name="data"></param>
    internal MedalTimes(string data)
    {
        string[] parts = data.Split(' ');

        Time?[] times = new Time?[4];

        for (int i = 0; i < parts.Length; i++) {
            try {
                times[i] = ParseTime(parts[i]);
            } catch
            {
                times[i] = null;
            };
        }

        this.championTime = times[0];
        this.goldTime = times[1];
        this.silverTime = times[2];
        this.bronzeTime = times[3];
    }

    public Medal? GetMedal(Time time)
    {

        if (this.championTime != null && time.CheckValidMedal(championTime)) { return Medal.Champion; }
        if (this.goldTime != null && time.CheckValidMedal(goldTime)) { return Medal.Gold; }
        if (this.silverTime != null && time.CheckValidMedal(silverTime)) { return Medal.Silver; }
        if (this.bronzeTime != null && time.CheckValidMedal(bronzeTime)) { return Medal.Bronze; }

        return null;
    }

    private Time ParseTime(string timeStr)
    {
        string[] tokens = timeStr.Split(':');

        try
        {
            if (tokens.Length != 2)
                throw new FormatException($"Invalid time format: {timeStr}");

            if (!UInt64.TryParse(tokens[0], out UInt64 minutes) || !UInt64.TryParse(tokens[1], out UInt64 seconds))
                throw new FormatException($"Invalid numbers in time: {timeStr}");

            return new Time(minutes, seconds);
        } catch (Exception)
        {
            throw;
        }
    }
}

internal static class MedalRegistry
{
    internal static readonly Dictionary<string, MedalTimes> AllMedals = new()
    {
        { "R1A1", new MedalTimes("03:00 03:30 05:10 10:00") },
        { "R1C1", new MedalTimes("20:35 21:00 26:00 35:00") },
    };
}
