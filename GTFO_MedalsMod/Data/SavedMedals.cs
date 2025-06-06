using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MedalsMod.Data;

/// <summary>
/// This is a class that can store the medals that 
/// were obtained by the player
/// </summary>
internal class SavedMedals
{

    private static Dictionary<string, Time> medalsObtained = new Dictionary<string, Time>();

    private static readonly string SavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GTFOMedalsMod",
        "medals.json"
    );

    /// <summary>
    /// Add a single run to the stored runs. Automatically saves the best time
    /// </summary>
    /// <param name="level"> name of level </param>
    /// <param name="time"> time in which the level was completed </param>
    public static void AddRun(string level, Time time)
    {
        Time? last_time = medalsObtained[level];

        if (last_time != null && last_time < time) {
            medalsObtained.Add(level, last_time);
        } else
        {
            medalsObtained.Add(level, time);
        }
    }

    /// <summary>
    /// Get a medal
    /// </summary>
    /// <param name="level"> level for which you want to see medal </param>
    /// <returns> medal or null if no medal is obtained on this level </returns>
    public static Medal? GetMedal(string level) {
        MedalTimes? times = MedalRegistry.AllMedals[level];
        Time? bestTime = medalsObtained[level];

        if (times != null && bestTime != null) {
            return times.GetMedal(bestTime);
        }

        return null;
    }

    public static void Load()
    {
        if (!File.Exists(SavePath))
        {
            medalsObtained = new();
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            medalsObtained = JsonSerializer.Deserialize<Dictionary<string, Time>>(json) ?? new();
        }
        catch (Exception)
        {
            medalsObtained = new();
        }
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);
            string json = JsonSerializer.Serialize(medalsObtained);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception)
        {
            throw;
        }
    }

}
