using UnityEngine;
using System.Collections.Generic;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSModelSwitch : PartModule
    {

        //the names of the transforms
        [KSPField]
        public string transformNames = string.Empty;

        [KSPField]
        public string transformVisibleNames = string.Empty;

        //--------------persistent states----------------
        [KSPField(isPersistant = true)]
        public int numModel = 0;

        //the list of models
        List<ModelTransforms> models;

        List<string> visibleNames;

        public bool initialized = false;

        //the part that is enabled and disabled
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            string[] transformGroupNames = transformNames.Split(',');
            string[] transformGroupVisibleNames = transformVisibleNames.Split(',');
            models = new List<ModelTransforms>();
            visibleNames = new List<string>();

            //----------------------------------------------------------
            //create the list of transforms to be made switchable
            //----------------------------------------------------------
            int i = 0;
            //foreach (string transformName in transformGroupNames)
            for (int k = 0; i < transformGroupNames.Length; i++)
            {
                name = transformGroupNames[k].Trim();

                List<Transform> transforms = new List<Transform>();
                transforms.AddRange(part.FindModelTransforms(name));

                ModelTransforms mt = new ModelTransforms();
                mt.transforms = new List<Transform>();
                mt.transforms.AddRange(transforms);

                models.Add(mt);
                if (transformGroupVisibleNames.Length == transformGroupNames.Length) {
                    visibleNames.Add(transformGroupVisibleNames[i]);
                }
                else
                {
                    visibleNames.Add(transformGroupNames[i]);
                }
                i++;
            }

            if (models.Count < 2)
            {
                Events["toggleModel"].active = false;
                Events["toggleModelNext"].active = false;
                Events["toggleModelPrev"].active = false;
            }
            else if (models.Count == 2)
            {
                Events["toggleModel"].active = true;
                Events["toggleModel"].guiName = "Switch to: " + getName(numModel + 1);
                Events["toggleModelNext"].active = false;
                Events["toggleModelPrev"].active = false;
            }
            else if (models.Count > 2)
            {
                Events["toggleModel"].active = false;
                Events["toggleModelNext"].active = true;
                Events["toggleModelNext"].guiName = "Next: " + getName(numModel + 1);
                Events["toggleModelPrev"].active = true;
                Events["toggleModelPrev"].guiName = "Previous: " + getName(numModel - 1);
            }
            


            if (!HighLogic.LoadedSceneIsEditor)
            {
                Events["toggleModel"].guiActive = false;
            }
            updateActiveModel();
        }

        /**
         * The update method of the module
         */
        public void Update()
        {
            if (!initialized)
            {
                updateActiveModel();
                initialized = true;
            }
        }

        /**
         * Toggle the visible part
         */
        [KSPEvent(name = "toggleModel", guiName = "Switch Model", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void toggleModel()
        {
            numModel++;
            if (numModel >= models.Count)
            {
                numModel = 0;
            }
            Events["toggleModel"].guiName = "Switch to: " + getName(numModel + 1);
            updateActiveModel();
        }

        /**
         * Toggle the visible part
         */
        [KSPEvent(name = "toggleModelNext", guiName = "Next: ", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void toggleModelNext()
        {
            numModel++;
            if (numModel >= models.Count)
            {
                numModel = 0;
            }
            Events["toggleModelNext"].guiName = "Next: " + getName(numModel + 1);
            Events["toggleModelPrev"].guiName = "Previous: " + getName(numModel - 1);
            updateActiveModel();
        }

        /**
         * Toggle the visible part
         */
        [KSPEvent(name = "toggleModelPrev", guiName = "Previous: ", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void toggleModelPrev()
        {
            numModel--;
            if (numModel < 0)
            {
                numModel = models.Count-1;
            }
            Events["toggleModelNext"].guiName = "Next: " + getName(numModel + 1);
            Events["toggleModelPrev"].guiName = "Previous: " + getName(numModel - 1);
            updateActiveModel();
        }

        private string getName(int index)
        {
            if ((visibleNames.Count > 0) && (visibleNames.Count == models.Count))
            {
                if (index < 0)
                {
                    index += visibleNames.Count;
                }
                else if (index >= models.Count)
                {
                    index -= visibleNames.Count;
                }

                return visibleNames[index];
            }
            return "";
        }


        /**
         * Update which model are active or inactive
         */
        private void updateActiveModel()
        {
            for (int i = 0; i < models.Count; i++)
            {
                //foreach (Transform tr in models[i].transforms)
                for (int j = 0; j < models[i].transforms.Count; j++)
                {
                    if (i == numModel)
                    {
                        models[i].transforms[j].gameObject.SetActive(true);
                    }
                    else
                    {
                        models[i].transforms[j].gameObject.SetActive(false);
                    }
                }
            }
        }



        /**
         * An internal struct that holds the data for the switchable parts
         */
        private class ModelTransforms
        {
            public List<Transform> transforms;
        }
    }
}
