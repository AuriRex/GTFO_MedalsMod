using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using MedalsMod.Data;
using TMPro;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MedalsMod.GameObj;

internal partial class MedalInfo
{
    [HideFromIl2Cpp]
    private TextMeshPro TimeText
    {
        get => _timeText.Get();
        set => _timeText.Set(value);
    }

    [HideFromIl2Cpp]
    private TextMeshPro MedalText
    {
        get => _medalText.Get();
        set => _medalText.Set(value);
    }
    
    [HideFromIl2Cpp]
    private SpriteRenderer SpriteRenderer
    {
        get => _spriteRenderer.Get();
        set => _spriteRenderer.Set(value);
    }
    
    private Il2CppReferenceField<TextMeshPro> _timeText = null!;
    private Il2CppReferenceField<TextMeshPro> _medalText = null!;
    private Il2CppReferenceField<SpriteRenderer> _spriteRenderer = null!;
    
    public void Setup(Medal medal, string time)
    {
        MedalText = transform.FindChild(GO_NAME_NAME_TMP).GetComponent<TextMeshPro>();
        TimeText = transform.FindChild(GO_NAME_TIME_TMP).GetComponent<TextMeshPro>();
        SpriteRenderer = transform.FindChild(GO_NAME_SPRITE_RENDERER).GetComponent<SpriteRenderer>();
        
        MedalText.SetText(medal.ToString());
        TimeText.SetText(time);
        SpriteRenderer.sprite = MedalImages.GetSpriteFromMedal(medal);
    }

    public void Reposition(int index)
    {
        transform.localPosition = Vector3.up * 50 * index;
    }
}