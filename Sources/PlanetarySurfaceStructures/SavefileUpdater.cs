using System;
using System.IO;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class SavefileUpdater : MonoBehaviour
    {
        private string saves_dir = "saves";
        private string backup_dir = "saves_BACKUP_KPBS";


        private void Awake()
        {
            /*Vector2 anchormin = new Vector2(0.5f, 0.5f);
            Vector2 anchormax = new Vector2(0.5f, 0.5f);

            Debug.Log("[KPBS] SavefileUpdater Awage");

            string msg =
                "Some changes in KPBS require an update of your saves, this will be done now.\n\n" +
                "For the case that anything goes wrong, a backup of your saves will be created under:\n" +
                "\""+ backup_dir+"\"";
            string title = "Planetary Base System Updater";

            UISkinDef skin = HighLogic.UISkin;
            DialogGUIBase[] dialogGUIBase = new DialogGUIBase[1];

            dialogGUIBase[0] = new DialogGUIButton("Ok", 
                delegate
                {
                    BackupAndConvert();
                },
                true);

            PopupDialog.SpawnPopupDialog(anchormin, anchormax,
                new MultiOptionDialog(msg, title, skin, dialogGUIBase), false, HighLogic.UISkin, true,
                string.Empty);*/
        }

        private void BackupAndConvert()
        {
            Vector2 anchormin = new Vector2(0.5f, 0.5f);
            Vector2 anchormax = new Vector2(0.5f, 0.5f);
            UISkinDef skin = HighLogic.UISkin;

            if (!BackupSaves())
            {
                

                string msg =
                    "Unable to create backup files.\n\n" +
                    "Please make a manual backup of the saves and click \"Convert\".\n" +
                    "If you want to skip for now, click \"Abort\". Be aware that your saves may be broken in this case";
                string title = "Planetary Base System Updater";

                
                DialogGUIBase[] dialogGUIBase = new DialogGUIBase[2];

                dialogGUIBase[0] = new DialogGUIButton("Convert",
                    delegate
                    {
                        ConvertSaves();
                    },
                    true);
                dialogGUIBase[1] = new DialogGUIButton("Abort", delegate {}, true);

                PopupDialog.SpawnPopupDialog(anchormin, anchormax,
                    new MultiOptionDialog(msg, title, skin, dialogGUIBase), false, HighLogic.UISkin, true,
                    string.Empty);
            }
            else
            {
                string msg = "\nUpdate complete!\n";
                string title = "Planetary Base System Updater";

                DialogGUIBase[] dialogGUIBase = new DialogGUIBase[1];
                dialogGUIBase[0] = new DialogGUIButton("Ok",delegate {},true);

                PopupDialog.SpawnPopupDialog(anchormin, anchormax,
                new MultiOptionDialog(msg, title, skin, dialogGUIBase), false, HighLogic.UISkin, true,
                string.Empty);
            }

            Debug.Log("[KPBS] save file update complete!");
        }

        //backup the save files directory
        private bool BackupSaves()
        {
            //recursively copy the directories
            return CopyDirectroy(saves_dir, backup_dir);
        }

        private bool CopyDirectroy(string sourceDirName, string destDirName)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                if (!dir.Exists)
                {
                    Debug.Log("[KPBS] ERR Directory not available: " + sourceDirName);
                    return false;
                }

                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    //delete the file when it already exists
                    if (File.Exists(temppath))
                    {
                        FileInfo fi = new FileInfo(temppath);
                        fi.Delete();
                    }
                        
                    file.CopyTo(temppath, false);
                }

                // If copying subdirectories, copy them and their contents to new location.
                bool success = true;
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    success &= CopyDirectroy(subdir.FullName, temppath);
                }

                return success;
            }
            catch (Exception e)
            {
                Debug.Log("[KPBS] ERR Save backup failed: " + e.Message);
                return false;
            }
        }


        private void ConvertSaves()
        {

        }
    }
}