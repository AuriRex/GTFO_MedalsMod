using MedalsMod.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppInterop.Runtime.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Time = MedalsMod.Data.Time;
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MedalsMod.GameObj;

internal partial class MedalInfo : MonoBehaviour
{
    private const string GO_NAME_TIME_TMP = "TimeInfoText";
    private const string GO_NAME_NAME_TMP = "MedalInfoText";
    private const string GO_NAME_SPRITE_RENDERER = "MedalRenderer";
    
    private const string NO_TIME = "<#333>--:--";
    
    private static bool _initialized;
    
    private static GameObject _prefab;
    
    private static GameObject _medalContainer = null!;
    
    private static MedalInfo _personalBest;
    private static MedalInfo _champion;
    private static MedalInfo _gold;
    private static MedalInfo _silver;
    private static MedalInfo _bronze;
    
    private static readonly Dictionary<Medal, MedalInfo> _medalToInfoMap = new();

    [HideFromIl2Cpp]
    public static void Initialize()
    {
        if (_initialized) return;

        try { 
            _initialized = true;

            Plugin.L.LogInfo("Setting up lobby medal displays ...");

            CoroutineManager.StartCoroutine(DelayedSetup().WrapToIl2Cpp());

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

            //
            // ******************************************###********#%@@            
            // *******************************************###********##%@@          
            // ********************************************##**********##%@@        
            // **********************************************##**********#%@#       
            // ***********************#**********************###**********#%@@      
            // ***********************##*********************####**********##@@     
            // ***********************##*********************#####**********#%%@    
            // **********************####*********************#####**********#%@    
            // **********************####*******#*********######**###*********#%@@  
            // ********************##****########************##*++**##*********#@@  
            // ***************#######****####****************#**++++**#********#@@  
            // *****************###*++++++**##*************####*******###******#%%@ 
            // ***************##************####*********###%%%%%%%%%%%%#******#%%@ 
            // **************##*****###%%%%%%%#*##******###%%@@@@@@@%#*+*#*****#%%  
            // *********#*#####%%@@@@@@@@@@@%#*++*##*****##%@@@@@@@@%#+++*##***#%@  
            // ##*******#*#%%%%%%@@@@@@@@@@%#*+++++***###**#%@@@@@%%%#**++##***#%%  
            // ##*******#******+#%@@@@@@@@@%#*++++++++***++******+++++++++*##*##%%@ 
            // ##*******#**+++++*#@@@%%%%%%##*+++++++++++++++++++++++++++++#%####%@ 
            // ##********#*++++***#****++++++++++++++++++++++++++++++++++++#%##*#%@ 
            // ##********#*++++++++++++++++++++++++++++++++++++++++++++++++*#%#**%%@
            // #*********#*+++++++++++++++++++++++++++++++++++++++++++++++++*%##*#%@
            // #*********#**+++++++++++++++++++++++++++++++++++++++++++++++++*##*#%@
            // #************+++++++++++++++++++++++++++++++++++++++++++++++++*#%*#%@
            // ##***********+++++++++++++++++++++++++++++++++++++++++++++++++*##*#%@
            // ##*********##+++++++++++++++++++++*#####**+++++++++++++++++++*###**#@
            // ##*********##*+++++++++++++++++++*#######*++++++++++++++++++*###***#@
            // ##**********#*++++++++++++++++++++******+++++++++++++++++++*###****#@
            // ##**********#*+++++++++++++++++++++++++++++++++++++++++++*#%#******#@
            // ###*********#**++++++++++++++++++++++++++++++++++++++**####********#@
            // ###*********##**+++++++++++++++++++++++++++++++++**#%%%%##********#%@
            // ###*********#%%%%##**++++++++++++++++++++++***#%%%%########*******#%@
            // ####********##%%%%%##**************#######%%%%%############*******#%@
            // #####*******#%%@@@%##***********#%%@@@@%%##################*******%%@
            // #####******##%%@@@%##***********#%@@@@@@@%%%###############******#%@ 
            // #####*******#%%@@@%##***********#%@@@@@@@@@@@%%%############*****#%@ 
            // ######******#%%@@@@%%#**********##%%@@@@@@@@@@@@%%%%########****#%%@ 
            // ######******#%%@@@@@@%#****##%%%%%%%@@@@@@@@@@@@@@@%%%####*#****#@@  
            //
            // Wtf is he yapping about??
            
        }
        catch (Exception e)
        {
            Plugin.L.LogError($"Crashed when initializing UWU: {e}");
        }
    }
    
