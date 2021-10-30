using System.Collections.Generic;
using System.IO;
using Verse;

namespace AnimalCommonalityTweaker
{
    public static class XMLWriter
    {
        public static void Writer()
        {
            string path = ACTMod.ModRootPath + "\\Patches\\GeneratedPatches.xml";

            //Log.Message(MyVariable.ToString());

            using (StreamWriter src = new StreamWriter(path, false))
            {
                Header(src);
                Footer(src);
                src.Close();
            }

            List<CCom> comAdjL;
            comAdjL = Filter.ZeroFil(CComFactory.CComL, "Adjusted");
            List<CBio> cBioL = CBioFactory.CBioL;

            if (ACTSettings.ACTActive == true && comAdjL.CountAllowNull() != 0)
            // only writes content if you've got the active selected. Will write a file with no action otherwise
            // a blank file will cause a fatal Rimboot.
            {
                using (StreamWriter sr = new StreamWriter(path, false))
                {
                    Header(sr);

                    foreach (CBio cBio in cBioL)
                    {
                        BioBody(sr, cBio.DefName, cBio.AnimalDensity);
                    }

                    foreach (CCom com in comAdjL)
                    {
                        string bio = (com.CBio == "BiomesIslands_DesertIsland" ? "BiomesIslands_DesertIslandBase" : com.CBio);
                        bio = (bio == "BiomesIslands_DesertArchipelago" ? "BiomesIslands_DesertIslandBase" : bio);

                        //bio = (bio == "BiomesIslands_BorealIsland" ? "BiomesIslands_BorealIslandBase" : bio); //Island is the base for this one. 
                        bio = (bio == "BiomesIslands_BorealArchipelago" ? "BiomesIslands_BorealIsland" : bio);

                        bio = (bio == "BiomesIslands_TemperateIsland" ? "BiomesIslands_TemperateIslandBase" : bio);
                        bio = (bio == "BiomesIslands_TemperateArchipelago" ? "BiomesIslands_TemperateIslandBase" : bio);

                        bio = (bio == "BiomesIslands_TropicalIsland" ? "BiomesIslands_TropicalIslandBase" : bio);
                        bio = (bio == "BiomesIslands_TropicalArchipelago" ? "BiomesIslands_TropicalIslandBase" : bio);

                        bio = (bio == "BiomesIslands_TundraIsland" ? "BiomesIslands_TundraIslandBase" : bio);
                        bio = (bio == "BiomesIslands_TundraArchipelago" ? "BiomesIslands_TundraIslandBase" : bio);
                        //because the wild animal defs are in the base for these.
                        //Could change to adding a line to the XML to check the base before writing
                        //the patches have to go where the data is stored, and there's 3 locations: BiomeDef, ThingKindDef, and the above for parent of the BiomeDef. 

                        Body(sr, bio, com.CAni, com.Com);
                    }
                    Footer(sr);
                    sr.Close();
                }
            }
        }
        public static void Header(StreamWriter sr)
        {
            sr.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sr.WriteLine("");
            sr.WriteLine("<Patch>");
            sr.WriteLine("");
            sr.WriteLine(" <Operation Class=\"PatchOperationSequence\">");
            sr.WriteLine("     <success>Always</success>");
            sr.WriteLine("");
            sr.WriteLine("     <operations>");
        }

        public static void BioBody(StreamWriter sr, string biodef, float anidensity)
        {
            sr.WriteLine("");
            sr.WriteLine("      <li Class=\"PatchOperationReplace\">");
            sr.WriteLine("      <success>Always</success>");
            sr.WriteLine("          <xpath>*/BiomeDef[defName=\"" + biodef + "\"]/animalDensity</xpath>");
            sr.WriteLine("              <value>");
            sr.WriteLine("                  <animalDensity>" + anidensity + "</animalDensity>");
            sr.WriteLine("              </value>");
            sr.WriteLine("      </li>");
        }


        public static void Body(StreamWriter sr, string bio, string ani, float com)
        {
            sr.WriteLine("");
            sr.WriteLine("<li Class=\"PatchOperationConditional\">");
            sr.WriteLine("     <success>Always</success>");
            sr.WriteLine("<xpath>/Defs/ThingDef[defName=\"" + ani + "\"]</xpath>");
            sr.WriteLine("");
            sr.WriteLine("     <match Class=\"PatchOperationConditional\">");
            sr.WriteLine("     <success>Always</success>");
            sr.WriteLine("     <xpath>/Defs/ThingDef[defName=\"" + ani + "\"]/race/wildBiomes/" + bio + "</xpath>");
            sr.WriteLine("");
            sr.WriteLine("          <match Class=\"PatchOperationReplace\">");
            sr.WriteLine("               <success>Always</success>");
            sr.WriteLine("               <xpath>/Defs/ThingDef[defName=\"" + ani + "\"]/race/wildBiomes/" + bio + "</xpath>");
            sr.WriteLine("                    <value>");
            sr.WriteLine("                         <" + bio + ">" + com + "</" + bio + ">");
            sr.WriteLine("                    </value>");
            sr.WriteLine("          </match>");
            sr.WriteLine("");
            sr.WriteLine("          <nomatch Class=\"PatchOperationConditional\">");
            sr.WriteLine("          <success>Always</success>");
            sr.WriteLine("          <xpath> */BiomeDef[defName=\"" + bio + "\"]</xpath>");
            sr.WriteLine("");
            sr.WriteLine("               <match Class=\"PatchOperationConditional\">");
            sr.WriteLine("               <success>Always</success>");
            sr.WriteLine("               <xpath> */BiomeDef[defName=\"" + bio + "\"]/wildAnimals/" + ani + "</xpath>");
            sr.WriteLine("");
            sr.WriteLine("                    <match Class=\"PatchOperationReplace\">");
            sr.WriteLine("                    <success>Always</success>");
            sr.WriteLine("                    <xpath> */BiomeDef[defName=\"" + bio + "\"]/wildAnimals/" + ani + "</xpath>");
            sr.WriteLine("                         <value>");
            sr.WriteLine("                              <" + ani + ">" + com + "</" + ani + ">");
            sr.WriteLine("                         </value>");
            sr.WriteLine("                    </match>");
            sr.WriteLine("");
            sr.WriteLine("                    <nomatch Class = \"PatchOperationAdd\">");
            sr.WriteLine("                    <success>Always</success>");
            sr.WriteLine("                    <xpath> */BiomeDef[defName=\"" + bio + "\"]/wildAnimals</xpath>");
            sr.WriteLine("                         <value>");
            sr.WriteLine("                              <" + ani + ">" + com + "</" + ani + ">");
            sr.WriteLine("                         </value>");
            sr.WriteLine("                    </nomatch>");
            sr.WriteLine("               </match>");
            sr.WriteLine("               <nomatch>");
            sr.WriteLine("               </nomatch>");
            sr.WriteLine("          </nomatch>");
            sr.WriteLine("     </match>");
            sr.WriteLine("");
            sr.WriteLine("     <nomatch>");
            sr.WriteLine("     </nomatch>");
            sr.WriteLine("");
            sr.WriteLine("</li>");

        }
        public static void Footer(StreamWriter sr)
        {
            sr.WriteLine("");
            sr.WriteLine("     </operations>");
            sr.WriteLine("");
            sr.WriteLine(" </Operation>");
            sr.WriteLine("");
            sr.WriteLine("</Patch>");
        }

    }
}