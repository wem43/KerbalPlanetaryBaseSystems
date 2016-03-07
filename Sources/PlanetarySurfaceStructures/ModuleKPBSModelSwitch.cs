using UnityEngine;
using System.Collections.Generic;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSModelSwitch : PartModule
    {

        //the names of the transforms
        [KSPField]
        public string transormNames = string.Empty;

        //--------------persistent states----------------
        [KSPField(isPersistant = true)]
        public int numModel = 0;

        //the list of models
        List<ModelTransforms> models;

        public bool initialized = false;

        //the part that is enabled and disabled
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            string[] transformGroupNames = transormNames.Split(',');
            models = new List<ModelTransforms>();

            //----------------------------------------------------------
            //create the list of transforms to be made switchable
            //----------------------------------------------------------

            foreach (string transformName in transformGroupNames)
            {
                name = transformName.Trim();

                List<Transform> transforms = new List<Transform>();
                transforms.AddRange(part.FindModelTransforms(name));

                ModelTransforms mt = new ModelTransforms();
                mt.transforms = new List<Transform>();
                mt.transforms.AddRange(transforms);

                models.Add(mt);
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
            updateActiveModel();
        }

        /**
         * Update which model are active or inactive
         */
        private void updateActiveModel()
        {
            for (int i = 0; i < models.Count; i++)
            {
                foreach (Transform tr in models[i].transforms)
                {
                    if (i == numModel)
                    {
                        tr.gameObject.SetActive(true);
                    }
                    else
                    {
                        tr.gameObject.SetActive(false);
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
