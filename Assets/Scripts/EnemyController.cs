// Assets/Scripts/Gameplay/EnemyController.cs
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    public float Speed = 1.5f;
    public int ContactDamage = 3;
    public float AttackCooldown = 1.2f;
    public Transform Hero;
    private float cd;
    private Health hp;

    public System.Action<EnemyController> OnDead;

    void Awake()
    {
        hp = GetComponent<Health>();
        hp.OnDied += () => { OnDead?.Invoke(this); Destroy(gameObject); };
    }

    void Update()
    {
        if (Hero == null) return;

        float dx = Hero.position.x - transform.position.x;
        float dist = Mathf.Abs(dx);

        if (dist > 1.2f)
        {
            // идём к герою
            float dir = Mathf.Sign(dx);
            transform.position += Vector3.right * dir * Speed * Time.deltaTime;
        }
        else
        {
            // в ближнем бою
            cd -= Time.deltaTime;
            if (cd <= 0f)
            {
                var heroHp = Hero.GetComponent<Health>();
                heroHp?.Damage(ContactDamage);
                DamagePopup.Show(Hero.position, ContactDamage);
                cd = AttackCooldown;
            }
        }
    }
}
