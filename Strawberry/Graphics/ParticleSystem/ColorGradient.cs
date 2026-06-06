using System.Collections.Generic;

/*
 * Strawberry Game Engine
 * File: ColorGradient.cs
 * Author: Koosha Aabedini Nassab
 *
 * Helper for color gradients used to interpolate particle color over time.
 */

namespace Strawberry.Graphics.ParticleSystem
{
    /// <summary>
    /// The keyframe for a color gradient at a specific time
    /// </summary>
    public struct ColorKeyframe
    {
        public float Time;
        public Color Color;

        public ColorKeyframe(float time, Color color)
        {
            Time = time;
            Color = color;
        }
    }

    /// <summary>
    /// Helper for color gradients used to interpolate particle color over time.
    /// </summary>
    public class ColorGradient
    {
        List<ColorKeyframe> keyframes = new List<ColorKeyframe>();

        public int KeyframeCount => keyframes.Count;

        public void AddKeyframe(float time, Color color)
        {
            keyframes.Add(new ColorKeyframe(time, color));
            keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));
        }

        public void Clear()
        {
            keyframes.Clear();
        }

        public Color Evaluate(float t)
        {
            if (keyframes.Count == 0)
                return Color.White;
            if (keyframes.Count == 1)
                return keyframes[0].Color;
            if (t <= keyframes[0].Time)
                return keyframes[0].Color;
            if (t >= keyframes[keyframes.Count - 1].Time)
                return keyframes[keyframes.Count - 1].Color;

            for (int i = 0; i < keyframes.Count - 1; i++)
            {
                if (t >= keyframes[i].Time && t <= keyframes[i + 1].Time)
                {
                    float localT = (t - keyframes[i].Time) / (keyframes[i + 1].Time - keyframes[i].Time);
                    return Lerp(keyframes[i].Color, keyframes[i + 1].Color, localT);
                }
            }

            return keyframes[keyframes.Count - 1].Color;
        }

        Color Lerp(Color a, Color b, float t)
        {
            return new Color(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t,
                a.A + (b.A - a.A) * t
            );
        }
    }
}
