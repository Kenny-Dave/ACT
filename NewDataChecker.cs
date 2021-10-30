using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;


namespace AnimalCommonalityTweaker
{
    class NDC
    {
        //Game data: 
        public static List<ThingDef> aniL;
        public static List<BiomeDef> bioL;
        //public static List<CCom> comL; //doesn't need to be defined here as it's in ComFactory.

        //AnimalCommonalityTweaker data: 
        public static List<CAni> cAniL = new List<CAni>();
        public static List<CBio> cBioL = new List<CBio>();
        public static List<CCom> cComL = new List<CCom>();
        public static List<CMod> cModL = new List<CMod>();

        private static int aniAdded;
        private static int bioAdded; //to message user. 
        private static int comAdded;
        private static int modAdded;

        //coms come from Settings file. Only added by this if they are missing, for example if there's a new biome or animal. The data is in the biome in the game. 

        public static void NDCBuild()
        {
            ComFactory ComFac = new ComFactory(); //game data

            CAniFactory cAniFac = new CAniFactory(); //ACTData
            CBioFactory cBioFac = new CBioFactory();
            CComFactory cComFac = new CComFactory();
            CModFactory cModFac = new CModFactory();

            try
            {
                if (cAniL != null)
                {
                    cAniL = ACTSettings.cAniL;
                    CAniFactory.CAniL = cAniL;
                }
                if (cBioL != null)
                {
                    cBioL = ACTSettings.cBioL;
                    CBioFactory.CBioL = cBioL;
                }
                if (cComL != null)
                {
                    cComL = ACTSettings.cComL;
                    CComFactory.CComL = cComL;
                }
                if (cModL != null)
                {
                    cModL = ACTSettings.cModL;
                    CModFactory.CModL = cModL;
                }
            }
            catch { Log.Message("ACT could not find pre-existing settings data to load."); }
            //this should be a link and only needs to be done once; check for the list of strings though.

            //_______________________________________________________________________________________________Animals

            AniPulls aPTAnimals = new AniPulls();
            aniL = aPTAnimals.GetList(); //this is the list of all animals.

            aniAdded = 0;
            if (CAniFactory.CAniL == null) { CAniFactory.CAniL = new List<CAni>(); }

            if (aniL.CountAllowNull() != 0)
            {
                foreach (ThingDef Animal in aniL) //animals into c
                {
                    if (CAniFactory.CAniL.Where(temp => temp.DefName == Animal.defName).Count() == 0)
                    {
                        cAniFac.NewCAni(Animal.defName, Animal.label, Animal.modContentPack.PackageId);
                        aniAdded += 1;
                    }
                    if (CModFactory.CModL == null
                        || (CModFactory.CModL.Where(temp => temp.PackageId.ToString() == Animal.modContentPack.PackageId).Count() == 0))
                    {
                        try {
                            cModFac.NewCMod(Animal.modContentPack.PackageId, Animal.modContentPack.Name);
                            modAdded += 1;
                        }
                        catch { }
                    }
                }
            }
            else { Log.Message("ACT error loading animal list from game data."); }

            //_______________________________________________________________________________________________Biomes

            try { bioL = DefDatabase<BiomeDef>.AllDefsListForReading; }
            catch { Log.Message("ACT failed to read biomeDefs from game."); }

            bioAdded = 0;

            if (CBioFactory.CBioL == null) { CBioFactory.CBioL = new List<CBio>(); }

            foreach (BiomeDef bio in bioL) //biomes into c,deduped;
            {
                //Log.Message("Biome: "+bio.label+", animalDensity: " + bio.animalDensity);

                if (CBioFactory.CBioL.Where(temp => temp.DefName == bio.defName).Count() == 0)
                {
                    List<string> wildAnimalList = new List<string>();

                    cBioFac.NewCBio(bio.defName, bio.label, bio.modContentPack.PackageId, bio.animalDensity);
                    bioAdded += 1;
                }
                else { }
                //___________________________________________________________________________________________Com

                foreach (ThingDef Ani in aniL)
                //commonality link table between biomes and animals, with commonality and default commonality record.
                {
                    string bioDefName = bio.defName;
                    string bioLabel = bio.label;
                    string aniDefName = Ani.defName;
                    string aniLabel = Ani.label;
                    string aniMod;
                    string aniModLabel;

                    try {
                        aniMod = Ani.modContentPack.PackageId;
                        aniModLabel = Ani.modContentPack.Name;
                    }
                    catch { continue; }

                    if(aniMod == null) { continue; }

                    float commonality;

                    try { string t = "PawnKindDef name pull: " + PawnKindDef.Named(Ani.defName).defName; }
                    catch
                    {
                        //Log.Message("ACT PawnKindDef Assessed and failed for "+Ani.defName);
                        continue;
                    }

                    try { commonality = bio.CommonalityOfAnimal(PawnKindDef.Named(Ani.defName)); }
                    catch
                    {
                        //Log.Message("ACT failed to pull com for " + bioDefName + " " + aniDefName);
                        continue;
                    }

                    if (bioDefName != null && aniDefName != null)
                    { ComFac.NewCom(bioDefName, bioLabel, aniDefName, aniLabel, aniMod, aniModLabel, commonality, commonality); }

                    //there's two com lists: the one stored in ACT cCom, and this one Com generated.
                    //Deduping and copying from the generated to the ACT is 
                    //done separately in it's own section below. 

                    //cCom includes zeros as that is necessary to zero off items that the player wants. 
                }
            }
            //_____________________________________________________ Dedupe and copy from comL to cComL

            foreach (CCom com in ComFactory.ComL)
            {
                if (CComFactory.CComL == null) { CComFactory.CComL = new List<CCom>(); }

                if (CComFactory.CComL.Where(temp => temp.CBio == com.CBio && temp.CAni == com.CAni).Count() == 0)
                {
                    CComFactory.CComL.Add(com);
                    comAdded += 1;
                }
                else
                //pops the live com into ComLiv every boot. Fails silently if it can't find it. 
                {
                    try { CComFactory.CComL.Where(temp => temp.CBio == com.CBio && temp.CAni == com.CAni).First().ComLiv = com.Com; }
                    catch { }
                }
            }

            //_____________________________________________________  

            if (aniAdded != 0 || bioAdded != 0)
            {
                Log.Message("ACT New animals added: " + aniAdded +
                    ", New biomes added: " + bioAdded + ", Animal mods added: " + modAdded);
            }

            ACTSettings.ACount = CAniFactory.CAniL.Count();
            ACTSettings.BCount = CBioFactory.CBioL.Count();
            ACTSettings.CCount = CComFactory.CComL.Count();

            ACTMod.ACount = ACTSettings.ACount;
            ACTMod.BCount = ACTSettings.BCount;
            ACTMod.CCount = ACTSettings.CCount;

            ACTMod.cAniL = CAniFactory.CAniL;
            ACTMod.cBioL = CBioFactory.CBioL;
            ACTMod.cComL = CComFactory.CComL;

            ACTSettings.cAniL = CAniFactory.CAniL;
            ACTSettings.cBioL = CBioFactory.CBioL;
            ACTSettings.cComL = CComFactory.CComL;
            ACTSettings.cModL = CModFactory.CModL;
        }

    }
}

