using System.Collections.Generic;

namespace AnimalCommonalityTweaker
{
    class InputDic
    {
        public static Dictionary<string, string> BiomeSubstitute;

        public static void InputDictionary()
        {
            BiomeSubstitute = new Dictionary<string, string>()
            {
                {"BiomesIslands_DesertIsland"        , "BiomesIslands_DesertIslandBase" },
                {"BiomesIslands_DesertArchipelago"   , "Discard" },
                //{"BiomesIslands_BorealIsland"        , "BiomesIslands_BorealIslandBase" }, //the former is the name of the base...
                {"BiomesIslands_BorealArchipelago"   , "Discard" },
                {"BiomesIslands_TemperateIsland"     , "BiomesIslands_TemperateIslandBase"},
                {"BiomesIslands_TemperateArchipelago", "Discard"},
                {"BiomesIslands_TropicalIsland"      , "BiomesIslands_TropicalIslandBase" },
                {"BiomesIslands_TropicalArchipelago" , "Discard" },
                {"BiomesIslands_TundraIsland"        , "BiomesIslands_TundraIslandBase" },
                { "BiomesIslands_TundraArchipelago"   , "Discard" },
                };

        }
    }
}
