using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSAirfilter : ModuleResourceConverter
    {

        //the names of the planets that have the specified resource e.g. Oxygen:Kerbin,Laythe;Hydrogen:Eve,Jool etc
        [KSPField]
        public string planetResourceAvailability = "";

        public bool methodSet = false;

        //-------------internal fields---------

        private void OnDestroy()
        {
            GameEvents.onVesselSOIChanged.Remove(OnBodyChange);
            methodSet = false;
        }

        List<String[]> bodyNames = new List<string[]>();
        List<String> resourceNames = new List<string>();


        [KSPField(guiActive = true, guiName = "Output Resource", guiActiveEditor = true)]
        public string outputResource = "";

        [KSPField(guiActive = true, guiName = "Condition")]
        public string condition = "Nominal";

        [KSPField(isPersistant = true)]
        public int numResource = -1;

        public bool initialized = false;

        /**
         * Init the module on start
         */
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            if (!methodSet)
            {
                GameEvents.onVesselSOIChanged.Add(OnBodyChange);
                methodSet = true;
            }

            ResourceRatio resDefault = outputList.ElementAt<ResourceRatio>(0);

            //get the resources and defs
            string[] resourcesDef = planetResourceAvailability.Split(';');
            for (int i = 0; i < resourcesDef.Length; i++)
            {
                //split between planets
                string[] resource = resourcesDef[i].Split(':');
                string[] planets = resource[1].Split(',');

                resourceNames.Add(resource[0]);
                bodyNames.Add(planets);
            }

            if (resourceNames.Count > 0) {
                outputResource = resourceNames[0];
            }
            if (numResource != -1)
            {
                outputResource = resourceNames[numResource];
            }

            /*if (!HighLogic.LoadedSceneIsEditor)
            {
                Events["changeResource"].guiActive = false;
            }*/
        }

        /**
         * Increase the rate
         */
        [KSPEvent(name = "changeResource", guiName = "Change output resource", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void changeResource()
        {
            numResource++;
            if (numResource >= resourceNames.Count)
            {
                numResource = 0;
            }
            outputResource = resourceNames.ElementAt(numResource);
        }

        private void OnBodyChange(GameEvents.HostedFromToAction<Vessel, CelestialBody> data)
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
            {
                return;
            }

            if ((data.host == null) || (data.host != vessel) || (data.to == null) || (data.to.name == null))
            {
                return;
            }

            //check whether this is the right body
            if ((numResource > -1) && (bodyNames != null) && (numResource < bodyNames.Count))
            {
                string[] bodies = bodyNames.ElementAt(numResource);
                bool valid = false;
                for (int i = 0; i < bodies.Length; i++)
                {
                    if (bodies[i] == data.to.name)
                    {
                        valid = true;
                        break;
                    }
                }
                if (valid)
                {
                    condition = "Nominal";
                }
                else
                {
                    condition = outputResource + " not available!";
                }
            }
        }

        /**
         * Prepare the recipe with regard to the amount of crew in this module
         */
        protected override ConversionRecipe PrepareRecipe(double deltatime)
        {

            ConversionRecipe newRecipe = new ConversionRecipe();

            if (ModuleIsActive() && (numResource > -1))
            {
                //check whether this is the right body
                string[] bodies = bodyNames.ElementAt(numResource);
                bool valid = false;
                for (int i = 0; i < bodies.Length; i++)
                {
                    if (bodies[i] == vessel.mainBody.name)
                    {
                        valid = true;
                        break;
                    }
                }

                if (valid)
                {
                    condition = "Nominal";
                    //add the inputs to the recipe
                    foreach (ResourceRatio res in inputList)
                    {
                        ResourceRatio newRes = new ResourceRatio();
                        newRes.ResourceName = res.ResourceName;
                        newRes.FlowMode = res.FlowMode;
                        newRes.Ratio = res.Ratio;
                        newRes.DumpExcess = res.DumpExcess;
                        newRecipe.Inputs.Add(newRes);
                    }
                    //add the outputs to the recipe
                    foreach (ResourceRatio res in outputList)
                    {
                        if (res.ResourceName == outputResource)
                        {
                            ResourceRatio newRes = new ResourceRatio();
                            newRes.ResourceName = res.ResourceName;
                            newRes.FlowMode = res.FlowMode;
                            newRes.DumpExcess = res.DumpExcess;
                            newRes.Ratio = res.Ratio;
                            newRecipe.Outputs.Add(newRes);
                        }
                    }
                    //only add the requirements to the recipe
                    foreach (ResourceRatio res in reqList)
                    {
                        ResourceRatio newRes = new ResourceRatio();
                        newRes.ResourceName = res.ResourceName;
                        newRes.FlowMode = res.FlowMode;
                        newRes.DumpExcess = res.DumpExcess;
                        newRes.Ratio = res.Ratio;
                        newRecipe.Outputs.Add(newRes);
                    }
                }
                else
                {
                    this.StopResourceConverter();
                    condition = outputResource + " not available!";
                }
            }
            else
            {
                condition = "Inactive";
            }

            //the amounts (use?)
            newRecipe.FillAmount = 1;
            newRecipe.TakeAmount = 1;
            return newRecipe;
        }

    }
}
