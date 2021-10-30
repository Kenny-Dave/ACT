using System.Collections.Generic;
using System.Globalization;
using Verse;

namespace AnimalCommonalityTweaker
{
    public class AniFactory
    {
        public AniFactory() //game data
        {
            AniL = new List<CAni>();
        }

        private static List<CAni> aniL;
        public static List<CAni> AniL { get { return aniL; } set { aniL = value; } }

        public CAni NewAni(string defName, string label, string mod)
        {
            CAni temp = new CAni(defName, label, mod);

            AniL.Add(temp);

            return null;
        }
    }
    //__________________________________________________________________________________________//

    public class CAniFactory : IExposable
    {
        public CAniFactory() //ACT data
        //loaded data - to use, to be deduped in NDC
        {
            cAniL = new List<CAni>();
        }

        private static List<CAni> cAniL;
        public static List<CAni> CAniL { get { return cAniL; } set { cAniL = value; } }

        private CAni temp;
        public CAni Temp { get { return temp; } set { temp = value; } }

        public CAni NewCAni(string defName, string label, string mod)
        {
            this.temp = new CAni(defName, label, mod);
            cAniL.Add(temp);
            return null;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look<CAni>(ref temp, "CAni");
        }
    }

    //__________________________________________________________________________________________//

    public class CAni : IExposable
    {
        public static TextInfo myTI = new CultureInfo("en-GB", false).TextInfo;

        public CAni(string defName, string label, string mod)
        {
            this.defName = defName;
            this.label = myTI.ToTitleCase(label);
            this.mod = mod;

        }
        public CAni() { }

        private string defName;
        public string DefName { get { return this.defName; } set { this.defName = value; } }

        private string label;
        public string Label { get { return this.label; } set { this.label = value; } }

        private string mod;
        public string Mod { get { return this.mod; } set { this.mod = value; } }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref defName, "DefName");
            Scribe_Values.Look<string>(ref label, "Label");
            Scribe_Values.Look<string>(ref mod, "Mod");
        }
    }
}

