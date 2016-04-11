using UnityEngine;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSIVASwitcher : PartModule
    {
        //the name of the packed internal
        [KSPField]
        public string packedInternalName = "";

        //the name of the extended internal
        [KSPField]
        public string extendedInternalName = "";

        //the name of the extended internal
        [KSPField]
        public bool switchInEditor = false;

        //the module the animation is dependent on
        private PlanetaryModule dependent = null;

        //the config nodes for the internal models
        ConfigNode packedInternalNode = null;
        ConfigNode extendedInternalNode = null;

        //indicate that the packed internal is currently used
        private bool isInternalPacked = true;

        //timer to make IVA visible
        private int visibleCounter = 0;

        //flag to indicate that the crew has to respawn
        private bool respawnCrew = false;

        /**
         * Called when the script instance is being loaded
         */
        public override void OnAwake()
        {
            base.OnAwake();

            bool foundextended = (extendedInternalName == "");
            bool foundpacked = (packedInternalName == "");

            //find the internals for the packed and the extendet internals
            foreach (UrlDir.UrlConfig cfg in GameDatabase.Instance.GetConfigs("INTERNAL"))
            {
                if ((cfg.name == extendedInternalName) && (!foundextended))
                {
                    extendedInternalNode = cfg.config;
                    foundextended = true;
                    Debug.Log("[KPBS] Extended Internal found: " + extendedInternalName);
                }
                if ((cfg.name == packedInternalName) && (!foundpacked))
                {
                    packedInternalNode = cfg.config;
                    foundpacked = true;
                    Debug.Log("[KPBS] Packed Internal found: " + packedInternalName);
                }
                if (foundextended && foundpacked)
                {
                    break;
                }
            }
        }

        //init the module of the windows
        public override void OnStart(PartModule.StartState state)
        {
            //find the dependent module 
            dependent = (PlanetaryModule)this.part.GetComponent("PlanetaryModule");

            if ((dependent != null))
            {
                //switch to the extended internal
                if (dependent.animationTime >= 0.9999f)
                {
                    SetVisibleInternal(extendedInternalNode, false);
                    isInternalPacked = false;
                    respawnCrew = true;
                }
                else
                {
                    SetVisibleInternal(packedInternalNode, false);
                    isInternalPacked = false;
                }
            }
            base.OnStart(state);
        }

        //switch to the internal with the given name
        private void SetVisibleInternal(ConfigNode intern, bool delay)
        {
            if (HighLogic.LoadedSceneIsFlight || switchInEditor)
            {
                //when the old internal exists, destroy it
                if ((part != null) && (part.internalModel != null))
                {
                    Destroy(part.internalModel.gameObject);
                    part.internalModel = null;
                }

                //create a new internal when it is specified
                if (intern != null)
                {
                    part.partInfo.internalConfig = intern;
                    part.internalModel = part.AddInternalPart(intern);
                    if (delay)
                    {
                        part.internalModel.SetVisible(false);
                        visibleCounter = 5;
                    }
                    part.CreateInternalModel();
                    part.internalModel.Initialize(part);
                }
            }
        }

        public void OnDestroy()
        {
            //we have to reset the iva to the original one when the part is destroyed to have the right one when reverting to the editor
            part.partInfo.internalConfig = packedInternalNode;  
            packedInternalNode = null;
            extendedInternalNode = null;
        }

        //update the transparency of the windows
        public void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor) return;

            if ((dependent != null))
            {
                
                //switch to the extended internal
                if ((dependent.animationTime >= 0.9999f) && (isInternalPacked))
                {
                    SetVisibleInternal(extendedInternalNode, true);
                    isInternalPacked = false;
                }
                else if ((dependent.animationTime < 0.9999f) && (!isInternalPacked))
                {
                    SetVisibleInternal(packedInternalNode, true);
                    isInternalPacked = true;
                }


                if (part.internalModel != null)
                {
                    if (visibleCounter > 0)
                    {
                        if (visibleCounter == 1)
                        {
                            part.internalModel.SetVisible(true);
                        }
                        else
                        {
                            part.internalModel.SetVisible(false);
                        }
                        visibleCounter--;
                    }
                }
                else
                {
                    visibleCounter = 0;
                }

                //respawn the crew when it is in the part when loaded
                if ((respawnCrew) && (vessel.loaded))
                {
                    //part.RegisterCrew();
                    part.internalModel.SpawnCrew();
                    respawnCrew = false;
                }
            }
        }
    }
}
