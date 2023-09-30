using HarmonyLib;
using Reptile;
using System.Drawing;
using UnityEngine;

namespace CrewBoom.Patches
{
    [HarmonyPatch(typeof(Reptile.MainMenuManager), nameof(Reptile.MainMenuManager.Init))]
    public class MainMenuPatch
    {
        public static void Postfix(MainMenuManager __instance)
        {
            ImageConverter converter = new ImageConverter();
            byte[] imageData = (byte[])converter.ConvertTo(Properties.Resources.logo_background, typeof(byte[]));

            Texture2D texture = new Texture2D(512, 512);
            texture.LoadImage(imageData);
            texture.Apply();

            GameObject logo = new GameObject("CrewBoom Logo");
            logo.transform.SetParent(__instance.transform, false);

            UnityEngine.UI.Image image = logo.AddComponent<UnityEngine.UI.Image>();
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            RectTransform transform = logo.RectTransform();
            transform.sizeDelta = new Vector2(texture.width, texture.height);
            float x = -__instance.transform.localPosition.x + texture.width * 0.5f;
            float y = __instance.transform.localPosition.y - texture.height * 0.25f;
            transform.localPosition = new Vector2(x + 32.0f, y - 32.0f);
        }
    }
}
