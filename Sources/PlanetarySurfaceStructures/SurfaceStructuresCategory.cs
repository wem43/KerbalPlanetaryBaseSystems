using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class SurfaceStructuresCategory : MonoBehaviour
    {
        private string manufacturer1 = "K&K Advanced Orbit and Surface Structures";
        private string manufacturer2 = "K&K Life-Support Devision";

        private List<string> excludedParts = new List<string>() {
            "KKAOSS.Landing.Gear.g",
            "KKAOSS.gangway.2.1",
            "KKAOSS.gangway.2.2",
            "KKAOSS.gangway.2.3",
            "KKAOSS.gangway.3.1",
            "KKAOSS.gangway.3.2",
            "KKAOSS.gangway.4.1",
            "KKAOSS.gangway.4.2",
            "KKAOSS.gangway.5.1",
            "KKAOSS.gangway.6.1",
            "KKAOSS.garage.adapter.g",
            "KKAOSS.garage.cover.g",
            "KKAOSS.garage.front.g",
            "KKAOSS.garage.struct.g",
            "KKAOSS.garage.side.g"};

        //create and the icons
        private Texture2D icon_surface_structures = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_k_and_k = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_k_lifesupport = new Texture2D(32, 32, TextureFormat.ARGB32, false);
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

        /**
         * Called when the script instance is being loaded
         */
        private void Awake()
        {
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


            GameEvents.onGUIEditorToolbarReady.Add(KKAOSS_Filter);

            //load the icons
            try
            {
                //load the icons
                if (!icon_surface_structures.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/KPBSicon.png")))
                {
                    Debug.Log("[KPBS] ERROR loading KPBSicon");
                    isValid = false;
                }
                if (!icon_k_and_k.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/KPBSbase.png")))
                {
                    Debug.Log("[KPBS] ERROR loading KPBSbase");
                    isValid = false;
                }
                if (!icon_k_lifesupport.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/KPBSlifesupport.png")))
                {
                    Debug.Log("[KPBS] ERROR loading KPBSlifesupport");
                    isValid = false;
                }
                if (!icon_category_ls.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/KPBSCategoryLifeSupport.png")))
                {
                    Debug.Log("[KPBS] ERROR loading KPBSCategoryLifeSupport");
                    isValid = false;
                }
                if (!icon_filter_pods.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_pods.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_pods");
                    isValid = false;
                }
                if (!icon_filter_fuel.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_fueltank.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_fueltank");
                    isValid = false;
                }
                if (!icon_filter_electrical.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_electrical.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_electrical");
                    isValid = false;
                }
                if (!icon_filter_thermal.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_thermal.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_thermal");
                    isValid = false;
                }
                if (!icon_filter_science.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_science.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_science");
                    isValid = false;
                }
                if (!icon_filter_engine.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_engine.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_engine");
                    isValid = false;
                }
                if (!icon_filter_ground.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_ground.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_ground");
                    isValid = false;
                }
                if (!icon_filter_coupling.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_coupling.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_coupling");
                    isValid = false;
                }
                if (!icon_filter_payload.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_payload.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_payload");
                    isValid = false;
                }
                if (!icon_filter_construction.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_construction.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_construction");
                    isValid = false;
                }
                if (!icon_filter_utility.LoadImage(File.ReadAllBytes("GameData/PlanetaryBaseInc/BaseSystem/Icons/filter_utility.png")))
                {
                    Debug.Log("[KPBS] ERROR loading filter_utility");
                    isValid = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[KPBS] ERROR EXC loading Images" + e.Message);
                isValid = false;
            }
        }

        /**
         * Filter the parts by their mod
         * 
         * @param[in] part : the part to test
         * 
         * @return[bool] true part is from KKAOSS, else false
         */
        private bool filter_KKAOSS(AvailablePart part)
        {
            return part.name.StartsWith("KKAOSS") && !excludedParts.Contains(part.name);
        }

        /**
         * Filter the parts by their mod and life support
         * 
         * @param[in] part : the part to test
         * 
         * @return[bool] true part is from KKAOSS, else false
         */
        private bool filter_KKAOSS_LS(AvailablePart part)
        {
            return (part.name.StartsWith("KKAOSS.LS") || part.name.Equals("KKAOSS.Greenhouse.g")) && !excludedParts.Contains(part.name);
        }

        /**
         * Filter the parts by their mod without life support
         * 
         * @param[in] part : the part to test
         * 
         * @return[bool] true part is from KKAOSS, else false
         */
        private bool filter_KKAOSS_NO_LS(AvailablePart part)
        {
            return part.name.StartsWith("KKAOSS") && !part.name.StartsWith("KKAOSS.LS") && !part.name.Equals("KKAOSS.Greenhouse.g") && !excludedParts.Contains(part.name);
        }


        /**
         * Filter the parts by their manufacturer
         * 
         * @param[in] part : the part to test
         * @param[in] category : the category of the part
         * 
         * @return[bool] true when categories match, else false
         */
        private bool filterCategories(AvailablePart part, PartCategories category)
        {
            return (part.name.StartsWith("KKAOSS") && (partCategories[part.name] == category) && !excludedParts.Contains(part.name));
        }

        /**
         * Filter the parts by their manufacturer
         * 
         * @param[in] part : the part to test
         * @param[in] manufacturer : the name of the manufacturer
         * 
         * @return[bool] true when manufacturers match, else false
         */
        private bool filterManufacturer(AvailablePart part, String manufacturer)
        {
            return (part.manufacturer == manufacturer) && !excludedParts.Contains(part.name);
        }


        /**
         * The function to add the modules of this mod to a separate category 
         */
        private void KKAOSS_Filter()
        {
            if (!isValid)
            {
                Debug.Log("[KPBS] invalid");
                return;
            }

            //if the configuration is null
            if (KPBSConfiguration.Instance() == null)
            {
                Debug.Log("[KPBS] ERROR Configuration Instance is null");
                return;
            }

            //----------------manufacturer------------------

            if (KPBSConfiguration.Instance().ShowManufacturer)
            {
                //Debug.Log("[KPBS] manufacturer icons");

                RUI.Icons.Selectable.Icon filterIconKandK = new RUI.Icons.Selectable.Icon("KKAOSS_icon_k_and_k", icon_k_and_k, icon_k_and_k, true);
                RUI.Icons.Selectable.Icon filterIconKLife = new RUI.Icons.Selectable.Icon("KKAOSS_icon_k_lifesupport", icon_k_lifesupport, icon_k_lifesupport, true);

                if (filterIconKandK == null)
                {
                    Debug.Log("[KPBS] ERROR filterIconKandK cannot be loaded");
                    return;
                }

                if (filterIconKLife == null)
                {
                    Debug.Log("[KPBS] ERROR filterIconKLife cannot be loaded");
                    return;
                }

                //get manufacturer filter
                KSP.UI.Screens.PartCategorizer.Category manufacturerFilter = KSP.UI.Screens.PartCategorizer.Instance.filters.Find(f => f.button.categoryName == "Filter by Manufacturer");

                //add the manufacturers
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(manufacturerFilter, manufacturer1, filterIconKandK, p => filterManufacturer(p, manufacturer1));

                //when there are life support parts available, add them to the new manufacturer
                if (lifeSupportAvailable)
                {
                    KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(manufacturerFilter, manufacturer2, filterIconKLife, p => filterManufacturer(p, manufacturer2));
                }
            }
            //----------------end manufacturer--------------

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
                KSP.UI.Screens.PartCategorizer.Category surfaceStructureFilter = KSP.UI.Screens.PartCategorizer.AddCustomFilter("Planetary Surface Structures", filterIconSurfaceStructures, new Color(0.63f, 0.85f, 0.63f));

                //add subcategories to the KPSS category you just added
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Pods", ic_pods, p => filterCategories(p, PartCategories.Pods));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Fuel Tank", ic_fuels, p => filterCategories(p, PartCategories.FuelTank));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Engines", ic_engine, p => filterCategories(p, PartCategories.Propulsion));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Structural", ic_structural, p => filterCategories(p, PartCategories.Structural));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Coupling", ic_coupling, p => filterCategories(p, PartCategories.Coupling));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Payload", ic_payload, p => filterCategories(p, PartCategories.Payload));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Ground", ic_ground, p => filterCategories(p, PartCategories.Ground));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Thermal", ic_thermal, p => filterCategories(p, PartCategories.Thermal));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Electrical", ic_electrical, p => filterCategories(p, PartCategories.Electrical));
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Science", ic_science, p => filterCategories(p, PartCategories.Science));

                if (lifeSupportAvailable)
                {
                    KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Utility", ic_utility, p => (filterCategories(p, PartCategories.Utility) && !filter_KKAOSS_LS(p)));
                    KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Life Support", ic_lifeSupport, p => filter_KKAOSS_LS(p));
                }
                else
                {
                    KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Utility", ic_utility, p => (filterCategories(p, PartCategories.Utility)));
                }
            }
            //-----------------end own category-----------------

            //------------subcategory in function filter---------
            if (KPBSConfiguration.Instance().ShowSeparateFunctionCategory)
            {
                RUI.Icons.Selectable.Icon filterIconSurfaceStructures = new RUI.Icons.Selectable.Icon("KKAOSS_icon_lifeSupport", icon_surface_structures, icon_surface_structures, true);

                if (filterIconSurfaceStructures == null)
                {
                    Debug.Log("[KPBS] ERROR filterIconSurfaceStructures cannot be loaded");
                    return;
                }


                //Find the function filter
                KSP.UI.Screens.PartCategorizer.Category functionFilter = KSP.UI.Screens.PartCategorizer.Instance.filters.Find(f => f.button.categoryName == "Filter by Function");

                //Add a new subcategory to the function filter
                KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(functionFilter, "Planetary Surface Structures", filterIconSurfaceStructures, p => filter_KKAOSS_NO_LS(p));

                //Remove the parts from all other categories
                List<AvailablePart> parts = PartLoader.Instance.loadedParts.FindAll(ap => ap.name.StartsWith("KKAOSS"));
                for (int i = 0; i < parts.Count; i++)
                {
                    parts[i].category = PartCategories.none;
                }
            }
            //---------end subcategory in function filter-------

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
                    KSP.UI.Screens.PartCategorizer.Category functionFilter = KSP.UI.Screens.PartCategorizer.Instance.filters.Find(f => f.button.categoryName == "Filter by Function");
                    KSP.UI.Screens.PartCategorizer.AddCustomSubcategoryFilter(functionFilter, "Life Support", filterIconLifeSupport, p => filter_KKAOSS_LS(p));

                    //set all the categories to none to prevent this part to be added
                    //for (int i = 0; i < LS_parts.Count; i++)
                    //{
                    //    LS_parts[i].category = PartCategories.none;
                    //}

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