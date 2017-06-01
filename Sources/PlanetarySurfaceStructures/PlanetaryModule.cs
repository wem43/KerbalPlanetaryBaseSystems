using UnityEngine;
using KSP.Localization;

namespace PlanetarySurfaceStructures 
{
    class PlanetaryModule : PartModule, IModuleInfo
    {
        //----------------KSPFields-----------------

        /** The name of the animation */
        [KSPField]
        public string animationName = string.Empty;

        /** The name of the startEvent */
        [KSPField]
        public string startEventGUIName = Localizer.GetStringByTag("#LOC_KPBS.planetarymodule.deploy");//"Deploy";
		
		/** The name of the end event */
        [KSPField]
        public string endEventGUIName = Localizer.GetStringByTag("#LOC_KPBS.planetarymodule.retract");//"Retract";

        /** Varaible defining if the animation is available in EVA */
        [KSPField]
        public bool availableInEVA = false;
		
		/** Varaible defining if the animation is available from the vessel */
        [KSPField]
        public bool availableInVessel = true;
		
		/** Varaible defining if the animation is available in the editor */
        [KSPField]
        public bool availableInEditor = true;
		
		/** The range the animaiton is available from EVA */
        [KSPField]
        public float EVArange = 5f;

        /** The crew capacity when the module is deployed */
        [KSPField]
        public int crewCapacityDeployed = 0;
		
		/** The crew capacity when the module is retracted */
        [KSPField]
        public int crewCapcityRetracted = 0;

        //the shown status of the Module
        [KSPField(guiActive = true, guiName = "Status")]
        public string status = "Retracted";
        public ModuleState moduleStatus = ModuleState.Retracted;

        /** The layer of the animation */
        [KSPField]
        public int layer = 1;

        //---------------------persistent states--------------------
        [KSPField(isPersistant = true)]
        public float animationTime;
        [KSPField(isPersistant = true)]
        public bool nextIsReverse;
        [KSPField(isPersistant = true)]
        public bool hasBeenInitialized = false;

        //------------------------------------for changes in the IVA-------------------------

        /** Varaible defining if the animation is available in EVA */
        [KSPField]
        public bool changeIVA = true;

        //the name of the packed internal
        [KSPField]
        public string packedInternalName = "packed";

        //the name of the extended internal
        [KSPField]
        public string extendedInternalName = "extended";

        /** Varaible defining if the animation is available in EVA */
        [KSPField]
        public bool JSIATPSupportEnabled = false;

        //the name of the additional internal
        [KSPField]
        public string packedOverlayHiddenName = string.Empty;

        //the name of the additional internal
        [KSPField]
        public string extendedOverlayHiddenName = string.Empty;

        //indicate that the packed internal is currently used
        private bool isInternalPacked = true;

        //the transform for the extended part
        Transform extendedInternalTransform;

        //the transform for the packed part
        Transform packedInternalTransform;

        //the transform for an additional depthmask
        private Transform packedOverlayHiddenTransform;

        //the transform for an additional depthmask
        private Transform extendedOverlayHiddenTransform;

        //----------------internal data-----------------

        //the stored animation
        private Animation deployAnim;


        //----------------actions-----------------

        /**
		 * The action that can be used for makros to toggle the state of the module
		 */
        [KSPAction("#LOC_KPBS.planetarymodule.event.toggle")]
        public void toggleAction(KSPActionParam param) 
		{
            if ((availableInVessel) && (!(moduleStatus == ModuleState.Deployed) || (this.part.protoModuleCrew.Count <= crewCapcityRetracted))) {
                toggleAnimation();
            }            
        }

		/**
		 * The retracts action that can be used for makros
		 */
        [KSPAction("#LOC_KPBS.planetarymodule.action.retract")]
        public void retractAction(KSPActionParam param) 
		{
			if (availableInVessel) 
			{
				//only retract when the module is retracting or retracted and no kerbals are inside
                if (((moduleStatus == ModuleState.Deployed) || (moduleStatus == ModuleState.Deploying)) && 
                    (part.protoModuleCrew.Count <= crewCapcityRetracted)) 
				{
                    toggleAnimation();
                }
            }
        }

