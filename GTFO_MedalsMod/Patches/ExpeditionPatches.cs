using HarmonyLib;
using MedalsMod.Core;

namespace MedalsMod.Patches;

[HarmonyPatch(typeof(GameStateManager), nameof(GameStateManager.DoChangeState))]
public class GameStateManager__DoChangeState__Patch
{
    public static void Postfix(eGameStateName nextState)
    {
        switch (nextState)
        {
            case eGameStateName.InLevel:
                SessionManager.OnExpeditionEntered();
                break;
            case eGameStateName.ExpeditionSuccess:
                SessionManager.OnExpeditionCompleted(success: true);
                break;
            case eGameStateName.ExpeditionFail:
                SessionManager.OnExpeditionCompleted(success: false);
                break;
        }
    }
}