using System.Drawing;
using UnityEngine;

namespace CrewBoom.Utility
{
    public class TextureUtil
    {
        public static Texture2D GetTextureFromBitmap(Bitmap bitmap, FilterMode filterMode = FilterMode.Bilinear)
        {
            ImageConverter converter = new ImageConverter();
            byte[] imageData = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

            Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height);
            texture.LoadImage(imageData);
            texture.filterMode = filterMode;
            texture.Apply();

            return texture;
        }
    }
}
