// Assets/Scripts/Util/SafeAreaFitter.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    Rect last;
    RectTransform rt;

    void Awake() => rt = GetComponent<RectTransform>();
    void OnEnable() => Apply();
    void Update() { if (last != Screen.safeArea) Apply(); }

    void Apply()
    {
        var sa = Screen.safeArea;
        last = sa;

        var anchorMin = sa.position;
        var anchorMax = sa.position + sa.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
    }
}
