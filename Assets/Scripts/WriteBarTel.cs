using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WriteBarTel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform inputPanel;
    [SerializeField] private RectTransform messagesArea;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private ChatManager chatManager;

    [Header("Settings")]
    [SerializeField] private float smoothTime = 0.08f;
    [SerializeField] private float extraPadding = 10f;

    private Canvas canvas;
    private Vector2 inputBasePos;
    private Vector2 velocity;

    private float originalMessagesBottom;
    private bool initialized;

    private void Awake()
    {
        if (chatManager == null)
            chatManager = FindFirstObjectByType<ChatManager>();

        if (inputField == null && chatManager != null)
            inputField = chatManager.inputField;

        canvas = GetComponentInParent<Canvas>();

        if (inputPanel != null && messagesArea != null)
        {
            inputBasePos = inputPanel.anchoredPosition;
            originalMessagesBottom = messagesArea.offsetMin.y;
            initialized = true;
        }
    }

    private void Start()
    {
        if (sendButton != null)
        {
            sendButton.onClick.RemoveAllListeners();
            sendButton.onClick.AddListener(OnSendPressed);
        }
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS

        if (!initialized)
            return;

        float keyboardHeight = GetKeyboardHeight();

        if (keyboardHeight > 0)
        {
            MoveAboveKeyboard(keyboardHeight);
        }
        else
        {
            RestoreLayout();
        }

#endif
    }

    private void MoveAboveKeyboard(float keyboardHeight)
    {
        float scale = canvas != null ? canvas.scaleFactor : 1f;

        float keyboardCanvasHeight =
            (keyboardHeight / scale) + extraPadding;

        Vector2 targetPos = new Vector2(
            inputBasePos.x,
            inputBasePos.y + keyboardCanvasHeight
        );

        inputPanel.anchoredPosition =
            Vector2.SmoothDamp(
                inputPanel.anchoredPosition,
                targetPos,
                ref velocity,
                smoothTime
            );

        float bottomSpace =
            originalMessagesBottom +
            keyboardCanvasHeight +
            inputPanel.rect.height;

        messagesArea.offsetMin = new Vector2(
            messagesArea.offsetMin.x,
            bottomSpace
        );
    }

    private void RestoreLayout()
    {
        inputPanel.anchoredPosition =
            Vector2.SmoothDamp(
                inputPanel.anchoredPosition,
                inputBasePos,
                ref velocity,
                smoothTime
            );

        messagesArea.offsetMin = new Vector2(
            messagesArea.offsetMin.x,
            originalMessagesBottom
        );
    }

    private float GetKeyboardHeight()
    {
        if (!TouchScreenKeyboard.visible)
            return 0;

        float height = TouchScreenKeyboard.area.height;

        if (height > 0)
            return height;

        return Screen.height * 0.4f;
    }

    private void OnSendPressed()
    {
        if (chatManager == null || inputField == null)
            return;

        string message = inputField.text.Trim();

        if (string.IsNullOrEmpty(message))
            return;

        chatManager.SubmitMessage(message);

        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }
}