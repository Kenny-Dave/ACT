using System;
//using ExtensionMethods;

namespace AnimalCommonalityTweaker
{
    public static class MyExtensions
    {
        public static string ComText(this float fl)
        {
            //returns fl as a string rounded to a certain number of decimals based on the size of the value. 
            float absfl = Math.Abs(fl);

            string txt = fl.ToString(
            (absfl >= 1f ? "F3" : (
                absfl >= 0.1f ? "F3" : (
                absfl >= 0.01f ? "F4" : (
                absfl == 0 ? "F2" : (
                "F4")))))
            );

            return txt;
        }
        public static float ComValCon(this float raw)
        {
            double absraw = Math.Abs(raw);

            double dub = (absraw >= 1d ? Math.Round(raw, 3) : (
                absraw >= 0.1d ? Math.Round(raw, 3) : (
                absraw >= 0.01d ? Math.Round(raw, 4) :
                Math.Round(raw, 4))));
            //it needs to be this many, even though it looks rubbish, because of the very small numbers with Megafauna and others.

            float flOut = Convert.ToSingle(dub);

            return flOut;
            // sets the value equal to the displayed number of decimal places. 
        }
    }
}
