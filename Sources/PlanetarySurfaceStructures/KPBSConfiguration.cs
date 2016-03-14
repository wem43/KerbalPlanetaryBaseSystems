using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

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
        private bool showManufacturer = false;
        private bool showModFilter = false;
        private bool separateLifeSupport = false;
        private bool showSeparateFunctionCategory = false;

        public bool ShowManufacturer
        {
            get { return showManufacturer; }
        }
        public bool ShowModFilter
        {
            get { return showModFilter; }
        }
        public bool ShowSeparateFunctionCategory
        {
            get { return showSeparateFunctionCategory; }
        }
        public bool SeparateLifeSupport
        {
            get { return separateLifeSupport; }
        }

        /**
         * The constructor for this class reading the settings
         */
        private KPBSConfiguration()
        {
            Debug.Log("[KPBS]Init settings");

            try
            {
                node = GameDatabase.Instance.GetConfigNodes("KPBSConfig").FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.Log("[KPBS] ERROR config node null exception");
            }


            if (node == null)
            {
                Debug.Log("[KPBS] ERROR config node is null");
            }

            try
            {
                showManufacturer = bool.Parse(node.GetValue("showManufacturers"));


                showModFilter = bool.Parse(node.GetValue("showModCategory"));
                showSeparateFunctionCategory = bool.Parse(node.GetValue("separateFunctionFilter"));
                separateLifeSupport = bool.Parse(node.GetValue("separateLifeSupport"));

                Debug.Log("[KPBS]showManufacturer: " + showManufacturer);
                Debug.Log("[KPBS]showModFilter: " + showModFilter);
                Debug.Log("[KPBS]showSeparateFunctionCategory: " + showSeparateFunctionCategory);
                Debug.Log("[KPBS]separateLifeSupport: " + separateLifeSupport);
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
