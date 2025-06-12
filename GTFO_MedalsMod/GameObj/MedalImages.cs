using MedalsMod.Data;
using System;
using System.IO;
using UnityEngine;

namespace MedalsMod.GameObj;

internal static class MedalImages
{
    private static Sprite _championMedal;
    private static Sprite _goldMedal;
    private static Sprite _silverMedal;
    private static Sprite _bronzeMedal;

    public static void Load()
    {
        var assemblyLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        _championMedal = LoadSingle(assemblyLocation + "/Resources/Champion.png");
        _goldMedal = LoadSingle(assemblyLocation + "/Resources/Gold.png");
        _silverMedal = LoadSingle(assemblyLocation + "/Resources/Silver.png");
        _bronzeMedal = LoadSingle(assemblyLocation + "/Resources/Bronze.png");
    }


    private static Sprite LoadSingle(string path)
    {
        if (!File.Exists(path))
        {
            Plugin.L.LogError($"Medal image file not found: {path}");
            return null;
        }
        
        try
        {
            var pngData = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(pngData);

            UnityEngine.Object.DontDestroyOnLoad(tex);
            tex.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
            
            // Just in case
            UnityEngine.Object.DontDestroyOnLoad(sprite);
            sprite.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            
            return sprite;
        }
        catch (Exception e)
        {
            Plugin.L.LogError(e);
            return null;
        }
    }

    public static Sprite GetSpriteFromMedal(Medal medal)
    {
        return medal switch
        {
            Medal.Bronze => _bronzeMedal,
            Medal.Silver => _silverMedal,
            Medal.Gold => _goldMedal,
            Medal.Champion => _championMedal,
            _ => _bronzeMedal
        };
    }
}
