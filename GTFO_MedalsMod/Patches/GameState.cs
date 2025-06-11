#nullable enable

using CellMenu;
using Discord;
using Expedition;
using GameData;
using GameEvent;
using HarmonyLib;
using MedalsMod.Data;
using MedalsMod.GameObj;
using Player;
using System.Text.RegularExpressions;
using UnityEngine;
using Time = MedalsMod.Data.Time;

namespace MedalsMod.Patches;

[HarmonyPatch]
internal class GameStatePatch
{
    [HarmonyPatch(typeof(GameEventManager), nameof(GameEventManager.PostEvent),
        [
            typeof(eGameEvent),
            typeof(PlayerAgent),
            typeof(float),
            typeof(string),
            typeof(Il2CppSystem.Collections.Generic.Dictionary<string, string>)
        ])]
    [HarmonyPrefix]
    public static void Patch_PostEvent_Overload2(
        eGameEvent e,
        PlayerAgent player,
        float floatVal,
        string stringVal,
        Il2CppSystem.Collections.Generic.Dictionary<string, string> customAnalyticsPayload) {

        switch (e)
        {
            case eGameEvent.gs_Startup:
                MedalInfo.Initialize();
                break;
            case eGameEvent.gs_InLevel:
                OnRunStart();
                break;
            case eGameEvent.gs_ExpeditionFail:
            case eGameEvent.gs_ExpeditionAbort:
            case eGameEvent.game_quit:
                TimeCollector.SetInvalid();
                TimeCollector.SetCheckpoint(false);
                break;
            // case eGameEvent.player_chat_message_sent:
            case eGameEvent.gs_ExpeditionSuccess:
                OnRunFinish();
                TimeCollector.SetCheckpoint(false);
                break;
            case eGameEvent.checkpoint_reload:
                TimeCollector.SetCheckpoint(true);
                TimeCollector.SetInvalid();
                break;
            default:
                break;
        }

        if (PlayerChatManager.InChatMode)
        {
            WardenObjectiveManager.ForceCompleteObjective(LevelGeneration.LG_LayerType.MainLayer);
        }

    }

    private static void OnRunStart()
    {
        TimeCollector.Start();

        Plugin.L.LogInfo("Started level: " + GetLevelName());
    }

    private static void OnRunFinish()
    {
        string exp = GetLevelName();
        Time? time = TimeCollector.FinishRun(exp);

        if (time != null)
        {
            Plugin.L.LogInfo("Finished level: " + exp);
            SavedMedals.AddRun(exp, time);
            SavedMedals.Save();
        }
    }

    private static string GetLevelName()
    {
        var exp_index = RundownManager.GetActiveExpeditionData().expeditionIndex + 1;
        var descriptive = RundownManager.ActiveExpedition.Descriptive;
        return descriptive.Prefix +
            (!descriptive.SkipExpNumberInName ? exp_index.ToString() : "");
    }

}
