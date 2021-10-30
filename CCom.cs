using System.Collections.Generic;
using System.Globalization;
using Verse;

namespace AnimalCommonalityTweaker
{
    public class ComFactory
    {
        public ComFactory() //game data
        {
            comL = new List<CCom>();
        }

        private static List<CCom> comL;
        public static List<CCom> ComL { get { return comL; } set { comL = value; } }

        public CCom NewCom(string cBio, string cBioLabel, string cAni, string cAniLabel, string cAniMod, string cAniModLabel, float defCom, float com)
        {
            CCom temp = new CCom(cBio, cBioLabel, cAni, cAniLabel, cAniMod, cAniModLabel, defCom, com);
            comL.Add(temp);
            return null;
        }
    }
    //___________________________________________________________________________________//

    public class CComFactory : IExposable
    {
        public CComFactory() //ACT Data
        {
            cComL = new List<CCom>();
        }

        private static List<CCom> cComL;
        public static List<CCom> CComL { get { return cComL; } set { cComL = value; } }

        private CCom temp;
        public CCom Temp { get { return temp; } set { temp = value; } }

        public CCom NewCCom(string cBio, string cBioLabel, string cAni, string cAniLabel, string cAniMod, string cAniModLabel, float defCom, float com)
        {
            this.temp = new CCom(cBio, cBioLabel, cAni, cAniLabel, cAniMod, cAniModLabel, defCom, com);
            cComL.Add(temp);
            return null;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look<CCom>(ref temp, "CCom");
        }
    }

    public class CCom : IExposable
    {
        public static TextInfo myTI = new CultureInfo("en-GB", false).TextInfo;

        public CCom(string cBio, string cBioLabel, string cAni, string cAniLabel, string cAniMod, string cAniModLabel, float defCom, float com)
        {
            this.cBio = cBio;
            this.cBioLabel = myTI.ToTitleCase(cBioLabel);
            this.cAni = cAni;
            this.cAniLabel = myTI.ToTitleCase(cAniLabel);
            this.cAniMod = cAniMod;
            this.cAniModLabel = myTI.ToTitleCase(cAniModLabel);
            this.defCom = defCom;
            this.com = com;
            this.comBacker = com;
            this.comLiv = com;
        }
        public CCom() { }

        private string cBio;
        public string CBio { get { return this.cBio; } set { this.cBio = value; } }

        private string cBioLabel;
        public string CBioLabel { get { return this.cBioLabel; } set { this.cBioLabel = value; } }

        private string cAni;
        public string CAni { get { return this.cAni; } set { this.cAni = value; } }

        private string cAniLabel;
        public string CAniLabel { get { return this.cAniLabel; } set { this.cAniLabel = value; } }

        private string cAniMod;
        public string CAniMod { get { return this.cAniMod; } set { this.cAniMod = value; } }

        private string cAniModLabel;
        public string CAniModLabel { get { return this.cAniModLabel; } set { this.cAniModLabel = value; } }

        private float defCom;
        public float DefCom { get { return this.defCom; } set { this.defCom = value; } }

        private float com;
        public float Com { get { return this.com; } set { this.com = value; } }

        private float comBacker;
        public float ComBacker { get { return this.comBacker; } set { this.comBacker = value; } }

        private float comLiv;
        public float ComLiv { get { return this.comLiv; } set { this.comLiv = value; } }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref cBio, "CBio");
            Scribe_Values.Look<string>(ref cBioLabel, "CBioLabel");
            Scribe_Values.Look<string>(ref cAni, "CAni");
            Scribe_Values.Look<string>(ref cAniLabel, "CAniLabel");
            Scribe_Values.Look<string>(ref cAniMod, "CAniMod");
            Scribe_Values.Look<string>(ref cAniModLabel, "CAniModLabel");
            Scribe_Values.Look<float>(ref defCom, "CDefCom");
            Scribe_Values.Look<float>(ref com, "Com");
            Scribe_Values.Look<float>(ref comBacker, "ComBacker");
        }
    }
}

