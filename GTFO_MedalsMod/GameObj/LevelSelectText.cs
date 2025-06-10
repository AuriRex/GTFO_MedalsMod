using GameData;
using Il2CppSystem.Security.Cryptography;
using MedalsMod.Data;
using MedalsMod.Patches;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Time = MedalsMod.Data.Time;

namespace MedalsMod.GameObj;


internal class LevelSelectText: MonoBehaviour
{
    static private bool _initialized = false;
    static private TextMeshPro textMeshPro;
    static private int linesToKeep = 0;

    public static void Initialize()
    {
        // Plugin.L.LogMessage("Tried to initialize.");
        if (_initialized) return;

        try { 
            _initialized = true;

            Plugin.L.LogMessage("LevelSelectText Instantiated");

            GameObject shareText = GuiManager.Current.m_mainMenuLayer.PageLoadout.transform
                .FindChildRecursive("ShareText").gameObject;
            bool state = shareText.activeSelf;
            shareText.SetActive(true);

            //
            // Introduction.
            //
            // One could argue this goes against common practices and that this method of solving
            // the problem would lead to further issues down the line. Unfortunately today I have
            // decided that instead of dealing with Unity's decision making that is 
            // completely mad and built for the devil himself, I will save my sanity and simply
            // commit the equivalent of arson in this codebase. I don't care if you are my employer,
            // I don't care if you are my mom, I don't care if you are any GOD out there. This will
            // happen and there is nothing that can stop me. This will reach the release built. 
            // The world will suffer for this and I don't regret it. 
            //
            // Chapter 1. How it came to be.
            // 
            // I was naive. I truly believed that all I had to do was clone the "Share Lobby ID" game 
            // object, move it down and then make the TextMeshPro... All was simple, all was easy...
            // But reality struck. The game object would just never render. I tried everything. Then,
            // it rendered. But upon changing one single text field, I had changed all of them. 
            // It was a shallow copy apparently. My task was clear. Manually copy the things I needed.
            // But it failed. Either the text would simply not appear or everything would be the same
            // single TextMeshPro. I then looked online... I only found people using TextMeshProUGUI...
            // 
            // Chapter 2. Unity and the rejection of human UI.
            //
            // Unity isn't built for coding. It is built for editing boxes in their UnityEditor...
            // It has never been built to instantiate manually objects. It doesn't allow constructors,
            // it doesn't allow data to be copied. It doesn't allow for things in other systems we would
            // find to be... simply put it "the bare minimum". I tried to create a simple text box... It
            // never worked. It was never meant to work. This was not built to allow a programmer to work
            // with these classes. It was built by a madman to make you scream. To make you feel the depths
            // of hell itself. It is a prison built by a being above us. I argue that this being represents
            // a rejection of God itself. It is either a true failure of the human nature or it is a 
            // creature playing God with our minds and bodies. It is the definition of a joke. To call
            // it anything other than this would be doing a disservice to humanity itself. How can we
            // accept this failure as our own? We simply can't. This cannot be a simple mistake. It is
            // something far greater. It is intentional. And which human out there would desire to create
            // such a thing? There are definitely people out there who would desire to do such damage,
            // however I fully believe they are simply not smart enough to achieve this. Especially in
            // this backwards yet insanely efficient way.
            textMeshPro = shareText.GetComponent<TextMeshPro>();
            textMeshPro.text = "<b><size=175%>SHARE LOBBY ID</size><size=115%></b>\r\nAny prisoner (friend or not) with your Lobby ID <u>in their clipboard</u> can join through a button that appears in their Rundown.";
            textMeshPro.text += "</size><size=200%><br><br><br><br><br><br><br>";
            textMeshPro.autoSizeTextContainer = false;
            textMeshPro.m_autoSizeTextContainer = false;
            textMeshPro.ForceMeshUpdate();

            linesToKeep = textMeshPro.text.Split("<br>").Length;

            shareText.SetActive(state);
        } catch (Exception e) {
            Plugin.L.LogError($"Crashed when initializing UWU: {e}");
        }
    }

    public static void SetVisible(bool visible)
    {
        
    }

    public static void UpdateMedalText(string levelName)
    {
        Medal? medal = SavedMedals.GetMedal(levelName);
        Time? time = SavedMedals.GetTime(levelName);
        string medalColor = MedalColors.GetMedalColor(medal);

        try
        {
            MedalTimes medalTimes = MedalRegistry.AllMedals[levelName];
            Plugin.L.LogMessage($"MEDAL TEXT UPDATED TO: {levelName}");

            textMeshPro.text = RemoveExcess(textMeshPro.text, linesToKeep);

            if (medal != null && time != null && medal == Medal.Champion) { textMeshPro.text += $"<{medalColor}>PERS BEST: " + time.GetString() + "</color><br>"; }
            textMeshPro.text += "CHAMPION:  " + medalTimes.championTime.GetString() + "<br>";
            if (medal != null && time != null && medal == Medal.Gold) { textMeshPro.text += $"<{medalColor}>PERS BEST: " + time.GetString() + "</color><br>"; }
            textMeshPro.text += "GOLD:      " + medalTimes.goldTime.GetString() + "<br>";
            if (medal != null && time != null && medal == Medal.Silver) { textMeshPro.text += $"<{medalColor}>PERS BEST: " + time.GetString() + "</color><br>"; }
            textMeshPro.text += "SILVER:    " + medalTimes.silverTime.GetString() + "<br>";
            if (medal != null && time != null && medal == Medal.Bronze) { textMeshPro.text += $"<{medalColor}>PERS BEST: " + time.GetString() + "</color><br>"; }
            textMeshPro.text += "BRONZE:    " + medalTimes.bronzeTime.GetString() + "<br>";
            if (time != null && medal == null) { textMeshPro.text += $"<{medalColor}>PERS BEST: " + time.GetString() + "</color><br>"; }
        } catch
        {
            textMeshPro.text = RemoveExcess(textMeshPro.text, linesToKeep);

            if (time != null)
            {
                textMeshPro.text += $"<{medalColor}>PERS BEST: " + time.GetString() + "</color><br>";
            }
            textMeshPro.text += "CHAMPION:  N/A" + "<br>";
            textMeshPro.text += "GOLD:      N/A" + "<br>";
            textMeshPro.text += "SILVER:    N/A" + "<br>";
            textMeshPro.text += "BRONZE:    N/A" + "<br>";
        }

        textMeshPro.ForceMeshUpdate();
    }

    private static string RemoveExcess(string input, int countKeep)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string[] lines = input.Split(new string[] { "<br>" }, System.StringSplitOptions.None);

        string result = string.Join("<br>", lines, 0, countKeep) + "<br>";
        return result;
    }
}
