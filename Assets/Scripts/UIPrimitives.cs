using UnityEngine;
using UnityEngine.UI;

public static class UIPrimitives
{
    private static Sprite _uiSprite;
    public static Sprite UISprite => _uiSprite ??= SpriteFactory.MakeRectSprite(Color.white, 16);

    public static Image AsPanel(this GameObject go, Transform parent, Color? tint = null)
    {
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.sprite = UISprite;
        img.type = Image.Type.Simple;
        if (tint.HasValue) img.color = tint.Value;
        return img;
    }
}
