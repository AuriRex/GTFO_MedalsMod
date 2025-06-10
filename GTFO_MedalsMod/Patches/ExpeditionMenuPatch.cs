using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using CellMenu;
using GameData;
using HarmonyLib;
using MedalsMod.GameObj;
using static Il2CppSystem.Globalization.CultureInfo;

namespace MedalsMod.Patches;


[HarmonyPatch]
internal class ExpeditionMenuPatch
{

    [HarmonyPatch(typeof(MainMenuGuiLayer), nameof(MainMenuGuiLayer.OnExpeditionUpdated), [
        typeof(pActiveExpedition),
        typeof(ExpeditionInTierData)
        ])]
    [HarmonyPostfix]
    public static void Postfix(pActiveExpedition activeExpedition, ExpeditionInTierData expeditionInTierData) 
    {
        var exp_index = activeExpedition.expeditionIndex + 1;
        var descriptive = expeditionInTierData.Descriptive;
        string levelName = descriptive.Prefix +
            (!descriptive.SkipExpNumberInName ? exp_index.ToString() : "");

        LevelSelectText.UpdateMedalText(levelName);
    }
}

