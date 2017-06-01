using System;
using System.Reflection;
using KSP.Localization;

namespace PlanetarySurfaceStructures
{
    class KPBSSettings : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomParameterUI("#LOC_KPBS.filtersettings.desc", autoPersistance = true, toolTip = "#LOC_KPBS.filtersettings.tooltip")]
        public bool functionFilter = false;


        //get the title
        public override string Title {
            get {
                return Localizer.GetStringByTag("#LOC_KPBS.filtersettings.name");//"Filter Settings";
            }
        }

        //get of the settings have presets
        public override bool HasPresets {
            get {
                return false;
            }
        }

        //get the game mode
        public override GameParameters.GameMode GameMode {
            get {
                return GameParameters.GameMode.ANY;
            }
        }

        //get the section
        public override string Section {
            get {
                return "Planetary Base System";
            }
            
        }
        
        //get the section displayed
        public override string DisplaySection
        {
            get
            {
                return Localizer.GetStringByTag("#LOC_KPBS.filtersettings.section");
            }
        }


        //get the section order
        public override int SectionOrder {
            get {
                return 1;
            }
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true; 
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return !HighLogic.LoadedSceneIsEditor;
        }
    }
}
