using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Verse;

namespace AnimalCommonalityTweaker
{
    //[StaticConstructorOnStartup]
    public class ACTMod : Mod
    {
        private ACTSettings settings;

        public ACTMod(ModContentPack content) : base(content)
        {
            //Log.Message("RT constructor");

            this.settings = base.GetSettings<ACTSettings>(); //load settings from save files. Runs 3 times

            ModRootPath = content.RootDir;
        }

        public static TextInfo myTI = new CultureInfo("en-GB", false).TextInfo;
        /*
        static int gbiome = 0;
        static int gzero = 0;
        static int gmod = 0;
        static int gani = 0;
        static int gcoms = 0;
        static int glabels = 0;
        static int gslider = 0;
        these are all tickers for the error messages, so they only hit the log once. 
        */

        static int gbuilder = 0;
        // so certain things happen only once in the settings window on initialize;

        public static int ACount; //Animals installed count.
        public static int BCount; //Biomes installed count. 
        public static int CCount; //Conversion table count (A*B).
        public static int GCCount; //?
        public static int ACountF; //how many animals are shown in the filtered list.

        public static List<CAni> cAniL;
        public static List<CBio> cBioL;
        public static List<CCom> cComL;
        public static List<CCom> cComLFS; //ACT lists

        public static float ScaleFloat = 0f; //for the batch scaling. 
        public static string ScaleFloatString;
        public static float ShiftFloat = 0f; //needs rounding appropriately, and the log on the scale. And a text entry...
        public static string ShiftFloatString; //needs rounding appropriately, and the log on the scale. And a text entry...

        public static float ScaleFloatLast = 0f;
        public static float ShiftFloatLast = 0f;
        public static CBio BioSLast;
        public static string ZerSLast;
        public static CMod ModSLast;
        public static CAni AniSLast;
        public static bool UndoExists;
        public static bool RedoExists;
        public static string UndoButtonText = " ";
        public static bool UndoButtonDraw = false; //section for undo control.

        public static float maxComDisp = 1f; //Max commonality allowed. Not used. 
        public static float sliderMax = 2f; //the maximum values of the slider in the table. 
        public float adjCom; //the list of com records that have been adjusted, for the generated patch file.

        public List<float> comL; //for the float list
        public float com;
        public static int counter = 0; //for drawing the black boxes in table;
        public static Color colorLab;

        FloatMenus fm = new FloatMenus();

        public int lg = 8;//vertical gap between labels
        public int TableRowHeight = 30; //height of rows in table;
        Color TitleColor = Color.HSVToRGB(0.61f, 0.64f, 0.77f); //colour of black boxes
        private static Vector2 scrollPosition = new Vector2(0f, 0f);
        private const float ScrollBarWidthMargin = 18f;
        public static Rect expr_0B; //this is the Rect for the current line in LS, called whenever needed. 
        public static float AnimalRectHeight; //adjusting height of the table

        public static string SortingKey = "Ani+"; //this is the sorter key to maintain the sorting order of the animals.

        public static string ModRootPath;//for saving the XML patch;

        private static CBio bioS; //selected biome;
        public static CBio BioS
        {
            get { return bioS; }
            set
            {
                bioS = value;

                if (BioS == null)
                {
                    BioSLabel = "All";
                }
                else
                {
                    int len;
                    BioSLabel = myTI.ToTitleCase(BioS.Label);
                    if (BioS == null && AniS == null) { len = 14; }
                    else { len = 19; }
                    if (BioSLabel.Length > len) { BioSLabel = BioSLabel.Substring(0, len); }
                }

                if (ZerS != null && cComL.CountAllowNull() != 0)
                {
                    cComLFS = Filter.ComFil();
                    ACTSettings.BioS = bioS;
                }
            }
        }
        public static string BioSLabel;
        //button label

        private static string zerS;
        public static string ZerS //Selection for zero filter.
        {
            get { return zerS; }
            set
            {
                zerS = value;
                //Log.Message("tripping in ZerS constructor");
                if (ZerS != null && cComL.CountAllowNull() != 0)
                {
                    cComLFS = Filter.ComFil();
                    ACTSettings.ZerS = zerS;
                }
            }
        }
        public string ZeroLabel;

