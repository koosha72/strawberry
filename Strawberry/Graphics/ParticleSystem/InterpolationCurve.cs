using System.Collections.Generic;

namespace Strawberry.Graphics.ParticleSystem
{
    public struct FloatKeyframe
    {
        public float Time;
        public float Value;

        public FloatKeyframe(float time, float value)
        {
            Time = time;
            Value = value;
        }
    }

    public class InterpolationCurve
    {
        List<FloatKeyframe> keyframes = new List<FloatKeyframe>();

        public int KeyframeCount => keyframes.Count;

        public void AddKeyframe(float time, float value)
        {
            keyframes.Add(new FloatKeyframe(time, value));
            keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));
        }

        public void Clear()
        {
            keyframes.Clear();
        }

        public float Evaluate(float t)
        {
            if (keyframes.Count == 0)
                return 1f;
            if (keyframes.Count == 1)
                return keyframes[0].Value;
            if (t <= keyframes[0].Time)
                return keyframes[0].Value;
            if (t >= keyframes[keyframes.Count - 1].Time)
                return keyframes[keyframes.Count - 1].Value;

            for (int i = 0; i < keyframes.Count - 1; i++)
            {
                if (t >= keyframes[i].Time && t <= keyframes[i + 1].Time)
                {
                    float localT = (t - keyframes[i].Time) / (keyframes[i + 1].Time - keyframes[i].Time);
                    return keyframes[i].Value + (keyframes[i + 1].Value - keyframes[i].Value) * localT;
                }
            }

            return keyframes[keyframes.Count - 1].Value;
        }
    }
}
