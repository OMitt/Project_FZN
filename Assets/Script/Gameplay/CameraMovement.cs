using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Offset")]
    public float offsetValue = 5f; // ×î´óÆ«ÒÆ¾àÀë

    [Header("Distance Curve")]
    public AnimationCurve distanceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Smooth")]
    public float smooth = 8f;

    Vector3 basePos;

    void Start()
    {
        basePos = transform.position;
    }

    void Update()
    {
        Vector2 mouse = Input.mousePosition;
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        Vector2 toMouse = mouse - center;

        float maxDist = center.magnitude;
        float dist01 = Mathf.Clamp01(toMouse.magnitude / maxDist);

        Vector2 dir = toMouse.sqrMagnitude > 0.0001f
            ? toMouse.normalized
            : Vector2.zero;

        float strength = distanceCurve.Evaluate(dist01);

        Vector3 targetPos = basePos +
            (Vector3)(dir * strength * offsetValue);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * smooth
        );
    }
}
