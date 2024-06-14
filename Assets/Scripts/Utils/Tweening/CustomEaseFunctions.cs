using System;

namespace SDVA.Utils
{
    public static class CustomEase
    {
        // Custom ease functions
        // Must return values between 0 and 1

        /// <summary>
        /// View graph of function: https://www.geogebra.org/m/fxxtc2pb
        /// </summary>
        /// <param name="time">Current time since start.</param>
        /// <param name="duration">Total time until completion.</param>
        public static float InHalfer(float time, float duration, float overshootOrAmplitude, float period)
        {
            var x = time / duration;

            var y = -MathF.Pow(1 - x, 2.5f) + 1f;
            
            return y;
        }

        /// <summary>
        /// View graph of function: https://www.geogebra.org/m/fxxtc2pb
        /// </summary>
        /// <param name="time">Current time since start.</param>
        /// <param name="duration">Total time until completion.</param>
        public static float OutHalfer(float time, float duration, float overshootOrAmplitude, float period)
        {
            var x = time / duration;

            var y = -MathF.Pow(1 - x, 0.4f) + 1f;
            
            return y;
        }

        /// <summary>
        /// View graph of function: https://www.geogebra.org/m/fxxtc2pb
        /// </summary>
        /// <param name="time">Current time since start.</param>
        /// <param name="duration">Total time until completion.</param>
        public static float InOutHalfer(float time, float duration, float overshootOrAmplitude, float period)
        {
            return InHalfer(time, duration, overshootOrAmplitude, period)
                + OutHalfer(time, duration, overshootOrAmplitude, period);
        }
    }
}
