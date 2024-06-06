using CrewBoom.Utility;
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
            Texture2D texture = TextureUtil.GetTextureFromBitmap(Properties.Resources.logo_background);

            GameObject logo = new GameObject("CrewBoom Logo");
            logo.transform.SetParent(__instance.transform, false);

            UnityEngine.UI.Image image = logo.AddComponent<UnityEngine.UI.Image>();
            image.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            RectTransform rect = logo.RectTransform();
            rect.sizeDelta = new Vector2(texture.width, texture.height);
            rect.anchorMin = new Vector2(0.0f, 1.0f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = rect.anchorMin;
            rect.anchoredPosition = Vector2.one * 32.0f;
        }
    }
}
