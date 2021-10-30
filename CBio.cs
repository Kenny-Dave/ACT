using System.Collections.Generic;
using System.Globalization;
using Verse;

namespace AnimalCommonalityTweaker
{
    public class BioFactory
    {
        public BioFactory() //game data
        {
            bioL = new List<CBio>();
        }

        private static List<CBio> bioL;
        public static List<CBio> BioL { get { return bioL; } set { bioL = value; } }

        public CBio NewBio(string defName, string label, string bioMod, float animalDensity)
        {
            CBio temp = new CBio(defName, label, bioMod, animalDensity);

            bioL.Add(temp);

            return null;
        }
    }
    //__________________________________________________________________________________________//

    public class CBioFactory : IExposable
    {
        public CBioFactory() //ACT data
        {
            cBioL = new List<CBio>();
        }

        private static List<CBio> cBioL;
        public static List<CBio> CBioL { get { return cBioL; } set { cBioL = value; } }

        private CBio temp;
        public CBio Temp { get { return temp; } set { temp = value; } }

        public CBio NewCBio(string defName, string label, string bioMod, float animalDensity)
        {
            this.temp = new CBio(defName, label, bioMod, animalDensity);

            cBioL.Add(temp);

            return null;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look<CBio>(ref temp, "CBio");
        }
    }

    //__________________________________________________________________________________________//

    public class CBio : IExposable
    {
        public static TextInfo myTI = new CultureInfo("en-GB", false).TextInfo;

        public CBio(string defName, string label, string bioMod, float animalDensity)
        {
            this.defName = defName;
            this.label = myTI.ToTitleCase(label);
            this.bioMod = myTI.ToTitleCase(bioMod);
            this.animalDensity = animalDensity;
            this.animalDensityDef = animalDensity;
        }
        public CBio() { }

        private string defName;
        public string DefName { get { return this.defName; } set { this.defName = value; } }

        private string label;
        public string Label { get { return this.label; } set { this.label = value; } }

        private string bioMod;
        public string BioMod { get { return this.bioMod; } set { this.bioMod = value; } }

        private float animalDensity;
        public float AnimalDensity { get { return this.animalDensity; } set { this.animalDensity = value; } }

        private float animalDensityDef;
        public float AnimalDensityDef { get { return this.animalDensityDef; } set { this.animalDensityDef = value; } }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref defName, "DefName");
            Scribe_Values.Look<string>(ref label, "Label");
            Scribe_Values.Look<string>(ref bioMod, "bioMod");
            Scribe_Values.Look<float>(ref animalDensity, "animalDensity");
            Scribe_Values.Look<float>(ref animalDensityDef, "animalDensityDef");

        }
    }
    /*
    public class BioSub
    {
        public BioSub(BiomeDef remove, BiomeDef add)
        {
            this.remove = remove;
            this.toAdd = add;
        }
        private BiomeDef remove;
        public BiomeDef Remove;

        private BiomeDef toAdd;
        public BiomeDef ToAdd;

    }
    */
}

