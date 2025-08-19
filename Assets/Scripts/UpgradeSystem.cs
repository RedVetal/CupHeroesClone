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
        hero.SetPaused(true);

        // очистка старого
        if (PanelRoot != null) Destroy(PanelRoot);

        // 1) Полупрозрачная вуаль по ВСЕЙ SafeArea (НЕ блокирует клики)
        var veil = new GameObject("UpgradeVeil");
        var veilImg = veil.AddComponent<UnityEngine.UI.Image>();
        veilImg.sprite = UIPrimitives.UISprite;
        veilImg.type = UnityEngine.UI.Image.Type.Simple;
        veilImg.color = new Color(0, 0, 0, 0.35f);
        veilImg.raycastTarget = false; // важно!
        veil.transform.SetParent(hud.SafeAreaRoot, false);
        var vrt = veil.GetComponent<RectTransform>();
        vrt.anchorMin = Vector2.zero; vrt.anchorMax = Vector2.one;
        vrt.offsetMin = vrt.offsetMax = Vector2.zero;

        // 2) Панель апгрейда — ТОЛЬКО верхняя половина экрана
        PanelRoot = new GameObject("UpgradePanel");
        PanelRoot.transform.SetParent(hud.SafeAreaRoot, false);
        var rt = PanelRoot.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.05f, 0.62f);   // низ панели = 55% высоты
        rt.anchorMax = new Vector2(0.95f, 0.95f);   // верх панели = 95% высоты
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var img = PanelRoot.AddComponent<UnityEngine.UI.Image>();
        img.sprite = UIPrimitives.UISprite;
        img.type = UnityEngine.UI.Image.Type.Simple;
        img.color = new Color(0, 0, 0, 0.6f);
        img.raycastTarget = true; // кликаем по самой панели/картам

        // Заголовок
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(PanelRoot.transform, false);
        var title = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
        var trt = title.GetComponent<RectTransform>();
        trt.anchorMin = new Vector2(0.1f, 0.85f);
        trt.anchorMax = new Vector2(0.9f, 0.98f);
        trt.offsetMin = trt.offsetMax = Vector2.zero;
        title.alignment = TMPro.TextAlignmentOptions.Center;
        title.fontSize = 44;
        title.text = "Выбери карту улучшения";

        // Сетка карточек
        var grid = new GameObject("Grid").AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        grid.transform.SetParent(PanelRoot.transform, false);
        var grt = grid.GetComponent<RectTransform>();
        grt.anchorMin = new Vector2(0.05f, 0.60f);
        grt.anchorMax = new Vector2(0.95f, 0.84f);
        grt.offsetMin = grt.offsetMax = Vector2.zero;
        grid.childControlWidth = grid.childControlHeight = true;
        grid.childForceExpandWidth = grid.childForceExpandHeight = true;
        grid.spacing = 20;

        MakeCard(grid.transform, "ATK +3", () => { hero.GetStats().ApplyAttack(3); CloseAndNext(); });
        MakeCard(grid.transform, "AS +0.5", () => { hero.GetStats().ApplyAttackSpeed(0.5f); CloseAndNext(); });
        MakeCard(grid.transform, "HP +15", () => { hero.GetStats().ApplyHP(15); CloseAndNext(); });

        // ЛОКАЛЬНАЯ функция, чтобы убрать вуаль вместе с панелью
        void CloseAndNext()
        {
            hero.SetPaused(false);
            Destroy(PanelRoot);
            Destroy(veil);
            hud.RefreshHeroStats();
            waves.NextWave();
        }
    }

    void MakeCard(Transform parent, string caption, System.Action onClick)
    {
        var go = new GameObject("Card");
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.sprite = UIPrimitives.UISprite;
        img.type = Image.Type.Simple;
        img.color = new Color(0f, 0.4f, 0f, 0.9f);
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

}