using System.Collections.Generic;
using System.Globalization;
using Verse;

namespace AnimalCommonalityTweaker
{
    public class CModFactory
    {
        public CModFactory() //game data
        {
            CModL = new List<CMod>();
        }

        private static List<CMod> cModL;
        public static List<CMod> CModL { get { return cModL; } set { cModL = value; } }

        private CMod temp;
        public CMod Temp { get { return temp; } set { temp = value; } }

        public CMod NewCMod(string packageId, string label)
        {
            CMod temp = new CMod(packageId, label);

            if (cModL == null) { cModL = new List<CMod>(); }
            cModL.Add(temp);

            return null;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look<CMod>(ref temp, "CMod");
        }
    }

    //__________________________________________________________________________________________//

    //__________________________________________________________________________________________//

    public class CMod : IExposable
    {
        public static TextInfo myTI = new CultureInfo("en-GB", false).TextInfo;

        public CMod(string packageId, string label)
        {
            this.packageId = packageId;
            this.label = myTI.ToTitleCase(label);
        }
        public CMod() { }

        private string packageId;
        public string PackageId { get { return this.packageId; } set { this.packageId = value; } }

        private string label;
        public string Label { get { return this.label; } set { this.label = value; } }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref packageId, "PackageId");
            Scribe_Values.Look<string>(ref label, "Label");
        }
    }
}