        /**
		 * The deploy action that can be used for makros
		 */
        [KSPAction("#LOC_KPBS.planetarymodule.action.deploy")]
        public void deployAction(KSPActionParam param) {
            
			if (availableInVessel) {
                
				if ((moduleStatus == ModuleState.Retracted) || (moduleStatus == ModuleState.Retracting)) {
                    toggleAnimation();
                }
			}
        }

        //----------------events-----------------

        /**
		 * Toggle the animation of the parts (retracting or deploying depending in the state)
		 */
        [KSPEvent(name = "toggleAnimation", guiName = "#LOC_KPBS.planetarymodule.action.deploy", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void toggleAnimation()
        {
            if (deployAnim != null)
            {
                if (nextIsReverse) //when the animation is played in reverse (retract)
                {
                    Events["toggleAnimation"].guiName = startEventGUIName;

                    //set the speed of the animation
                    deployAnim[animationName].speed = -1f;

                    //set the animation to 1f when ended            
                    if (deployAnim[animationName].normalizedTime == 0f)
                    {
                        deployAnim[animationName].normalizedTime = 1f;
                    }
                }
                else //when the normal animation is played
                {
                    Events["toggleAnimation"].guiName = endEventGUIName;

                    //set the speed of the animation
                    deployAnim[animationName].speed = 1f;

                    //set the animation to 0f when ended
                    if (deployAnim[animationName].normalizedTime == 1f)
                    {
                        deployAnim[animationName].normalizedTime = 0f;
                    }
                }
                deployAnim.Play(animationName);

                //reverse the animation
                nextIsReverse = !nextIsReverse;
            }
        }

        //----------------methods-----------------

        /**
		 * Called at the start. Initialize animation and state of this module
		 */
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            part.CheckTransferDialog();

            isInternalPacked = true;

            //get the deploy animation
            deployAnim = part.FindModelAnimators(animationName)[0];

            //Only initialize when an animation is available
            if (deployAnim != null) 
            {
				// Run Only on first launch
                if (!hasBeenInitialized) 
                {
                    if (part.protoModuleCrew.Count > crewCapcityRetracted)
                    {
                        nextIsReverse = true;
                        animationTime = 0.999f;
                        deployAnim[animationName].normalizedTime = 1f;
                    }
                    else
                    {
                        nextIsReverse = false;
                        animationTime = 0f;
                        deployAnim[animationName].normalizedTime = 0f;
                    }

                    hasBeenInitialized = true;
                }

                if (part.protoModuleCrew.Count > crewCapcityRetracted)
                {
                    animationTime = 0.999f;
                    deployAnim[animationName].speed = 1f;
                }
                else if (nextIsReverse)
                {
                    if (animationTime == 0f)
                    {
                        animationTime = 1f;
                    }

                    if (animationTime == 1f)
                    {
                        status = "Deployed";
                        moduleStatus = ModuleState.Deployed;
                        part.CrewCapacity = crewCapacityDeployed;
                    }
                    else
                    {
                        status = "Deploying..";
                        moduleStatus = ModuleState.Deploying;
                        part.CrewCapacity = crewCapacityDeployed;
                    }
                    deployAnim[animationName].speed = 1f;
                }
                else
                {
                    if (animationTime == 0f)
                    {
                        status = "Retracted";
                        moduleStatus = ModuleState.Retracted;
                    }
                    else
                    {
                        status = "Retracting..";
                        moduleStatus = ModuleState.Retracting;
                    }
                    part.CrewCapacity = crewCapcityRetracted;

                    deployAnim[animationName].speed = -1f;
                }

                // Set up animation state according to persistent values
                deployAnim[animationName].layer = layer;
                deployAnim[animationName].normalizedTime = animationTime;
                deployAnim.Play(animationName);   

                // Settings for the GUI
                if (nextIsReverse)
                {
                    Events["toggleAnimation"].guiName = endEventGUIName;
                }
                else {
                    Events["toggleAnimation"].guiName = startEventGUIName;
                }

                Events["toggleAnimation"].guiActiveEditor = availableInEditor;
                Events["toggleAnimation"].guiActiveUnfocused = availableInEVA;
                Events["toggleAnimation"].guiActive = availableInVessel;
                Events["toggleAnimation"].unfocusedRange = EVArange;
            }
			// When the animation can not be found deactivate it in the GUI
            else 
			{
                Events["toggleAnimation"].guiActiveEditor = false;
                Events["toggleAnimation"].guiActive = false;
                Events["toggleAnimation"].guiActiveUnfocused = false;
                Debug.Log("[KPBS] Deploy animation not found: " + animationName);
            }
        }

		/**
		 * Get informations about this module
		 */
        public override string GetInfo()
        {
            return Localizer.GetStringByTag("#LOC_KPBS.planetarymodule.info1") + crewCapcityRetracted.ToString() + Localizer.GetStringByTag("#LOC_KPBS.planetarymodule.info2") + crewCapacityDeployed.ToString() + "\n";
        }

		/**
		 * The update method of the module
		 */
        public void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
            {
                return;
            }
			
            //when there is an animation playing
            if (deployAnim != null)
            {
				//when the animation is not playing
                if (!deployAnim.IsPlaying(animationName))
                {
                    //update the status
                    if (nextIsReverse)
                    {
                        animationTime = 1f;
						status = "Deployed";
                        moduleStatus = ModuleState.Deployed;

                        //switch to the extended internal
                        if (isInternalPacked)
                        {
                            if (changeIVA)
                            {
                                isInternalPacked = false;
                            } 
                        }

                        //update crew capacity and contract state
                        if (part.CrewCapacity != crewCapacityDeployed)
						{
							part.CrewCapacity = crewCapacityDeployed;
                            GameEvents.onVesselWasModified.Fire(vessel);
						}
						
                    }
                    else
                    {
                        if ((!isInternalPacked))
                        {
                            if (changeIVA)
                            {
                                isInternalPacked = true;
                            }
                            
                        }
                        animationTime = 0f;
						status = "Retracted";
                        moduleStatus = ModuleState.Retracted;
                    }
                    
					//set the normalized time of the animation 
					deployAnim[animationName].normalizedTime = animationTime;
                }
				//while the animation is playing update the data
                else 
                {
                    animationTime = deployAnim[animationName].normalizedTime;

                    if ((!isInternalPacked))
                    {
                        if (changeIVA)
                        {
                            isInternalPacked = true;
                        }                       
                    }

                    //update the status text
                    if (nextIsReverse)
					{
						status = "Deploying..";
                        moduleStatus = ModuleState.Deploying;
                    }
					else 
					{
						status = "Retracting..";
                        moduleStatus = ModuleState.Retracting;
                    }
					
					//update crew capacity and contract state
					if (part.CrewCapacity != crewCapcityRetracted)
                    {
                        part.CrewCapacity = crewCapcityRetracted;
                        GameEvents.onVesselWasModified.Fire(vessel);
                    } 
                }
            }

            //show/hide GUI element depending on the crew count
            if (part.protoModuleCrew.Count > crewCapcityRetracted)
            {
                Events["toggleAnimation"].active = false;
            }
            else
            {
                Events["toggleAnimation"].active = true;
            }

            int newCrew = part.protoModuleCrew.Count;

            //check if the part has to be deployed because of too many kerbals inside
            if (newCrew > part.CrewCapacity) {
				if ((!(moduleStatus == ModuleState.Deployed)) && (!(moduleStatus == ModuleState.Deploying)))
                {
                    toggleAnimation();    
                }
			}

            CheckIVAState();
        }

