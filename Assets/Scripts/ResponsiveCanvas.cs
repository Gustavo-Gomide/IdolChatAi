using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class ResponsiveCanvas : MonoBehaviour
{
    [Header("Reference Resolution")]
    [SerializeField] private float referenceWidth = 1080f;
    [SerializeField] private float referenceHeight = 1920f;

    [Header("Scale Limits")]
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.5f;

    [Header("Text")]
    [SerializeField] private bool scaleFonts = true;
    [SerializeField] private bool enableAutoSizing = true;

    [Header("Layout")]
    [SerializeField] private bool scaleRects = true;
    [SerializeField] private bool applySafeArea = true;

    private RectTransform canvasRect;

    private readonly Dictionary<TMP_Text, float> originalFonts =
        new Dictionary<TMP_Text, float>();

    private readonly Dictionary<RectTransform, Vector2> originalSizes =
        new Dictionary<RectTransform, Vector2>();

    private int lastWidth;
    private int lastHeight;

    private void Awake()
    {
        canvasRect = GetComponent<RectTransform>();

        CacheElements();

        ApplyResponsiveLayout();
    }

    private void Update()
    {
        if (lastWidth != Screen.width ||
            lastHeight != Screen.height)
        {
            ApplyResponsiveLayout();
        }
    }

    private void CacheElements()
    {
        originalFonts.Clear();
        originalSizes.Clear();

        TMP_Text[] texts =
            GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text text in texts)
        {
            if (!originalFonts.ContainsKey(text))
            {
                originalFonts.Add(text, text.fontSize);
            }
        }

        RectTransform[] rects =
            GetComponentsInChildren<RectTransform>(true);

        foreach (RectTransform rect in rects)
        {
            if (rect == canvasRect)
                continue;

            if (!originalSizes.ContainsKey(rect))
            {
                originalSizes.Add(rect, rect.sizeDelta);
            }
        }
    }

    private void ApplyResponsiveLayout()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        float scale =
            Mathf.Min(
                Screen.width / referenceWidth,
                Screen.height / referenceHeight
            );

        scale = Mathf.Clamp(
            scale,
            minScale,
            maxScale
        );

        if (applySafeArea)
        {
            ApplySafeArea();
        }

        if (scaleFonts)
        {
            ApplyFonts(scale);
        }

        if (scaleRects)
        {
            ApplyRects(scale);
        }
    }

    private void ApplySafeArea()
    {
        if (canvasRect == null)
            return;

        Rect safeArea = Screen.safeArea;

        Vector2 min = safeArea.position;
        Vector2 max = safeArea.position + safeArea.size;

        min.x /= Screen.width;
        min.y /= Screen.height;

        max.x /= Screen.width;
        max.y /= Screen.height;

        canvasRect.anchorMin = min;
        canvasRect.anchorMax = max;

        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;
    }

    private void ApplyFonts(float scale)
    {
        foreach (var pair in originalFonts)
        {
            TMP_Text text = pair.Key;

            if (text == null)
                continue;

            float originalSize = pair.Value;

            TMP_InputField inputField =
                text.GetComponentInParent<TMP_InputField>();

            if (inputField != null)
            {
                // Texto digitado mantém tamanho fixo
                text.enableAutoSizing = false;
                text.fontSize = originalSize;
                continue;
            }

            text.enableAutoSizing = false;
            text.fontSize = originalSize * scale;
        }
    }

    private void ApplyRects(float scale)
    {
        foreach (var pair in originalSizes)
        {
            RectTransform rect = pair.Key;

            if (rect == null)
                continue;

            LayoutElement layout =
                rect.GetComponent<LayoutElement>();

            if (layout != null)
                continue;

            Vector2 originalSize = pair.Value;

            rect.sizeDelta =
                originalSize * scale;
        }
    }
}