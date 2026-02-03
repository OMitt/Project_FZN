using System.Collections;
using UnityEngine;

public class CreditScene : MonoBehaviour
{
    public GameObject[] groups;
    public float duration = 4f;
    public float amplitude = 5f;     // 抖动幅度（像素）
    public float frequency = 12f;    // 抖动频率
    Vector3[] originPos;

    public static CreditScene Instance;

    public void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void EnterCredit()
    {
        this.gameObject.SetActive(true);
        originPos = new Vector3[groups.Length];
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] != null)
                originPos[i] = groups[i].transform.localPosition;
        }
        StartCoroutine(ShakeAll());
    }

    IEnumerator ShakeAll()
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float damper = 1f - (t / duration); // 后期逐渐减弱

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] == null) continue;

                float x = Mathf.Sin(Time.time * frequency + i) * amplitude * damper;
                float y = Mathf.Cos(Time.time * frequency * 0.8f + i) * amplitude * damper;

                groups[i].transform.localPosition = originPos[i] + new Vector3(x, y, 0);
            }

            yield return null;
        }
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] != null)
                groups[i].transform.localPosition = originPos[i];
        }
        this.gameObject.SetActive(false);
    }
}
