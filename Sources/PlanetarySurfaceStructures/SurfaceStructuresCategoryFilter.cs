using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace PlanetarySurfaceStructures
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class SurfaceStructuresCategoryFilter : MonoBehaviour
    {
        //create and the icons
        private Texture2D icon_surface_structures = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_category_ls = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        private Texture2D icon_filter_pods = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_engine = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_fuel = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_payload = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_construction = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_coupling = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_electrical = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_ground = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_utility = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_science = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_filter_thermal = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        internal string iconName = "PlanetaryBaseSystemsEditor";
        internal bool filter = true;

        //set to false when an icon could not be loaded
        private bool isValid = true;

        //a dictionary storing all the categories of the parts
        private Dictionary<string, PartCategories> partCategories;

        //stores wheter life support is available
        private bool lifeSupportAvailable = false;

        //stores wheter the Community Category Kit is availables
        private bool CCKavailable = false;

        //The name of the category for the KPBS filter
        private string functionFilterName = Localizer.GetStringByTag("#LOC_KPBS.categoryfilter.function.name");//"Planetary Surface Structures";

        //The name of the function filter
        private string filterName = "Filter by function";

        //The instance of this filter
        public static SurfaceStructuresCategoryFilter Instance;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            Instance = this;
            DontDestroyOnLoad(this);

            //search for Community Category Kit
            int numAssemblies = AssemblyLoader.loadedAssemblies.Count;
            for (int i = 0; i < numAssemblies; i++)
            {
                if (AssemblyLoader.loadedAssemblies[i].name.Equals("CCK"))
                {
                    CCKavailable = true;
                    break;
                }
            }

            //save all the categories from the parts of this mod
            List<AvailablePart> all_parts = PartLoader.Instance.loadedParts.FindAll(ap => ap.name.StartsWith("KKAOSS"));
            partCategories = new Dictionary<string, PartCategories>();
            int num = all_parts.Count;
            for (int i = 0; i < num; i++)
            {
                partCategories.Add(all_parts[i].name, all_parts[i].category);
            }

            //save whether life support parts are available
            lifeSupportAvailable = (all_parts.FindAll(ap => ap.name.StartsWith("KKAOSS.LS")).Count > 0);

            //load the icons
            try
            {
                string errors = string.Empty;

                //load the icons
                if (!icon_surface_structures.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/KPBSicon.png")))
                {
                    errors += "KPBSicon.png ";
                    isValid = false;
                }
                if (!icon_category_ls.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/KPBSCategoryLifeSupport.png")))
                {
                    errors += "KPBSCategoryLifeSupport.png ";
                    isValid = false;
                }
                if (!icon_filter_pods.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_pods.png")))
                {
                    errors += "ilter_pods.png ";
                    isValid = false;
                }
                if (!icon_filter_fuel.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_fueltank.png")))
                {
                    errors += "filter_fueltank.png ";
                    isValid = false;
                }
                if (!icon_filter_electrical.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_electrical.png")))
                {
                    errors += "filter_electrical.png ";
                    isValid = false;
                }
                if (!icon_filter_thermal.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_thermal.png")))
                {
                    errors += "filter_thermal.png ";
                    isValid = false;
                }
                if (!icon_filter_science.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_science.png")))
                {
                    errors += "filter_science.png ";
                    isValid = false;
                }
                if (!icon_filter_engine.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_engine.png")))
                {
                    errors += "filter_engine.png ";
                    isValid = false;
                }
                if (!icon_filter_ground.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_ground.png")))
                {
                    errors += "filter_ground.png ";
                    isValid = false;
                }
                if (!icon_filter_coupling.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_coupling.png")))
                {
                    errors += "filter_coupling.png ";
                    isValid = false;
                }
                if (!icon_filter_payload.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_payload.png")))
                {
                    errors += "filter_payload.png ";
                    isValid = false;
                }
                if (!icon_filter_construction.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_construction.png")))
                {
                    errors += "filter_construction.png ";
                    isValid = false;
                }
                if (!icon_filter_utility.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_utility.png")))
                {
                    errors += "filter_utility.png ";
                    isValid = false;
                }

                if (!isValid)
                {
                    Debug.LogError("[KPBS] ERROR loading: " + errors);
                }
                
            }
            catch (Exception e)
            {
                Debug.LogError("[KPBS] ERROR EXC loading Images" + e.Message);
                isValid = false;
            }

            GameEvents.onGUIEditorToolbarReady.Add(KPBSMainFilter);
            GameEvents.OnGameSettingsApplied.Add(updateFilterSettings);
        }

        /// <summary>
        /// Removes all listeners from the GameEvents when Class is destroyed
        /// </summary>
        protected void OnDestroy()
        {
            GameEvents.onGUIEditorToolbarReady.Remove(KPBSMainFilter);
            GameEvents.OnGameSettingsApplied.Remove(updateFilterSettings);
        }

        /// <summary>
        /// Update the settings from the filters
        /// </summary>
        public void updateFilterSettings()
        {
            Debug.Log("[KPBS] updateFilterSettings");
            GameEvents.onGUIEditorToolbarReady.Remove(KPBSFunctionFilter);

            if (HighLogic.CurrentGame != null)
            {
                RemoveFunctionFilter();
                AddPartCategories();

                if (HighLogic.CurrentGame.Parameters.CustomParams<KPBSSettings>().functionFilter)
                {
                    RemovePartCategories();
                    GameEvents.onGUIEditorToolbarReady.Add(KPBSFunctionFilter);
                }
            }
        }

        /// <summary>
        /// Remove the fuction filte category
        /// </summary>
        private void RemoveFunctionFilter()
        {
            if (PartCategorizer.Instance != null)
            {
                PartCategorizer.Category Filter = PartCategorizer.Instance.filters.Find(f => f.button.categoryName == filterName);
                if (Filter != null)
                {
                    PartCategorizer.Category subFilter = Filter.subcategories.Find(f => f.button.categoryName == functionFilterName);
                    if (subFilter != null)
                    {
                        subFilter.DeleteSubcategory();
                    }
                }
            }
        }

        /// <summary>
        /// Remove the fuction filte category
        /// </summary>
        private void AddFunctionFilter()
        {
            if (PartCategorizer.Instance != null)
            {
                PartCategorizer.Category Filter = PartCategorizer.Instance.filters.Find(f => f.button.categoryName == filterName);
                if (Filter != null)
                {
                    PartCategorizer.Category subFilter = Filter.subcategories.Find(f => f.button.categoryName == functionFilterName);
                    if (subFilter != null)
                    {
                        subFilter.DeleteSubcategory();
                    }
                }
            }
        }

        /// <summary>
        /// Add the stored categories to all the parts of KPBS
        /// </summary>
        private void AddPartCategories()
        {
            if (partCategories != null)
            {
                List<AvailablePart> parts = PartLoader.Instance.loadedParts.FindAll(ap => ap.name.StartsWith("KKAOSS"));
                for (int i = 0; i < parts.Count; i++)
                {
                    if (partCategories.ContainsKey(parts[i].name)) { 
                        parts[i].category = partCategories[parts[i].name];
                    }
                }
            }
        }

        /// <summary>
        /// Remove the categories from all parts of KPBS
        /// </summary>
        private void RemovePartCategories()
        {
            List<AvailablePart> parts = PartLoader.Instance.loadedParts.FindAll(ap => ap.name.StartsWith("KKAOSS"));
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].category = PartCategories.none;
            }
        }

        /// <summary>
        /// ilter the parts by their mod
        /// </summary>
        /// <param name="part">the part to test</param>
        /// <returns>true part is from KPBS, else false</returns>
        private bool filter_KKAOSS(AvailablePart part)
        {
            return part.name.StartsWith("KKAOSS");
        }

        /// <summary>
        /// Filter the parts by their mod and life support
        /// </summary>
        /// <param name="part">the part to test</param>
        /// <returns>true part is for live support from KPBS, else false</returns>
        private bool filter_KKAOSS_LS(AvailablePart part)
        {
            return (part.name.StartsWith("KKAOSS.LS") || part.name.Equals("KKAOSS.Greenhouse.g"));
        }

        /// <summary>
        /// Filter the parts by their mod without life support
        /// </summary>
        /// <param name="part">the part to test</param>
        /// <returns>true part is from KKAOSS but not for life support, else false</returns>
        private bool filter_KKAOSS_NO_LS(AvailablePart part)
        {
            return part.name.StartsWith("KKAOSS") && !part.name.StartsWith("KKAOSS.LS") && !part.name.Equals("KKAOSS.Greenhouse.g");
        }

        /// <summary>
        /// Filter the parts by their categorys
        /// </summary>
        /// <param name="part">the part to test</param>
        /// <param name="category">the category to check for</param>
        /// <returns>true when categories match, else false</returns>
        private bool filterCategories(AvailablePart part, PartCategories category)
        {
            return (part.name.StartsWith("KKAOSS") && (partCategories[part.name] == category));
        }

        /// <summary>
        /// Add the function filter to the filter
        /// </summary>
        private void KPBSFunctionFilter()
        {
            if (!isValid)
            {
                Debug.LogError("[KPBS] invalid");
                return;
            }

            RUI.Icons.Selectable.Icon filterIconSurfaceStructures = new RUI.Icons.Selectable.Icon("KKAOSS_icon_lifeSupport", icon_surface_structures, icon_surface_structures, true);

            if (filterIconSurfaceStructures == null)
            {
                Debug.LogError("[KPBS] ERROR filterIconSurfaceStructures cannot be loaded");
                return;
            }

            //Find the function filter
            PartCategorizer.Category functionFilter = PartCategorizer.Instance.filters.Find(f => f.button.categoryName == filterName);

            //Add a new subcategory to the function filter
            PartCategorizer.AddCustomSubcategoryFilter(functionFilter, functionFilterName, functionFilterName, filterIconSurfaceStructures, p => filter_KKAOSS(p));
        }

        /// <summary>
        /// The function to add the modules of this mod to a separate category.
        /// </summary>
        private void KPBSMainFilter()
        {
            if (!isValid)
            {
                Debug.LogError("[KPBS] invalid");
                return;
            }

            //if the configuration is null
            if (KPBSConfiguration.Instance() == null)
            {
                Debug.LogError("[KPBS] ERROR Configuration Instance is null");
                return;
            }

            //-----------------own category-----------------
            if (KPBSConfiguration.Instance().ShowModFilter)
            {
                //create the icon for the filter
                RUI.Icons.Selectable.Icon filterIconSurfaceStructures = new RUI.Icons.Selectable.Icon("KKAOSS_icon_KPSS", icon_surface_structures, icon_surface_structures, true);

                //icons for KPSS's own category
                RUI.Icons.Selectable.Icon ic_pods = new RUI.Icons.Selectable.Icon("KKAOSS_filter_pods", icon_filter_pods, icon_filter_pods, true);
                RUI.Icons.Selectable.Icon ic_fuels = new RUI.Icons.Selectable.Icon("KKAOSS_filter_fuel", icon_filter_fuel, icon_filter_fuel, true);
                RUI.Icons.Selectable.Icon ic_engine = new RUI.Icons.Selectable.Icon("KKAOSS_filter_fuel", icon_filter_engine, icon_filter_engine, true);
                RUI.Icons.Selectable.Icon ic_structural = new RUI.Icons.Selectable.Icon("KKAOSS_filter_fuel", icon_filter_construction, icon_filter_construction, true);
                RUI.Icons.Selectable.Icon ic_payload = new RUI.Icons.Selectable.Icon("KKAOSS_filter_fuel", icon_filter_payload, icon_filter_payload, true);
                RUI.Icons.Selectable.Icon ic_utility = new RUI.Icons.Selectable.Icon("KKAOSS_filter_fuel", icon_filter_utility, icon_filter_utility, true);
                RUI.Icons.Selectable.Icon ic_coupling = new RUI.Icons.Selectable.Icon("KKAOSS_filter_coupling", icon_filter_coupling, icon_filter_coupling, true);
                RUI.Icons.Selectable.Icon ic_ground = new RUI.Icons.Selectable.Icon("KKAOSS_filter_ground", icon_filter_ground, icon_filter_ground, true);
                RUI.Icons.Selectable.Icon ic_thermal = new RUI.Icons.Selectable.Icon("KKAOSS_filter_thermal", icon_filter_thermal, icon_filter_thermal, true);
                RUI.Icons.Selectable.Icon ic_electrical = new RUI.Icons.Selectable.Icon("KKAOSS_filter_electrical", icon_filter_electrical, icon_filter_electrical, true);
                RUI.Icons.Selectable.Icon ic_science = new RUI.Icons.Selectable.Icon("KKAOSS_filter_fuel", icon_filter_science, icon_filter_science, true);
                RUI.Icons.Selectable.Icon ic_lifeSupport = new RUI.Icons.Selectable.Icon("KKAOSS_icon_KPSS", icon_category_ls, icon_category_ls, true);

                //add KPBS to the categories
                PartCategorizer.Category surfaceStructureFilter = KSP.UI.Screens.PartCategorizer.AddCustomFilter("Planetary Surface Structures", functionFilterName,  filterIconSurfaceStructures, new Color(0.63f, 0.85f, 0.63f));

                //add subcategories to the KPSS category you just added
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Pods", Localizer.GetStringByTag("#autoLOC_453549"), ic_pods, p => filterCategories(p, PartCategories.Pods));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Fuel Tank", Localizer.GetStringByTag("#autoLOC_453552"), ic_fuels, p => filterCategories(p, PartCategories.FuelTank));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Engines", Localizer.GetStringByTag("#autoLOC_453555"), ic_engine, p => filterCategories(p, PartCategories.Propulsion));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Structural", Localizer.GetStringByTag("#autoLOC_453561"), ic_structural, p => filterCategories(p, PartCategories.Structural));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Coupling", Localizer.GetStringByTag("#autoLOC_453564"), ic_coupling, p => filterCategories(p, PartCategories.Coupling));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Payload", Localizer.GetStringByTag("#autoLOC_453567"), ic_payload, p => filterCategories(p, PartCategories.Payload));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Ground", Localizer.GetStringByTag("#autoLOC_453573"), ic_ground, p => filterCategories(p, PartCategories.Ground));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Thermal", Localizer.GetStringByTag("#autoLOC_453576"), ic_thermal, p => filterCategories(p, PartCategories.Thermal));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Electrical", Localizer.GetStringByTag("#autoLOC_453579"), ic_electrical, p => filterCategories(p, PartCategories.Electrical));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Science", Localizer.GetStringByTag("#autoLOC_453585"), ic_science, p => filterCategories(p, PartCategories.Science));

                if (lifeSupportAvailable)
                {
                    PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Utility", Localizer.GetStringByTag("#autoLOC_453588"), ic_utility, p => (filterCategories(p, PartCategories.Utility) && !filter_KKAOSS_LS(p)));
                    PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Life Support", Localizer.GetStringByTag("#LOC_KPBS.categoryfilter.category.lifesupport"), ic_lifeSupport, p => filter_KKAOSS_LS(p));
                }
                else
                {
                    PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Utility", Localizer.GetStringByTag("#autoLOC_453588"), ic_utility, p => (filterCategories(p, PartCategories.Utility)));
                }
            }
            //-----------------end own category-----------------

            //------------subcategory for life support---------

            //when the community category kit is not available, add own category for life support
            if (!CCKavailable)
            {
                //only continue when there are parts for life support
                if (lifeSupportAvailable)
                {
                    RUI.Icons.Selectable.Icon filterIconLifeSupport = new RUI.Icons.Selectable.Icon("KKAOSS_icon_KPSS", icon_category_ls, icon_category_ls, true);

                    if (filterIconLifeSupport == null)
                    {
                        Debug.Log("[KPBS] ERROR filterIconLifeSupport cannot be loaded");
                        return;
                    }

                    //Find the function filter
                    PartCategorizer.Category functionFilter = PartCategorizer.Instance.filters.Find(f => f.button.categoryName == filterName);
                    PartCategorizer.AddCustomSubcategoryFilter(functionFilter, "Life Support", Localizer.GetStringByTag("#LOC_KPBS.categoryfilter.category.lifesupport"), filterIconLifeSupport, p => filter_KKAOSS_LS(p));

                    //add the greenhouse the the LS mods when other ls mods were found
                    List<AvailablePart> greenhouses = PartLoader.Instance.loadedParts.FindAll(ap => ap.name.Equals("KKAOSS.Greenhouse.g"));
                    for (int i = 0; i < greenhouses.Count; i++)
                    {
                        greenhouses[i].category = PartCategories.none;
                    }
                }
            }
            else if (lifeSupportAvailable)
            {
                List<AvailablePart> greenhouses = PartLoader.Instance.loadedParts.FindAll(ap => ap.name.Equals("KKAOSS.Greenhouse.g"));
                for (int i = 0; i < greenhouses.Count; i++)
                {
                    greenhouses[i].category = PartCategories.none;
                }
            }
            //---------end subcategory for life support-------
        }
    }
}