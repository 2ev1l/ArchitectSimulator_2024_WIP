using UnityEngine;

namespace Game.Serialization.World
{
    [System.Serializable]
    public class GameData
    {
        #region fields & properties
        public static readonly string SaveName = "save";
        public static readonly string SaveExtension = ".data";

        public static GameData Data => data;
        private static GameData data;

        public PlayerData PlayerData => playerData;
        [SerializeField] private PlayerData playerData = new();
        public CompanyData CompanyData => companyData;
        [SerializeField] private CompanyData companyData = new();

        public BrowserData BrowserData => browserData;
        [SerializeField] private BrowserData browserData = new();
        public LocationsData LocationsData => locationsData;
        [SerializeField] private LocationsData locationsData = new();
        public EnvironmentData EnvironmentData => environmentData;
        [SerializeField] private EnvironmentData environmentData = new();
        
        public BlueprintsData BlueprintsData => blueprintsData;
        [SerializeField] private BlueprintsData blueprintsData = new();
        public ConstructionsData ConstructionsData => constructionsData;
        [SerializeField] private ConstructionsData constructionsData = new();
        #endregion fields & properties

        #region methods
        public static void SetData(GameData value) => data = value ?? new GameData();
        #endregion methods
    }
}