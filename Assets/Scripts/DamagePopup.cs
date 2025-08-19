// Assets/Scripts/Combat/DamagePopup.cs
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public static void Show(Vector3 worldPos, int amount)
    {
        var go = new GameObject("DamagePopup");
        var tmpro = go.AddComponent<TextMeshPro>();
        tmpro.fontSize = 3;
        tmpro.alignment = TextAlignmentOptions.Center;
        tmpro.text = amount.ToString();
        tmpro.sortingOrder = 300;

        go.transform.position = worldPos + new Vector3(0, 1.6f, 0);
        go.AddComponent<DamagePopup>().life = 0.8f;
    }

    float life = 1f;
    float t;

    void Update()
    {
        t += Time.deltaTime;
        transform.position += Vector3.up * (Time.deltaTime * 0.8f);
        var text = GetComponent<TextMeshPro>();
        text.alpha = Mathf.Lerp(1f, 0f, t / life);
        if (t >= life) Destroy(gameObject);
    }
}
