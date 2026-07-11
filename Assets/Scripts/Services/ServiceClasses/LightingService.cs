using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightingService : MonoBehaviour, ILightingService
{
    private static Dictionary<string, Color> _processedColors => new();
    private static List<string> _cachedKeys;

    private Camera _targetCamera;

    static LightingService()
    {
        var rawHex = new Dictionary<string, string>
        {
            { "clear_sky", "#73C2FB" },
            { "horizon_sun", "#FFF4E0" },
            { "sunset_glow", "#FF5E7E" },
            { "sunset_dusk", "#2C1B4D" },
            { "overcast", "#7A8B99" },
            { "toxic_neon", "#1A2F1A" },
            { "cyber_night", "#0B0C10" },
            { "nebula_pink", "#A12568" },
            { "alien_dawn", "#3B7A57" }
        };

        foreach (var pair in rawHex)
        {
            if (ColorUtility.TryParseHtmlString(pair.Value, out Color color))
                _processedColors.Add(pair.Key.ToLower(), color);
        }

        _cachedKeys = _processedColors.Keys.ToList();
    }

    public IReadOnlyList<string> AvailableColorKeys => _cachedKeys;

    public void Initialize(Camera targetCamera) => _targetCamera = targetCamera;

    public bool GetColorFromKey(string key, out Color color)
    {
        color = Color.magenta;

        if (_processedColors.TryGetValue(key.ToLower(), out color))
            return true;

        if (ColorUtility.TryParseHtmlString(key, out color))
            return true;

        Debug.Log($"Color key \"{key}\" was invalid!");
        return false;
    }

    public bool SetBackgroundColor(Color color)
    {
        if (_targetCamera == null)
        {
            Debug.LogError("LightingService: Target camera was missing!");
            return false;
        }

        _targetCamera.backgroundColor = color;
        return true;
    }
}