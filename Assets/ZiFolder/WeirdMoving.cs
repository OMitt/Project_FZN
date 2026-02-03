using UnityEngine;
using System.Collections;

public class WeirdMoving : MonoBehaviour
{
    public RectTransform trans;

    public float amplitude = 4f;
    public float frequency = 10f;

    Vector3 originPos;
    Coroutine routine;
    float phase;

    void Awake()
    {
        if (trans == null)
            trans = GetComponent<RectTransform>();

        originPos = trans.localPosition;
        phase = Random.Range(0f, 1000f);
    }

    void OnEnable()
    {
        routine = StartCoroutine(ShakeLoop());
    }

    void OnDisable()
    {
        if (routine != null)
            StopCoroutine(routine);

        trans.localPosition = originPos;
    }

    IEnumerator ShakeLoop()
    {
        while (true)
        {
            float t = Time.time * frequency + phase;

            float x = Mathf.Sin(t) * amplitude;
            float y = Mathf.Cos(t * 0.83f) * amplitude;

            trans.localPosition = originPos + new Vector3(x, y, 0);
            yield return null;
        }
    }
}
