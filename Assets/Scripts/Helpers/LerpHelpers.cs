using UnityEngine;

namespace Helpers
{
    public static class Ease
    {
        public enum EaseType
        {
            Linear, InQuart, OutQuart, InOutQuart, InOutExpo
        }

        public static float EvaluateEaseType(EaseType ease, float t)
        {
            switch (ease)
            {
                case EaseType.Linear: return Linear(t);
                case EaseType.InQuart: return InQuart(t);
                case EaseType.OutQuart: return OutQuart(t);
                case EaseType.InOutQuart: return InOutQuart(t);
                case EaseType.InOutExpo: return InOutExpo(t);
            }

            return 0;
        }

        public static float Linear(float t) => t;

        public static float InQuart(float t) => Mathf.Pow(t, 4);  

        public static float OutQuart(float t) => 1 - InQuart(1 - t);

        public static float InOutQuart(float t) => t < 0.5f ? 8 * InQuart(t) : 1 - Mathf.Pow(-2f * t + 2, 4) / 2;

        public static float InOutExpo(float t)
        {
            return t == 0
            ? 0
            : t == 1
            ? 1
            : t < 0.5 ? Mathf.Pow(2, 20 * t - 10) / 2
            : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
        }
    }    
}