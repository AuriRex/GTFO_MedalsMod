using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using CellMenu;
using MedalsMod.Data;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Time = MedalsMod.Data.Time;

namespace MedalsMod.GameObj;

public class MedalEndScreenDisplay
{
    private static GameObject _prefab;
    private const string GO_NAME_SPRITE_RENDERER = "MedalRenderer";
    
    public static void Setup(CM_PageExpeditionSuccess pageSuccess)
    {
        CoroutineManager.StartCoroutine(DelayedSetup(pageSuccess).WrapToIl2Cpp());
    }

    private static IEnumerator DelayedSetup(CM_PageExpeditionSuccess pageSuccess)
    {
        yield return null;
        yield return null;
        
        CreatePrefab();
        
        yield return null;
        yield return null;
        
        // TODO
        var containerGO = new GameObject("MedalContainer");
        containerGO.transform.SetParent(pageSuccess.m_movingContentHolder);
        containerGO.transform.localPosition = new Vector3(-378f, 364f, 0);
        containerGO.transform.localScale = Vector3.one;
        var rectTrans = containerGO.AddComponent<RectTransform>();
        containerGO.layer = LayerManager.LAYER_UI;
        
        var sortingGroup = containerGO.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "MenuPopupSprite";
        
        var bronze = CreateMedal(containerGO, Medal.Bronze);
        var silver = CreateMedal(containerGO, Medal.Silver);
        silver.transform.localPosition += Vector3.right * 200f;
        var gold = CreateMedal(containerGO, Medal.Gold);
        gold.transform.localPosition += Vector3.right * 400f;
        var champion = CreateMedal(containerGO, Medal.Champion);
        champion.transform.localPosition += Vector3.right * 600f;
    }

    private static GameObject CreateMedal(GameObject containerGO, Medal medal)
    {
        var holderGO = Object.Instantiate(_prefab, containerGO.transform);
        holderGO.SetActive(true);
        var mask = holderGO.GetComponent<SpriteMask>();
        CoroutineManager.StartCoroutine(Test(mask).WrapToIl2Cpp()); // TODO: Remove
        
        var spriteRenderer = holderGO.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = MedalImages.GetSpriteFromMedal(medal);
        
        return holderGO;
    }

    // TODO: Remove this, just a POC
    private static IEnumerator Test(SpriteMask mask)
    {
        var state = true;
        var value = 0f;
        while (true)
        {
            if (state)
            {
                value += UnityEngine.Time.deltaTime;
            }
            else
            {
                value -= UnityEngine.Time.deltaTime;
            }

            if (value >= 1f || value <= 0f)
            {
                value = Mathf.Clamp01(value);
                state = !state;
            }
            
            mask.alphaCutoff = value;
            yield return null;
        }
    }

    private static void CreatePrefab()
    {
        var mainGO = new GameObject("MedalHolder");
        
        mainGO.AddComponent<RectTransform>();
        mainGO.transform.SetParent(null);
        mainGO.transform.localPosition = Vector3.zero;
        mainGO.transform.localScale = Vector3.one * 13.2f;

        var spriteMask = mainGO.AddComponent<SpriteMask>();
        spriteMask.sprite = MedalImages.Mask;
        spriteMask.alphaCutoff = 0;
        
        var spriteGO = new GameObject(GO_NAME_SPRITE_RENDERER);
        spriteGO.AddComponent<RectTransform>();
        spriteGO.transform.SetParent(mainGO.transform);
        spriteGO.transform.localPosition = Vector3.zero;
        spriteGO.transform.localScale = Vector3.one;
        var spriteRenderer = spriteGO.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = MedalImages.GetSpriteFromMedal(Medal.Champion);
        spriteRenderer.sortingLayerName = "MenuPopupSprite";
        spriteRenderer.sortingOrder = 100;
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        
        mainGO.SetActive(false);
        
        mainGO.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        UnityEngine.Object.DontDestroyOnLoad(mainGO);
        
        DefaultControls.SetLayerRecursively(mainGO, LayerManager.LAYER_UI);

        _prefab = mainGO;
    }
}