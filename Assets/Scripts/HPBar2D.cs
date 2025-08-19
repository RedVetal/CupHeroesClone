// Assets/Scripts/Combat/HPBar2D.cs
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HPBar2D : MonoBehaviour
{
    public Vector3 Offset = new(0, 1.2f, 0);
    private Transform barRoot;
    private Transform fill;
    private Camera cam;
    private Health hp;

    void Start()
    {
        cam = Camera.main;
        hp = GetComponent<Health>();

        barRoot = new GameObject("HPBar").transform;
        barRoot.SetParent(transform, false);
        barRoot.localPosition = Offset;

        var back = new GameObject("Back").AddComponent<SpriteRenderer>();
        back.transform.SetParent(barRoot, false);
        back.sprite = SpriteFactory.MakeRectSprite(new Color(0, 0, 0, 0.6f));
        back.drawMode = SpriteDrawMode.Sliced;
        back.size = new Vector2(1.4f, 0.18f);
        back.sortingOrder = 100;

        var fillSr = new GameObject("Fill").AddComponent<SpriteRenderer>();
        fill = fillSr.transform;
        fill.SetParent(barRoot, false);
        fillSr.sprite = SpriteFactory.MakeRectSprite(new Color(0.2f, 1f, 0.2f, 0.95f));
        fillSr.drawMode = SpriteDrawMode.Sliced;
        fillSr.size = new Vector2(1.36f, 0.14f);
        fillSr.sortingOrder = 101;
        fill.localPosition = new Vector3(-0.02f, 0, 0);

        hp.OnChanged += OnHpChanged;
        OnHpChanged(hp.CurrentHP, hp.MaxHP);
    }

    void LateUpdate()
    {
        if (cam) barRoot.rotation = Quaternion.identity; // без поворота в 2D
    }

    void OnHpChanged(int current, int max)
    {
        float t = Mathf.Clamp01(max > 0 ? (float)current / max : 0f);
        var sr = fill.GetComponent<SpriteRenderer>();
        sr.size = new Vector2(1.36f * t, 0.14f);
        sr.color = Color.Lerp(new Color(1f, 0.25f, 0.2f, 0.95f), new Color(0.2f, 1f, 0.2f, 0.95f), t);
    }
}