        public void OnDestroy()
        {
            //we have to reset the iva to the original one when the part is destroyed to have the right one when reverting to the editor
            isInternalPacked = true;
            CheckIVAState();
        }


        //find a camera by its name
        private bool isStockOverlayActive()
        {
            int count = Camera.allCamerasCount;
            for (int i = 0; i < count; ++i)
            {
                if (Camera.allCameras[i].name.Equals("InternalSpaceOverlay Host"))
                {
                    return true;
                }
            }
            return false;
        }

        //switch to the internal with the given name
        private void CheckIVAState()
        {
            if (part.internalModel != null)
            {
                if ((extendedInternalName != string.Empty) && (extendedInternalTransform == null))
                {
                    extendedInternalTransform = part.internalModel.FindModelTransform(extendedInternalName);
                }

                if ((packedInternalName != string.Empty) && (packedInternalTransform == null))
                {
                    packedInternalTransform = part.internalModel.FindModelTransform(packedInternalName);
                }

                //when the support for JSI advanced transpared pods is enabled
                if (JSIATPSupportEnabled)
                {
                    if ((extendedOverlayHiddenName != string.Empty) && (extendedOverlayHiddenTransform == null))
                    {
                        extendedOverlayHiddenTransform = part.internalModel.FindModelTransform(extendedOverlayHiddenName);
                    }

                    if ((packedOverlayHiddenName != string.Empty) && (packedOverlayHiddenTransform == null))
                    {
                        packedOverlayHiddenTransform = part.internalModel.FindModelTransform(packedOverlayHiddenName);
                    }

                    bool overlayActive = isStockOverlayActive();

                    //set the visibility of the additional internal model that is used for JSIAdvancedTransparendPods
                    if (extendedOverlayHiddenTransform != null)
                    {
                        if (HighLogic.LoadedSceneIsEditor)
                        {
                            if (extendedOverlayHiddenTransform.gameObject.activeSelf)
                            {
                                extendedOverlayHiddenTransform.gameObject.SetActive(false);
                            }
                        }
                        else if (!isInternalPacked)
                        {
                            //when the stock IVA is active
                            if (overlayActive)
                            {
                                if (extendedOverlayHiddenTransform.gameObject.activeSelf)
                                {
                                    extendedOverlayHiddenTransform.gameObject.SetActive(false);
                                }
                            }
                            else if (!extendedOverlayHiddenTransform.gameObject.activeSelf)
                            {
                                extendedOverlayHiddenTransform.gameObject.SetActive(true);
                            }
                        }
                        else if (extendedOverlayHiddenTransform.gameObject.activeSelf)
                        {
                            extendedOverlayHiddenTransform.gameObject.SetActive(false);
                        }
                    }

                    //set the visibility of the additional internal model that is used for JSIAdvancedTransparendPods
                    if (packedOverlayHiddenTransform != null)
                    {
                        if (HighLogic.LoadedSceneIsEditor)
                        {
                            if (packedOverlayHiddenTransform.gameObject.activeSelf)
                            {
                                packedOverlayHiddenTransform.gameObject.SetActive(false);
                            }
                        }
                        else if (isInternalPacked)
                        {
                            //when the stock IVA is active
                            if (overlayActive)
                            {
                                if (packedOverlayHiddenTransform.gameObject.activeSelf)
                                {
                                    packedOverlayHiddenTransform.gameObject.SetActive(false);
                                }
                            }
                            else if (!packedOverlayHiddenTransform.gameObject.activeSelf)
                            {
                                packedOverlayHiddenTransform.gameObject.SetActive(true);
                            }
                        }
                        else if (packedOverlayHiddenTransform.gameObject.activeSelf)
                        {
                            packedOverlayHiddenTransform.gameObject.SetActive(false);
                        }
                    }

                }

                //set the visibility of the extended internal model
                if ((extendedInternalTransform != null) && (extendedInternalTransform.gameObject.activeSelf == isInternalPacked))
                {
                    extendedInternalTransform.gameObject.SetActive(!isInternalPacked);
                }
                //Set the visibility of the packed internal model
                if ((packedInternalTransform != null) && (packedInternalTransform.gameObject.activeSelf != isInternalPacked))
                {
                    packedInternalTransform.gameObject.SetActive(isInternalPacked);
                }
            }
        }

        public string GetModuleTitle()
        {
            return Localizer.GetStringByTag("#LOC_KPBS.planetarymodule.name");// "Deployable Part";
        }

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }

        public string GetPrimaryField()
        {
            return null;
        }

        public enum ModuleState
        {
            Retracted,
            Retracting,
            Deployed,
            Deploying
        }
    }
}
