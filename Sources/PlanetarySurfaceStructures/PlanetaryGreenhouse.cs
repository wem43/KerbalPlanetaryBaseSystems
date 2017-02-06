using System.Text;
using UnityEngine;

namespace PlanetarySurfaceStructures 
{
    class PlanetaryGreenhouse : ModuleResourceConverter, IModuleInfo
    {
        //------------------------SETTINGS------------------------

        //-------minimal and maximal crews-------
        [KSPField]
        public int minimalCrew = 1;
        [KSPField]
        public int maximalCrew = 2;

        //-------the production rates-------
        [KSPField]
        public float minimalRate = 0.5f;
        [KSPField]
        public float maximalRate = 1.0f;


        //-----------------------Internal members-----------------

        //the last crew count
        private int numCurrentCrew = 0;

        //-------------------------GUI------------------------
        
        [KSPField(guiActive = true, guiName = "Greenhouse Status")]
        public string gHstatus = "Operational";

        private float productionRateModifier = 1.0f;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = false, guiName = "Production Speed", guiUnits = "%"), UI_FloatRange(minValue = 10f, maxValue = 100f, stepIncrement = 10f)]
        public  float productionSpeed = 100;


        /**
         * Returns true when the number of crew changed in the module
         */
        private bool checkNumCrewChanged()
        {
            if (numCurrentCrew != part.protoModuleCrew.Count)
            {
                numCurrentCrew = part.protoModuleCrew.Count;
                return true;
            }
            return false;
        }


        //-------------------Overriden functionality----------------

        //the situation is only valid when the minimal crew is in the lab
        public override bool IsSituationValid()
        {
            return (part.protoModuleCrew.Count >= minimalCrew);
        }


        /**
         * Prepare the recipe with regard to the amount of crew in this module
         */
        protected override ConversionRecipe PrepareRecipe(double deltatime)
        {
            ConversionRecipe recipe = base.PrepareRecipe(deltatime);

            if (recipe != null)
            {
                for (int i = 0; i < recipe.Inputs.Count; i++)
                {
                    ResourceRatio res = recipe.Inputs[i];
                    res.Ratio = inputList[i].Ratio * (productionSpeed / 100.0f) * productionRateModifier;
                    recipe.Inputs[i] = res;
                }
                //change the rate of the outputs
                for (int i = 0; i < recipe.Outputs.Count; i++)
                {
                    ResourceRatio res = recipe.Outputs[i];
                    res.Ratio = outputList[i].Ratio * (productionSpeed / 100.0f) * productionRateModifier;
                    recipe.Outputs[i] = res;
                }
                //change the value of the requirements
                for (int i = 0; i < recipe.Requirements.Count; i++)
                {
                    ResourceRatio res = recipe.Requirements[i];
                    res.Ratio = reqList[i].Ratio * (productionSpeed / 100.0f) * productionRateModifier;
                    recipe.Requirements[i] = res;
                }
            }

            return recipe;
        }

        /**
         * Init the module on start
         */
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            updateEfficiency();

            string stateOperational = ((int)(productionRateModifier * 100.0f)).ToString() + "%";

            if (part.protoModuleCrew.Count < minimalCrew)
            {
                gHstatus = "No Crew! (" + part.protoModuleCrew.Count + "/" + minimalCrew + ")";
            }
            else
            {
                gHstatus = stateOperational + " Operational";
            }
        }

        public void Update()
        {

            //when the number of crew in this part changed
            if (checkNumCrewChanged())
            {
                //set the efficiency based on the crewcount
                updateEfficiency();

                string eff = ((int)(productionRateModifier * 100.0f)).ToString() + "%";

                if (part.protoModuleCrew.Count < minimalCrew)
                {
                    gHstatus = "No Crew! (" + part.protoModuleCrew.Count + "/" + minimalCrew + ")";
                }
                else
                {
                    gHstatus = eff + " Operational";
                }
            }
        }

        /**
         * Set the efficiency of the conversion depending on the crew available in this module
         */
        private void updateEfficiency()
        {
            int crewRange = maximalCrew - minimalCrew;
            float efficiencyRange = maximalRate - minimalRate;
            float step = efficiencyRange / crewRange;
            productionRateModifier = minimalRate + ((part.protoModuleCrew.Count - minimalCrew)* step);

            if (productionRateModifier < 0f)
            {
                productionRateModifier = 0f;
            }
            else if (productionRateModifier > 1f)
            {
                productionRateModifier = 1f;
            }
        }


        public override string GetInfo()
        {
            string baseString = base.GetInfo();
            //get the lines of the 
            string[] lines = baseString.Split('\n');

            StringBuilder info = new StringBuilder();

            info.AppendLine(lines[0]);
            info.AppendLine();
            info.AppendLine("<b>Minimum Crew to Operate:</b>");
            info.AppendLine("  " + minimalCrew + " at " + ((int)(minimalRate * 100.0f)).ToString() + "% Efficiency" );
            info.AppendLine("<b>Maximal Efficieny with: </b>");
            info.AppendLine("  " + maximalCrew + " Kerbals");

            for (int i = 1; i < lines.Length; i++)
            {
                info.AppendLine(lines[i]);
            }

            return info.ToString();
        }

        public string GetModuleTitle()
        {
            return "Resource Converter";
        }

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }

        public string GetPrimaryField()
        {
            return null;
        }
    }
}
