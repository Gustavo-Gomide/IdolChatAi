using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class ChatInputGrow : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    [Header("Heights")]
    [SerializeField] private float oneLineHeight = 60f;
    [SerializeField] private float maxHeight = 140f;

    private RectTransform rect;

    private void Awake()
    {
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();

        rect = GetComponent<RectTransform>();

        inputField.lineType =
            TMP_InputField.LineType.MultiLineNewline;

        inputField.onValueChanged.AddListener(UpdateSize);
    }

    private void Start()
    {
        UpdateSize(inputField.text);
    }

    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(UpdateSize);
    }

    private void UpdateSize(string value)
    {
        float preferred =
            inputField.textComponent.GetPreferredValues(
                value,
                rect.rect.width - 20f,
                0
            ).y;

        float target =
            Mathf.Clamp(
                preferred + 25f,
                oneLineHeight,
                maxHeight
            );

        rect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            target
        );
    }
}