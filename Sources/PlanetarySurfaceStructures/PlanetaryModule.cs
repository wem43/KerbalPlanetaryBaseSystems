using UnityEngine;

namespace PlanetarySurfaceStructures 
{
    class PlanetaryModule : PartModule  
	{
        //----------------KSPFields-----------------

        /** The name of the animation */
        [KSPField]
        public string animationName;

        /** The name of the startEvent */
        [KSPField]
        public string startEventGUIName = "Deploy";
		
		/** The name of the end event */
        [KSPField]
        public string endEventGUIName = "Retract";

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

        //the name of the additional internal
        [KSPField]
        public string additionalInternalName;

        //indicate that the packed internal is currently used
        private bool isInternalPacked = true;

        //the transform for the extended part
        Transform extendedInternalTransform;

        //the transform for the packed part
        Transform packedInternalTransform;

        //the transform for an additional depthmask
        private Transform additionalInternalTransform;

        //----------------internal data-----------------

        //the stored animation
        private Animation deployAnim;


        //----------------actions-----------------

        /**
		 * The action that can be used for makros to toggle the state of the module
		 */
        [KSPAction("Toggle deployment")]
        public void toggleAction(KSPActionParam param) 
		{
            if ((availableInVessel) && (!status.Equals("Deployed") || (this.part.protoModuleCrew.Count <= crewCapcityRetracted))) {
                toggleAnimation();
            }            
        }

		/**
		 * The retracts action that can be used for makros
		 */
        [KSPAction("Retract")]
        public void retractAction(KSPActionParam param) 
		{
			if (availableInVessel) 
			{
				//only retract when the module is retracting or retracted and no kerbals are inside
                if ((status.Equals("Deployed") || status.Equals("Deploying..")) && 
                    (part.protoModuleCrew.Count <= crewCapcityRetracted)) 
				{
                    toggleAnimation();
                }
            }
        }

        /**
		 * The deploy action that can be used for makros
		 */
        [KSPAction("Deploy")]
        public void deployAction(KSPActionParam param) {
            
			if (availableInVessel) {
                
				if (status.Equals("Retracted") || status.Equals("Retracting..")) {
                    toggleAnimation();
                }
			}
        }

        //----------------events-----------------

        /**
		 * Toggle the animation of the parts (retracting or deploying depending in the state)
		 */
        [KSPEvent(name = "toggleAnimation", guiName = "Deploy", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
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
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            part.CheckTransferDialog();

            isInternalPacked = true;

            //get the deploy animation
            deployAnim = part.FindModelAnimators(animationName)[0];

            //find the transforms
            extendedInternalTransform = part.internalModel.FindModelTransform(extendedInternalName);
            packedInternalTransform = part.internalModel.FindModelTransform(packedInternalName);
            additionalInternalTransform = part.internalModel.FindModelTransform(additionalInternalName);

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
                        part.CrewCapacity = crewCapacityDeployed;
                    }
                    else
                    {
                        status = "Deploying..";
                        part.CrewCapacity = crewCapacityDeployed;
                    }
                    deployAnim[animationName].speed = 1f;
                }
                else
                {
                    if (animationTime == 0f)
                    {
                        status = "Retracted";
                    }
                    else
                    {
                        status = "Retracting..";
                    }
                    this.part.CrewCapacity = crewCapcityRetracted;

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
            return "\nCrew Capacity\n Retracted:\t" + crewCapcityRetracted.ToString() + "\n Deployed:\t" + crewCapacityDeployed.ToString() + "\n";
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
					}
					else 
					{
						status = "Retracting..";
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
				if ((!status.Equals("Deployed")) && (!status.Equals("Deploying..")))
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
        private Camera findCamera(string cameranName)
        {
            int count = Camera.allCamerasCount;
            for (int i = 0; i < count; ++i)
            {
                if (Camera.allCameras[i].name.Equals(cameranName))
                {
                    return Camera.allCameras[i];
                }
            }
            return null;
        }

        //switch to the internal with the given name
        private void CheckIVAState()
        {
            if (part.internalModel != null)
            {
                //set the visibility of the additional internal model that is used for JSIAdvancedTransparendPods
                if (additionalInternalTransform != null)
                {
                    if (!isInternalPacked) 
                    {
                        //when the stock IVA is active
                        if ((findCamera("InternalSpaceOverlay Host") != null) && (additionalInternalTransform.gameObject.activeSelf))
                        {
                            additionalInternalTransform.gameObject.SetActive(false);
                        }
                        else if (!additionalInternalTransform.gameObject.activeSelf)
                        {
                            additionalInternalTransform.gameObject.SetActive(true);
                        }
                    }
                    else if (additionalInternalTransform.gameObject.activeSelf)
                    {
                        additionalInternalTransform.gameObject.SetActive(false);
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

        public bool IsInternalExtended()
        {
            return !isInternalPacked;
        }
    }
}