        private static CMod modS;
        public static CMod ModS //Selection for Mod filter.
        {
            get { return modS; }
            set
            {
                modS = value;
                ModSLabel = (value == null ? "All" : value.Label);

                if (ModSLabel.Length > 19) { ModSLabel = ModSLabel.Substring(0, 19); }

                if (ZerS != null && cComL.CountAllowNull() != 0)
                {
                    cComLFS = Filter.ComFil();
                    ACTSettings.ModS = modS;
                }

                if (AniS != null && cComL.CountAllowNull() != 0)
                {
                    if (AniS.Mod != ModS.PackageId)
                    {
                        AniS = null;
                    }
                }
                //If the selected animal is not in the mod that you just selected, it sets the animal filter to all. 
            }
        }

        public static List<string> ModList; //list for the filter drop down menu. 
        public static string ModSLabel; //label button on the filter menu drop down.

        private static CAni aniS;
        public static CAni AniS //Selection for Animal filter
        {
            get { return aniS; }
            set
            {
                aniS = value;

                if (cAniL != null && ZerS != null && cComL.CountAllowNull() != 0)
                {
                    cComLFS = Filter.ComFil();
                    ACTSettings.AniS = aniS;
                }
            }
        }
        public static string AniSLabel; //label for the animal selector
        //Ani drop down is an active list depending on the other selections. 

        //_________________________________________________________________________________________________________//WindowSettings
        //_________________________________________________________________________________________________________//

