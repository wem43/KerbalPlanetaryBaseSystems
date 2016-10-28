using System;
using SaveUpgradePipeline;
using UnityEngine;

namespace PlanetarySurfaceStructures
{
    [UpgradeModule(LoadContext.SFS | LoadContext.Craft, sfsNodeUrl = "GAME/FLIGHTSTATE/VESSEL",
        craftNodeUrl = "VESSEL")]
    public class SaveFileUpgrader : UpgradeScript
    {
        public override string Name
        {
            get
            {
                Debug.Log("[KPBS] Upgrader: Getting NAME");
                return "Planetary Base Systems Stack-Node Upgrader";
            }
        }

        public override string Description
        {
            get
            {
                Debug.Log("[KPBS] Upgrader: Getting Description");
                return "Upgrades the names of attachment nodes from some KPBS parts to fix a bug that causes the nodes to disappear";
            }
        }

        public override Version EarliestCompatibleVersion
        {
            get
            {
                Debug.Log("[KPBS] Upgrader: EarliestCompatibleVersion");
                return new Version(1, 0, 0);
            }
        }

        public override Version TargetVersion
        {
            get
            {
                Debug.Log("[KPBS] Upgrader: TargetVersion");
                return new Version(1, 1, 0);
            }
        }

        public override TestResult OnTest(ConfigNode node, LoadContext loadContext, ref string nodeName)
        {
            //node.name
            nodeName = NodeUtil.GetPartNodeName(node, loadContext);

            Debug.Log("[KPBS] NodeUtil.GetPartNodeName: " + nodeName + " " + node.name);

            TestResult testResult = TestResult.Pass;
            /*ConfigNode[] nodes = node.GetNodes("PART");
            TestResult result;
            int num;
            for (int i = nodes.Length - 1; i >= 0; i = num)
            {
                string value = nodes[i].GetValue("name");
                string a = value;
                if (a == "ModuleAmpYearPPTRCS" || a == "ModuleAmpYearPoweredRCS")
                {
                    result = TestResult.Upgradeable;
                    return result;
                }
                num = i - 1;
            }*/
            //result = testResult;
            return testResult;
        }

        public override void OnUpgrade(ConfigNode node, LoadContext loadContext)
        {
            Debug.Log("[KPBS] OnUpgrade.GetPartNodeName");

            /*
            ConfigNode[] nodes = node.GetNodes("PART");
            

            int num;
            for (int i = nodes.Length - 1; i >= 0; i = num)
            {
                string value = nodes[i].GetValue("name");
                string a = value;
                if (a == "ModuleAmpYearPPTRCS")
                {
                    nodes[i].SetValue("name", "ModulePPTPoweredRCS", false);
                }
                else
                {
                    if (a == "ModuleAmpYearPoweredRCS")
                    {
                        nodes[i].SetValue("name", "ModuleIONPoweredRCS", false);
                    }
                }
                num = i - 1;
            }*/
        }

    }
}