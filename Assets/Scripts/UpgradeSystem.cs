// Assets/Scripts/Gameplay/UpgradeSystem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSystem : MonoBehaviour
{
    public GameObject PanelRoot { get; private set; }
    private HeroController hero;
    private WaveManager waves;
    private UIHud hud;

    public void Init(HeroController h, WaveManager w, UIHud ui)
    {
        hero = h; waves = w; hud = ui;
        waves.OnWaveCleared += ShowPanel;
    }

    void ShowPanel()
    {
        if (PanelRoot != null) Destroy(PanelRoot);

        PanelRoot = new GameObject("UpgradePanel");
        hero.SetPaused(true);                         // changes
        var canvas = hud.RootCanvas;
        PanelRoot.transform.SetParent(canvas.transform, false);

        var rt = PanelRoot.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.1f, 0.1f);
        rt.anchorMax = new Vector2(0.9f, 0.9f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var img = PanelRoot.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.6f);

        var grid = new GameObject("Grid").AddComponent<HorizontalLayoutGroup>();
        grid.transform.SetParent(PanelRoot.transform, false);
        var grt = grid.GetComponent<RectTransform>();
        grt.anchorMin = new(0.1f, 0.3f); grt.anchorMax = new(0.9f, 0.7f);
        grt.offsetMin = grt.offsetMax = Vector2.zero;
        grid.childControlHeight = grid.childControlWidth = true; grid.spacing = 20;

        // три карты
        MakeCard(grid.transform, "ATK +3", () => { hero.GetStats().ApplyAttack(3); CloseAndNext(); });
        MakeCard(grid.transform, "AS +0.5", () => { hero.GetStats().ApplyAttackSpeed(0.5f); CloseAndNext(); });
        MakeCard(grid.transform, "HP +15", () => { hero.GetStats().ApplyHP(15); CloseAndNext(); });

        // заголовок
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(PanelRoot.transform, false);
        var title = titleObj.AddComponent<TextMeshProUGUI>();
        var trt = title.GetComponent<RectTransform>();
        trt.anchorMin = new(0.1f, 0.75f); trt.anchorMax = new(0.9f, 0.95f);
        trt.offsetMin = trt.offsetMax = Vector2.zero;
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize = 48;
        title.text = "Выбери карту улучшения";
    }

    void MakeCard(Transform parent, string caption, System.Action onClick)
    {
        var go = new GameObject("Card");
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0.9f);
        var btn = go.AddComponent<Button>();
        btn.onClick.AddListener(() => onClick?.Invoke());

        var txt = new GameObject("Text").AddComponent<TextMeshProUGUI>();
        txt.transform.SetParent(go.transform, false);
        txt.alignment = TextAlignmentOptions.Center;
        txt.fontSize = 40;
        txt.text = caption;
        var tr = txt.GetComponent<RectTransform>();
        tr.anchorMin = new(0.1f, 0.1f); tr.anchorMax = new(0.9f, 0.9f);
        tr.offsetMin = tr.offsetMax = Vector2.zero;

        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 0);
    }

    void CloseAndNext()
    {
        Destroy(PanelRoot);
        hud.RefreshHeroStats(); // сразу видно бафф
        hero.SetPaused(false);  // changed
        waves.NextWave();
    }
}
