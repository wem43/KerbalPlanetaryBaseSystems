using System;
using System.Linq;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSWaterHarvester : ModuleResourceHarvester
    {
        //the names of the planets that have the specified resource e.g. Kerbin,Laythe
        [KSPField]
        public string planetResourceAvailability = "";

        [KSPField(guiActive = true, guiName = "Condition")]
        public string condition = "Nominal";

        public bool methodSet = false;

        String[] bodyNames = null;

        private void OnDestroy()
        {
            GameEvents.onVesselSOIChanged.Remove(OnBodyChange);
            methodSet = false;
        }

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


            //get the resources and defs
            bodyNames = planetResourceAvailability.Split(',');
        }

        private void OnBodyChange(GameEvents.HostedFromToAction<Vessel, CelestialBody> data)
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
            {
                return;
            }

            if ((data.to == null) || (data.to.name == null) || (bodyNames == null))
            {
                return;
            }

            //check whether this is the right body
            if (bodyNames != null)
            {
                bool valid = false;
                for (int i = 0; i < bodyNames.Length; i++)
                {
                    if (bodyNames[i] == data.to.name)
                    {
                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    condition = "Resource not available!";
                }
                else
                {
                    condition = "Nominal";
                }
            }
        }


        /**
         * Prepare the recipe with regard to the amount of crew in this module
         */
        protected override ConversionRecipe PrepareRecipe(double deltatime)
        {
            ConversionRecipe recipe = base.PrepareRecipe(deltatime);

            //check whether this is the right body
            bool valid = false;
            for (int i = 0; i < bodyNames.Length; i++)
            {
                if (bodyNames[i] == vessel.mainBody.name)
                {
                    valid = true;
                    break;
                }
            }

            if (!valid)
            {
                StopResourceConverter();
                condition = recipe.Outputs.ElementAt(0) + "not available!";
                return new ConversionRecipe();
            }
            else
            {
                condition = "Nominal";
            }
            return recipe;
        }

    }
}
