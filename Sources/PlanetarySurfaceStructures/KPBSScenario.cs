namespace PlanetarySurfaceStructures
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER)]
    class KPBSScenario : ScenarioModule
    {
        public override void OnLoad(ConfigNode gameNode)
        {
            base.OnLoad(gameNode);

            //Apply the filter settings for the function filter
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                if (SurfaceStructuresCategoryFilter.Instance != null)
                {
                    SurfaceStructuresCategoryFilter.Instance.updateFilterSettings();
                }
            }
        }

    }
}
