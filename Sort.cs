using System.Collections.Generic;
using System.Linq;
using Verse;


namespace AnimalCommonalityTweaker
{
    public class Sort
    {
        public static List<CCom> cComLFS;
        public static List<CCom> cComLS;

        public static List<CCom> ComSort(List<CCom> cComLF)
        {
            if (cComLF == null) { return cComLF; }

            IEnumerable<CCom> cComLIS;
            IEnumerable<CCom> comLIS;

            List<CCom> cComLFS = new List<CCom>();

            string SortingKey = ACTMod.SortingKey;

            //Log.Message("CAni for cComLF[0]: " + cComLF[0].CAni);

            //Log.Message("Getting to case");
            //Log.Message("aniL ==null:"+ (aniLF ==null).ToString());
            //Log.Message("SORT aniLF:" + aniLF.Count().ToString());
            //Log.Message("SortingKey: "+SortingKey);
            switch (SortingKey)
            {
                case "Ani+":
                    comLIS = from CCom in cComLF
                             orderby CCom.CAniLabel
                             ascending
                             select CCom;
                    cComLFS = comLIS.ToList();
                    break;

                case "Ani-":
                    comLIS = from CCom in cComLF
                             orderby CCom.CAniLabel
                             descending
                             select CCom;
                    cComLFS = comLIS.ToList();
                    break;

                case "Mod+":
                    comLIS = from CCom in cComLF
                             orderby CCom.CAniMod
                             ascending
                             select CCom;
                    cComLFS = comLIS.ToList();
                    break;

                case "Mod-":
                    comLIS = from CCom in cComLF
                             orderby CCom.CAniMod
                             descending
                             select CCom;

                    cComLFS = comLIS.ToList();
                    break;
                case "Liv+":
                    cComLIS = from CCom in cComLF
                              orderby CCom.ComLiv
                              ascending
                              select CCom;

                    cComLFS = cComLIS.ToList();
                    break;

                case "Liv-":
                    cComLIS = from CCom in cComLF
                              orderby CCom.ComLiv
                              descending
                              select CCom;

                    cComLFS = cComLIS.ToList();
                    break;
                case "Def+":
                    cComLIS = from CCom in cComLF
                              orderby CCom.DefCom
                              ascending
                              select CCom;

                    cComLFS = cComLIS.ToList();
                    break;

                case "Def-":
                    cComLIS = from CCom in cComLF
                              orderby CCom.DefCom
                              descending
                              select CCom;

                    cComLFS = cComLIS.ToList();
                    break;

                case "Com+":
                    cComLIS = from CCom in cComLF
                              orderby CCom.Com
                              ascending
                              select CCom;

                    cComLFS = cComLIS.ToList();
                    break;

                case "Com-":
                    cComLIS = from CCom in cComLF
                              orderby CCom.Com
                              descending
                              select CCom;

                    cComLFS = cComLIS.ToList();
                    break;
                case "Bio+":
                    cComLIS = from CCom in cComLF
                              orderby CCom.CBioLabel
                              ascending
                              select CCom;

                    cComLFS = cComLIS.ToList();
                    break;

                case "Bio-":
                    cComLIS = from CCom in cComLF
                              orderby CCom.CBioLabel
                              descending
                              select CCom;

                    cComLFS = cComLIS.ToList();
                    break;
                default:
                    Log.Message("In the default ACT switch in comsort. This should never happen.");
                    break;
            }

            ACTMod.ACountF = cComLFS.Count;

            MaxComDispFull(cComLFS);

            return cComLFS;
        }

        public static void MaxComDispFull(List<CCom> cComLFS)
        //checks the max value of the displayed coms and sets the slider max to X x that. 
        {
            if (cComLFS != null)
            {
                ACTMod.maxComDisp = 0.1f;
                //this acts as a floor

                foreach (CCom com in cComLFS)
                {
                    if (com.Com > ACTMod.maxComDisp)
                    {
                        ACTMod.maxComDisp = com.Com;
                    }
                }
                ACTMod.sliderMax = ACTMod.maxComDisp * 3f;
                //Log.Message("maxComDisp: " + ACTMod.maxComDisp.ToString());
            }
        }

        public static List<CAni> AniDedupe(List<CCom> cComL)
        //the animal list drop down menu is active, and depends on the zero selection. So must come off the cComLFS, not AniL. Then requires deduping, which 
        // is what this is. 
        {
            List<CAni> cAniLF = new List<CAni>();
            CAni ani;

            if (cComL == null) { return null; }

            foreach (CCom com in cComL)
            {
                if (cComL.Where(CCom => CCom.CAni == com.CAni).First() == com)
                {
                    ani = new CAni(com.CAni, com.CAniLabel, com.CAniMod);
                    cAniLF.Add(ani);
                }
                else { }
            }
            return cAniLF;
        }
    }
}
