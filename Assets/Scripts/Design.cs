using UnityEngine;

public static class Design
{
    // Resolução usada no Canvas Scaler
    public const float ReferenceWidth = 1080f;
    public const float ReferenceHeight = 1920f;

    public enum DeviceType
    {
        SmallPhone,
        Phone,
        LargePhone,
        Tablet
    }

    public static DeviceType CurrentDevice
    {
        get
        {
            float shortestSide = Mathf.Min(Screen.width, Screen.height);

            if (shortestSide < 720)
                return DeviceType.SmallPhone;

            if (shortestSide < 1080)
                return DeviceType.Phone;

            if (shortestSide < 1440)
                return DeviceType.LargePhone;

            return DeviceType.Tablet;
        }
    }

    public static float WidthScale =>
        Screen.width / ReferenceWidth;

    public static float HeightScale =>
        Screen.height / ReferenceHeight;

    public static float Scale =>
        Mathf.Min(WidthScale, HeightScale);

    public static float Font(float baseSize)
    {
        float multiplier = CurrentDevice switch
        {
            DeviceType.SmallPhone => 0.90f,
            DeviceType.Phone => 1.00f,
            DeviceType.LargePhone => 1.10f,
            DeviceType.Tablet => 1.25f,
            _ => 1.00f
        };

        return Mathf.Round(
            Mathf.Clamp(
                baseSize * Scale * multiplier,
                baseSize * 0.85f,
                baseSize * 1.50f
            )
        );
    }

    public static float Size(float baseSize)
    {
        return Mathf.Round(baseSize * Scale);
    }

    public static Vector2 Size(float width, float height)
    {
        return new Vector2(
            Mathf.Round(width * Scale),
            Mathf.Round(height * Scale)
        );
    }

    public static Rect SafeArea =>
        Screen.safeArea;

    public static float SafeTop =>
        Screen.height - SafeArea.yMax;

    public static float SafeBottom =>
        SafeArea.yMin;

    public static float SafeLeft =>
        SafeArea.xMin;

    public static float SafeRight =>
        Screen.width - SafeArea.xMax;

    public static float Margin(float baseMargin)
    {
        return Mathf.Round(baseMargin * Scale);
    }

    public static float Padding(float basePadding)
    {
        return Mathf.Round(basePadding * Scale);
    }

    public static bool IsTablet =>
        CurrentDevice == DeviceType.Tablet;

    public static bool IsPhone =>
        !IsTablet;

    public static bool IsLandscape =>
        Screen.width > Screen.height;

    public static bool IsPortrait =>
        Screen.height >= Screen.width;

    public static int Columns(int mobile, int tablet)
    {
        return IsTablet ? tablet : mobile;
    }

    public static float MaxContentWidth
    {
        get
        {
            return IsTablet
                ? Mathf.Min(Screen.width * 0.75f, 1200f)
                : Screen.width;
        }
    }

    public static float ReadableText(float baseSize)
    {
        return Mathf.Clamp(
            Font(baseSize),
            14f,
            40f
        );
    }

    public static Vector2 ChatBubble(float maxWidthPercent = 0.75f)
    {
        float width = Screen.width * maxWidthPercent;

        if (IsTablet)
            width *= 0.8f;

        return new Vector2(width, 0);
    }

    public static float KeyboardOffset()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (TouchScreenKeyboard.visible)
        {
            float height = TouchScreenKeyboard.area.height;

            if (height > 0)
                return height;
        }
#endif
        return 0;
    }

    public static float VisibleHeight()
    {
        return Screen.height - KeyboardOffset();
    }

    public static bool IsKeyboardVisible()
    {
#if UNITY_ANDROID || UNITY_IOS
        return TouchScreenKeyboard.visible;
#else
        return false;
#endif
    }
}