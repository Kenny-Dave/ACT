using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AnimalCommonalityTweaker
{
    class Filter
    {
        //_________________________________________________________________________________________________________//filter collator.

        static string SortingKey = ACTMod.SortingKey;

        public static List<CCom> cComLF; //filtered
        public static List<CCom> cComLFS; //filtered and sorted output

        public static List<CCom> ComFil()
        {
            // Triggers when the selected filter variables change in the getset.

            if (ACTMod.cComL.CountAllowNull() == 0) { return null; }

            cComLF = ACTMod.cComL;
            //Log.Message("cComLF.count after zeros: " + cComLF.Count().ToString());
            List<CCom> cComLFB = BioFil(cComLF);
            //Log.Message("cComLB.count after bios: " + cComLFB.Count().ToString());
            List<CCom> cComLFZ = ZeroFil(cComLFB, ACTMod.ZerS);
            //Log.Message("cComLF.count after zeros: " + cComLFZ.Count().ToString());
            List<CCom> cComLFA = AniFil(cComLFZ);
            //Log.Message("cComLF.count after Anis: " + cComLFA.Count().ToString());
            cComLF = ModFil(cComLFA);
            //Log.Message("cComLF.count after mods: " + cComLF.Count().ToString()); 

            try { cComLFS.Clear(); }

            catch { /*Log.Message("Tried to clear ComLFS and failed");*/ }

            cComLFS = Sort.ComSort(cComLF);

            return cComLFS;
        }

        //_________________________________________________________________________________________________________//Filter by Biome

        public static List<CCom> BioFil(List<CCom> cComLF)
        {

            if (cComLF.CountAllowNull() == 0) { return cComLF; }

            IEnumerable<CCom> cComLI;

            if (ACTMod.BioS != null)
            {
                cComLI = cComLF.Where(temp => temp.CBio == ACTMod.BioS.DefName); //filter C by biome... Need to sep this out too. 
                cComLF = cComLI.ToList();
            }

            return cComLF;
        }

        //_________________________________________________________________________________________________________//filter by mod.

        public static List<CCom> ModFil(List<CCom> cComLF)
        {
            if (cComLF.CountAllowNull() == 0) { return cComLF; }

            CMod modS = ACTMod.ModS;

            if (modS == null) { return cComLF; }

            IEnumerable<CCom> IE =
                from CCom in cComLF
                where CCom.CAniMod == modS.PackageId
                select CCom;

            cComLF = IE.ToList();

            return cComLF;
        }

        //_________________________________________________________________________________________________________//filter by animal.

        public static List<CCom> AniFil(List<CCom> cComFL)
        {
            if (cComLF.CountAllowNull() == 0) { return cComLF; }

            CAni aniS = ACTMod.AniS;
            if (aniS == null) { return cComFL; }
            if (aniS.DefName == null) { return cComFL; }

            IEnumerable<CCom> IE =
                from CCom in cComFL
                where CCom.CAni == aniS.DefName
                select CCom;

            cComFL = IE.ToList();

            return cComFL;
        }

        //_________________________________________________________________________________________________________//filter by zeros.

        public static List<CCom> ZeroFil(List<CCom> cComLF, string ZerS)
        {
            if (cComLF.CountAllowNull() == 0) { return cComLF; }

            IEnumerable<CCom> cComLI;
            cComLI = cComLF.AsEnumerable();

            switch (ZerS)
            {
                case "All":
                    break;
                case "Non Zeros":
                    cComLI = cComLI.Where(temp => temp.Com != 0);
                    break;
                case "Only Zeros":
                    cComLI = cComLI.Where(temp => temp.Com == 0);
                    break;
                case "Default Non-zeros":
                    cComLI = cComLI.Where(temp => temp.DefCom != 0);
                    break;
                case "Default Zeros":
                    cComLI = cComLI.Where(temp => temp.DefCom == 0);
                    break;
                case "Added":
                    cComLI = cComLI.Where(temp => temp.Com != 0f && temp.DefCom == 0f);
                    break;
                case "Removed":
                    cComLI = cComLI.Where(temp => temp.Com == 0f && temp.DefCom != 0f);
                    break;
                case "Adjusted":
                    cComLI = cComLI.Where(temp => temp.Com != temp.DefCom);
                    break;
                case "Live ≠ Com":
                    cComLI = cComLI.Where(temp => temp.Com != temp.ComLiv);
                    break;
                default:
                    break;
            }
            cComLF = cComLI.ToList();

            return cComLF;
        }
        //_________________________________________________________________________________________________________//

    }
}
