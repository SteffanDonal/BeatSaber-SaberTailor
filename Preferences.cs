using IllusionPlugin;
using System;
using System.Globalization;
using UnityEngine;

namespace SaberTailor
{
    public static class Preferences
    {
        public static float Length { get; private set; }

        public static bool IsTrailEnabled { get; private set; }
        public static int TrailLength { get; private set; }

        public static Vector3 GripLeftPosition { get; private set; }
        public static Vector3 GripRightPosition { get; private set; }

        public static Quaternion GripLeftRotation { get; private set; }
        public static Quaternion GripRightRotation { get; private set; }

        static Preferences()
        {
            Load();
        }

        public static void Load()
        {
            Length = ModPrefs.GetFloat(Plugin.Name, nameof(Length), 1f, true);
            Length = Math.Max(0.01f, Math.Min(2f, Length));

            IsTrailEnabled = ModPrefs.GetBool(Plugin.Name, nameof(IsTrailEnabled), true, true);

            TrailLength = ModPrefs.GetInt(Plugin.Name, nameof(TrailLength), 20, true);
            TrailLength = Math.Max(5, Math.Min(100, TrailLength));

            GripLeftPosition = ParseVector3(ModPrefs.GetString(Plugin.Name, nameof(GripLeftPosition), "0,0,0", true)) / 100f;
            GripLeftPosition = new Vector3
            {
                x = Mathf.Clamp(GripLeftPosition.x, -0.5f, 0.5f),
                y = Mathf.Clamp(GripLeftPosition.y, -0.5f, 0.5f),
                z = Mathf.Clamp(GripLeftPosition.z, -0.5f, 0.5f)
            };
            GripLeftRotation = Quaternion.Euler(ParseVector3(ModPrefs.GetString(Plugin.Name, nameof(GripLeftRotation), "0,0,0", true)));

            GripRightPosition = ParseVector3(ModPrefs.GetString(Plugin.Name, nameof(GripRightPosition), "0,0,0", true)) / 100f;
            GripRightPosition = new Vector3
            {
                x = Mathf.Clamp(GripRightPosition.x, -0.5f, 0.5f),
                y = Mathf.Clamp(GripRightPosition.y, -0.5f, 0.5f),
                z = Mathf.Clamp(GripRightPosition.z, -0.5f, 0.5f)
            };
            GripRightRotation = Quaternion.Euler(ParseVector3(ModPrefs.GetString(Plugin.Name, nameof(GripRightRotation), "0,0,0", true)));
        }


        static Vector3 ParseVector3(string originalString)
        {
            var components = originalString.Trim().Split(',');
            var parsedVector = Vector3.zero;

            if (components.Length != 3) return parsedVector;

            TryParseInvariantFloat(components[0], out parsedVector.x);
            TryParseInvariantFloat(components[1], out parsedVector.y);
            TryParseInvariantFloat(components[2], out parsedVector.z);

            return parsedVector;
        }

        /// <summary>
        /// Tries to parse a float using invariant culture.
        /// </summary>
        /// <param name="number">The string containing the float to parse.</param>
        /// <param name="result">The parsed float, if successful.</param>
        /// <returns>True on success, false on failure.</returns>
        static bool TryParseInvariantFloat(string number, out float result)
        {
            return float.TryParse(
                number,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out result
            );
        }
    }
}
