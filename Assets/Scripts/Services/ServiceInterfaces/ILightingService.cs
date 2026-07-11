using System.Collections.Generic;
using UnityEngine;

public interface ILightingService
{
    IReadOnlyList<string> AvailableColorKeys { get; }
    bool GetColorFromKey(string key, out Color color);
    bool SetBackgroundColor(Color color);
}