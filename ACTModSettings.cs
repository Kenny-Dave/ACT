using System.Collections.Generic;
using Verse;


namespace AnimalCommonalityTweaker
{
    public class ACTSettings : ModSettings
    {
        public static List<CAni> cAniL;
        public static List<CBio> cBioL;
        public static List<CCom> cComL;
        public static List<CMod> cModL;

        public static int BCount;
        public static int ACount;
        public static int CCount;

        public static CBio BioS;
        public static string ZerS;
        public static CMod ModS;
        public static CAni AniS;

        public static bool ACTActive = true;

        public static int Save; //to have different actions on load and exit the settings menu.

        public override void ExposeData()
        {

            Scribe_Values.Look<bool>(ref ACTActive, "ACTActive");

            Scribe_Values.Look<int>(ref ACount, "ACount");
            Scribe_Values.Look<int>(ref BCount, "BCount");
            Scribe_Values.Look<int>(ref CCount, "CCount");

            Scribe_Deep.Look<CBio>(ref BioS, "BiomeSelected");
            Scribe_Values.Look<string>(ref ZerS, "ZeroSelected");
            Scribe_Deep.Look<CMod>(ref ModS, "ModSelected");
            Scribe_Deep.Look<CAni>(ref AniS, "AniSelected");

            Scribe_Collections.Look<CAni>(ref cAniL, "CAniL", LookMode.Deep);
            Scribe_Collections.Look<CBio>(ref cBioL, "CBioL", LookMode.Deep);
            Scribe_Collections.Look<CCom>(ref cComL, "CComL", LookMode.Deep);
            Scribe_Collections.Look<CMod>(ref cModL, "CModL", LookMode.Deep);

            /*
            try
            {
                Log.Message("SET CAniL: " + cAniL.Count);
                Log.Message("SET CBioL: " + cBioL.Count);
                Log.Message("SET CComL: " + cComL.Count);
            }
            catch
            {
                Log.Message("ACT: nulls in saved data");
            }
            */
            ACTMod.BioS = BioS;
            ACTMod.ZerS = ZerS;
            ACTMod.ModS = ModS;
            ACTMod.AniS = AniS;

            if (Save > 3)
            {
                XMLWriter.Writer();
                //writes the data table to an XML patch;
                Save = 0;
            }


            base.ExposeData();
        }
    }
}
