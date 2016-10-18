using System;
using UnityEngine;

namespace PlanetarySurfaceStructures 
{
    class PlanetaryGreenhouse : ModuleResourceConverter
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
        [KSPField]
        public float rateStepSize = 0.25f;
        [KSPField]
        public float fertilizerBenefit = 1.5f;


        //-------the resource names-------

        //the name of the used fertilizer
        [KSPField]
        public string boosterName = "";
        
        //the name of the main input resource
        [KSPField]
        public string boostedOutputName = "";


        //-----------------------Internal members-----------------

        //the production rate
        [KSPField(isPersistant = true)]
        public float currentRate = -1.0f;

        [KSPField(isPersistant = true)]
        public bool useGrowthPromoter = false;

        //the last crew count
        public int numCurrentCrew = 0;

        //quick references to the resources
        private bool fertilizerFound = false;

        //-------------------------GUI------------------------
        
        [KSPField(guiActive = true, guiName = "Greenhouse Status")]
        public string gHstatus = "Operational";

        [KSPField(guiActive = true, guiName = "Max Production Speed")]
        public string effText = "100%";

        [KSPField(guiActive = true, guiName = "Production Speed")]
        public string guiProductionRate = "MAX";

        private float maxProductionRate = 1.0f;
        private float efficiencyFactor = 1.0f;


        //-------------------------BUTTONS--------------------

        [KSPEvent(guiActive = true, guiName = "Use growth promoter")]
        public void ToggleGrowthMode()
        {
            toggleFertilizer();
        }

        [KSPEvent(guiActive = true, guiName = "Increase Production")]
        public void IncreaseRateBtn()
        {
            changeProductionRate(true);
        }

        [KSPEvent(guiActive = true, guiName = "Decrease Production")]
        public void DecreaseRateBtn()
        {
            changeProductionRate(false);
        }

        //-----------------------Actions-------------------------

        [KSPAction("Increase Output")]
        public void IncreaseRateAction(KSPActionParam param)
        {
            changeProductionRate(true);
        }

        [KSPAction("Decrease Output")]
        public void DecreaseRateAction(KSPActionParam param)
        {
            changeProductionRate(false);
        }

        [KSPAction("Toggle growth promoter usage")]
        public void ToggleGrowthAction(KSPActionParam param)
        {
            toggleFertilizer();
        }

        //--------------------Internal functions--------------------

        /**
         * Switch the usage of a groth promoter on or off
         */
        private void toggleFertilizer()
        {
            if (fertilizerFound)
            {
                useGrowthPromoter = !useGrowthPromoter;
                if (useGrowthPromoter)
                {
                    Events["ToggleGrowthMode"].guiName = "Disable growth promoter";
                }
                else
                {
                    Events["ToggleGrowthMode"].guiName = "Use growth promoter";
                }
            }
        }

        /**
         * Change the rate for the production
         */
        private void changeProductionRate(bool increase)
        {
            if (increase)
            {
                //when the max is reached keep at max
                if (currentRate == maxProductionRate)
                {
                    currentRate = -1f;
                }
                else
                {
                    //increase the rate
                    currentRate += rateStepSize;
                    if (currentRate > maxProductionRate)
                    {
                        currentRate = maxProductionRate;
                    }
                }
            }
            else
            {
                if (currentRate < 0.0f)
                {
                    currentRate = maxProductionRate;
                }
                else
                {
                    currentRate -= rateStepSize;

                    if (currentRate < 0.0f)
                    {
                        currentRate = 0.0f;
                    }
                }


            }
            updateRateGUI();
        }

        /**
         * Update the displayed production rate for the greenhouse
         */
        private void updateRateGUI()
        {
            if (currentRate < 0.0f)
            {
                guiProductionRate = "MAX";
            }
            else
            {
                guiProductionRate = (int)(Math.Round(currentRate * 100.0f)) + "%";
            }
        }

        /**
         * Returns true when the number of crew changed in the module
         */
        private bool checkNumCrewChanged()
        {
            if (numCurrentCrew != this.part.protoModuleCrew.Count)
            {
                numCurrentCrew = this.part.protoModuleCrew.Count;
                return true;
            }
            return false;
        }


        //-------------------Overriden functionality----------------

        //the situation is only valid when the minimal crew is in the lab
        public override bool IsSituationValid()
        {
            return (this.part.protoModuleCrew.Count >= minimalCrew);
        }


