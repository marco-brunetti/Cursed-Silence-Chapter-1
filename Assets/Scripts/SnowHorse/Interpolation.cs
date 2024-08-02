using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnowHorse.Utils
{
    public class Interpolation
    {
        //Refer to article for lerp methods: https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/

        //Linear interpolation between two values
        public static float Linear(float duration, ref float currentLerpTime, bool unscaledTime = false) 
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            return t;
        }

        //Interpolation smooth at start and end
        public static float Smooth(float duration, ref float currentLerpTime, bool unscaledTime = false) 
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = t * t * (3f - 2f * t); //Smooth step formula

            return t;
        }

        //Interpolation smoother at start and end
        public static float Smoother(float duration, ref float currentLerpTime, bool unscaledTime = false) 
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = t * t * t * (t * (6f * t - 15f) + 10f); //Smoother step formula

            return t;
        }

        //Eases out at the end. See effect: https://chicounity3d.files.wordpress.com/2014/05/interp-sinerp.png
        public static float Sinerp(float duration, ref float currentLerpTime, bool unscaledTime = false)
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = Mathf.Sin(t * Mathf.PI * 0.5f); //Sinerp formula

            return t;
        }


        //Eases in at start. See effect: https://chicounity3d.files.wordpress.com/2014/05/interp-coserp.png
        public static float Coserp(float duration, ref float currentLerpTime, bool unscaledTime = false)
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f); //Coserp formula

            return t;
        }


        //Exponential interpolation. See effect: https://chicounity3d.files.wordpress.com/2014/05/interp-quad.png
        public static float Exponential(float duration, ref float currentLerpTime, bool unscaledTime = false)
        {
            float t = CalculateLinearInterpolation(duration, ref currentLerpTime, unscaledTime);

            t = t * t; //Quadratic formula

            return t;
        }

        private static float CalculateLinearInterpolation(float duration, ref float currentLerpTime, bool unscaledTime)
        {
            if (unscaledTime)
            {
                currentLerpTime += Time.unscaledDeltaTime;
            }
            else
            {
                currentLerpTime += Time.deltaTime;
            }


            if (currentLerpTime > duration)
            {
                currentLerpTime = duration;
            }

            float t = currentLerpTime / duration;

            return t;
        }
    }
}