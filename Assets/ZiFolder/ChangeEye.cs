using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangeEye : MonoBehaviour
{
    public Sprite[] sprites;
    public Image target;
    private void OnEnable()
    {
        StartCoroutine(BlinkCoroutine());
    }
    IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            target.sprite = sprites[0];
            yield return new WaitForSeconds(1f);

            // 切到眨眼
            target.sprite = sprites[1];
            yield return new WaitForSeconds(0.12f);

            // 切回正常
            target.sprite = sprites[0];
            yield return new WaitForSeconds(2.5f);
        }
    }
}
