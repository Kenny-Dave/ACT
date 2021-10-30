using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AnimalCommonalityTweaker
{
    public class BatchButtons
    {
        static List<CCom> cComLFSB;
        //list of CComs for restoring on the undo button. 

        public static void SelectedToBackup()
        {
            ACTMod.BioSLast = ACTMod.BioS;
            ACTMod.ZerSLast = ACTMod.ZerS;
            ACTMod.ModSLast = ACTMod.ModS;
            ACTMod.AniSLast = ACTMod.AniS;

            //as a straight = is by ref.
            if (ACTMod.cComLFS.CountAllowNull() != 0)
            {
                try { cComLFSB.Clear(); } catch { cComLFSB = new List<CCom>(); }

                foreach (CCom com in ACTMod.cComLFS)
                {
                    cComLFSB.Add(com);
                }
            }
        }

        public static void BackupToSelected()
        {
            ACTMod.BioS = ACTMod.BioSLast;
            ACTMod.ZerS = ACTMod.ZerSLast;
            ACTMod.ModS = ACTMod.ModSLast;
            ACTMod.AniS = ACTMod.AniSLast;
            //cComLFS recalced on all of these so don't need to set LFS to LFSB.
        }

        public static void UndoEnable()
        {
            ACTMod.UndoExists = true;
            ACTMod.RedoExists = false;
            ACTMod.UndoButtonText = "Undo last";
            ACTMod.UndoButtonDraw = true;
        }
        public static void RedoEnable()
        {
            ACTMod.UndoExists = false;
            ACTMod.RedoExists = true;
            ACTMod.UndoButtonText = "Redo last";
            ACTMod.UndoButtonDraw = true;
        }
        public static void Reset()
        {
            ACTMod.ScaleFloat = 0f;
            ACTMod.ShiftFloat = 0f;
        }

        public static void Apply()
        {
            if (ACTMod.cComLFS.CountAllowNull() != 0)
            {
                SelectedToBackup();

                foreach (CCom com in cComLFSB)
                {
                    com.ComBacker = com.Com; //for the undo
                    com.Com = Math.Max((((ACTMod.ScaleFloat / 100) + 1) * com.Com) + ACTMod.ShiftFloat, 0);
                }

                ACTMod.ScaleFloatLast = ACTMod.ScaleFloat;
                ACTMod.ShiftFloatLast = ACTMod.ShiftFloat;

                UndoEnable();

                ACTMod.ScaleFloat = 0f;
                ACTMod.ShiftFloat = 0f;

                Sort.MaxComDispFull(ACTMod.cComLFS);
            }
        }

        public static void SetToDef()
        {
            if (ACTMod.cComLFS.CountAllowNull() != 0)
            {
                SelectedToBackup();

                foreach (CCom com in cComLFSB)
                {
                    com.ComBacker = com.Com; //for the undo
                    com.Com = com.DefCom;
                }
                UndoEnable();

                Sort.MaxComDispFull(ACTMod.cComLFS);
            }
        }

        public static void UndoRedo()
        {
            if (ACTMod.cComL.CountAllowNull() != 0)
            {
                if (ACTMod.UndoExists == true)
                {
                    BackupToSelected();

                    foreach (CCom com in cComLFSB)
                    {
                        float temp = com.Com;
                        com.Com = com.ComBacker;
                        com.ComBacker = temp;
                    }

                    RedoEnable();

                    if (cComLFSB.CountAllowNull() != 0)
                    {
                        ACTMod.cComLFS.Clear();

                        foreach (CCom com in cComLFSB)
                        {
                            ACTMod.cComLFS.Add(com);
                        }
                    }
                }
                else if (ACTMod.RedoExists == true)
                {
                    BackupToSelected();

                    foreach (CCom com in cComLFSB)
                    {
                        float temp = com.ComBacker;
                        com.ComBacker = com.Com;
                        com.Com = temp;
                    }

                    UndoEnable();

                    if (cComLFSB.CountAllowNull() != 0)
                    {
                        ACTMod.cComLFS.Clear();

                        foreach (CCom com in cComLFSB)
                        {
                            ACTMod.cComLFS.Add(com);
                        }
                    }
                }
                Sort.MaxComDispFull(ACTMod.cComLFS);
            }
        }

        public static void DeleteAll()
        {
            try { ACTSettings.cAniL.Clear(); } catch { }
            try { ACTSettings.cBioL.Clear(); } catch { }
            try { ACTSettings.cComL.Clear(); } catch { }
            try { ACTSettings.cModL.Clear(); } catch { }

            try { CAniFactory.CAniL.Clear(); } catch { }
            try { CBioFactory.CBioL.Clear(); } catch { }
            try { CComFactory.CComL.Clear(); } catch { }
            try { CModFactory.CModL.Clear(); } catch { }

            try { NDC.cAniL.Clear(); } catch { }
            try { NDC.cBioL.Clear(); } catch { }
            try { NDC.cComL.Clear(); } catch { }

            try { ACTMod.cAniL.Clear(); } catch { }
            try { ACTMod.cBioL.Clear(); } catch { }
            try { ACTMod.cComL.Clear(); } catch { }

            try { ACTMod.cComLFS.Clear(); } catch { }
        }

        public static void CleanLists()
        {
            //this removes any items from the ACT lists that isn't in the live data
            //which will happen when mods containing animals or biomes are removed. 

            if (ACTMod.cComL.CountAllowNull() != 0)
            {
                int comRemoved = 0;
                int bioRemoved = 0;
                int aniRemoved = 0;
                int modRemoved = 0;


                List<int> comKillList = new List<int>();
                List<int> bioKillList = new List<int>();
                List<int> aniKillList = new List<int>();
                List<int> modKillList = new List<int>();

                //________________________________________________________________________________________//Com
                for (int i = 0; i < ACTMod.cComL.Count(); i++)
                {
                    if (ComFactory.ComL.Where(temp => temp.CBio == ACTMod.cComL[i].CBio && temp.CAni == ACTMod.cComL[i].CAni).Count() == 0)
                    {
                        comRemoved += 1;
                        comKillList.Add(i);
                    }
                }

                for (int j = comKillList.Count() - 1; j > -1; j--)
                {
                    int i = comKillList[j];
                    ACTMod.cComL.Remove(ACTMod.cComL[i]);
                }

                //________________________________________________________________________________________//Bio

                for (int i = 0; i < ACTMod.cBioL.Count(); i++)
                {
                    if (NDC.bioL.Where(temp => temp.defName == ACTMod.cBioL[i].DefName).Count() == 0)
                    {
                        bioRemoved += 1;
                        bioKillList.Add(i);
                    }
                }
                for (int j = bioKillList.Count() - 1; j > -1; j--)
                {
                    int i = bioKillList[j];
                    ACTMod.cBioL.Remove(ACTMod.cBioL[i]);
                }

                //________________________________________________________________________________________//Ani

                for (int i = 0; i < ACTMod.cAniL.Count(); i++)
                {
                    if (NDC.aniL.Where(temp => temp.defName == ACTMod.cAniL[i].DefName).Count() == 0)
                    {
                        aniRemoved += 1;
                        aniKillList.Add(i);
                    }
                }

                for (int j = aniKillList.Count() - 1; j > -1; j--)
                {
                    int i = aniKillList[j];
                    ACTMod.cAniL.Remove(ACTMod.cAniL[i]);
                }
                //________________________________________________________________________________________//Mod list

                int MCount = CModFactory.CModL.CountAllowNull();

                if (NDC.aniL.CountAllowNull() != 0)
                {
                    List<ThingDef> aniL = NDC.aniL;

                    CModFactory.CModL.Clear();
                    CModFactory cModFac = new CModFactory();

                    foreach (ThingDef Animal in aniL) //animals into c
                    {
                        if (CModFactory.CModL == null
                            || (CModFactory.CModL.Where(temp => temp.PackageId == Animal.modContentPack.PackageId).Count() == 0) &&
                            Animal.modContentPack.PackageId != "torann.arimworldofmagic" &&
                            Animal.modContentPack.PackageId != "scurvyez.fireflies")
                        {
                            cModFac.NewCMod(Animal.modContentPack.PackageId, Animal.modContentPack.Name);
                        }
                    }
                }
                else { Log.Message("ACT error loading animal list from game data."); }

                modRemoved = MCount - CModFactory.CModL.CountAllowNull();

                //________________________________________________________________________________________//Message

                if (bioRemoved + aniRemoved + modRemoved != 0)
                {
                    Log.Message("ACT items removed: Biome records: " + bioRemoved.ToString() +
                        ", Animal records: " + aniRemoved.ToString() +
                        ", Mods: " + modRemoved.ToString());
                }

                ACTSettings.cAniL = ACTMod.cAniL;
                ACTSettings.cBioL = ACTMod.cBioL;
                ACTSettings.cComL = ACTMod.cComL;
                ACTSettings.cModL = CModFactory.CModL;

                CAniFactory.CAniL = ACTMod.cAniL;
                CBioFactory.CBioL = ACTMod.cBioL;
                CComFactory.CComL = ACTMod.cComL;
            }
        }
    }
}
