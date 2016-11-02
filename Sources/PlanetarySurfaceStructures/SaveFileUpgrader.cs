using System;
using SaveUpgradePipeline;

namespace PlanetarySurfaceStructures
{
    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME", craftNodeUrl = "PART")]
    public class SaveFileUpgrader : UpgradeScript
    {
        ///FLIGHTSTATE/VESSEL/PART"

        //visible name of the upgrader
        public override string Name
        {
            get
            {
                return "KPBS Upgrader_1.3.3";
            }
        }

        //description for the upgrade process 
        public override string Description
        {
            get
            {
                return "Renamed attachment nodes to fix disappearing nodes";
            }
        }

        //check the maximal version
        protected override bool CheckMaxVersion(Version v)
        {
            return v <= TargetVersion;
        }

        //get the earliest version that can be upgraded
        public override Version EarliestCompatibleVersion
        {
            get
            {
                return new Version(1, 0, 4);
            }
        }

        //get the target version of the save file
        public override Version TargetVersion
        {
            get
            {
                return new Version(1, 2, 1);
            }
        }

        //test the save file for upgrades
        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName)
        {
            
            //when the parts of a craft should be checked
            if (loadContext == LoadContext.Craft)
            {
                TestResult tr = checkPart(node, loadContext);
                return tr;
            }
            //when the savefile should be updated
            else if (loadContext == LoadContext.SFS)
            {
                //iterate over all vessels in the savefile
                ConfigNode[] vessels = node.GetNode("FLIGHTSTATE").GetNodes("VESSEL");
                for (int i = 0; i < vessels.Length; i++)
                {
                    //iterate of all parts in the vessel
                    ConfigNode[] parts = vessels[i].GetNodes("PART");
                    for (int j = 0; j < parts.Length; j++)
                    {
                        if (checkPart(parts[j], loadContext) == TestResult.Upgradeable)
                        {
                            return TestResult.Upgradeable;
                        }
                    }
                }
                return TestResult.Pass;
            }
            return TestResult.Pass;
        }

        //upgrade the save file
        public override void OnUpgrade(ConfigNode node, LoadContext loadContext)
        {
            //when the part of a craft should be upgraded
            if (loadContext == LoadContext.Craft)
            {
                upgradePart(node, loadContext);
            }
            else if (loadContext == LoadContext.SFS)
            {
                string partName = NodeUtil.GetPartNodeName(node, loadContext);
                
                //iterate over all vessels in the savefile
                ConfigNode[] vessels = node.GetNode("FLIGHTSTATE").GetNodes("VESSEL");
                for (int i = 0; i < vessels.Length; i++)
                {
                    //iterate of all parts in the vessel
                    ConfigNode[] parts = vessels[i].GetNodes("PART");
                    for (int j = 0; j < parts.Length; j++)
                    {
                        upgradePart(parts[j], loadContext);
                    }
                }
            }
        }

        //Check of the parts has to be upgraded
        private TestResult checkPart(ConfigNode part, LoadContext loadContext)
        {
            string partName = NodeUtil.GetPartNodeName(part, loadContext).Split('_')[0];
            string[] attachementNodes = part.GetValues("attN");
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


        //Upgrade the part
        private void upgradePart(ConfigNode part, LoadContext loadContext)
        {
            string partName = NodeUtil.GetPartNodeName(part, loadContext).Split('_')[0];
            string[] attachementNodes = part.GetValues("attN");

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
                            part.SetValue("attN", "left" + left + "," + values[1], i, false);
                            left++;
                        }
                        else if (values[0] == "right")
                        {
                            part.SetValue("attN", "right" + right + "," + values[1], i, false);
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
                            part.SetValue("attN", "leg" + leg + "," + values[1], i, false);
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
                            if (inner == 1)
                            {
                                part.SetValue("attN", "innerbottom" + "," + values[1], i, false);
                            }
                            else
                            {
                                part.SetValue("attN", "innertop" + "," + values[1], i, false);
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
                            part.SetValue("attN", "front" + front + "," + values[1], i, false);
                            front++;
                        }
                    }
                    break;
            }
        }
    }
}