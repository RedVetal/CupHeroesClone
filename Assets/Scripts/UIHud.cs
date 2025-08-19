// Assets/Scripts/UI/UIHud.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHud : MonoBehaviour
{
    // ‘иксированна€ дол€ экрана под HUD
    const float HUD_HEIGHT = 0.28f;

    public RectTransform SafeAreaRoot { get; private set; }
    public Canvas RootCanvas { get; private set; }

    TextMeshProUGUI txtCoins, txtAtk, txtAspd, txtHP;

    private WaveManager waves;
    private HeroController hero;

    // держим ссылку на нижнюю панель и р€д (дл€ фиксации)
    RectTransform bottomPanelRt;

    public void Init(WaveManager w, HeroController h)
    {
        waves = w; hero = h;
        BuildCanvas();
        RefreshAll();

        waves.OnStateChanged += RefreshAll;
        var heroHp = hero.GetComponent<Health>();
        heroHp.OnChanged += (c, m) => RefreshHP();
    }

    void BuildCanvas()
    {
        // 1) —татичный Overlay-канвас Ч вообще не зависит от камеры
        var canvasGo = new GameObject("CanvasHUD");
        RootCanvas = canvasGo.AddComponent<Canvas>();
        RootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 1f; // портрет
        canvasGo.AddComponent<GraphicRaycaster>();

        // 2) SafeAreaRoot Ч просто фуллскрин (никаких SafeAreaFitter!)
        var safeGo = new GameObject("SafeArea");
        SafeAreaRoot = safeGo.AddComponent<RectTransform>();
        SafeAreaRoot.SetParent(canvasGo.transform, false);
        SafeAreaRoot.anchorMin = Vector2.zero;
        SafeAreaRoot.anchorMax = Vector2.one;
        SafeAreaRoot.offsetMin = SafeAreaRoot.offsetMax = Vector2.zero;

        // 3) Ќижн€€ панель HUD фиксированной высоты
        var panelImg = new GameObject("BottomPanel").AsPanel(SafeAreaRoot, new Color(0.08f, 0.08f, 0.1f, 0.85f));
        bottomPanelRt = panelImg.GetComponent<RectTransform>();
        bottomPanelRt.anchorMin = new Vector2(0f, 0f);
        bottomPanelRt.anchorMax = new Vector2(1f, HUD_HEIGHT);
        bottomPanelRt.offsetMin = bottomPanelRt.offsetMax = Vector2.zero;

        // 4) √оризонтальный р€д статов
        var row = new GameObject("Row").AddComponent<HorizontalLayoutGroup>();
        row.transform.SetParent(panelImg.transform, false);
        var rowRt = row.GetComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(0.05f, 0.1f);
        rowRt.anchorMax = new Vector2(0.95f, 0.9f);
        rowRt.offsetMin = rowRt.offsetMax = Vector2.zero;
        row.childAlignment = TextAnchor.MiddleCenter;
        row.spacing = 40;
        row.childControlHeight = row.childControlWidth = true;
        row.childForceExpandHeight = row.childForceExpandWidth = true;

        txtCoins = MakeStat(row.transform, "Coins");
        txtAtk = MakeStat(row.transform, "Attack");
        txtAspd = MakeStat(row.transform, "Atk/sec");
        txtHP = MakeStat(row.transform, "HP");
    }

    // ∆®—“ јя ‘» —ј÷»я: если какой-то лэйаут/эмул€ци€ Ђподнимаетї панель Ч возвращаем €кор€ назад
    void LateUpdate()
    {
        if (!bottomPanelRt) return;
        if (bottomPanelRt.anchorMin != new Vector2(0f, 0f) ||
            bottomPanelRt.anchorMax != new Vector2(1f, HUD_HEIGHT) ||
            bottomPanelRt.offsetMin != Vector2.zero ||
            bottomPanelRt.offsetMax != Vector2.zero)
        {
            bottomPanelRt.anchorMin = new Vector2(0f, 0f);
            bottomPanelRt.anchorMax = new Vector2(1f, HUD_HEIGHT);
            bottomPanelRt.offsetMin = bottomPanelRt.offsetMax = Vector2.zero;
        }
    }

    TextMeshProUGUI MakeStat(Transform parent, string label)
    {
        var boxImg = new GameObject(label).AsPanel(parent, new Color(1, 1, 1, 0.08f));
        var layout = boxImg.gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = 240;
        layout.flexibleWidth = 1;

        var title = new GameObject("Title").AddComponent<TextMeshProUGUI>();
        title.transform.SetParent(boxImg.transform, false);
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize = 36; title.text = label;
        title.color = new Color(0.8f, 0f, 0.8f);
        var tr1 = title.GetComponent<RectTransform>();
        tr1.anchorMin = new(0.1f, 0.55f); tr1.anchorMax = new(0.9f, 0.95f);
        tr1.offsetMin = tr1.offsetMax = Vector2.zero;

        var value = new GameObject("Value").AddComponent<TextMeshProUGUI>();
        value.transform.SetParent(boxImg.transform, false);
        value.alignment = TextAlignmentOptions.Center;
        value.fontSize = 52; value.text = "-";
        value.color = new Color(0.5f, 0f, 1f);
        var tr2 = value.GetComponent<RectTransform>();
        tr2.anchorMin = new(0.1f, 0.05f); tr2.anchorMax = new(0.9f, 0.5f);
        tr2.offsetMin = tr2.offsetMax = Vector2.zero;

        return value;
    }

    void RefreshAll()
    {
        txtCoins.text = waves.SoftCurrency.ToString();
        RefreshHeroStats();
        RefreshHP();
    }

    public void RefreshHeroStats()
    {
        var s = hero.GetStats();
        txtAtk.text = s.Attack.ToString();
        txtAspd.text = s.AttackPerSecond.ToString("0.0");
    }

    void RefreshHP()
    {
        var h = hero.GetComponent<Health>();
        txtHP.text = $"{h.CurrentHP}/{h.MaxHP}";
    }
}
