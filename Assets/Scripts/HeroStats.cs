// Assets/Scripts/Gameplay/HeroStats.cs
using UnityEngine;

public class HeroStats : MonoBehaviour
{
    public int Attack = 4;          // начальная атака (не ваншотит)
    public float AttackPerSecond = 1.0f; // 1 выстрел/сек
    public int MaxHP = 30;

    public void ApplyAttack(int add) => Attack = Mathf.Max(1, Attack + add);
    public void ApplyAttackSpeed(float add) => AttackPerSecond = Mathf.Max(0.2f, AttackPerSecond + add);
    public void ApplyHP(int add)
    {
        MaxHP = Mathf.Max(1, MaxHP + add);
        var hp = GetComponent<Health>();
        hp.SetMax(MaxHP, heal: true);
    }
}
