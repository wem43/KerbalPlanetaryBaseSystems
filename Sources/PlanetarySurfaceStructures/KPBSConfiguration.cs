using System;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    class KPBSConfiguration
    {
        //static instance of itself
        private static KPBSConfiguration kPBSConfig;

        //get the instance of this config file
        public static KPBSConfiguration Instance()
        {
            if (kPBSConfig == null)
                kPBSConfig = new KPBSConfiguration();
            return kPBSConfig;
        }

        private ConfigNode node;

        //saves where the manufacturer should be shown
        private bool showManufacturer = false;

        //saves whether the mod shoud add a filter for all its parts
        private bool showModFilter = false;

        //saves wheter the parts of this mod have their own category
        private bool showSeparateFunctionCategory = false;

        /**
         * Get if the manufacturer should be shown
         **/
        public bool ShowManufacturer
        {
            get { return showManufacturer; }
        }

        /**
          * Get if the mod should have its own filter
          * @return true when it should be shown, else false
         **/
        public bool ShowModFilter
        {
            get { return showModFilter; }
        }

        /**
          * Get if the mod should have its own category
          * @return true when it should be shown, else false
         **/
        public bool ShowSeparateFunctionCategory
        {
            get { return showSeparateFunctionCategory; }
        }

        // The constructor for this class reading the settings
        private KPBSConfiguration()
        {
            Debug.Log("[KPBS]Init settings");

            //try to get the config node
            try
            {
                node = GameDatabase.Instance.GetConfigNodes("KPBSConfig")[0];
            }
            catch (Exception e)
            {
                Debug.Log("[KPBS] ERROR config node null exception");
            }

            //when ne node is null, report an error
            if (node == null)
            {
                Debug.Log("[KPBS] ERROR config node is null");
            }

            //try to read and set all the settings
            try
            {
                showManufacturer = bool.Parse(node.GetValue("showManufacturers"));
                showModFilter = bool.Parse(node.GetValue("showModCategory"));
                showSeparateFunctionCategory = bool.Parse(node.GetValue("separateFunctionFilter"));

                //Log the settings that are read from the config file
                Debug.Log("[KPBS]showManufacturer: " + showManufacturer);
                Debug.Log("[KPBS]showModFilter: " + showModFilter);
                Debug.Log("[KPBS]showSeparateFunctionCategory: " + showSeparateFunctionCategory);
            }
            catch (ArgumentNullException exception)
            {
                Debug.Log("[KPBS] ERROR config node argument is null " + exception.Message);
            }
            catch (FormatException exception)
            {
                Debug.Log("[KPBS] ERROR config node argument malformed " + exception.Message);
            }

        }
    }
}
