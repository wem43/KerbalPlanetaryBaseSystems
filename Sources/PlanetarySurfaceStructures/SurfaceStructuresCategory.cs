using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class SurfaceStructuresCategory : MonoBehaviour
    {
        private string manufacturer1 = "K&K Advanced Orbit and Surface Structures";
        private string manufacturer3 = "K&K Life-Support Devision";

        //create and the icons
        private Texture2D icon_surface_structures = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_k_and_k = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_k_lifesupport = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        private Texture2D icon_category_ls = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        //set to false when an icon could not be loaded
        private bool isValid = true;

        /**
         * Called when the script instance is being loaded
         */
        private void Awake()
        {
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
                if (!icon_k_lifesupport.LoadImage(File.ReadAllBytes("Gamedata/PlanetaryBaseInc/BaseSystem/Icons/KPBSlifesupport.png")))
                {
                    Debug.Log("[KPBS] ERROR loading KPBSlifesupport");
                    isValid = false;
                }
                if (!icon_category_ls.LoadImage(File.ReadAllBytes("Gamedata/PlanetaryBaseInc/BaseSystem/Icons/KPBSCategoryLifeSupport.png")))
                {
                    Debug.Log("[KPBS] ERROR loading KPBSCategoryLifeSupport");
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
            if (part != null)
            {
                return part.name.StartsWith("KKAOSS");

            }
            else
            {

                return false;
            }
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
            return part.name.StartsWith("KKAOSS.LS") || part.name.Equals("KKAOSS.Greenhouse.g");
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
            return part.name.StartsWith("KKAOSS") && !part.name.StartsWith("KKAOSS.LS") && !part.name.Equals("KKAOSS.Greenhouse.g");
        }


        /**
         * Filter the parts by their manufacturer
         * 
         * @param[in] part : the part to test
         * @param[in] category : the category of the part
         * 
         * @return[bool] true when categories match, else false
         */
        private bool filterCategories(AvailablePart part,PartCategories category) 
        {
            return (part.name.StartsWith("KKAOSS") && (part.category == category));
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
            return (part.manufacturer == manufacturer);
        }

        /**
         * Filter the parts by ther profiles 
         * 
         * @param[in] part : the part to test
         * 
         * @return[bool] true part has planetary profile, else false
         */
        private bool filterProfiles(AvailablePart part)
        {
            //return false when no category given
            if (part.bulkheadProfiles == null)
            {
                return false;
            }
            
            return part.bulkheadProfiles.Contains("PlanetaryBase");
        }


        /**
         * The function to add the modules of this mod to a separate category 
         */
        private void KKAOSS_Filter()
        {
            if (!isValid)
            {
                return;
            }

            //if the configuration is null
            if (KPBSConfiguration.Instance() == null) {
                Debug.Log("[KPBS] ERROR Configuration Instance is null");
                return;
            }
            //when the textures could not be loaded
            List<AvailablePart> LS_parts = PartLoader.Instance.parts.FindAll(ap => ap.name.StartsWith("KKAOSS.LS"));

            //----------------manufacturer------------------

            if (KPBSConfiguration.Instance().ShowManufacturer)
            {
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
                PartCategorizer.Category manufacturerFilter = PartCategorizer.Instance.filters.Find(f => f.button.categoryName == "Filter by Manufacturer");

                //add the manufacturers
                PartCategorizer.AddCustomSubcategoryFilter(manufacturerFilter, manufacturer1, filterIconKandK, p => filterManufacturer(p, manufacturer1));
               
                //when there are life support parts available, add them to the new manufacturer
                List<AvailablePart> parts = PartLoader.Instance.parts.FindAll(ap => (ap.category == PartCategories.Utility && ap.name.StartsWith("KKAOSS.LS")));
                if (parts.Count > 0)
                {
                    PartCategorizer.AddCustomSubcategoryFilter(manufacturerFilter, manufacturer3, filterIconKLife, p => filterManufacturer(p, manufacturer3));
                }
            }
            //----------------end manufacturer--------------

            //-----------------own category-----------------

            if ((KPBSConfiguration.Instance().ShowModFilter) && (!KPBSConfiguration.Instance().ShowSeparateFunctionCategory))
            {
                //create the icon for the filter
                RUI.Icons.Selectable.Icon filterIconSurfaceStructures = new RUI.Icons.Selectable.Icon("KKAOSS_icon_KPSS", icon_surface_structures, icon_surface_structures, true);

                //icons for KPSS's own category
                RUI.Icons.Selectable.Icon ic_pods = PartCategorizer.Instance.iconLoader.GetIcon("RDicon_commandmodules");
                RUI.Icons.Selectable.Icon ic_fuels = PartCategorizer.Instance.iconLoader.GetIcon("RDicon_fuelSystems-advanced");
                RUI.Icons.Selectable.Icon ic_engine = PartCategorizer.Instance.iconLoader.GetIcon("R&D_node_icon_generalrocketry");
                RUI.Icons.Selectable.Icon ic_structural = PartCategorizer.Instance.iconLoader.GetIcon("R&D_node_icon_generalconstruction");
                RUI.Icons.Selectable.Icon ic_aero = PartCategorizer.Instance.iconLoader.GetIcon("R&D_node_icon_stability");
                RUI.Icons.Selectable.Icon ic_utility = PartCategorizer.Instance.iconLoader.GetIcon("R&D_node_icon_generic");
                RUI.Icons.Selectable.Icon ic_science = PartCategorizer.Instance.iconLoader.GetIcon("R&D_node_icon_advsciencetech");
                RUI.Icons.Selectable.Icon ic_lifeSupport = new RUI.Icons.Selectable.Icon("KKAOSS_icon_KPSS", icon_category_ls, icon_category_ls, true);

                if (ic_pods == null)
                {
                    Debug.Log("[KPBS] ERROR ic_pods cannot be loaded");
                    return;
                }
                if (ic_fuels == null)
                {
                    Debug.Log("[KPBS] ERROR ic_fuels cannot be loaded");
                    return;
                }
                if (ic_engine == null)
                {
                    Debug.Log("[KPBS] ERROR ic_engine cannot be loaded");
                    return;
                }
                if (ic_structural == null)
                {
                    Debug.Log("[KPBS] ERROR ic_structural cannot be loaded");
                    return;
                }
                if (ic_aero == null)
                {
                    Debug.Log("[KPBS] ERROR ic_aero cannot be loaded");
                    return;
                }
                if (ic_utility == null)
                {
                    Debug.Log("[KPBS] ERROR ic_utility cannot be loaded");
                    return;
                }
                if (ic_science == null)
                {
                    Debug.Log("[KPBS] ERROR ic_science cannot be loaded");
                    return;
                }
                if (ic_lifeSupport == null)
                {
                    Debug.Log("[KPBS] ERROR ic_lifeSupport cannot be loaded");
                    return;
                }

                //add KPSS to the categories
                PartCategorizer.Category surfaceStructureFilter = PartCategorizer.AddCustomFilter("Planetary Surface Structures", filterIconSurfaceStructures, new Color(0.63f, 0.85f, 0.63f));

                //add subcategories to the KPSS category you just added
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Pods", ic_pods, p => filterCategories(p, PartCategories.Pods));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Fuel Tank", ic_fuels, p => filterCategories(p, PartCategories.FuelTank));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Engines", ic_engine, p => filterCategories(p, PartCategories.Propulsion));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Structural", ic_structural, p => filterCategories(p, PartCategories.Structural));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Aerodynamics", ic_aero, p => filterCategories(p, PartCategories.Aero));
                PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Science", ic_science, p => filterCategories(p, PartCategories.Science));

                if (LS_parts.Count > 0)
                {
                    PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Life Support", ic_lifeSupport, p => filter_KKAOSS_LS(p));
                    PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Utility", ic_utility, p => (filterCategories(p, PartCategories.Utility) && !filter_KKAOSS_LS(p)));
                }
                else
                {
                    PartCategorizer.AddCustomSubcategoryFilter(surfaceStructureFilter, "Utility", ic_utility, p => (filterCategories(p, PartCategories.Utility)));
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
                PartCategorizer.Category functionFilter = PartCategorizer.Instance.filters.Find(f => f.button.categoryName == "Filter by Function");

                //Add a new subcategory to the function filter
                if ((KPBSConfiguration.Instance().SeparateLifeSupport) && (LS_parts.Count > 0))
                    PartCategorizer.AddCustomSubcategoryFilter(functionFilter, "Planetary Surface Structures", filterIconSurfaceStructures, p => filter_KKAOSS_NO_LS(p));
                else
                    PartCategorizer.AddCustomSubcategoryFilter(functionFilter, "Planetary Surface Structures", filterIconSurfaceStructures, p => filter_KKAOSS(p));

                //Force the icon to show (>_<)
                RUIToggleButtonTyped button = functionFilter.button.activeButton;
                button.SetFalse(button, RUIToggleButtonTyped.ClickType.FORCED);
                button.SetTrue(button, RUIToggleButtonTyped.ClickType.FORCED);

                List<AvailablePart> parts = PartLoader.Instance.parts.FindAll(ap => ap.name.StartsWith("KKAOSS"));

                //iterate over all categories and remove them from the list
                //set all the categories to none to prevent this part to be added
                foreach (AvailablePart part in parts)
                {
                    part.category = PartCategories.none;
                }
            }

            //---------end subcategory in function filter-------

            //------------subcategory for life support---------

            //when life support should be separated
            if (KPBSConfiguration.Instance().SeparateLifeSupport)
            {

                Debug.Log("[KPBS] Life Support Modules found: " + LS_parts.Count());

                //only continue when there are parts for life support
                if (LS_parts.Count() > 0)
                {
                    RUI.Icons.Selectable.Icon filterIconLifeSupport = new RUI.Icons.Selectable.Icon("KKAOSS_icon_KPSS", icon_category_ls, icon_category_ls, true);

                    if (filterIconLifeSupport == null)
                    {
                        Debug.Log("[KPBS] ERROR filterIconLifeSupport cannot be loaded");
                        return;
                    }

                    //Find the function filter
                    PartCategorizer.Category functionFilter = PartCategorizer.Instance.filters.Find(f => f.button.categoryName == "Filter by Function");
                    PartCategorizer.AddCustomSubcategoryFilter(functionFilter, "Life Support", filterIconLifeSupport, p => filter_KKAOSS_LS(p));

                    //set all the categories to none to prevent this part to be added
                    foreach (AvailablePart part in LS_parts)
                    {
                        part.category = PartCategories.none;
                    }

                    //add the greenhouse the the LS mods when other ls mods were found
                    List<AvailablePart> greenhouses = PartLoader.Instance.parts.FindAll(ap => ap.name.Equals("KKAOSS.Greenhouse.g"));
                    foreach (AvailablePart part in greenhouses)
                    {
                        part.category = PartCategories.none;
                    }

                    //Force the icon to show (>_<)
                    RUIToggleButtonTyped button = functionFilter.button.activeButton;
                    button.SetFalse(button, RUIToggleButtonTyped.ClickType.FORCED);
                    button.SetTrue(button, RUIToggleButtonTyped.ClickType.FORCED);
                }
            }

            //---------end subcategory for life support-------
        }
    }
}
