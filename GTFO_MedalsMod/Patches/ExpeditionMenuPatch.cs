using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using CellMenu;
using GameData;
using GameEvent;
using GTFO.API;
using HarmonyLib;
using MedalsMod.Data;
using MedalsMod.GameObj;
using Player;
using TMPro;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using Time = MedalsMod.Data.Time;

namespace MedalsMod.Patches;


[HarmonyPatch]
internal class ExpeditionMenuPatch
{

    [HarmonyPatch(typeof(MainMenuGuiLayer), nameof(MainMenuGuiLayer.OnExpeditionUpdated), [
        typeof(pActiveExpedition),
        typeof(ExpeditionInTierData)
        ])]
    [HarmonyPostfix]
    public static void PostfixLobby(pActiveExpedition activeExpedition, ExpeditionInTierData expeditionInTierData) 
    {
        var exp_index = activeExpedition.expeditionIndex + 1;
        var descriptive = expeditionInTierData.Descriptive;
        string levelName = descriptive.Prefix +
            (!descriptive.SkipExpNumberInName ? exp_index.ToString() : "");

        LevelSelectText.UpdateMedalText(levelName);
    }
}

[HarmonyPatch(typeof(CM_PageExpeditionSuccess), "OnEnable")]
internal static class Inject_CM_Expedition
{
    private static GameObject icon;

    private static void Postfix(CM_PageExpeditionSuccess __instance)
    {
        Time? time = TimeCollector.GetLastTime();
        string? exp = TimeCollector.GetLastLevel();

        var medal = MedalRegistry.AllMedals[exp].GetMedal(time);

        if (medal == null) { return; }

        try {

            // add medal here somehow????
            // code info: 
            //
            // `MedalImages.GetSpriteFromMedal((Medal)medal)` returns a sprite with the correct medal
            // `TimeCollector.GetLastLevel()` grabs the last level name. I do not know how to grab
            //          the objectives existent in the level sadly.
            // 
            // 

            // this adds the text for the medal in the color of the medal. I will probably delete this as it is clutter
            __instance.m_expeditionName.text += $"   <{MedalColors.GetMedalColor(medal)}>{medal}</color>";
            __instance.m_expeditionName.ForceMeshUpdate();
            
        } catch (Exception e) {
            Plugin.L.LogError(e);
        }
    }
}

