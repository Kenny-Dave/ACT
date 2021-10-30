using Verse;

namespace AnimalCommonalityTweaker
{
    [StaticConstructorOnStartup]
    public class StartUp
    {
        static StartUp()
        {
            //has to run from a static constructor, as in the ACTMod : Mod constructor is before the game data is loaded. It seems. 
            NDC.NDCBuild();
            XMLWriter.Writer();
        }
    }
}
