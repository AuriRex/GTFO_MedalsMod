using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFOMedalsMod.Data;

/// <summary>
/// This is a class that can store the medals that 
/// were obtained by the player
/// </summary>
internal class SavedMedals
{

    private Dictionary<string, Time> medalsObtained;

    /// <summary>
    /// default constructor, empty dictionary
    /// </summary>
    public SavedMedals()
    {
        this.medalsObtained = new Dictionary<string, Time>();
    }

    /// <summary>
    /// Add a single run to the stored runs. Automatically saves the best time
    /// </summary>
    /// <param name="level"> name of level </param>
    /// <param name="time"> time in which the level was completed </param>
    public void AddRun(string level, Time time)
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
    public Medal? GetMedal(string level) {
        MedalTimes? times = MedalRegistry.AllMedals[level];
        Time? bestTime = this.medalsObtained[level];

        if (times != null && bestTime != null) {
            return times.GetMedal(bestTime);
        }

        return null;
    }

}
