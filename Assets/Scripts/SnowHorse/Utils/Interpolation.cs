using UnityEngine;

namespace SnowHorse.Utils
{
    //Refer to article for lerp methods: https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
    public class Interpolation
    {
        /// <summary>
        /// Inverse linear interpolation between two values, starting at 1 and ending at 0.
        /// 
        /// <param name="duration">The amount of time this interpolation should take.</param>
        /// <param name="currentLerpTime">The current time of the interpolation. This value will be incremented by deltaTime each frame.</param>
        /// <param name="unscaledTime">Whether or not to use Time.unscaledDeltaTime or Time.deltaTime to increment currentLerpTime.</param>
        /// <returns>The value of the interpolation at the current time.</returns>
        /// </summary>
        public static float InverseLinear(float duration, ref float currentLerpTime, bool unscaledTime = false)
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            return 1 - t;
        }


        /// <summary>
        /// Linear interpolation between two values, starting at 0 and ending at 1.
        /// 
        /// <param name="duration">The amount of time this interpolation should take.</param>
        /// <param name="currentLerpTime">The current time of the interpolation. This value will be incremented by deltaTime each frame.</param>
        /// <param name="unscaledTime">Whether or not to use Time.unscaledDeltaTime or Time.deltaTime to increment currentLerpTime.</param>
        /// <returns>The value of the interpolation at the current time.</returns>
        /// </summary>
        public static float Linear(float duration, ref float currentLerpTime, bool unscaledTime = false) 
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            return t;
        }

        /// <summary>
        /// Smooth interpolation between two values, with a gradual start and end.
        /// 
        /// <param name="duration">The amount of time this interpolation should take.</param>
        /// <param name="currentLerpTime">The current time of the interpolation. This value will be incremented by deltaTime each frame.</param>
        /// <param name="unscaledTime">Whether or not to use Time.unscaledDeltaTime or Time.deltaTime to increment currentLerpTime.</param>
        /// <returns>The value of the interpolation at the current time.</returns>
        /// </summary>
        public static float Smooth(float duration, ref float currentLerpTime, bool unscaledTime = false) 
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = t * t * (3f - 2f * t);

            return t;
        }
        
        /// <summary>
        /// Smoother interpolation between two values, with an even more gradual start and end.
        /// 
        /// <param name="duration">The amount of time this interpolation should take.</param>
        /// <param name="currentLerpTime">The current time of the interpolation. This value will be incremented by deltaTime each frame.</param>
        /// <param name="unscaledTime">Whether or not to use Time.unscaledDeltaTime or Time.deltaTime to increment currentLerpTime.</param>
        /// <returns>The value of the interpolation at the current time.</returns>
        /// </summary>
        public static float Smoother(float duration, ref float currentLerpTime, bool unscaledTime = false) 
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = t * t * t * (t * (6f * t - 15f) + 10f);

            return t;
        }

        /// <summary>
        /// Sine interpolation between two values, creating a linear start and easing out at the end. See effect: https://chicounity3d.files.wordpress.com/2014/05/interp-sinerp.png
        /// 
        /// <param name="duration">The amount of time this interpolation should take.</param>
        /// <param name="currentLerpTime">The current time of the interpolation. This value will be incremented by deltaTime each frame.</param>
        /// <param name="unscaledTime">Whether or not to use Time.unscaledDeltaTime or Time.deltaTime to increment currentLerpTime.</param>
        /// <returns>The value of the interpolation at the current time.</returns>
        /// </summary>
        public static float Sinerp(float duration, ref float currentLerpTime, bool unscaledTime = false)
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            return t;
        }

        /// <summary>
        /// Cosine interpolation between two values, easing in at start. See effect: https://chicounity3d.files.wordpress.com/2014/05/interp-coserp.png
        /// 
        /// <param name="duration">The amount of time this interpolation should take.</param>
        /// <param name="currentLerpTime">The current time of the interpolation. This value will be incremented by deltaTime each frame.</param>
        /// <param name="unscaledTime">Whether or not to use Time.unscaledDeltaTime or Time.deltaTime to increment currentLerpTime.</param>
        /// <returns>The value of the interpolation at the current time.</returns>
        /// </summary>
        public static float Coserp(float duration, ref float currentLerpTime, bool unscaledTime = false)
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

            return t;
        }

        /// <summary>
        /// Exponential interpolation between two values. See effect: https://chicounity3d.files.wordpress.com/2014/05/interp-quad.png
        /// 
        /// <param name="duration">The amount of time this interpolation should take.</param>
        /// <param name="currentLerpTime">The current time of the interpolation. This value will be incremented by deltaTime each frame.</param>
        /// <param name="unscaledTime">Whether or not to use Time.unscaledDeltaTime or Time.deltaTime to increment currentLerpTime.</param>
        /// <returns>The value of the interpolation at the current time.</returns>
        /// </summary>
        public static float Exponential(float duration, ref float currentLerpTime, bool unscaledTime = false)
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = t * t; //Quadratic formula

            return t;
        }

        private static float CalculateLinearInterpolation(float duration, ref float currentLerpTime, bool unscaledTime)
        {
            currentLerpTime = unscaledTime ? currentLerpTime += Time.unscaledDeltaTime : currentLerpTime += Time.deltaTime;
            if (currentLerpTime > duration) currentLerpTime = duration;
            return currentLerpTime / duration;
        }
    }
}