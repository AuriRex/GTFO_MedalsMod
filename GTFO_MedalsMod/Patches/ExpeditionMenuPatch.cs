using System;
using CellMenu;
using GameData;
using HarmonyLib;
using MedalsMod.Data;
using MedalsMod.GameObj;
using UnityEngine;

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

        MedalInfo.UpdateMedals(levelName);
    }
}

[HarmonyPatch(typeof(CM_PageLoadout), nameof(CM_PageLoadout.Setup))]
internal static class CM_PageLoadout__Setup__Patch
{
    public static void Postfix(CM_PageLoadout __instance)
    {
        MedalInfo.Initialize();
    }
}

[HarmonyPatch(typeof(CM_PageExpeditionSuccess), nameof(CM_PageExpeditionSuccess.Setup))]
internal static class CM_PageExpeditionSuccess__Setup__Patch
{
    public static void Postfix(CM_PageExpeditionSuccess __instance)
    {
        MedalEndScreenDisplay.Setup(__instance);
    }
}

[HarmonyPatch(typeof(CM_PageExpeditionSuccess), nameof(CM_PageExpeditionSuccess.OnEnable))]
internal static class Inject_CM_Expedition
{
    private static GameObject icon;

    private static void Postfix(CM_PageExpeditionSuccess __instance)
    {
        var time = TimeCollector.GetLastTime();
        var exp = TimeCollector.GetLastLevel();
        
        if (exp == null || !MedalRegistry.AllMedals.TryGetValue(exp, out var medalTimes))
        {
            return;
        }
        
        var medal = medalTimes.GetMedal(time);

        if (medal == null)
        {
            return;
        }

        try {

            // TODO lol
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

