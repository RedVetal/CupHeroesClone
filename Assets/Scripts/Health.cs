// Assets/Scripts/Combat/Health.cs
using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public int MaxHP = 20;
    public int CurrentHP { get; private set; }

    public event Action<int, int> OnChanged; // current, max
    public event Action<int> OnDamaged; // dmg
    public event Action OnDied;

    void Awake() => CurrentHP = MaxHP;

    public void Damage(int dmg)
    {
        if (dmg <= 0 || CurrentHP <= 0) return;
        int prev = CurrentHP;
        CurrentHP = Mathf.Max(0, CurrentHP - dmg);
        OnDamaged?.Invoke(prev - CurrentHP);
        OnChanged?.Invoke(CurrentHP, MaxHP);
        if (CurrentHP == 0) OnDied?.Invoke();
    }

    public void HealFull()
    {
        CurrentHP = MaxHP;
        OnChanged?.Invoke(CurrentHP, MaxHP);
    }

    public void SetMax(int newMax, bool heal = true)
    {
        MaxHP = Mathf.Max(1, newMax);
        if (heal) CurrentHP = MaxHP;
        OnChanged?.Invoke(CurrentHP, MaxHP);
    }
}
