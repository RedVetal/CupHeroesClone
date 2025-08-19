using UnityEngine;

[RequireComponent(typeof(Health))]
public class HPBar2D : MonoBehaviour
{
    public Vector3 Offset = new(0, 1.2f, 0);

    // мирные размеры в юнитах
    const float WIDTH = 1.4f;
    const float HEIGHT = 0.18f;
    const float FILL_PAD = 0.02f;

    private Transform barRoot;
    private Transform backTf;
    private Transform fillTf;
    private SpriteRenderer fillSr;
    private Health hp;

    void Start()
    {
        hp = GetComponent<Health>();

        barRoot = new GameObject("HPBar").transform;
        barRoot.SetParent(transform, false);
        barRoot.localPosition = Offset;

        // фон
        var backSr = new GameObject("Back").AddComponent<SpriteRenderer>();
        backTf = backSr.transform;
        backTf.SetParent(barRoot, false);
        backSr.sprite = SpriteFactory.MakeRectSprite(new Color(0, 0, 0, 0.6f));
        backSr.sortingOrder = 100;
        // 1x1 спрайт Ч просто масштабируем до нужного размера
        backTf.localScale = new Vector3(WIDTH, HEIGHT, 1f);

        // заполнение
        fillSr = new GameObject("Fill").AddComponent<SpriteRenderer>();
        fillTf = fillSr.transform;
        fillTf.SetParent(barRoot, false);
        fillSr.sprite = SpriteFactory.MakeRectSprite(new Color(0.2f, 1f, 0.2f, 0.95f));
        fillSr.sortingOrder = 101;

        hp.OnChanged += OnHpChanged;
        OnHpChanged(hp.CurrentHP, hp.MaxHP);
    }

    void OnHpChanged(int current, int max)
    {
        float t = Mathf.Clamp01(max > 0 ? (float)current / max : 0f);

        // целева€ ширина заполнени€ (чуть меньше фона, чтобы не торчали кра€)
        float w = (WIDTH - FILL_PAD * 2f) * t;
        float h = HEIGHT - FILL_PAD * 2f;

        // масштабируем полосу и "€корим" еЄ слева: смещаем на половину еЄ ширины минус половину полной ширины
        fillTf.localScale = new Vector3(w, h, 1f);
        float leftEdge = -WIDTH * 0.5f + FILL_PAD + w * 0.5f;
        fillTf.localPosition = new Vector3(leftEdge, 0f, 0f);

        // цвет от красного к зелЄному
        fillSr.color = Color.Lerp(new Color(1f, 0.25f, 0.2f, 0.95f), new Color(0.2f, 1f, 0.2f, 0.95f), t);
    }
}
