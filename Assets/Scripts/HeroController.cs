// Assets/Scripts/Gameplay/HeroController.cs
using UnityEngine;

[RequireComponent(typeof(HeroStats))]
[RequireComponent(typeof(Health))]
public class HeroController : MonoBehaviour
{
    public float RunSpeed = 2.2f;
    public float AttackRange = 7f;
    public Transform ProjectileRoot;

    private HeroStats stats;
    private Health hp;
    private float shootCd;
    private WaveManager waves;
    private bool paused = false;   // changes
    public void SetPaused(bool v) { paused = v; }   // changes

    public void Init(WaveManager w) => waves = w;

    void Awake()
    {
        stats = GetComponent<HeroStats>();
        hp = GetComponent<Health>();
        hp.SetMax(stats.MaxHP, heal: true);
    }

    void Update()
    {
        if (paused) return;    // changes

        Transform target = AcquireTarget();

        bool enemyAhead = target != null && (target.position.x > transform.position.x - 0.1f);
        if (!enemyAhead)
        {
            // бежим вправо
            transform.position += Vector3.right * RunSpeed * Time.deltaTime;
        }
        else
        {
            // стреляем
            shootCd -= Time.deltaTime;
            if (shootCd <= 0f)
            {
                Shoot(target);
                shootCd = 1f / Mathf.Max(0.1f, stats.AttackPerSecond);
            }
        }
    }

    Transform AcquireTarget()
    {
        if (waves == null) return null;
        float best = float.MaxValue;
        Transform bestT = null;
        foreach (var e in waves.ActiveEnemies)
        {
            if (e == null) continue;
            float dx = e.transform.position.x - transform.position.x;
            if (dx < -0.2f) continue; // только спереди
            float d = Mathf.Abs(dx);
            if (d < AttackRange && d < best) { best = d; bestT = e.transform; }
        }
        return bestT;
    }

    void Shoot(Transform target)
    {
        var go = new GameObject("Projectile");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.MakeRectSprite(new Color(0.7f, 0.9f, 1f, 1f));
        sr.sortingOrder = 50;
        go.transform.position = (ProjectileRoot ? ProjectileRoot.position : transform.position + new Vector3(0.4f, 0.6f, 0));
        go.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
        var pr = go.AddComponent<Projectile>();
        pr.Init(target, stats.Attack, speed: 8f, arc: 1.2f);
    }

    public HeroStats GetStats() => stats;
}
