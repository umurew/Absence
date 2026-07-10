using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public static class Parser
{
    public static string JoinArguments(string[] args, int startIndex, int count)
    {
        if (args == null || startIndex < 0 || startIndex >= args.Length)
            return string.Empty;

        int actualCount = Math.Min(count, args.Length - startIndex);
        if (actualCount <= 0)
            return string.Empty;

        return string.Join(' ', args, startIndex, actualCount);
    }

    public static string[] SplitArguments(string rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            return Array.Empty<string>();

        List<string> arguments = new();
        StringBuilder currentArgument = new();

        char contextOpener = '\0';

        for (int i = 0; i < rawInput.Length; i++)
        {
            char character = rawInput[i];

            if (contextOpener == '\0')
            {
                if (character == '(' || character == '[' || character == '"')
                {
                    contextOpener = character;
                    currentArgument.Append(character);
                }
                else if (character == ' ')
                {
                    if (currentArgument.Length > 0)
                    {
                        arguments.Add(currentArgument.ToString());
                        currentArgument.Clear();
                    }
                }
                else
                    currentArgument.Append(character);
            }
            else
            {
                if ((contextOpener == '"' && character == '"') ||
                    (contextOpener == '(' && character == ')') ||
                    (contextOpener == '[' && character == ']'))
                {
                    contextOpener = '\0';
                    currentArgument.Append(character);
                }
                else
                    currentArgument.Append(character);
            }
        }

        if (currentArgument.Length > 0)
            arguments.Add(currentArgument.ToString());

        return arguments.ToArray();
    }

    public static bool TryParseVector3(string input, out Vector3 result)
    {
        // Set fallback value
        result = Vector3.zero;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.ToLower() == "player")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            result = player.transform.position + (player.transform.forward * 2f) + (player.transform.up * 1.85f);

            return true;
        }

        // Clean up common wrapper characters
        string sanitized = input.Trim('(', ')', '[', ']', '{', '}');

        // Split by commas, or spaces
        string[] rawComponents = sanitized.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // We strictly need exactly 3 components for Vector3
        /// We could actually show tolerance and grab the first 3 but why would I?
        if (rawComponents.Length != 3)
            return false;

        // Using InvariantCulture to ensure dot '.' is always treated as the decimal separator 
        // regardless of the player's system region/language settings.
        if (float.TryParse(rawComponents[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(rawComponents[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(rawComponents[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
        {
            result = new Vector3(x, y, z);
            return true;
        }

        return false;
    }

    public static bool TryParseBoolean(string input, out bool result)
    {
        // Set fallback value
        result = false;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        string sanitized = input.Trim().ToLower();

        // Try to parse as true/false
        if (bool.TryParse(sanitized, out result))
            return true;

        // Try to parse as zero/one
        if (int.TryParse(sanitized, out int intValue))
        {
            if (intValue == 1)
            {
                result = true;
                return true;
            }
            else if (intValue == 0)
            {
                result = false;
                return true;
            }
        }

        return false;
    }
}