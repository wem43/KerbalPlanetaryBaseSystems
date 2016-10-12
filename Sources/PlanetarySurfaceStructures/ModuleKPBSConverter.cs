using System;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSConverter : ModuleResourceConverter
    {
        //the minimal rate of the converter
        [KSPField]
        public float minimalRate = 0.25f;

        //the maximal rate of the converter
        [KSPField]
        public float maximalRate = 1.0f;

        //the size of the steps between min and max
        [KSPField]
        public float rateStepSize = 0.25f;
		
		//the string to display to change the conversion rate
		[KSPField]
        public string changeRateString = "Change conversion rate";
		
        //the name of the converter rate
		[KSPField]
        public string converterRateName = "Converter rate";
		
        //the production rate
        [KSPField(isPersistant = true)]
        public float currentRate = 1.0f;
		
		
        //the production rate that is displayed to the user
        [KSPField(guiActive = true, guiName = "Conversion Rate", guiActiveEditor = true)]
        public string guiProductionRate = "100%";

        //-----------------------Actions-------------------------

        /**
         * Change the production rate
         */
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
		
		/**
          *set the names of the actions
          **/
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            updateRateGUI();

            Events["changeRate"].guiName = changeRateString;
            Fields["guiProductionRate"].guiName = converterRateName;
        }
		
        // Change the rate for the production
        private void changeProductionRate()
        {
            currentRate += rateStepSize;
			
			if (currentRate > maximalRate) {
				currentRate = minimalRate;
			}
            updateRateGUI();
        }

        // Update the displayed production rate for the greenhouse
        private void updateRateGUI()
        {
            guiProductionRate = (int)(Math.Round(currentRate * 100.0f)) + "%";
        }

        // Prepare the recipe with regard to the amount of crew in this module
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
            newRecipe.FillAmount = 1;
            newRecipe.TakeAmount = 1;

            //add the inputs to the recipe
            for (int i = 0; i < inputList.Count; i++)
            {
                ResourceRatio newRes = new ResourceRatio();
                newRes.ResourceName = inputList[i].ResourceName;
                newRes.FlowMode = inputList[i].FlowMode;
                newRes.Ratio = inputList[i].Ratio * rate;
                newRes.DumpExcess = inputList[i].DumpExcess;
                newRecipe.Inputs.Add(newRes);
            }
            //add the outputs to the recipe
            for (int i = 0; i < outputList.Count; i++)
            {
                ResourceRatio newRes = new ResourceRatio();
                newRes.ResourceName = outputList[i].ResourceName;
                newRes.FlowMode = outputList[i].FlowMode;
                newRes.Ratio = outputList[i].Ratio * rate;
                newRes.DumpExcess = outputList[i].DumpExcess;
                newRecipe.Outputs.Add(newRes);
            }
            //only add the fertilizer as a requirement when it is used
            for (int i = 0; i < reqList.Count; i++)
            {
                ResourceRatio newRes = new ResourceRatio();
                newRes.ResourceName = reqList[i].ResourceName;
                newRes.FlowMode = reqList[i].FlowMode;
                newRes.Ratio = reqList[i].Ratio * rate;
                newRes.DumpExcess = outputList[i].DumpExcess;
                newRecipe.Requirements.Add(newRes);
            }
            return newRecipe;
        }
    }
}