    [HideFromIl2Cpp]
    private static void CreatePrefab()
    {
        var pageLoadoutTrans = GuiManager.Current.m_mainMenuLayer.PageLoadout.transform;
            
        var shareText = pageLoadoutTrans.FindChildRecursive("ShareText").gameObject;
        
        var mainGO = new GameObject("MedalInfoHolder");
        
        mainGO.AddComponent<RectTransform>();
        mainGO.transform.SetParent(null);
        mainGO.transform.localPosition = Vector3.zero;

        var spriteGO = new GameObject(GO_NAME_SPRITE_RENDERER);
        spriteGO.AddComponent<RectTransform>();
        spriteGO.transform.SetParent(mainGO.transform);
        spriteGO.transform.localPosition = new Vector3(-40f, -10f, 0f);
        spriteGO.transform.localScale = Vector3.one * 4.5f;
        var spriteRenderer = spriteGO.AddComponent<SpriteRenderer>();
        
        var medalInfoTextGO = Instantiate(shareText, mainGO.transform);
        medalInfoTextGO.SetActive(true);
        Destroy(medalInfoTextGO.GetComponent<TMP_Localizer>());
        medalInfoTextGO.name = GO_NAME_NAME_TMP;
        medalInfoTextGO.transform.localPosition = new Vector3(140f, 10f, 0f);
        medalInfoTextGO.transform.localScale = Vector3.one * 2;
        var medalTmp = medalInfoTextGO.GetComponent<TextMeshPro>();
        medalTmp.SetText("Medal Here");

        var timeInfoTextGO = Instantiate(shareText, mainGO.transform);
        timeInfoTextGO.SetActive(true);
        Destroy(timeInfoTextGO.GetComponent<TMP_Localizer>());
        timeInfoTextGO.name = GO_NAME_TIME_TMP;
        timeInfoTextGO.transform.localPosition = new Vector3(5f, 10f, 0f);
        timeInfoTextGO.transform.localScale = Vector3.one * 2;
        var timeTmp = timeInfoTextGO.GetComponent<TextMeshPro>();
        timeTmp.SetText("TIMER HERE");

        
        var info = mainGO.AddComponent<MedalInfo>();
        // This doesn't work :(
        // As in; these values won't get copied over by `Instantiate`, so we have to re-assign them later in `Setup`
        info.SpriteRenderer = spriteRenderer;
        info.MedalText = medalTmp;
        info.TimeText = timeTmp;
        
        mainGO.SetActive(false);
        
        mainGO.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        DontDestroyOnLoad(mainGO);
        
        DefaultControls.SetLayerRecursively(mainGO, LayerManager.LAYER_UI);
        
        _prefab = mainGO;
    }

