﻿using MedalsMod.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MedalsMod.GameObj;

internal class MedalImages
{

    public static Sprite championMedal;
    public static Sprite goldMedal;
    public static Sprite silverMedal;
    public static Sprite bronzeMedal;

    public static void Load()
    {
        string FOLDER_PATH = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        championMedal = LoadSingle(FOLDER_PATH + "/Resources/Champion.png");
        championMedal = LoadSingle(FOLDER_PATH + "/Resources/Gold.png");
        championMedal = LoadSingle(FOLDER_PATH + "/Resources/Silver.png");
        championMedal = LoadSingle(FOLDER_PATH + "/Resources/Bronze.png");
    }


    private static Sprite? LoadSingle(string path)
    {
        try
        {
            byte[] pngData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(pngData);

            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }
        catch (Exception e)
        {
            Plugin.L.LogError(e);
            return null;
        }
    }

    public static Sprite GetSpriteFromMedal(Medal medal)
    {
        switch (medal)
        {
            case Medal.Bronze:
                return bronzeMedal;
            case Medal.Silver:
                return silverMedal;
            case Medal.Gold:
                return goldMedal;
            case Medal.Champion:
                return championMedal;
        }

        return bronzeMedal;
    }

}
