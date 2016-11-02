using System;
using System.IO;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class SavefileBackup : MonoBehaviour
    {
        private string saves_dir = "saves";
        private string gameData_dir = "GameData/PlanetaryBaseInc";
        private string backup_flag = "KPBS.Backup";
        private string backup_dir = "saves.kpbs_backup";


        private void Awake()
        {
            string backupFlagDir = Path.Combine(gameData_dir, backup_flag);
            if (!File.Exists(backupFlagDir) && !Directory.Exists(backup_dir))
            {
                Vector2 anchormin = new Vector2(0.5f, 2f);
                Vector2 anchormax = new Vector2(0.5f, 2f);

                Debug.Log("[KPBS] SavefileBackup Awake");

                string msg =
                    "Kerbal Planetary Base Systems needs to upgrade save files when KPBS before version 1.3.2. was used.\n\n" +
                    "Do you want to make a backup of the save files before you continue?";
                string title = "Planetary Base System Updater";

                UISkinDef skin = HighLogic.UISkin;
                DialogGUIBase[] dialogGUIBase = new DialogGUIBase[2];

                dialogGUIBase[0] = new DialogGUIButton("Yes",
                    delegate
                    {
                        TryBackup();
                    },
                    true);
                dialogGUIBase[1] = new DialogGUIButton("No", 
                    delegate {
                        CreateBackupFlag();
                    }, 
                    true);

                PopupDialog pp = PopupDialog.SpawnPopupDialog(anchormin, anchormax,
                    new MultiOptionDialog(msg, title, skin, dialogGUIBase), true, HighLogic.UISkin, true,
                    string.Empty);
            }
        }

        private void TryBackup()
        {
            Vector2 anchormin = new Vector2(0.5f, 2f);
            Vector2 anchormax = new Vector2(0.5f, 2f);
            UISkinDef skin = HighLogic.UISkin;

            string msg;
            string title;

            if (!BackupSaves())
            {
                msg ="\nCannot create backuo\n\n" +
                    "Please consider making a manual backup of the saves";
                title = "Planetary Base System Updater";
            }
            else
            {
                msg = "\nBackup successfully created at: \"Kerbal Space Program/" + backup_dir + "\"!\n";
                title = "Planetary Base System Updater";
            }

            DialogGUIBase[] dialogGUIBase = new DialogGUIBase[1];
            dialogGUIBase[0] = new DialogGUIButton("Ok", delegate { }, true);

            PopupDialog.SpawnPopupDialog(anchormin, anchormax,
                new MultiOptionDialog(msg, title, skin, dialogGUIBase), false, HighLogic.UISkin, true,
                string.Empty);
        }

        //backup the save files directory
        private bool BackupSaves()
        {
            //recursively copy the directories
            return CopyDirectroy(saves_dir, backup_dir);
        }

        //create the backup flag
        private void CreateBackupFlag()
        {
            try
            {
                string flagPath = Path.Combine(gameData_dir, backup_flag);
                if (!File.Exists(flagPath))
                {
                    File.WriteAllText(flagPath, "1.3.3");
                }
            }
            catch (Exception e)
            {
                Debug.Log("[KPBS] ERR Cannot write backup flag: " + e.Message);
            }
        }

        private bool CopyDirectroy(string sourceDirName, string destDirName)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                CreateBackupFlag();

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
    }
}