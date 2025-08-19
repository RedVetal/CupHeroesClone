// Assets/Scripts/Util/SpriteFactory.cs
using UnityEngine;

public static class SpriteFactory
{
    // Создаёт одноцветный Sprite нужного размера в юнитах
    public static Sprite MakeRectSprite(Color color, int px = 32)
    {
        var tex = new Texture2D(px, px, TextureFormat.RGBA32, false);
        var fill = new Color32((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), (byte)(color.a * 255));
        var data = new Color32[px * px];
        for (int i = 0; i < data.Length; i++) data[i] = fill;
        tex.SetPixels32(data);
        tex.Apply();
        var sp = Sprite.Create(tex, new Rect(0, 0, px, px), new Vector2(0.5f, 0.5f), px);
        sp.name = $"Rect_{px}px_{color}";
        return sp;
    }
}
