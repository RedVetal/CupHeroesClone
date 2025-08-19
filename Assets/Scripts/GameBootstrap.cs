// Assets/Scripts/GameBootstrap.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        // Камера
        var camGo = new GameObject("MainCamera");
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.6f, 0.8f, 1f, 1f);
        cam.tag = "MainCamera";

        // Земля (просто фон полоса)
        var ground = new GameObject("Ground");
        var gsr = ground.AddComponent<SpriteRenderer>();
        gsr.sprite = SpriteFactory.MakeRectSprite(new Color(0.2f, 0.7f, 0.25f, 1f));
        gsr.sortingOrder = 0;
        ground.transform.localScale = new Vector3(100f, 1.5f, 1f);
        ground.transform.position = new Vector3(0, -0.75f, 0);

        // Герой (синяя капсула на базе прямоугольника)
        var heroGo = new GameObject("Hero");
        var hsr = heroGo.AddComponent<SpriteRenderer>();
        hsr.sprite = SpriteFactory.MakeRectSprite(new Color(0.3f, 0.5f, 1f, 1f));
        hsr.sortingOrder = 10;
        heroGo.transform.localScale = new Vector3(0.9f, 1.6f, 1f);
        heroGo.transform.position = new Vector3(0, 0, 0);

        var heroHp = heroGo.AddComponent<Health>();
        var heroStats = heroGo.AddComponent<HeroStats>();
        heroGo.AddComponent<HPBar2D>();
        var hero = heroGo.AddComponent<HeroController>();

        // Камера следует за героем (простенько)
        camGo.AddComponent<Follow2D>().Target = heroGo.transform;

        // Менеджер волн
        var wavesGo = new GameObject("WaveManager");
        var waves = wavesGo.AddComponent<WaveManager>();
        waves.Hero = heroGo.transform;

        // HUD
        var uiGo = new GameObject("UI");
        var hud = uiGo.AddComponent<UIHud>();
        hud.Init(waves, hero);

        // Upgrade System
        var upGo = new GameObject("UpgradeSystem");
        var ups = upGo.AddComponent<UpgradeSystem>();
        ups.Init(hero, waves, hud);

        // Инициализация героя (перед стартом волн)
        hero.Init(waves);

        // EventSystem для кнопок
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();

        // Первая волна
        waves.StartFirstWave();
    }
}

// Простой фолловер для камеры
public class Follow2D : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset = new(0, 2.5f, -10f);
    void LateUpdate()
    {
        if (!Target) return;
        var p = Target.position + Offset;
        p.y = 2.5f; // фикс по высоте
        transform.position = p;
    }
}
