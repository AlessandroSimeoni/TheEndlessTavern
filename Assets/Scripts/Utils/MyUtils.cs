using UnityEngine;

namespace Utils
{
    public static class MyUtils
    {
        private const float DPI_TO_PCM = 0.393701f;

        /// <summary>
        /// (From the lessons)
        /// Scale a vector from pixel to centimeter
        /// </summary>
        /// <param name="distance">the vector to scale</param>
        /// <returns>the scaled vector</returns>
        public static Vector2 ScaleByDPI(Vector2 distance)
        {
            float dpi = Screen.dpi;

            if (dpi <= 0.0f)
                dpi = 240.0f;

            float pcm = dpi * DPI_TO_PCM;

            return distance / pcm;
        }
    }
}
