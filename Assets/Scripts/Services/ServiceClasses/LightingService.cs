using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightingService : MonoBehaviour, ILightingService
{
    private readonly Dictionary<string, Color> _processedColors = new();
    private List<string> _cachedKeys;
    private Camera _mainCamera;
    private bool _initialized = false;

    public IReadOnlyList<string> AvailableColorKeys => _cachedKeys;

    public void Initialize(Camera mainCamera)
    {
        if (_initialized)
        {
            Debug.LogWarning($"{nameof(LightingService)}: {nameof(Initialize)} can't be called after initialization.");
            return;
        }

        _mainCamera = mainCamera;

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

        _initialized = true;
    }

    public bool GetColorFromKey(string key, out Color color)
    {
        InitializedCheck();

        color = Color.magenta;

        if (string.IsNullOrEmpty(key))
            return false;

        string lowerKey = key.ToLower();

        if (_processedColors.TryGetValue(lowerKey, out color))
            return true;

        if (ColorUtility.TryParseHtmlString(key, out color))
            return true;

        Debug.LogWarning($"{nameof(LightingService)}: Color key \"{key}\" is invalid!");
        return false;
    }

    public bool SetBackgroundColor(Color color)
    {
        InitializedCheck();

        _mainCamera.backgroundColor = color;
        return true;
    }

    private void InitializedCheck()
    {
        if (!_initialized)
            throw new InvalidOperationException($"{nameof(LightingService)} must be initialized before use.");
    }
}