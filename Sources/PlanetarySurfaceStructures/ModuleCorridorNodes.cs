using System;
using UnityEngine;
using System.Collections.Generic;

namespace PlanetarySurfaceStructures
{
    class ModuleCorridorNodes : PartModule
    {
        //the names of the nodes
        [KSPField]
        public string nodeNames = string.Empty;

        //the names of the transforms
        [KSPField]
        public string transormNames = string.Empty;


        //the names of the nodes for the replaces parts
        [KSPField]
        public string replaceNodeNames = string.Empty;

        //the names of the transforms to be replaced
        [KSPField]
        public string replaceTransformNames = string.Empty;

        //the names of the nodes
        [KSPField]
        public bool allowSurfaceAttach = false;

        //the names of the nodes
        [KSPField]
        public string surfaceAttachNode = string.Empty;

        [KSPField]
        public bool showAllWithNoAttachment = true;

        //the list of the corridors
        private List<CorridorPart> corridors = new List<CorridorPart>();

        //the list of transforms to replace
        List<ReplacedPart> replaceParts = new List<ReplacedPart>();

        //the part that is enabled and disabled
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            string[] nodenames = nodeNames.Split(',');
            string[] transformGroupNames = transormNames.Split(',');

            List<string[]> transformnames = new List<string[]>();

            //when the lengths are not equal
            if (transformGroupNames.Length != nodenames.Length)
                return;

            //----------------------------------------------------------
            //create the list of transforms to be made visible on attach
            //----------------------------------------------------------

            //remove the whitespaces
            for (int i = 0; i < nodenames.Length; i++)
            {
                nodenames[i] = nodenames[i].Trim();

                //split up the names for the nodes
                string[] transformGroup = transformGroupNames[i].Split('|');

                for (int j = 0; j < transformGroup.Length; j++)
                {
                    transformGroup[j] = transformGroup[j].Trim();
                }
                transformnames.Add(transformGroup);
            }

            int num = 0;
            foreach (string nodeName in nodenames)
            {
                AttachNode node = part.findAttachNode(nodeName);

                List<Transform> transforms = new List<Transform>();
                for (int k = 0; k < transformnames[num].Length; k++) {
                    transforms.AddRange(part.FindModelTransforms(transformnames[num][k]));
                }

                //when nodes and transforms are valid and found
                if ((node != null) && (transforms.Count > 0))
                {
                    //create the new corridor data
                    CorridorPart corridor = new CorridorPart();
                    corridor.node = node;
                    corridor.transforms = transforms;

                    if ((allowSurfaceAttach) && (surfaceAttachNode == nodeName) && (part.srfAttachNode != null))
                    {                        
                        corridor.isSurfaceAttachPoint = true;
                    }
                    else
                    {
                        corridor.isSurfaceAttachPoint = false;
                    }

                    corridor.lastAttached = true;
                    corridors.Add(corridor);
                }
                num++;
            }

            //------------------------------------------------------------
            //create the list of transforms to be made invisible on attach
            //------------------------------------------------------------
            string[] replaceTransformGroupNames = null;
            string[] replaceNodeGroupNames = null;
            List<string[]> replacetransformnames = new List<string[]>();
            List<string[]> replacenodenames = new List<string[]>();

            if ((replaceTransformNames != string.Empty) && (replaceNodeNames != string.Empty))
            {
                replaceTransformGroupNames = replaceTransformNames.Split(',');
                replaceNodeGroupNames = replaceNodeNames.Split(',');


                if (replaceNodeGroupNames.Length == replaceTransformGroupNames.Length)
                {

                    //split up all the name for the groups
                    for (int i = 0; i < replaceNodeGroupNames.Length; i++)
                    {
                        //split up the names for the nodes
                        string[] transformGroup = replaceTransformGroupNames[i].Split('|');
                        for (int j = 0; j < transformGroup.Length; j++)
                        {
                            transformGroup[j] = transformGroup[j].Trim();
                        }
                        replacetransformnames.Add(transformGroup);

                        string[] nodeGroup = replaceNodeGroupNames[i].Split('|');
                        for (int j = 0; j < nodeGroup.Length; j++)
                        {
                            nodeGroup[j] = nodeGroup[j].Trim();
                        }
                        replacenodenames.Add(nodeGroup);
                    }


                    //for all transform groups
                    num = 0;
                    foreach (string[] tNames in replacetransformnames)
                    {


                        List<Transform> rTransforms = new List<Transform>();
                        List<AttachNode> rAttachnodes = new List<AttachNode>();

                        foreach (string tName in tNames)
                        {
                            Debug.Log("[KPBS] Replace Tranform Name: " + tName);
                            rTransforms.AddRange(part.FindModelTransforms(tName));
                        }
                        foreach (string nName in replacenodenames[num])
                        {
                            Debug.Log("[KPBS] Replace Node Name: " + nName);
                            rAttachnodes.Add(part.findAttachNode(nName));
                        }

                        if ((rTransforms.Count > 0) && (rAttachnodes.Count > 0))
                        {
                            ReplacedPart rp = new ReplacedPart();
                            rp.nodes = rAttachnodes;
                            rp.transforms = rTransforms;
                            replaceParts.Add(rp);
                        }
                        num++;
                    }
                }
            }          
            //check the visibility of all the parts
            updateAllCorridors();
        }

        public void Update()
        {
            updateAllCorridors();
        }

        //iterate over all nodes to see if it must be connected
        private void updateAllCorridors()
        {
            bool noAttachment = true;
            bool attachmentChanged = false;

            //check all the corridors for changes
            foreach (CorridorPart p in corridors)
            {
                //when the attachment situation has changed
                bool attached = (p.node.attachedPart != null);

                noAttachment = noAttachment && !attached;

                if (p.isSurfaceAttachPoint)
                {
                    attached |= (part.srfAttachNode.attachedPart != null);
                    noAttachment = noAttachment && (part.srfAttachNode.attachedPart == null);
                }

                if (attached != p.lastAttached)
                {
                    attachmentChanged = true;

                    foreach (Transform t in p.transforms)
                    {
                        t.gameObject.SetActive(attached);
                    }
                    p.lastAttached = attached;
                }
            }

            if (attachmentChanged)
            {

                foreach (ReplacedPart rp in replaceParts)
                {
                    bool attached = false;

                    //check all nodes for attachments
                    foreach (AttachNode an in rp.nodes)
                    {
                        attached = attached | (an.attachedPart != null);
                    }

                    //apply state to all transforms
                    foreach (Transform tf in rp.transforms)
                    {
                        tf.gameObject.SetActive(!attached);
                    }
                }
            }

            //make all the nodes visible when none is attached
            if ((showAllWithNoAttachment) && (noAttachment))
            {
                //show all the attach parts
                foreach (CorridorPart p in corridors)
                {
                    foreach (Transform t in p.transforms)
                    {
                        t.gameObject.SetActive(true);             
                    }
                    p.lastAttached = true;
                }

                //hide all the replace parts
                foreach (ReplacedPart p in replaceParts)
                {
                    foreach (Transform t in p.transforms)
                    {
                        t.gameObject.SetActive(false);
                    }
                }
            }
        }

        /**
         * An internal struct that holds the data for the switchable parts
         */
        private class CorridorPart
        {
            public AttachNode node;
            public List<Transform> transforms;
            public bool lastAttached;
            public bool isSurfaceAttachPoint;
        }

        /**
         * An internal struct that holds the data for parts that are replaced
         */
        private class ReplacedPart
        {
            public List<AttachNode> nodes;
            public List<Transform> transforms;
        }
    }
}
