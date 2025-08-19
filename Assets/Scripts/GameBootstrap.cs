// Assets/Scripts/GameBootstrap.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        // ������
        var cam = Camera.main;
        GameObject camGo;
        if (cam == null)
        {
            camGo = new GameObject("MainCamera");
            cam = camGo.AddComponent<Camera>();
            cam.tag = "MainCamera";
        }
        else camGo = cam.gameObject;

        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.6f, 0.8f, 1f, 1f);


        // ����� (������ ��� ������)
        var ground = new GameObject("Ground");
        var gsr = ground.AddComponent<SpriteRenderer>();
        gsr.sprite = SpriteFactory.MakeRectSprite(new Color(0.2f, 0.7f, 0.25f, 1f));
        gsr.sortingOrder = 0;
        ground.transform.localScale = new Vector3(1000f, 1.5f, 1f);  // changed
        ground.transform.position = new Vector3(0, -0.75f, 0);

        // ����� (����� ������� �� ���� ��������������)
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

        // ������ ������� �� ������
        var follow = camGo.GetComponent<Follow2D>();
        if (follow == null) follow = camGo.AddComponent<Follow2D>();
        follow.Target = heroGo.transform;
        follow.ViewportX = 0.38f;
        follow.ViewportY = 0.38f;
        follow.LeadXWorld = 2.0f;
        follow.SmoothTime = 0.15f;



        // �������� ����
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

        // ������������� ����� (����� ������� ����)
        hero.Init(waves);

        // EventSystem ��� ������
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // ������ �����
        waves.StartFirstWave();
    }
}

// ������� �������� ��� ������
// ����������� ���� � �������� ����� �������� (0..1 �� X/Y).
// ���� ������ ����� -> ������� ������, ����� ������ SmoothDamp.
public class Follow2D : MonoBehaviour
{
    public Transform Target;

    [Range(0f, 1f)] public float ViewportX = 0.38f; // ������� ����� (����� ������ ��� ������)
    [Range(0f, 1f)] public float ViewportY = 0.38f; // ���� ���� HUD (HUD ~0.28)
    public float LeadXWorld = 2.0f;                 // ���������� ����� � ����
    public Vector3 Offset = new(0, 0, -10f);        // Z ������
    public float SmoothTime = 0.15f;                // ����� ����������� (���)

    Camera cam;
    Vector3 vel;            // ��� SmoothDamp
    int snapFrames = 2;     // ������ 2 ����� � ������ ����

    void Awake() { cam = GetComponent<Camera>(); }

    Vector3 ComputeDesired()
    {
        var t = Target ? Target.position : Vector3.zero;
        t.x += LeadXWorld;

        float ortho = cam.orthographicSize;
        float halfW = ortho * cam.aspect;

        float camX = t.x - Mathf.Lerp(-halfW, halfW, ViewportX);
        float camY = t.y - Mathf.Lerp(-ortho, ortho, ViewportY);

        return new Vector3(camX, camY, 0f) + Offset;
    }

    void OnEnable()
    {
        if (!cam) cam = GetComponent<Camera>();
        // ��������� ����� ���� �����, ����� �� ���� ������ ��� ������/������ �����
        var p = ComputeDesired();
        transform.position = p;
        vel = Vector3.zero;
        snapFrames = 2;
    }

    void LateUpdate()
    {
        if (!Target || !cam || !cam.orthographic) return;
        var desired = ComputeDesired();

        if (snapFrames > 0)         // ������ ����� � ������ ��������
        {
            transform.position = desired;
            snapFrames--;
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref vel, SmoothTime);
    }
}
