using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    [SerializeField] private RectTransform contentRectTransform;

    private Rect lastSafeArea;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        ApplySafeArea();
    }

    private void Update()
    {
        if (lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        lastSafeArea = Screen.safeArea;

        Vector2 anchorMin = lastSafeArea.position;
        Vector2 anchorMax = lastSafeArea.position + lastSafeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        if (contentRectTransform != null)
        {
            contentRectTransform.offsetMin = new Vector2(lastSafeArea.xMin, lastSafeArea.yMin);
            contentRectTransform.offsetMax = new Vector2(Screen.width - lastSafeArea.xMax, Screen.height - lastSafeArea.yMax);
        }
    }
}
