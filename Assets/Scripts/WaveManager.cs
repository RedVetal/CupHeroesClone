// Assets/Scripts/Gameplay/WaveManager.cs
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<EnemyController> ActiveEnemies { get; } = new();

    public int WaveIndex { get; private set; } = 0;
    public int SoftCurrency { get; private set; } = 0;

    public Transform Hero;

    public System.Action OnWaveCleared;
    public System.Action OnStateChanged;

    public void StartFirstWave() => SpawnWave(WaveIndex);

    void SpawnWave(int index)
    {
        ActiveEnemies.Clear();
        int count = 3 + index; // каждую волну +1 враг
        float startX = (Hero != null ? Hero.position.x : 0f) + 10f;
        for (int i = 0; i < count; i++)
        {
            float x = startX + i * 2.2f;
            var e = SpawnEnemy(new Vector3(x, 0, 0));
            ActiveEnemies.Add(e);
        }
        OnStateChanged?.Invoke();
    }

    EnemyController SpawnEnemy(Vector3 pos)
    {
        var go = new GameObject("Enemy");
        go.transform.position = pos;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.MakeRectSprite(Color.red);
        sr.sortingOrder = 10;
        go.transform.localScale = new Vector3(1f, 1f, 1f);

        var hp = go.AddComponent<Health>();
        hp.MaxHP = 12; // базовое Ч не ваншотитс€
        hp.HealFull();
        go.AddComponent<HPBar2D>();

        var ai = go.AddComponent<EnemyController>();
        ai.Speed = 1.6f;
        ai.ContactDamage = 2;
        ai.AttackCooldown = 1.1f;
        ai.Hero = Hero;
        ai.OnDead += OnEnemyDead;
        return ai;
    }

    void OnEnemyDead(EnemyController e)
    {
        SoftCurrency += 1; // 1 монета за убийство
        ActiveEnemies.Remove(e);

        if (ActiveEnemies.Count == 0)
        {
            WaveIndex++;
            OnWaveCleared?.Invoke();
        }
        OnStateChanged?.Invoke();
    }

    public void NextWave() => SpawnWave(WaveIndex);
}
