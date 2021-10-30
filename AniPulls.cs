using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AnimalCommonalityTweaker
{

    public class AniPulls
    {
        //Pulls the ThingDef list and filters for animals. No doubt there is a better way to do this...    
        public List<ThingDef> allAnimalDefs = new List<ThingDef>();
        List<ThingDef> allDefs = DefDatabase<ThingDef>.AllDefsListForReading;

        public List<ThingDef> GetList()
        {
            return allAnimalDefs;
        }

        public AniPulls()
        {
            try
            {
                IEnumerable<ThingDef> AAD = from aallDefs in allDefs
                                            where aallDefs.race != null &&
                                            aallDefs.race.Animal == true &&
                                            aallDefs.race.IsMechanoid == false &&
                                            aallDefs.defName != "VAECaves_MorrowRim_TrollCave" &&
                                            //this one is missing a modContentPack.PackageID when VAE Caves and MorrowWind Trolls are both loaded. 
                                            aallDefs.defName != "FO_RBehemoth" &&
                                            aallDefs.defName != "FO_RMurkling" &&
                                            aallDefs.defName != "ooze_green" &&
                                            aallDefs.defName != "BiomesIslands_BaitfishBase" &&
                                            aallDefs.modContentPack.PackageId != null &&
                                            //missing PawnKindDefs
                                            aallDefs.modContentPack.PackageId != "torann.arimworldofmagic" &&
                                            aallDefs.modContentPack.PackageId != "scurvyez.fireflies" &&
                                            !(aallDefs.defName.Contains("Dryad"))
                                            select aallDefs;

                allAnimalDefs = AAD.ToList();
            
            }

            catch (Exception e)
            {
                Log.Message("LINQ failed." + e.Message);
            }
        }
    }
}


