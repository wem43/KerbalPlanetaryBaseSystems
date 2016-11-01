using System;
using SaveUpgradePipeline;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/FLIGHTSTATE/VESSEL/PART", craftNodeUrl = "PART")]
    public class SaveFileUpgrader : UpgradeScript
    {
        bool DEBUG = false;

        public override string Name
        {
            get
            {
                return "Planetary Base Systems Stack-Node Upgrader";
            }
        }

        public override string Description
        {
            get
            {
                return "Upgrades the names of attachment nodes from some KPBS parts to fix a bug that causes the nodes to disappear";
            }
        }

        protected override bool CheckMaxVersion(Version v)
        {
            return v <= TargetVersion;
        }

        public override Version EarliestCompatibleVersion
        {
            get
            {
                return new Version(1, 0, 4);
            }
        }

        public override Version TargetVersion
        {
            get
            {
                return new Version(1, 2, 0);
            }
        }

        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName)
        {
            string partName = NodeUtil.GetPartNodeName(node, loadContext).Split('_')[0];
            string[] attachementNodes = node.GetValues("attN");
            switch (partName)
            {
                case "KKAOSS.Storage.g":
                case "KKAOSS.Storage.mid.g":
                case "KKAOSS.Storage.size2.m":
                case "KKAOSS.Storage.size2.s":
                    for (int j = 0; j < attachementNodes.Length; j++)
                    {
                        string[] values = attachementNodes[j].Split(',');
                        if ((values[0] == "left") || (values[0] == "right"))
                        {
                            return TestResult.Upgradeable;
                        }
                    }
                    break;
                case "KKAOSS.Service.g":
                    for (int j = 0; j < attachementNodes.Length; j++)
                    {
                        string[] values = attachementNodes[j].Split(',');
                        if (values[0] == "inner")
                        {
                            return TestResult.Upgradeable;
                        }
                    }
                    break;
                case "KKAOSS.gangway.2.adapter":
                case "KKAOSS.garage.front.g.2":
                case "KKAOSS.garage.struct.g.2":
                    for (int j = 0; j < attachementNodes.Length; j++)
                    {
                        string[] values = attachementNodes[j].Split(',');
                        if (values[0].StartsWith("node_leg"))
                        {
                            return TestResult.Upgradeable;
                        }
                    }
                    break;
                case "KKAOSS.container.SEP":
                    for (int j = 0; j < attachementNodes.Length; j++)
                    {
                        string[] values = attachementNodes[j].Split(',');
                        if (values[0] == "front")
                        {
                            return TestResult.Upgradeable;
                        }
                    }
                    break;  
            }
            return TestResult.Pass;
        }

        public override void OnUpgrade(ConfigNode node, LoadContext loadContext)
        {
            string partName = NodeUtil.GetPartNodeName(node, loadContext).Split('_')[0];
            string[] attachementNodes = node.GetValues("attN");

            switch (partName)
            {
                case "KKAOSS.Storage.g":
                case "KKAOSS.Storage.mid.g":
                case "KKAOSS.Storage.size2.m":
                case "KKAOSS.Storage.size2.s":
                    int left = 1;
                    int right = 1;
                    for (int i = 0; i < attachementNodes.Length; i++)
                    {
                        string[] values = attachementNodes[i].Split(',');
                        if (values[0] == "left")
                        {
                            if (DEBUG)
                            {
                                Debug.Log("[KPBS] replacing left node: " + left);
                            }
                            node.SetValue("attN", "left" + left + "," + values[1], i, false);
                            left++;
                        }
                        else if (values [0] == "right")
                        {
                            if (DEBUG)
                            {
                                Debug.Log("[KPBS] replacing right node: " + left);
                            }
                            node.SetValue("attN", "right" + right + "," + values[1], i, false);
                            right++;
                        }
                    }
                    break;
                case "KKAOSS.garage.front.g.2":
                case "KKAOSS.garage.struct.g.2":
                case "KKAOSS.gangway.2.adapter":
                    int leg = 1;
                    for (int i = 0; i < attachementNodes.Length; i++)
                    {
                        string[] values = attachementNodes[i].Split(',');
                        if (values[0].StartsWith("node_leg"))
                        {
                            if (DEBUG)
                            {
                                Debug.Log("[KPBS] replacing leg: " + leg);
                            }
                            node.SetValue("attN", "leg" + leg + "," + values[1], i, false);
                            leg++;
                        }
                    }
                    break;
                case "KKAOSS.Service.g":
                    int inner = 1;
                    for (int i = 0; i < attachementNodes.Length; i++)
                    {
                        string[] values = attachementNodes[i].Split(',');
                        if (values[0] == "inner")
                        {
                            if (DEBUG)
                            {
                                Debug.Log("[KPBS] replacing inner: " + inner);
                            }
                            if (inner == 1)
                            {
                                node.SetValue("attN", "innerbottom" + "," + values[1], i, false);
                            }
                            else
                            {
                                node.SetValue("attN", "innertop" + "," + values[1], i, false);
                            }
                            inner++;
                        }
                    }
                    break;
                case "KKAOSS.container.SEP":
                    int front = 1;
                    for (int i = 0; i < attachementNodes.Length; i++)
                    {
                        string[] values = attachementNodes[i].Split(',');
                        if (values[0] == "front")
                        {
                            if (DEBUG)
                            {
                                Debug.Log("[KPBS] replacing front: " + front);
                            }
                            node.SetValue("attN", "front" + front + "," + values[1], i, false);
                            front++;
                        }
                    }
                    break;
            }
        }
    }
}