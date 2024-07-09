using UnityEngine;

namespace SDVA.Utils.UI
{
    public static class ImageHelpers
    {
        /// <summary>
        /// Combines two textures.
        /// </summary>
        /// <param name="topTexture">The texture to place on top of this texture. Must be the same size.</param>
        /// <returns>A texture made by laying topTexture on bottomTexture.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>

        public static Texture2D AlphaBlend(this Texture2D bottomTexture, Texture2D topTexture)
        {
            if (bottomTexture.width != topTexture.width || bottomTexture.height != topTexture.height)
            { throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images"); }

            var bottomData = bottomTexture.GetPixels();
            var topData = topTexture.GetPixels();

            int count = bottomData.Length;
            var resultData = new Color[count];

            for (int i = 0; i < count; i++)
            {
                Color bottomPixel = bottomData[i];
                Color topPixel = topData[i];

                float srcF = topPixel.a;
                float destF = 1f - topPixel.a;
                float alpha = srcF + destF * bottomPixel.a;

                Color resultPixel = (topPixel * srcF + bottomPixel * bottomPixel.a * destF) / alpha;

                resultPixel.a = alpha;
                resultData[i] = resultPixel;
            }

            var resultTexture = new Texture2D(topTexture.width, topTexture.height);
            resultTexture.SetPixels(resultData);
            resultTexture.Apply();
            return resultTexture;
        }
    }
}
