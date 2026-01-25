using UnityEngine;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;

    private void Awake()
    {
        if (targetCanvas == null)
            targetCanvas = GetComponentInChildren<Canvas>(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            bool newEnabled = !targetCanvas.enabled;
            targetCanvas.enabled = newEnabled;
        }
    }
}
