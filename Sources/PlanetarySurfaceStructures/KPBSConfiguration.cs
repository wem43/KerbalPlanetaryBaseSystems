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

        //saves whether the mod shoud add a filter for all its parts
        private bool showModFilter = false;

        /**
          * Get if the mod should have its own filter
          * @return true when it should be shown, else false
         **/
        public bool ShowModFilter
        {
            get { return showModFilter; }
        }

        // The constructor for this class reading the settings
        private KPBSConfiguration()
        {
            //Debug.Log("[KPBS]Init settings");

            //try to get the config node
            try
            {
                node = GameDatabase.Instance.GetConfigNodes("KPBSConfig")[0];
            }
            catch (Exception e)
            {
                Debug.LogError("[KPBS] ERROR config exception: " +e.Message);
            }

            //when ne node is null, report an error
            if (node == null)
            {
                Debug.LogError("[KPBS] ERROR config node is null");
            }

            //try to read and set all the settings
            try
            {
                showModFilter = bool.Parse(node.GetValue("showModCategory"));
            }
            catch (ArgumentNullException exception)
            {
                Debug.LogError("[KPBS] ERROR config node argument is null " + exception.Message);
            }
            catch (FormatException exception)
            {
                Debug.LogError("[KPBS] ERROR config node argument malformed " + exception.Message);
            }

        }
    }
}