    [HideFromIl2Cpp]
    private static IEnumerator DelayedSetup()
    {
        // We wait 2 frames for unity / IL2CPP Jank to its thing ... 
        yield return null;
        yield return null;

        CreatePrefab();
        
        yield return null;
        yield return null;
        
        try
        {
            _medalContainer = new GameObject("MedalContainer");
            _medalContainer.AddComponent<RectTransform>();
            _medalContainer.layer = LayerManager.LAYER_UI;
            
            _medalContainer.transform.SetParent(GuiManager.Current.m_mainMenuLayer.PageLoadout.m_movingContentHolder);
            _medalContainer.transform.localPosition = new Vector3(1340f, 25f, 0f);
            _medalContainer.transform.localScale = Vector3.one * 0.8f;
            
            _champion = CreateMedalInfo(Medal.Champion, NO_TIME);
            _gold = CreateMedalInfo(Medal.Gold, NO_TIME);
            _silver = CreateMedalInfo(Medal.Silver, NO_TIME);
            _bronze = CreateMedalInfo(Medal.Bronze, NO_TIME);
            
            _personalBest = CreateMedalInfo(Medal.Bronze, NO_TIME);
            _personalBest.transform.localPosition = Vector3.up * 50 * -1;
            _personalBest.SpriteRenderer.enabled = false;
            _personalBest.MedalText.SetText("PB");

            var separator = new GameObject("Separator");
            var rectTrans = separator.AddComponent<RectTransform>();
            rectTrans.sizeDelta = Vector2.one;
            separator.transform.SetParent(_medalContainer.transform);
            var tmp = separator.AddComponent<TextMeshPro>();
            tmp.SetText("-");
            separator.transform.localPosition = new Vector3(72f, 200f, 0f);
            separator.transform.localScale = new Vector3(350f, 5f, 1f);
            separator.layer = LayerManager.LAYER_UI;

            var headerGO = Instantiate(_personalBest.TimeText.gameObject, _medalContainer.transform);
            headerGO.name = "Header";
            tmp = headerGO.GetComponent<TextMeshPro>();
            tmp.SetText("<color=orange>Speedrun Medals</color>");
            headerGO.transform.localPosition = Vector3.up * 50 * 5;
            
            _medalToInfoMap[Medal.Bronze] = _bronze;
            _medalToInfoMap[Medal.Silver] = _silver;
            _medalToInfoMap[Medal.Gold] = _gold;
            _medalToInfoMap[Medal.Champion] = _champion;
        }
        catch (Exception ex)
        {
            Plugin.L.LogError($"Exception occured during medal container creation: {ex}");
        }
    }

    [HideFromIl2Cpp]
    private static MedalInfo CreateMedalInfo(Medal medal, string time)
    {
        var medalInfoGO = Instantiate(_prefab, _medalContainer.transform);
        medalInfoGO.transform.localPosition = Vector3.up * 50 * (int) medal;
        var medalInfo = medalInfoGO.GetComponent<MedalInfo>();
        medalInfo.Setup(medal, time);
        medalInfoGO.SetActive(true);

        return medalInfo;
    }

    [HideFromIl2Cpp]
    public static void UpdateMedals(string levelName)
    {
        var pbMedal = SavedMedals.GetMedal(levelName);
        var pbTime = SavedMedals.GetTime(levelName);
        var pbMedalColor = MedalColors.GetMedalColor(pbMedal);
        
        UpdateMedals(levelName, pbMedal, pbTime, pbMedalColor);
    }
    
    [HideFromIl2Cpp]
    public static void UpdateMedals(string levelName, Medal? pbMedal, Time pbTime, string pbMedalColor)
    {
        Plugin.L.LogInfo("Updating MedalInfos ...");

        if (!MedalRegistry.AllMedals.TryGetValue(levelName, out var medalTimes))
        {
            _medalContainer.SetActive(false);
            return;
        }

        _medalContainer.SetActive(true);
        
        _personalBest.TimeText.SetText(NO_TIME);
        
        if (pbTime != null)
            _personalBest.TimeText.SetText($"<{pbMedalColor}>{pbTime}");
        _bronze.TimeText.SetText($"{medalTimes.bronzeTime}");
        _silver.TimeText.SetText($"{medalTimes.silverTime}");
        _gold.TimeText.SetText($"{medalTimes.goldTime}");
        _champion.TimeText.SetText($"{medalTimes.championTime}");
        
        _personalBest.SpriteRenderer.enabled = pbMedal != null;
        _personalBest.SpriteRenderer.sprite = MedalImages.GetSpriteFromMedal(pbMedal ?? Medal.Bronze);
        
        _bronze.gameObject.SetActive(medalTimes.bronzeTime != null);
        _silver.gameObject.SetActive(medalTimes.silverTime != null);
        _gold.gameObject.SetActive(medalTimes.goldTime != null);
        _champion.gameObject.SetActive(medalTimes.championTime != null);

        var sortValue = 3;
        var pbWasPositioned = false;

        foreach (var medal in Enum.GetValues<Medal>().Reverse())
        {
            if (!_medalToInfoMap.TryGetValue(medal, out var info))
                continue;
            
            if (!pbWasPositioned && pbMedal >= medal)
            {
                _personalBest.Reposition(sortValue--);
                pbWasPositioned = true;
            }

            info.Reposition(sortValue--);
        }
        
        if (!pbWasPositioned)
            _personalBest.Reposition(sortValue);
    }
}
