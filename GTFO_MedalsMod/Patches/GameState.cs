#nullable enable

using GameEvent;
using HarmonyLib;
using MedalsMod.Data;
using Player;
using UnityEngine;
using Time = MedalsMod.Data.Time;

namespace MedalsMod.Patches;

#if DEBUG
[HarmonyPatch(typeof(GS_InLevel), nameof(GS_InLevel.Update))]
internal static class GS_InLevel__Update__Patch
{
    public static void Postfix()
    {
        var focusState = FocusStateManager.CurrentState;
        var guiLayer = MainMenuGuiLayer.Current;

        if (guiLayer.PageExpeditionSuccess.m_isActive && focusState == eFocusState.FPS)
        {
            // In case we press ESC instead of the combo :p
            guiLayer.HidePage(eCM_MenuPage.CMP_EXPEDITION_SUCCESS);
            guiLayer.ShowPage(guiLayer.m_currentPageEnum);
        }
        
        if (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.RightShift) || !Input.GetKeyDown(KeyCode.I))
        {
            return;
        }

        switch (focusState)
        {
            case eFocusState.FPS:
                GameStatePatch.OnRunFinish();
                FocusStateManager.ToggleMenu();
                guiLayer.HidePage(guiLayer.m_currentPageEnum);
                guiLayer.ShowPage(eCM_MenuPage.CMP_EXPEDITION_SUCCESS);
                break;
            case eFocusState.MainMenu:
                FocusStateManager.ToggleMenu();
                guiLayer.HidePage(eCM_MenuPage.CMP_EXPEDITION_SUCCESS);
                guiLayer.ShowPage(guiLayer.m_currentPageEnum);
                break;
        }
    }
}
#endif

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
    }

    private static void OnRunStart()
    {
        TimeCollector.Start();

        Plugin.L.LogInfo("Started level: " + GetLevelName());
    }

    internal static void OnRunFinish()
    {
        var exp = GetLevelName();
        var time = TimeCollector.FinishRun(exp);

        if (time == null)
            return;
        
        Plugin.L.LogInfo("Finished level: " + exp);
        SavedMedals.AddRun(exp, time);
        SavedMedals.Save();
    }

    private static string GetLevelName()
    {
        var exp_index = RundownManager.GetActiveExpeditionData().expeditionIndex + 1;
        var descriptive = RundownManager.ActiveExpedition.Descriptive;
        return descriptive.Prefix +
            (!descriptive.SkipExpNumberInName ? exp_index.ToString() : "");
    }

}
