using UnityEngine;
using Verse;

namespace AnimalCommonalityTweaker
{


    class GUIElements
    {
        public static void CounterFlipper()
        {
            if (ACTMod.counter == 0)
            {
                ACTMod.colorLab = Color.gray;
                ACTMod.counter = 1;
            }
            else
            {
                ACTMod.colorLab = Color.white;
                ACTMod.counter = 0;
            }
        }

        private static Texture2D _staticRectTexture;
        private static GUIStyle _staticRectStyle;

        public static void tableBlackRecs()
        {
            int counter = 0;

            Rect blackRect = new Rect(0, 0, 864 - 18 - 220, ACTMod.AnimalRectHeight);

            Listing_Standard LS3 = new Listing_Standard();

            LS3.Begin(blackRect);

            foreach (CCom com in ACTMod.cComLFS)
            {
                ACTMod.expr_0B = LS3.GetRect(30f);

                GUIElements.CounterFlipper();

                if (counter == 1)
                {
                    GUIElements.GUIDrawRect(ACTMod.expr_0B, Color.black);
                }
            }
        }


        // Note that this function is only meant to be called from OnGUI() functions.
        public static void GUIDrawRect(Rect position, Color color)
        {
            if (_staticRectTexture == null)
            {
                _staticRectTexture = new Texture2D(1, 1);
            }

            if (_staticRectStyle == null)
            {
                _staticRectStyle = new GUIStyle();
            }

            _staticRectTexture.SetPixel(0, 0, color);
            _staticRectTexture.Apply();

            _staticRectStyle.normal.background = _staticRectTexture;

            GUI.Box(position, GUIContent.none, _staticRectStyle);
        }
    }
}
