// Assets/Scripts/UI/UIHud.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHud : MonoBehaviour
{
    public RectTransform SafeAreaRoot { get; private set; }
    public Canvas RootCanvas { get; private set; }

    TextMeshProUGUI txtCoins, txtAtk, txtAspd, txtHP;

    private WaveManager waves;
    private HeroController hero;

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
        var canvasGo = new GameObject("CanvasHUD");
        RootCanvas = canvasGo.AddComponent<Canvas>();

        // 1) –ендер через основную камеру Ч железно видно и в Game, и в Simulator
        var cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>();
        RootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        RootCanvas.worldCamera = cam;
        RootCanvas.planeDistance = 1f;    // близко к камере
        RootCanvas.sortingOrder = 1000;  // поверх всего

        var scaler = canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 1f;

        canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();



        // Safe Area wrapper
        var safe = new GameObject("SafeArea").AddComponent<SafeAreaFitter>();
#if UNITY_EDITOR
        // ѕолупрозрачна€ плашка по всей SafeArea Ч чтобы 100% увидеть UI
        var dbg = new GameObject("DBG_SafeAreaFill").AsPanel(safe.transform, new Color(1, 0, 0, 0.05f));
        var dbgRt = dbg.GetComponent<RectTransform>();
        dbgRt.anchorMin = Vector2.zero; dbgRt.anchorMax = Vector2.one;
        dbgRt.offsetMin = dbgRt.offsetMax = Vector2.zero;
#endif

        SafeAreaRoot = safe.GetComponent<RectTransform>();
        safe.transform.SetParent(canvasGo.transform, false);
        var safeRt = safe.GetComponent<RectTransform>();
        safeRt.anchorMin = Vector2.zero; safeRt.anchorMax = Vector2.one;
        safeRt.offsetMin = safeRt.offsetMax = Vector2.zero;

        // Ќижн€€ половина UI панель
        var panel = new GameObject("BottomPanel").AsPanel(safe.transform, new Color(0.08f, 0.08f, 0.1f, 0.85f));

        var prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = new Vector2(0f, 0f);
        prt.anchorMax = new Vector2(1f, 0.45f); // ~нижн€€ половина
        prt.offsetMin = prt.offsetMax = Vector2.zero;

        // ¬нутри Ч горизонтальна€ строка метрик
        var row = new GameObject("Row").AddComponent<HorizontalLayoutGroup>();
        row.transform.SetParent(panel.transform, false);
        var rowRt = row.GetComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(0.05f, 0.1f);
        rowRt.anchorMax = new Vector2(0.95f, 0.9f);
        rowRt.offsetMin = rowRt.offsetMax = Vector2.zero;
        row.childAlignment = TextAnchor.MiddleCenter;
        row.spacing = 40;
        row.childControlHeight = row.childControlWidth = true;

        txtCoins = MakeStat(row.transform, "Coins");
        txtAtk = MakeStat(row.transform, "Attack");
        txtAspd = MakeStat(row.transform, "Atk/sec");
        txtHP = MakeStat(row.transform, "HP");
    }

    TextMeshProUGUI MakeStat(Transform parent, string label)
    {
        var boxImg = new GameObject(label).AsPanel(parent, new Color(1, 1, 1, 0.08f));
        var box = boxImg;
        var layout = box.gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = 240;

        var title = new GameObject("Title").AddComponent<TextMeshProUGUI>();
        title.transform.SetParent(box.transform, false);
        title.alignment = TextAlignmentOptions.Center;
        title.fontSize = 36; title.text = label;
        var tr1 = title.GetComponent<RectTransform>();
        tr1.anchorMin = new(0.1f, 0.55f); tr1.anchorMax = new(0.9f, 0.95f);
        tr1.offsetMin = tr1.offsetMax = Vector2.zero;

        var value = new GameObject("Value").AddComponent<TextMeshProUGUI>();
        value.transform.SetParent(box.transform, false);
        value.alignment = TextAlignmentOptions.Center;
        value.fontSize = 52; value.text = "-";
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
