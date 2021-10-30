using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Verse;

namespace AnimalCommonalityTweaker
{
    class FloatMenus
    {
        public static TextInfo myTI = new CultureInfo("en-GB", false).TextInfo;

        //_______________________________________________________________________________________//Filter Biome

        public FloatMenu BioMenuLabels()
        {
            FloatMenuOption bioFloatMenuOptionI;
            List<FloatMenuOption> bioFloatMenuOptionL = new List<FloatMenuOption> { };
            FloatMenu bioFloatMenuOptionML;

            bioFloatMenuOptionI = BioMenuBuilder("All", "All");
            bioFloatMenuOptionL.Add(bioFloatMenuOptionI);

            foreach (var biome in NDC.bioL)
            {
                string defName = biome.defName;
                string label = myTI.ToTitleCase(biome.label);
                bioFloatMenuOptionI = BioMenuBuilder(defName, label);
                bioFloatMenuOptionL.Add(bioFloatMenuOptionI);
            }
            bioFloatMenuOptionML = new FloatMenu(bioFloatMenuOptionL);

            return bioFloatMenuOptionML;
        }

        //________________________________________________

        public FloatMenuOption BioMenuBuilder(string defName, string label)
        {
            FloatMenuOption bioFloatMenuOptionI = new FloatMenuOption("BioI error.", delegate { Log.Message("BioI error."); });

            if (label == "All")
            {
                bioFloatMenuOptionI.Label = label;
                bioFloatMenuOptionI.action = delegate
                {
                    ACTMod.BioS = null;
                };
            }
            else
            {
                bioFloatMenuOptionI.Label = label;
                bioFloatMenuOptionI.action = delegate
                {
                    ACTMod.BioS = ACTMod.cBioL.Where(CBio => CBio.DefName == defName).First();
                };
            }
            return bioFloatMenuOptionI;
        }

        //_______________________________________________________________________________________//Filter Zeros

        public FloatMenu ZeroMenuLabels()
        {
            FloatMenuOption FilterI;
            List<FloatMenuOption> FilterL = new List<FloatMenuOption> { };//for the Biome float menu.
            FloatMenu FilterML;

            foreach (var c in FilterMenuOption)
            {
                FilterI = ZeroMenuBuilder(c);
                FilterL.Add(FilterI);
            }
            FilterML = new FloatMenu(FilterL);

            return FilterML;
        }

        public static List<String> FilterMenuOption = new List<string>
        { "All", "Non Zeros", "Only Zeros", "Default Non-zeros", "Default Zeros", "Added", "Removed", "Adjusted", "Live ≠ Com" };

        //_______________________________________________________________________________________//

        public FloatMenuOption ZeroMenuBuilder(string c)
        {
            FloatMenuOption filterI = new FloatMenuOption("FilterI error.", delegate { Log.Message("FilterI error."); });

            filterI.Label = c.ToString();
            filterI.action = delegate
            {
                ACTMod.ZerS = c;
            };
            return filterI;
        }

        //_______________________________________________________________________________________//Filter Mod

        public FloatMenu ModMenuLabels()
        {
            FloatMenuOption ModI;
            List<FloatMenuOption> ModL = new List<FloatMenuOption> { };
            FloatMenu ModML;

            ModI = ModMenuBuilder("All", "All");
            ModL.Add(ModI);

            try { /*Log.Message(CModFactory.CModL.CountAllowNull().ToString()); */} catch { }

            if (CModFactory.CModL.CountAllowNull() != 0)
            {
                foreach (var c in CModFactory.CModL)
                {
                    ModI = ModMenuBuilder(c.PackageId, c.Label);
                    ModL.Add(ModI);
                }
            }

            ModML = new FloatMenu(ModL);

            return ModML;
        }

        //_______________________________________________________________________________________//


        public FloatMenuOption ModMenuBuilder(string packageId, string label)
        {
            FloatMenuOption ModI = new FloatMenuOption("ModI error.", delegate { Log.Message("ModI error."); });

            if (label == "All")
            {
                ModI.Label = "All";
                ModI.action = delegate
                {
                    ACTMod.ModS = null;
                };
                return ModI;
            }
            CMod tempMod = null;

            tempMod = CModFactory.CModL.Where(temp => temp.PackageId == packageId).First();

            ModI.Label = label;
            ModI.action = delegate
            {
                ACTMod.ModS = tempMod;
            };

            return ModI;
        }

        //_______________________________________________________________________________________//Filter Ani

        public FloatMenu AniMenuBuilder(CAni AniS)
        {
            List<CCom> cComL = ACTMod.cComL;
            List<FloatMenuOption> aniMenuL = new List<FloatMenuOption> { };
            FloatMenuOption aniMenuI = new FloatMenuOption("anMenuI error.", delegate { Log.Message("anMenuI error."); });

            aniMenuL.Add(AniMenuLabels("All", "All"));
            //This calculates a list of the visible line items. For use in the drop down menu. To keep the size down where possible.
            List<CCom> aniMenuList = Filter.BioFil(cComL);
            aniMenuList = Filter.ZeroFil(aniMenuList, ACTMod.ZerS);
            aniMenuList = Filter.ModFil(aniMenuList);
            aniMenuList = Sort.ComSort(aniMenuList);
            List<CAni> aniMenuListA = Sort.AniDedupe(aniMenuList);

            if (aniMenuList.Count > 2)
            {
                foreach (CAni anitemp2 in aniMenuListA)
                {
                    string labelText2 = anitemp2.Label;
                    string actiontarget2 = anitemp2.DefName;
                    aniMenuL.Add(AniMenuLabels(labelText2, actiontarget2));
                }
            }
            else
            {
                foreach (CAni anitemp in ACTMod.cAniL)
                {
                    string labelText2 = anitemp.Label;
                    string actiontarget2 = anitemp.DefName;
                    aniMenuL.Add(AniMenuLabels(labelText2, actiontarget2));
                }
            }
            FloatMenu aniMenu = new FloatMenu(aniMenuL);
            return aniMenu;
        }
        public static FloatMenuOption AniMenuLabels(string labelText, string actiontarget)
        {
            FloatMenuOption anMenuI = new FloatMenuOption("anMenuI error.", delegate { Log.Message("anMenuI error."); });

            anMenuI.Label = labelText;

            if (actiontarget == "All")
            {
                anMenuI.action = delegate { ACTMod.AniS = null; };
            }
            else
            {
                CAni targetAni = ACTMod.cAniL.Where(CAni => CAni.DefName == actiontarget).First();
                anMenuI.action = delegate { ACTMod.AniS = targetAni; };
            }

            return anMenuI;
        }
    }
}