        /**
         * Prepare the recipe with regard to the amount of crew in this module
         */
        protected override ConversionRecipe PrepareRecipe(double deltatime)
        {
            double rate = currentRate;
            if (rate < 0.0d) {
                rate = maxProductionRate;
            }
            //rate = rate *deltatime * 50;

            ConversionRecipe newRecipe = new ConversionRecipe();

            if (this.ModuleIsActive())
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
            for (int i = 0; i < inputList.Count; i++)
            {
                

                if (useGrowthPromoter || !inputList[i].ResourceName.Equals(boosterName))
                {
                    ResourceRatio newRes = new ResourceRatio();
                    newRes.ResourceName = inputList[i].ResourceName;
                    newRes.FlowMode = inputList[i].FlowMode;
                    newRes.Ratio = inputList[i].Ratio * rate;
                    newRes.DumpExcess = inputList[i].DumpExcess;
                    newRecipe.Inputs.Add(newRes);
                }
            }
            //add the outputs to the recipe
            for (int i = 0; i < outputList.Count; i++)
            {
                ResourceRatio newRes = new ResourceRatio();
                newRes.ResourceName = outputList[i].ResourceName;
                newRes.FlowMode = outputList[i].FlowMode;
                newRes.DumpExcess = outputList[i].DumpExcess;

                //when we have the main resource and the fertilizer active
                if (useGrowthPromoter && outputList[i].ResourceName.Equals(boostedOutputName))
                {
                    newRes.Ratio = outputList[i].Ratio * rate * fertilizerBenefit;
                }
                else
                {
                    newRes.Ratio = outputList[i].Ratio * rate;
                }  

                newRecipe.Outputs.Add(newRes);
            }

            //only add the fertilizer as a requirement when it is used
            for (int i = 0; i < reqList.Count; i++)
            {
                if (reqList[i].ResourceName.Equals(boosterName))
                {
                    if (useGrowthPromoter) {
                        newRecipe.Requirements.Add(reqList[i]);
                    }
                }
                else {
                    newRecipe.Requirements.Add(reqList[i]);
                }
            }

            return newRecipe;
        }

        /**
         * Init the module on start
         */
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            //calculate the factor for the efficiency of the greenhouse
            float diffEfficiency = maximalRate - minimalRate;
            float diffCrew = maximalCrew - minimalCrew;
            if (diffCrew <= 0)
            {
                efficiencyFactor = 0;
            }
            else
            {
                efficiencyFactor = diffEfficiency / diffCrew;
            }

            //set the efficiency based on the crewcount
            setEfficiency();

            effText = ((int)(maxProductionRate * 100.0f)).ToString() + "%";

            if (part.protoModuleCrew.Count < minimalCrew)
            {
                gHstatus = "No Crew! (" + part.protoModuleCrew.Count + "/" + minimalCrew + ")";
            }
            else
            {
                gHstatus = "Operational";
            }

            //init the resources
            initResources();
            updateRateGUI();

            if (!fertilizerFound)
            {
                Events["ToggleGrowthMode"].guiActive = false;
                Debug.Log("[KPBS]Button disabled");
            }
            else
            {
                Events["ToggleGrowthMode"].guiActive = true;
                Debug.Log("[KPBS]Button enabled");
            }
        }

        public void Update()
        {
            //when the number of crew in this part changed
            if (checkNumCrewChanged())
            {
                //set the efficiency based on the crewcount
                setEfficiency();

                effText = ((int)(maxProductionRate * 100.0f)).ToString() + "%";

                if (part.protoModuleCrew.Count < minimalCrew)
                {
                    gHstatus = "No Crew! (" + part.protoModuleCrew.Count + "/" + minimalCrew + ")";
                }
                else
                {
                    gHstatus = "Operational";
                }
            }
        }

        /**
         * Initialize the indices for the the resources
         */
        private void initResources()
        {
            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i].ResourceName.Equals(boosterName))
                {
                    fertilizerFound = true;
                }
            }
        }

        /**
         * Set the efficiency of the conversion depending on the crew available in this module
         */
        private void setEfficiency()
        {
            maxProductionRate = minimalRate + (part.protoModuleCrew.Count - minimalCrew) * efficiencyFactor;
            if (maxProductionRate < 0.0f)
            {
                maxProductionRate = 0.0f;
            }
            else if (maxProductionRate > 1.0f)
            {
                maxProductionRate = 1.0f;
            }

            if (currentRate > maxProductionRate)
            {
                currentRate = maxProductionRate;
                updateRateGUI();
            }
        }
    }
}
