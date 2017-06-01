using UnityEngine;
using System.Collections.Generic;
using System.Text;
using KSP.Localization;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSModelSwitch : PartModule, IModuleInfo
    {

        //the names of the transforms
        [KSPField]
        public string transformNames = string.Empty;

        [KSPField]
        public string transformVisibleNames = string.Empty;

        //--------------persistent states----------------
        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "#LOC_KPBS.modelswitch.model")]
        [UI_ChooseOption(scene = UI_Scene.Editor)]
        public int numModel = 0;
        private int oldModelNum = -1;

        //the list of models
        List<ModelTransforms> models;

        List<string> visibleNames;

        public bool initialized = false;

        BaseField modelBaseField;
        UI_ChooseOption modelUIChooser;

        //the part that is enabled and disabled
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            modelBaseField = Fields["numModel"];
            modelUIChooser = (UI_ChooseOption)modelBaseField.uiControlEditor;

            string[] transformGroupNames = transformNames.Split(',');
            string[] transformGroupVisibleNames = transformVisibleNames.Split(',');
            models = new List<ModelTransforms>();
            visibleNames = new List<string>();

            //----------------------------------------------------------
            //create the list of transforms to be made switchable
            //----------------------------------------------------------
            for (int k = 0; k < transformGroupNames.Length; k++)
            {
                name = transformGroupNames[k].Trim();

                List<Transform> transforms = new List<Transform>();
                transforms.AddRange(part.FindModelTransforms(name));

                ModelTransforms mt = new ModelTransforms();
                mt.transforms = new List<Transform>();
                mt.transforms.AddRange(transforms);

                models.Add(mt);
                if (transformGroupVisibleNames.Length == transformGroupNames.Length) {
                    visibleNames.Add(transformGroupVisibleNames[k]);
                }
                else
                {
                    visibleNames.Add(transformGroupNames[k]);
                }


            }

            //set the changes for the modelchooser
            modelUIChooser.options = visibleNames.ToArray();

            //when there is only one model, we do not need to show the controls
            if (models.Count < 2)
            {
                modelBaseField.guiActive = false;
                modelBaseField.guiActiveEditor = false;
            }

            updateActiveModel();
        }

        /**
         * The update method of the module
         */
        public void Update()
        {
            if (oldModelNum != numModel)
            {
                updateActiveModel();
            }
        }

        // Update which model are active or inactive
        private void updateActiveModel()
        {
            for (int i = 0; i < models.Count; i++)
            {
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


        /// <summary>
        /// Get the description shown for this resource 
        /// </summary>
        /// <returns>The description of the module</returns>
        public override string GetInfo()
        {
            StringBuilder info = new StringBuilder();

            string[] transformGroupNames = transformNames.Split(',');
            string[] transformGroupVisibleNames = transformVisibleNames.Split(',');
            visibleNames = new List<string>();

            //----------------------------------------------------------
            //create the list of transforms to be made switchable
            //----------------------------------------------------------
            for (int k = 0; k < transformGroupNames.Length; k++)
            {
                if (transformGroupVisibleNames.Length == transformGroupNames.Length)
                {
                    visibleNames.Add(transformGroupVisibleNames[k]);
                }
                else
                {
                    visibleNames.Add(transformGroupNames[k]);
                }
            }

            if (visibleNames.Count > 1)
            {
                info.AppendLine(Localizer.GetStringByTag("#LOC_KPBS.modelswitch.models"));
                info.AppendLine();

                for(int i = 0; i < visibleNames.Count; i++)
                {
                    info.Append("• ");
                    info.Append(visibleNames[i]);
                    info.AppendLine();
                }
            }
            return info.ToString();
        }

        public string GetModuleTitle()
        {
            return Localizer.GetStringByTag("#LOC_KPBS.modelswitch.name");// "Model Switch";
        }

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }

        public string GetPrimaryField()
        {
            return null;
        }

        // An internal struct that holds the data for the switchable parts
        private class ModelTransforms
        {
            public List<Transform> transforms;
        }
    }
}
