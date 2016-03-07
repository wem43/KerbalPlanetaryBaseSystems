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

        //the name of the animation
        [KSPField]
        public string animationName;

        //the texts for the deployment of the module
        [KSPField]
        public string startEventGUIName = "Deploy";
        [KSPField]
        public string endEventGUIName = "Retract";

        //availability of the animation
        [KSPField]
        public bool availableInEVA = false;
        [KSPField]
        public bool availableInVessel = true;
        [KSPField]
        public bool availableInEditor = false;
        [KSPField]
        public float EVArange = 5f;

        //the capacities of the module
        [KSPField]
        public int crewCapacityDeployed = 0;
        [KSPField]
        public int crewCapcityRetracted = 0;

        //the shown status of the Module
        [KSPField(guiActive = true, guiName = "Status")]
        public string status = "Retracted";

        //the layer of the animation
        [KSPField]
        public int layer = 1;

        private int cnt = 0;
        private bool flightReady = false;

        //time that stores the starttime of the vessel
        private System.DateTime startTime;

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

        //the action to toggle the animation
        [KSPAction("Toggle deployment")]
        public void toggleAction(KSPActionParam param)
        {
            if ((availableInVessel) && (!status.Equals("Deployed") || (this.part.protoModuleCrew.Count() <= crewCapcityRetracted)))
            {
                toggleAnimation();
            }
        }

        //the action to toggle the animation
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

        //the action to toggle the animation
        [KSPAction("Deploy")]
        public void deployAction(KSPActionParam param)
        {
            if (availableInVessel)
                if (status.Equals("Retracted") || status.Equals("Retracting.."))
                {
                    toggleAnimation();
                }
        }

        //----------------events-----------------

        //the Event triggered when the module is deployed or retracted
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

        public override void OnAwake()
        {
            base.OnAwake();

            //register for the right game events
            Debug.Log("[KPBS] PM Register Flight Ready");
            GameEvents.onFlightReady.Add(setFlightReady);
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            //set the counter to 0 and the ready state to false
            cnt = 0;
            flightReady = false;

            startTime = DateTime.Now;

            //get the deploy animation
            deployAnim = part.FindModelAnimators(animationName).FirstOrDefault();

            if (deployAnim != null) //Only Init when an animation is available
            {
                if (!hasBeenInitialized) // Run Only on first launch
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

                //set up animation state according to persistent values
                deployAnim[animationName].layer = layer;
                deployAnim[animationName].normalizedTime = animationTime;
                deployAnim.Play(animationName);   

                //Settings for the GUI
                if (nextIsReverse)
                {
                    Events["toggleAnimation"].guiName = endEventGUIName;
                }
                else
                {
                    Events["toggleAnimation"].guiName = startEventGUIName;
                }

                Events["toggleAnimation"].guiActiveEditor = availableInEditor;
                Events["toggleAnimation"].guiActiveUnfocused = availableInEVA;
                Events["toggleAnimation"].guiActive = availableInVessel;
                Events["toggleAnimation"].unfocusedRange = EVArange;
            }
            else //When the animation can not be found deactivate it in the GUI
            {
                Events["toggleAnimation"].guiActiveEditor = false;
                Events["toggleAnimation"].guiActive = false;
                Events["toggleAnimation"].guiActiveUnfocused = false;
                Debug.Log("[KPBS] Deploy animation not found: " + animationName);
            }
        }

        public override string GetInfo()
        {
            return "\nCrew Capacity\n Retracted:\t" + crewCapcityRetracted.ToString() + "\n Deployed:\t" + crewCapacityDeployed.ToString() + "\n";
        }

        public void OnDestroy() 
        {
            flightReady = false;
            GameEvents.onFlightReady.Remove(setFlightReady);
        }

        private void setFlightReady()
        {
            flightReady = true;
            //Debug.Log("[KPBS] PM Flight set");
        }

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
							updateContracts();
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
                        updateContracts();
                    } 
                }

                //Update the status and crew capacity depending on the animation state
                /*if (animationTime == 1f)
                {
                    this.status = "Deployed";
                    if (this.part.CrewCapacity != crewCapacityDeployed)
                    {
                        this.part.CrewCapacity = crewCapacityDeployed;
                        updateContracts();
                    }
                }
                else if (animationTime == 0f)
                {
                    this.status = "Retracted";
                    if (this.part.CrewCapacity != crewCapcityRetracted)
                    {
                        this.part.CrewCapacity = crewCapcityRetracted;
                        updateContracts();
                    }
                   
                }
                else if (nextIsReverse)
                {
                    this.status = "Deploying..";
                    if (this.part.CrewCapacity != crewCapcityRetracted)
                    {
                        this.part.CrewCapacity = crewCapcityRetracted;
                        updateContracts();
                    }
                }
                else
                {
                    this.status = "Retracting..";
                    if (this.part.CrewCapacity != crewCapcityRetracted)
                    {
                        this.part.CrewCapacity = crewCapcityRetracted;
                        updateContracts();
                    }
                }*/
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
			
            //pop out the crew when it has no space
            /*if ((flightReady) && (this.part.protoModuleCrew.Count() > this.part.CrewCapacity))
            {
                //Debug.Log("[KPBS] PM Flight Ready and with more crew");

                if ((!status.Equals("Deployed")) && (!status.Equals("Deploying..")))
                {
                    //Debug.Log("[KPBS] PM Deploying module because of too high crew count");
                    toggleAnimation();    
                }
                else if (status.Equals("Deployed"))
                {
                    //only eject one kerbal at a time.
                    if (cnt > 25)
                    {
                        cnt = 0;

                        //find the first crew member
                        System.Collections.Generic.List<ProtoCrewMember> crewMembers = this.part.protoModuleCrew;
                        ProtoCrewMember crewMember = crewMembers.First();

                        if (FlightEVA.SpawnEVA(crewMember.seat.kerbalRef))
                        {
                            ScreenMessages.PostScreenMessage(crewMembers.First().name + "has left the vessel. There was not enough space!");
                        }
                    }
                    else
                    {
                        cnt++;
                    }
                }
            }
            //set the flightready to true after two seconds (missed somehow?)
            else if (!flightReady)
            {
                if (System.DateTime.Now.Subtract(startTime).TotalMilliseconds > 5000)
                {
                    flightReady = true;
                }
            }*/
        }

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