        public override void DoSettingsWindowContents(Rect inRect)
        {
            //Rect is 864 wide. 
            //584 high.

            if (gbuilder == 0)
            {
                //runs once at open to set default view.
                try { if (BioS == null) { BioS = cBioL[0]; } } catch { }

                try { if (ZerS == "") { ZerS = "Non Zeros"; } } catch { }
                try { if (ZerS == null) { ZerS = "Non Zeros"; } } catch { }

                if (ModS == null)
                {
                    ModSLabel = "All";
                }
                else
                {
                    ModSLabel = ModS.Label;
                    if (ModSLabel.Length > 19) { ModSLabel = ModSLabel.Substring(0, 19); }
                }

                gbuilder = 1;
                cComLFS = Filter.ComFil();
            }
            //_________________________________________________________________________________________________________//LS Titles with sorters

            float TopRectStart = 48f;
            float TopRectHeight = 32f;
            Rect TopRect = new Rect(0f, TopRectStart, inRect.width, TopRectHeight);

            Listing_Standard LS = new Listing_Standard();

            LS.Begin(TopRect);

            LS.ColumnWidth = 220f;

            GenUI.SetLabelAlign(TextAnchor.LowerLeft);

            expr_0B = LS.GetRect(30f);
            Widgets.Label(expr_0B, "All changes require a restart!".Colorize(Color.red));

            GenUI.SetLabelAlign(TextAnchor.MiddleCenter);
            Text.Font = GameFont.Medium;

            if (AniS == null)
            {
                LS.NewColumn();

                if (BioS == null && ModS == null && AniS == null)
                {
                    LS.ColumnWidth = 110f;
                }
                else
                {
                    LS.ColumnWidth = 140f;
                }

                if (Widgets.ButtonText(LS.GetRect(30f), "Animal".Colorize(TitleColor), false, true, true))
                {
                    SortingKey = (SortingKey == "Ani+" ? "Ani-" : "Ani+");
                    cComLFS = Sort.ComSort(cComLFS);
                }
            }

            if (BioS == null)
            {
                LS.NewColumn();

                if (BioS == null && ModS == null && AniS == null)
                {
                    LS.ColumnWidth = 110f;
                }
                else
                {
                    LS.ColumnWidth = 140f;
                }


                if (Widgets.ButtonText(LS.GetRect(30f), "Biome".Colorize(TitleColor), false, true, true))
                {
                    SortingKey = (SortingKey == "Bio+" ? "Bio-" : "Bio+");
                    cComLFS = Sort.ComSort(cComLFS);
                }
            }

            if (ModS == null)
            {
                LS.NewColumn();

                LS.ColumnWidth = 110f;
                if (Widgets.ButtonText(LS.GetRect(30f), "Mod".Colorize(TitleColor), false, true, true))
                {
                    SortingKey = (SortingKey == "Mod+" ? "Mod-" : "Mod+");
                    cComLFS = Sort.ComSort(cComLFS);
                }
            }

            LS.NewColumn();

            LS.ColumnWidth = 50f;

            if (!(AniS == null && BioS == null && ModS == null))
            {
                if (Widgets.ButtonText(LS.GetRect(30f), "Live".Colorize(TitleColor), false, true, true))
                {
                    SortingKey = (SortingKey == "Liv-" ? "Liv+" : "Liv-");
                    cComLFS = Sort.ComSort(cComLFS);
                }

                LS.NewColumn();

                LS.ColumnWidth = 50f;
            }
            expr_0B = LS.GetRect(30f);
            if (Widgets.ButtonText(expr_0B, "Def".Colorize(TitleColor), false, true, true))
            {
                SortingKey = (SortingKey == "Def-" ? "Def+" : "Def-");
                cComLFS = Sort.ComSort(cComLFS);
            }

            LS.NewColumn();

            float LastColHeaderMax = expr_0B.xMax;
            LS.ColumnWidth = inRect.width - LastColHeaderMax - 48;

            expr_0B = LS.GetRect(30f);

            if (Widgets.ButtonText(expr_0B, "Commonality".Colorize(TitleColor), false, true, true))
            {
                SortingKey = (SortingKey == "Com-" ? "Com+" : "Com-");
                cComLFS = Sort.ComSort(cComLFS);
            }

            LS.End();

            //_________________________________________________________________________________________________________//LS2 Filters

            Listing_Standard LS2 = new Listing_Standard();
            Rect titleRect = new Rect(0, TopRectStart + TopRectHeight + lg, 210, inRect.height - TopRectStart + TopRectHeight + lg);

            LS2.Begin(titleRect);
            LS2.ColumnWidth = 210f;

            //_________________________________________________________________________________________________________//filter by biome

            expr_0B = LS2.GetRect(30f);

            GenUI.SetLabelAlign(TextAnchor.MiddleLeft);
            Widgets.Label(expr_0B.LeftPart(0.33f), "Biome".Colorize(TitleColor));

            try
            {
                if (Widgets.ButtonText(expr_0B.RightPart(0.67f), BioSLabel))
                {
                    Find.WindowStack.Add(fm.BioMenuLabels());
                    scrollPosition = new Vector2(0f, 0f);
                }
            }
            catch { /*Log.Message("AnimalCommonalityTweaker Biome button failed. "); gbiome += 1; */}
            //_________________________________________________________________________________________________________//biome density

            try
            {
                float comVal = BioS.AnimalDensity;
                string comText = comVal.ToString("F2");

                LS2.Gap(lg);
                expr_0B = LS2.GetRect(30f);

                Rect lab = expr_0B.LeftPartPixels(140);
                Rect entry = expr_0B.RightPartPixels(70);

                Widgets.Label(lab, "Animal Density: ".Colorize(TitleColor) + BioS.AnimalDensityDef.ToString("F2"));
                Widgets.TextFieldNumeric(entry, ref comVal, ref comText);
                BioS.AnimalDensity = comVal;

                LS2.Gap(lg);
                comVal = LS2.Slider(comVal, 0f, 8f);
                BioS.AnimalDensity = comVal;
                CBioFactory.CBioL.Where(temp => temp.DefName == BioS.DefName).First().AnimalDensity = BioS.AnimalDensity;
            }
            //catch { if (gcoms == 0) { Log.Message("ACT Animal Density failed."); gcoms += 1; } }
            catch { LS2.Gap(lg + 62f); }
            //_________________________________________________________________________________________________________//filter by com zeros
            LS2.Gap(lg);

            expr_0B = LS2.GetRect(30f);
            Widgets.Label(expr_0B.LeftPart(0.33f), "Zeros:".Colorize(TitleColor));

            try
            {
                if (Widgets.ButtonText(expr_0B.RightPart(0.67f), ZerS))
                {
                    Find.WindowStack.Add(fm.ZeroMenuLabels());
                    scrollPosition = new Vector2(0f, 0f);
                }
            }
            catch { /*Log.Message("AnimalCommonalityTweaker Zero button failed. "); gzero += 1; */}

            LS2.Gap(lg);

            //_________________________________________________________________________________________________________//filter by Mod

            expr_0B = LS2.GetRect(30f);
            Widgets.Label(expr_0B.LeftPart(0.33f), "Mods:".Colorize(TitleColor));

            try
            {

                if (Widgets.ButtonText(expr_0B.RightPart(0.67f), ModSLabel))
                {
                    Find.WindowStack.Add(fm.ModMenuLabels());
                    scrollPosition = new Vector2(0f, 0f);
                }
            }
            catch {/* if (gmod == 0) { Log.Message("AnimalCommonalityTweaker Mod button failed. "); } gmod += 1; */ModS = null; }

            LS2.Gap(lg);

            //_________________________________________________________________________________________________________//filter by animal.

            expr_0B = LS2.GetRect(30f);
            Widgets.Label(expr_0B.LeftPart(0.33f), "Animal:".Colorize(TitleColor));

            try
            {
                if (AniS == null) { AniSLabel = "All"; }
                else
                {
                    AniSLabel = AniS.Label;
                    if (AniSLabel.Length > 19) { AniSLabel = AniSLabel.Substring(0, 19); }
                }

                if (Widgets.ButtonText(expr_0B.RightPart(0.67f), AniSLabel))
                {
                    Find.WindowStack.Add(fm.AniMenuBuilder(AniS));
                    scrollPosition = new Vector2(0f, 0f);
                }
            }
            catch { /*Log.Message("AnimalCommonalityTweaker Animal button failed. "); gani += 1; */}

            if (cComLFS.CountAllowNull() > 1000 && cAniL.CountAllowNull() != 0)
            {
                AniS = cAniL[0]; //  so there isn't a list of 4k items as it chugs if it does.
            }

            //_________________________________________________________________________________________________________// batch adjust

            LS2.Gap();
            LS2.GapLine();
            LS2.Gap();

            Text.Font = GameFont.Medium;
            LS2.Label("Adjust all visible:".Colorize(TitleColor));
            Text.Font = GameFont.Small;
            LS2.Gap();

            //Scale
            expr_0B = LS2.GetRect(30f);
            ScaleFloatString = ScaleFloat.ToString("N0");

            Widgets.Label(expr_0B.LeftPartPixels(50), "Scale:".Colorize(TitleColor));//"Multiply all commonality values by a %");

            Widgets.TextFieldNumeric(expr_0B.RightPartPixels(155).LeftPartPixels(30), ref ScaleFloat, ref ScaleFloatString, -100f, 100f);
            Widgets.Label(expr_0B.RightPartPixels(120).LeftPartPixels(20), "%");

            try { ScaleFloat = float.Parse(ScaleFloatString); }
            catch { ScaleFloat = 0; } // as it fails if SFS is null. This will catch text and other stuff too.

            ScaleFloat = Widgets.HorizontalSlider(expr_0B.RightPartPixels(100).BottomPartPixels(20f), ScaleFloat, -100f, 100f);

            LS2.Gap(lg);
            //shift
            expr_0B = LS2.GetRect(30f);
            ShiftFloatString = ShiftFloat.ComText();

            Widgets.Label(expr_0B.LeftPartPixels(50), "Shift:".Colorize(TitleColor));

            Widgets.TextFieldNumeric(expr_0B.RightPartPixels(155).LeftPartPixels(50), ref ShiftFloat, ref ShiftFloatString, -1f, 1f);

            ShiftFloat = Widgets.HorizontalSlider(expr_0B.RightPartPixels(100).BottomPartPixels(20f), ShiftFloat, -1f, 1f);

            //_________________________________________________________________________________________________________// Control buttons

            LS2.Gap(lg);

            expr_0B = LS2.GetRect(30f);

            if (Widgets.ButtonText(expr_0B.LeftPartPixels(102f), "Reset"))
            {
                BatchButtons.Reset();

            }

            if (Widgets.ButtonText(expr_0B.RightPartPixels(102f), "Apply"))
            {
                BatchButtons.Apply();
            }

            LS2.Gap(lg);

            expr_0B = LS2.GetRect(30f);

            if (Widgets.ButtonText(expr_0B.LeftPartPixels(102f), "Set to def"))
            {
                BatchButtons.SetToDef();
            }

            GenUI.SetLabelAlign(TextAnchor.MiddleCenter);

            if (Widgets.ButtonText(expr_0B.RightPartPixels(102f), UndoButtonText, UndoButtonDraw))
            {
                BatchButtons.UndoRedo();
            }

            GenUI.SetLabelAlign(TextAnchor.MiddleLeft);

            LS2.Gap(lg);

            expr_0B = LS2.GetRect(30f);

            if (Widgets.ButtonText(expr_0B.LeftPartPixels(102f), "Delete ALL"))
            {
                BatchButtons.DeleteAll();
            }

            if (Widgets.ButtonText(expr_0B.RightPartPixels(102f), "Clean"))
            {
                BatchButtons.CleanLists();
            }

            LS2.Gap(lg);
            LS2.CheckboxLabeled("Use ACT data".Colorize(TitleColor), ref ACTSettings.ACTActive);

            //_________________________________________________________________________________________________________//Data labels

            LS2.ColumnWidth = 210f;
            GenUI.SetLabelAlign(TextAnchor.MiddleLeft);

            LS2.Gap(12f);
            LS2.Label("Biomes".Colorize(TitleColor) + ": " + BCount.ToString() + ", " +
                "Animals".Colorize(TitleColor) + ": " + ACount.ToString());
            LS2.Label("Animals Displayed".Colorize(TitleColor) + ": " + ACountF.ToString());

            LS2.End();

            //_________________________________________________________________________________________________________//black lines in table

            Listing_Standard LS4 = new Listing_Standard();
            Listing_Standard LS3 = new Listing_Standard();

            AnimalRectHeight = Math.Max(ACountF * 30, 30);

            Rect tableViewWindow = new Rect(215, 88, 864 - 215, inRect.height - 46);
            // starts 5 to the left of tableRect
            //right is the same end point
            //height is the viewable window
            Rect tableScroll = new Rect(0, 0, 864 - 215 - 18, AnimalRectHeight);
            //18 less width than table view window
            //18 less width than table rect
            //start position is with reference to tableScrollView, oddly.
            Widgets.BeginScrollView(tableViewWindow, ref scrollPosition, tableScroll, true);

            Rect blackRect = new Rect(0, 0, 864 - 215 - 18, AnimalRectHeight);
            //position xy is relative to the widget above, so 0,0 start.

            try
            {
                LS3.Begin(blackRect);
                //just black boxes
                foreach (CCom com in cComLFS)
                {
                    expr_0B = LS3.GetRect(30f);

                    GUIElements.CounterFlipper();

                    if (counter == 1)
                    {
                        GUIElements.GUIDrawRect(expr_0B, Color.black);
                    }
                }

                LS3.End();
            }
            catch { }

            //_________________________________________________________________________________________________________//animal label in column 2.

            Rect tableRect = new Rect(20, 0, 864 - 220 - 18, AnimalRectHeight);

            LS4.Begin(tableRect);

            LS4.maxOneColumn = true;
            LS4.ColumnWidth = 140f;

            if (AniS == null) //shows only if
            {
                int labelLength = 17;
                if (BioS == null && ModS == null && AniS == null)
                {
                    LS4.ColumnWidth = 110f;
                    labelLength = 12;
                }
                try
                {
                    counter = 0;

                    foreach (CCom com in cComLFS)
                    {
                        string tempAniLabel = com.CAniLabel;

                        if (com.CAniLabel.Length > labelLength)
                        {
                            tempAniLabel = tempAniLabel.Substring(0, labelLength);
                        }

                        expr_0B = LS4.GetRect(TableRowHeight);
                        Widgets.Label(expr_0B, tempAniLabel);

                    }
                }
                catch { /*if (glabels == 0) { Log.Message("Labels failed."); glabels += 1; }*/ }

                LS4.NewColumn();
            }

            //_________________________________________________________________________________________________________//Biome labels in column 3

            if (BioS == null) //shows only if
            {
                int labelLength = 17;
                if (BioS == null && ModS == null && AniS == null)
                {
                    LS4.ColumnWidth = 110f;
                    labelLength = 12;
                }

                try
                {
                    counter = 0;

                    foreach (CCom com in cComLFS)
                    {
                        string tempBioLabel = com.CBioLabel;

                        if (com.CBioLabel.Length > labelLength)
                        {
                            tempBioLabel = tempBioLabel.Substring(0, labelLength);
                        }
                        expr_0B = LS4.GetRect(TableRowHeight);
                        Widgets.Label(expr_0B, tempBioLabel);
                    }
                }
                catch { /*if (glabels == 0) { Log.Message("Labels failed."); glabels += 1; } */}

                LS4.NewColumn();
            }

            //_________________________________________________________________________________________________________//mod label in col 4

            if (ModS == null)
            {
                LS4.ColumnWidth = 110f;
                try
                {

                    foreach (CCom com in cComLFS)
                    {
                        string labtemp = com.CAniModLabel;
                        labtemp = (labtemp.Length > 12 ? labtemp.Substring(0, 12) : labtemp);

                        expr_0B = LS4.GetRect(TableRowHeight);
                        Widgets.Label(expr_0B, labtemp);
                    }
                }
                catch { /*if (glabels == 0) { Log.Message("Labels failed."); glabels += 1; } */}

                LS4.NewColumn();
            }

            LS4.ColumnWidth = 50f;
            //_________________________________________________________________________________________________________//Live com in 5;

            if (!(AniS == null && BioS == null && ModS == null))
            {
                try
                {
                    foreach (CCom com in cComLFS)
                    {
                        expr_0B = LS4.GetRect(TableRowHeight);
                        Widgets.Label(expr_0B, com.ComLiv.ToString());
                    }
                }
                catch { /*if (gdefcoms == 0) { Log.Message("Default values failed"); gdefcoms = +1; }*/ }

                LS4.NewColumn();
                LS4.ColumnWidth = 50f;
            }

            //_________________________________________________________________________________________________________//default value in column 6;
            //Needs to be pulled with ACT not active, as it pulls the ACT data rather than the default otherwise. 

            try
            {
                foreach (CCom ani in cComLFS)
                {
                    expr_0B = LS4.GetRect(TableRowHeight);
                    Widgets.Label(expr_0B, ani.DefCom.ToString());
                }
            }
            catch { /*if (gdefcoms == 0) { Log.Message("Default values failed"); gdefcoms = +1; }*/ }

            LS4.NewColumn();
            LS4.ColumnWidth = 50f;

            //_________________________________________________________________________________________________________//com editable in 7;

            try
            {
                foreach (CCom ani in cComLFS)
                {
                    float comVal = ani.Com.ComValCon();
                    string comText = comVal.ComText();

                    expr_0B = LS4.GetRect(TableRowHeight);
                    Widgets.TextFieldNumeric(expr_0B, ref comVal, ref comText);
                    ani.Com = comVal;
                }
            }
            catch {/* if (gcoms == 0) { Log.Message("Text Entry column failed"); gcoms += 1; }*/ }

            LS4.NewColumn();

            float LastColMax = expr_0B.xMax;

            LS4.ColumnWidth = 864 - 220 - LastColMax - 70;
            //rect width - left column filters width - position of last column - 70 for luck. This includes 18 for the slider, and some for the column gap, and
            //no idea on the rest. 

            //_________________________________________________________________________________________________________//comslider in column 6;
            try
            {
                foreach (CCom ani in cComLFS)
                {
                    expr_0B = LS4.GetRect(TableRowHeight);
                    GUIElements.CounterFlipper();

                    float comValField = ani.Com.ComValCon();
                    float comVal = comValField + Mathf.Pow(2, -4);
                    float comValLog2 = Mathf.Log(comVal, 2);
                    comValLog2 = Widgets.HorizontalSlider(expr_0B, comValLog2, -4f, Mathf.Log(sliderMax, 2) + Mathf.Pow(2, -4), true);

                    comVal = Mathf.Pow(2, comValLog2);
                    comValField = comVal - Mathf.Pow(2, -4);

                    ani.Com = comValField.ComValCon();
                }
            }
            catch { /*if (gslider == 0) { Log.Message("Slider1 entry failed"); gslider += 1; } */}

            GenUI.ResetLabelAlign();
            Widgets.EndScrollView();
            LS4.End();

            if (cComL.CountAllowNull() != 0)
            {
                ACTSettings.cComL = cComL; //why you need to pass them back I don't know, but apparently you do.
                ACTSettings.cBioL = CBioFactory.CBioL;
                ACTSettings.Save = 4; // turns on saving only options. 
                base.DoSettingsWindowContents(inRect); //window end.
            }
        }

        public override string SettingsCategory()
        {
            return "Animal Commonality Tweaker";
        }
    }
}
//Datasets for the commonalities are: 
//BioS.CommonalityOfAnimal(PawnKindDef.Named(Animal.defName)
