using UnityEngine;

public class ColorProvider : MonoBehaviour
{
    public static ColorProvider Instance { get; private set; }
    public GizmoColorsClass GizmoColors { get; } = new();
    public UIColorsClass UIColors { get; } = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public class GizmoColorsClass
    {
        public Color IInteractableCollider { get; } = Color.softYellow;
        public Color DoorAxis { get; } = Color.darkCyan;
        public Color DoorArrow { get; } = Color.cyan;
        public Color HandleLabel { get; } = Color.antiqueWhite;
    }

    public class UIColorsClass
    {
        public string ErrorColor => "#CC4141";
        public string PrimaryColor => "#E1E1E1";
        public string AccentColor => "#77BAFF";
        public string GrayedColor => "#8493A3";
    }
}