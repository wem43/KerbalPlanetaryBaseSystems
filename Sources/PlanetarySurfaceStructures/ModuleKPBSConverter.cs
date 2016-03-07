using System;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSConverter : ModuleResourceConverter
    {
        [KSPField]
        public float minimalRate = 0.25f;
        [KSPField]
        public float maximalRate = 1.0f;
        [KSPField]
        public float rateStepSize = 0.25f;
		
		
		[KSPField]
        public string changeRateString = "Change conversion rate";
		
		[KSPField]
        public string converterRateName = "Converter rate";
		
        //the production rate
        [KSPField(isPersistant = true)]
        public float currentRate = 1.0f;
		
		

        [KSPField(guiActive = true, guiName = "Conversion Rate", guiActiveEditor = true)]
        public string guiProductionRate = "100%";

        //-----------------------Actions-------------------------
		
		[KSPAction("Change Production Rate")]
        public void ChangeRateAction(KSPActionParam param)
        {
            changeProductionRate();
        }

		
		/**
         * Change the production rate
         */
        [KSPEvent(name = "changeRate", guiName = "Change conversion rate", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void changeRate()
        {
            changeProductionRate();
        }
		
		//set the names of the actions
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["changeRate"].guiName = changeRateString;
            Fields["guiProductionRate"].guiName = converterRateName;
        }
		

        /**
         * Change the rate for the production
         */
        private void changeProductionRate()
        {
            currentRate += rateStepSize;
			
			if (currentRate > maximalRate) {
				currentRate = minimalRate;
			}
            updateRateGUI();
        }

        /**
         * Update the displayed production rate for the greenhouse
         */
        private void updateRateGUI()
        {
            guiProductionRate = (int)(Math.Round(currentRate * 100.0f)) + "%";
        }

        /**
         * Prepare the recipe with regard to the amount of crew in this module
         */
        protected override ConversionRecipe PrepareRecipe(double deltatime)
        {
            double rate = currentRate;

            ConversionRecipe newRecipe = new ConversionRecipe();

			//the amounts (use?)
            newRecipe.FillAmount = 1;
            newRecipe.TakeAmount = 1;
			
            if (ModuleIsActive())
            {
                status = "In Progress";
            }
            else
            {
                status = "Inactive";
            }

            //the amounts (use?)
            newRecipe.FillAmount = 1;// (float)rate;
            newRecipe.TakeAmount = 1;// (float)rate;

            //add the inputs to the recipe
            foreach (ResourceRatio res in inputList)
            {
                ResourceRatio newRes = new ResourceRatio();
                newRes.ResourceName = res.ResourceName;
                newRes.FlowMode = res.FlowMode;
                newRes.Ratio = res.Ratio * rate;
                newRes.DumpExcess = res.DumpExcess;
                newRecipe.Inputs.Add(newRes);
            }
            //add the outputs to the recipe
            foreach (ResourceRatio res in outputList)
            {
                ResourceRatio newRes = new ResourceRatio();
                newRes.ResourceName = res.ResourceName;
                newRes.FlowMode = res.FlowMode;
                newRes.DumpExcess = res.DumpExcess;
                newRes.Ratio = res.Ratio * rate;
                newRecipe.Outputs.Add(newRes);
            }

            //only add the fertilizer as a requirement when it is used
            foreach (ResourceRatio res in reqList)
            {
                ResourceRatio newRes = new ResourceRatio();
                newRes.ResourceName = res.ResourceName;
                newRes.FlowMode = res.FlowMode;
                newRes.DumpExcess = res.DumpExcess;
                newRes.Ratio = res.Ratio * rate;
                newRecipe.Outputs.Add(newRes);
            }
            return newRecipe;
        }
    }
}
