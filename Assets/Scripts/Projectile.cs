// Assets/Scripts/Gameplay/Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private int damage;
    private Vector3 start;
    private float t;
    private float flightTime;
    private float arcHeight;

    public void Init(Transform target, int damage, float speed = 8f, float arc = 1.5f)
    {
        this.target = target;
        this.damage = damage;
        start = transform.position;
        arcHeight = arc;
        float dist = Vector2.Distance(start, target.position);
        flightTime = Mathf.Max(0.15f, dist / speed);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }
        t += Time.deltaTime / flightTime;
        float clamped = Mathf.Clamp01(t);
        Vector3 a = start;
        Vector3 b = target.position;
        Vector3 pos = Vector3.Lerp(a, b, clamped);
        // добавляем параболу в Y
        pos.y += arcHeight * 4f * clamped * (1f - clamped);
        transform.position = pos;

        if (clamped >= 1f)
        {
            var hp = target.GetComponent<Health>();
            if (hp != null)
            {
                hp.Damage(damage);
                DamagePopup.Show(target.position, damage);
            }
            Destroy(gameObject);
        }
    }
}
