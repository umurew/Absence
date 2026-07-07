using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LightningManager : MonoBehaviour
{
    public static LightningManager Instance { get; private set; }

    public ReadOnlyDictionary<string, string> SkyboxColors => new(_skyboxColors);

    private Camera mainCamera;
    private readonly Dictionary<string, string> _skyboxColors = new() {
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

    private void Start()
    {
        if (Camera.main != null)
            mainCamera = Camera.main;
    }

    public void SetEnvironmentBackgroundColor(Color color)
    {
        mainCamera.backgroundColor = color;
    }
}