using UnityEngine;
using KSP.Localization;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSDependentLight : PartModule
    {
        //----------------KSPFields-----------------

        //the name of the animation
        [KSPField]
        public string animationName;

        //the texts for the deployment of the module
        [KSPField]
        public string startEventGUIName = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.action.on");
        [KSPField]
        public string endEventGUIName = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.action.off");

        //availability of the animation
        [KSPField]
        public bool availableInEVA = true;
        [KSPField]
        public bool availableInVessel = true;
        [KSPField]
        public bool availableInEditor = true;
        [KSPField]
        public float EVArange = 5f;
        //the layer of the animation
        [KSPField]
        public int layer = 2;

        //--------------persistent states----------------
        [KSPField(isPersistant = true)]
        public float animationTime;
        [KSPField(isPersistant = true)]
        public bool nextIsReverse;
        [KSPField(isPersistant = true)]
        public bool hasBeenInitialized = false;

        //----------------internal data-----------------

        //the stored animation
        private Animation anim;
        //the module the animation is dependent on
        private PlanetaryModule dependent = null;
        //the state of the lights
        public string lightStatus = "Off";
        public LightState lightStatusT = LightState.Off;

        //----------------actions-----------------

        //the action to toggle the animation
        [KSPAction("#autoLOC_502011")]
        public void toggleAction(KSPActionParam param)
        {
            if (availableInVessel)
                toggleAnimation();
        }

        //the action to toggle the animation
        [KSPAction("#LOC_KPBS.dependendlight.action.on")]
        public void turnOnAction(KSPActionParam param)
        {
            if ((availableInVessel) && ((lightStatusT == LightState.Off) || (lightStatusT == LightState.TurningOff)))
                toggleAnimation();
        }

        //the action to toggle the animation
        [KSPAction("#LOC_KPBS.dependendlight.action.off")]
        public void turnOffAction(KSPActionParam param)
        {
            if ((availableInVessel) && ((lightStatusT == LightState.On) || (lightStatusT == LightState.TurningOn)))
            {
                toggleAnimation();
            }
        }

        //----------------events-----------------

        //the Event triggered when the module is deployed or retracted
        [KSPEvent(name = "toggleAnimation", guiName = "#autoLOC_502011", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void toggleAnimation()
        {
            //variable for other modules to show the module is animating 
            if (anim != null)
            {

                if (nextIsReverse) //when the animation is played in reverse (turn lights off)
                {
                    Events["toggleAnimation"].guiName = startEventGUIName;

                    //set the speed of the animation
                    anim[animationName].speed = -1f;

                    //set the animation to 1f when ended            
                    if (anim[animationName].normalizedTime == 0f)
                    {
                        anim[animationName].normalizedTime = 1f;
                    }
                }
                else //when the normal animation is played
                {
                    Events["toggleAnimation"].guiName = endEventGUIName;

                    //set the speed of the animation
                    anim[animationName].speed = 1f;

                    //set the animation to 0f when ended
                    if (anim[animationName].normalizedTime == 1f)
                    {
                        anim[animationName].normalizedTime = 0f;
                    }
                }
                anim.Play(animationName);

                //reverse the animation
                nextIsReverse = !nextIsReverse;
            }
        }

        //----------------methods-----------------

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //find the animation
            anim = part.FindModelAnimators(animationName)[0];

            //find the dependent module 
            dependent = (PlanetaryModule)part.GetComponent("PlanetaryModule");


            if (anim != null) //Only Init when an animation is available
            {
                if (!hasBeenInitialized) // Run Only on first launch
                {
                    nextIsReverse = false;
                    animationTime = 0f;
                    anim[animationName].normalizedTime = 0f;
                    hasBeenInitialized = true;
                }

                // make sure you stay in your place on launch, and don't start going in the wrong direction if deployed/retracted
                if (nextIsReverse)
                {
                    anim[animationName].speed = 1f;
                    if (animationTime == 0f)
                    {
                        animationTime = 1f;
                    }

                    if (animationTime == 1f)
                    {
                        lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.on");
                        lightStatusT = LightState.On;
                    }
                    else
                    {
                        lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.turningon");
                        lightStatusT = LightState.TurningOn;
                    }

                }
                else
                {
                    if (animationTime == 0f)
                    {
                        lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.off");
                        lightStatusT = LightState.Off;
                    }
                    else
                    {
                        lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.turningoff");
                        lightStatusT = LightState.TurningOff;
                    }

                    anim[animationName].speed = -1f;
                }

                //set up animation state according to persistent values
                anim[animationName].layer = layer;
                anim.Play(animationName);
                anim[animationName].normalizedTime = animationTime;

                //Settings for the GUI
                if (nextIsReverse)
                {
                    Events["toggleAnimation"].guiName = endEventGUIName;
                }
                else
                {
                    Events["toggleAnimation"].guiName = startEventGUIName;
                }

                //check whether the element can be active or not
                bool bShowGUI = true;
                if (dependent != null)
                {
                    bShowGUI = dependent.moduleStatus == PlanetaryModule.ModuleState.Deployed;
                }

                Events["toggleAnimation"].guiActiveEditor = availableInEditor;
                Events["toggleAnimation"].guiActiveUnfocused = availableInEVA && bShowGUI;
                Events["toggleAnimation"].guiActive = availableInVessel && bShowGUI;
                Events["toggleAnimation"].unfocusedRange = EVArange;
            }
            else //When the animation can not be found deactivate it in the GUI
            {
                Events["toggleAnimation"].guiActiveEditor = false;
                Events["toggleAnimation"].guiActive = false;
                Events["toggleAnimation"].guiActiveUnfocused = false;
                Debug.Log("KBILightAnimation: Animation not found: " + animationName);
            }
        }

        public void OnDestroy()
        {
            //free referenced module
            dependent = null;
        }

        public void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor) return;

            //when the animation is not playing
            if (anim != null)
            {              
                //only update the status when the dependent exists
                if ((dependent != null))
                {
                    //set the animation to the beginning when the state is not "Deployed"
                    if (dependent.animationTime < 0.999f)
                    {
                        //disable the GUI
                        Events["toggleAnimation"].guiActiveEditor = false;
                        Events["toggleAnimation"].guiActiveUnfocused = false;
                        Events["toggleAnimation"].guiActive = false;
                        Events["toggleAnimation"].guiName = startEventGUIName;

                        if (anim.IsPlaying(animationName))
                        {
                            if ((anim[animationName].speed > 0))
                            {
                                anim[animationName].speed = -1f;
                                nextIsReverse = false;
                            }
                            animationTime = anim[animationName].normalizedTime;

                            lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.turningoff");
                            lightStatusT = LightState.TurningOff;
                        }
                        else
                        {
                            //when the animation is in the active state
                            if (nextIsReverse)
                            {

                                anim[animationName].speed = -1f;
                                if (anim[animationName].normalizedTime == 0f)
                                {
                                    anim[animationName].normalizedTime = 1f;
                                }
                                anim.Play(animationName);
                                nextIsReverse = false;
                                animationTime = 1f;
                            }
                            else
                            {
                                animationTime = 0f;
                            }

                            lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.off");
                            lightStatusT = LightState.Off;
                        }
                    }
                    else
                    {
                        //update the animation state
                        if ((!anim.IsPlaying(animationName))) //when the animation is not playing
                        {
                            //update the status
                            if (nextIsReverse)
                            {
                                animationTime = 1f;
                                lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.on");
                                lightStatusT = LightState.On;
                            }
                            else
                            {
                                animationTime = 0f;
                                lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.off");
                                lightStatusT = LightState.Off;
                            }
                            anim[animationName].normalizedTime = animationTime;
                        }
                        else //while the animation is playing update the data
                        {
                            if (nextIsReverse)
                            {
                                lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.turningon");
                                lightStatusT = LightState.TurningOn;
                            }
                            else
                            {
                                lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.turningoff");
                                lightStatusT = LightState.TurningOff;
                            }
                            animationTime = anim[animationName].normalizedTime;
                        }


                        //enable the GUI
                        Events["toggleAnimation"].guiActiveEditor = availableInEditor;
                        Events["toggleAnimation"].guiActiveUnfocused = availableInEVA;
                        Events["toggleAnimation"].guiActive = availableInVessel;
                    }
                }
                //normal behaviour when no dependent is found
                else if ((!anim.IsPlaying(animationName))) //when the animation is not playing
                {
                    //update the status
                    if (nextIsReverse)
                    {
                        animationTime = 1f;
                        lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.on");
                        lightStatusT = LightState.On;
                    }
                    else
                    {
                        animationTime = 0f;
                        lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.off");
                        lightStatusT = LightState.Off;
                    }
                    anim[animationName].normalizedTime = animationTime;
                }
                else //while the animation is playing update the data
                {
                    if (nextIsReverse)
                    {
                        lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.turningon");
                        lightStatusT = LightState.TurningOn;
                    }
                    else
                    {
                        lightStatus = Localizer.GetStringByTag("#LOC_KPBS.dependendlight.status.turningoff");
                        lightStatusT = LightState.TurningOff;
                    }

                    animationTime = anim[animationName].normalizedTime;
                }

            }
        }
    }

    public enum LightState
    {
        On,
        Off,
        TurningOn,
        TurningOff
    }
}
