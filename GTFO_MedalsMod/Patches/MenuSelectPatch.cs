using CellMenu;
using GameData;
using HarmonyLib;
using MedalsMod.Data;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using Time = MedalsMod.Data.Time;

namespace MedalsMod.Patches;

[HarmonyPatch]
internal class MenuSelectPatch
{
    static List<GameObject> timeObjects = new List<GameObject>();

    [HarmonyPatch(typeof(CM_ExpeditionIcon_New), nameof(CM_ExpeditionIcon_New.SetStatus))]
    [HarmonyPostfix]
    private static void Postfix(CM_ExpeditionIcon_New __instance)
    {

        // straight up yeeted from Auri's Archive Mod
        // https://github.com/AuriRex/GTFO_TheArchive/blob/c49b798e8d0b5c030b84378bedba59e4bd5b13df/TheArchive.Essentials/Features/Hud/LogVisualizer.cs#L669C17-L670C1
        var originalStatus = __instance.m_statusText.text.Contains("<br>") ? __instance.m_statusText.text.Split("<br>", StringSplitOptions.None)[0] : __instance.m_statusText.text;
        var expeditionData = __instance.DataBlock;
        int expIndex = __instance.ExpIndex;

        string levelName = GetLevelName(expeditionData, expIndex);
        Time? levelTime = SavedMedals.GetTime(levelName);
        if (levelTime == null) { return; }
        
        string levelTimeText = levelTime.GetString();
        string color = GetColor(levelName);

        __instance.m_statusText.SetText($"{originalStatus}<br>Best Time: <{color}><size=120%>{levelTimeText}</size></color>");

        // Plugin.L.LogMessage("FEMBOYS GO HARD: " + __instance.m_statusText.text);
    }

    private static string? GetString(string levelName)
    {
        Time? time = SavedMedals.GetTime(levelName);

        if (time == null) { return null; }

        return "Best time: " + time.GetString();
    }

    public static string GetColor(string levelName)
    {
        Medal? medal = SavedMedals.GetMedal(levelName);
        return MedalColors.GetMedalColor(medal);
    }

    private static string GetLevelName(ExpeditionInTierData data, int expIndex)
    {
        var exp_index = expIndex + 1;
        var descriptive = data.Descriptive;
        return descriptive.Prefix +
            (!descriptive.SkipExpNumberInName ? exp_index.ToString() : "");
    }

}
