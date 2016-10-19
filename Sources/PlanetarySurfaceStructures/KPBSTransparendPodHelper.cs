using System.Collections.Generic;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    class KPBSTransparendPodHelper : PartModule
    {
        //the name of the additional internal
        [KSPField]
        public string hiddenOverlayTransformNames = string.Empty;

        //the transform for an additional depthmask
        //private Transform hiddenOverlayTransform;

        private string[] trasformNames;

        private List<Transform> transforms;

        /**
         * Called at the start. Initialize animation and state of this module
         */
        public override void OnStart(PartModule.StartState state)
        {
            if (hiddenOverlayTransformNames != string.Empty)
            {
                transforms = new List<Transform>();
                trasformNames = hiddenOverlayTransformNames.Split('|');
                int numNames = trasformNames.Length;

                for (int i = 0; i < numNames; i++)
                {
                    if (trasformNames[i] != string.Empty)
                    {
                        Transform newTransform = part.internalModel.FindModelTransform(trasformNames[i]);
                        if (newTransform != null)
                        {
                            transforms.Add(newTransform);
                        }
                    }
                }
            }
        }


        /**
         * The update method of the module
         */
        public void Update()
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
            {
                return;
            }

            if ((transforms == null) && (hiddenOverlayTransformNames != string.Empty))
            {
                transforms = new List<Transform>();
                trasformNames = hiddenOverlayTransformNames.Split('|');
                int numNames = trasformNames.Length;

                for (int i = 0; i < numNames; i++)
                {
                    if (trasformNames[i] != string.Empty)
                    {
                        Transform newTransform = part.internalModel.FindModelTransform(trasformNames[i]);
                        if (newTransform != null)
                        {
                            transforms.Add(newTransform);
                        }
                    }
                }
            }

            if ((transforms != null) && (transforms.Count > 0)) {
                int numTransforms = transforms.Count;
                for (int i = 0; i < numTransforms; i++)
                {
                    if (HighLogic.LoadedSceneIsEditor)
                    {
                        if ((transforms[i].gameObject.activeSelf))
                        {
                            transforms[i].gameObject.SetActive(false);
                        }
                    }
                    else if ((isStockOverlayActive()) && (transforms[i].gameObject.activeSelf))
                    {
                        transforms[i].gameObject.SetActive(false);
                    }
                    else if ((!isStockOverlayActive()) && (!transforms[i].gameObject.activeSelf))
                    {
                        transforms[i].gameObject.SetActive(true);
                    }
                }
            }
        }

        /**
         * When the module is destroyed
         */
        public void OnDestroy()
        {
            if ((transforms != null) && (transforms.Count > 0))
            {
                int numTransforms = transforms.Count;
                for (int i = 0; i < numTransforms; i++)
                {
                    transforms[i].gameObject.SetActive(true);
                }
                transforms = null;
            }
        }

        //find a camera by its name
        private bool isStockOverlayActive()
        {
            int count = Camera.allCamerasCount;
            for (int i = 0; i < count; ++i)
            {
                //when the camera for the overly exists return true
                if (Camera.allCameras[i].name.Equals("InternalSpaceOverlay Host"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
