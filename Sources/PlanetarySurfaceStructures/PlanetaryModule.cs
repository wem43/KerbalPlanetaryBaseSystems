using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public bool availableInEditor = false;
		
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

        //--------------persistent states----------------
        [KSPField(isPersistant = true)]
        public float animationTime;
        [KSPField(isPersistant = true)]
        public bool nextIsReverse;
        [KSPField(isPersistant = true)]
        public bool hasBeenInitialized = false;

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
            if ((availableInVessel) && (!status.Equals("Deployed") || (this.part.protoModuleCrew.Count() <= crewCapcityRetracted))) {
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
                    (part.protoModuleCrew.Count() <= crewCapcityRetracted)) 
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

            //get the deploy animation
            deployAnim = part.FindModelAnimators(animationName).FirstOrDefault();

			//Only initialize when an animation is available
            if (deployAnim != null) 
            {
				// Run Only on first launch
                if (!hasBeenInitialized) 
                {
                    nextIsReverse = false;
                    animationTime = 0f;
                    deployAnim[animationName].normalizedTime = 0f;
                    hasBeenInitialized = true;
                }

                if (nextIsReverse)
                {
                    if (animationTime == 0f)
                    {
                        animationTime = 1f;
                    }

                    if (animationTime == 1f)
                    {
                        status = "Deployed";
                        this.part.CrewCapacity = crewCapacityDeployed;
                    }
                    else
                    {
                        status = "Deploying..";
                        this.part.CrewCapacity = crewCapacityDeployed;
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
            base.OnUpdate();

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
						this.status = "Deployed";
						
						//update crew capacity and contract state
						if (this.part.CrewCapacity != crewCapacityDeployed)
						{
							this.part.CrewCapacity = crewCapacityDeployed;
                            //if (this.vessel == FlightGlobals.ActiveVessel) {
                                GameEvents.onVesselWasModified.Fire(vessel);
                            //    GameEvents.onVesselChange.Fire(this.vessel);
							//}
							
						}
						
                    }
                    else
                    {
                        animationTime = 0f;
						this.status = "Retracted";
                    }
                    
					//set the normalized time of the animation 
					deployAnim[animationName].normalizedTime = animationTime;
                }
				//while the animation is playing update the data
                else 
                {
                    animationTime = deployAnim[animationName].normalizedTime;
					
					//update the status text
					if (nextIsReverse)
					{
						this.status = "Deploying..";
					}
					else 
					{
						this.status = "Retracting..";
					}
					
					//update crew capacity and contract state
					if (this.part.CrewCapacity != crewCapcityRetracted)
                    {
                        this.part.CrewCapacity = crewCapcityRetracted;
                        GameEvents.onVesselWasModified.Fire(vessel);
                        //if (this.vessel == FlightGlobals.ActiveVessel) {
                        //	GameEvents.onVesselChange.Fire(this.vessel);
                        //}
                    } 
                }
            }

            //show/hide GUI element depending on the crew count
            if (this.part.protoModuleCrew.Count() > crewCapcityRetracted)
            {
                Events["toggleAnimation"].active = false;
            }
            else
            {
                Events["toggleAnimation"].active = true;
            }

			//check if the part has to be deployed because of too many kerbals inside
			if (this.part.protoModuleCrew.Count() > this.part.CrewCapacity) {
				if ((!status.Equals("Deployed")) && (!status.Equals("Deploying..")))
                {
                    toggleAnimation();    
                }
			}
        }

		
		/**
		 * Update all the contracts with the new crew capacity
		 */
        private void updateContracts()
        {
			//go through all contracts and trigger them to update their status
            foreach (Contracts.Contract contract in Contracts.ContractSystem.Instance.Contracts)
            {
                contract.Reset();
            }
        }

        /**
         * Interface to listen to changes in the state of the deployable module
         */
        public interface DeployableModuleStateChangeListener
        {
            /**
             * Function called when the state of this module has changed
             * @param status: The new status of this module
             */
            void moduleStateChanged(string status);
        }
    }
}
