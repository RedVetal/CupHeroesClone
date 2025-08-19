using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform rt;
    Rect appliedSa = Rect.zero;
    Vector2 appliedRes = Vector2.zero;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        Apply(force: true);
    }

    void OnEnable() => Apply(force: true);

    void Update()
    {
        // В РЕДАКТОРЕ не трогаем ничего — используем фуллскрин (никаких чёлок)
#if UNITY_EDITOR
        return;
#else
        if (Screen.width != (int)appliedRes.x || Screen.height != (int)appliedRes.y || Screen.safeArea != appliedSa)
            Apply(force:false);
#endif
    }

    void Apply(bool force)
    {
        Rect sa = Screen.safeArea;
        if (sa.width <= 0 || sa.height <= 0)
            sa = new Rect(0, 0, Screen.width, Screen.height);

#if UNITY_EDITOR
        // Всегда весь экран в эдиторе — UI не исчезает и не зацикливается
        sa = new Rect(0, 0, Screen.width, Screen.height);
#endif

        if (!force && sa == appliedSa && appliedRes.x == Screen.width && appliedRes.y == Screen.height)
            return;

        appliedSa = sa;
        appliedRes = new Vector2(Screen.width, Screen.height);

        Vector2 min = sa.position;
        Vector2 max = sa.position + sa.size;
        min.x /= Screen.width; min.y /= Screen.height;
        max.x /= Screen.width; max.y /= Screen.height;

        // Если значения те же — ничего не меняем (чтобы не триггерить лишние события)
        if (Approximately(rt.anchorMin, min) && Approximately(rt.anchorMax, max))
            return;

        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    bool Approximately(Vector2 a, Vector2 b) =>
        Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
}
