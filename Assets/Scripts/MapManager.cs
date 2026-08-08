using UnityEngine;
using System.Collections.Generic;

namespace VehicleCoinCollector
{
    [System.Serializable]
    public class MapData
    {
        public string id;
        public string name;
        public string environment;
        public string difficulty;
        public int unlockCostCoins;
        public bool defaultUnlocked;
        public Color groundColor;
        public Color fogColor;
    }

    /// <summary>
    /// Map selection and environment setup manager.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        public List<MapData> AvailableMaps = new List<MapData>
        {
            new MapData { id = "green_valley", name = "GREEN VALLEY", environment = "Grassland", difficulty = "EASY", unlockCostCoins = 0, defaultUnlocked = true, groundColor = new Color(0.35f, 0.74f, 0.35f), fogColor = new Color(0.53f, 0.81f, 0.92f) },
            new MapData { id = "desert_run", name = "DESERT RUN", environment = "Desert", difficulty = "MEDIUM", unlockCostCoins = 0, defaultUnlocked = true, groundColor = new Color(0.91f, 0.73f, 0.44f), fogColor = new Color(0.96f, 0.82f, 0.62f) },
            new MapData { id = "coastal_drive", name = "COASTAL DRIVE", environment = "Coastal Track", difficulty = "MEDIUM", unlockCostCoins = 0, defaultUnlocked = true, groundColor = new Color(0.24f, 0.65f, 0.82f), fogColor = new Color(0.44f, 0.82f, 0.95f) },
            new MapData { id = "city_night", name = "CITY NIGHT", environment = "Urban City", difficulty = "HARD", unlockCostCoins = 50000, defaultUnlocked = false, groundColor = new Color(0.12f, 0.15f, 0.22f), fogColor = new Color(0.08f, 0.10f, 0.18f) },
            new MapData { id = "mountain_road", name = "MOUNTAIN ROAD", environment = "Alpine Pass", difficulty = "HARD", unlockCostCoins = 100000, defaultUnlocked = false, groundColor = new Color(0.45f, 0.52f, 0.58f), fogColor = new Color(0.70f, 0.75f, 0.82f) },
            new MapData { id = "winter_pass", name = "WINTER PASS", environment = "Snow & Ice", difficulty = "EXPERT", unlockCostCoins = 250000, defaultUnlocked = false, groundColor = new Color(0.92f, 0.95f, 0.98f), fogColor = new Color(0.85f, 0.92f, 0.98f) }
        };

        public MapData SelectedMap { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SelectedMap = AvailableMaps[0];
        }

        public void SelectMap(string mapId)
        {
            MapData data = AvailableMaps.Find(m => m.id.Equals(mapId, System.StringComparison.OrdinalIgnoreCase));
            if (data != null)
            {
                SelectedMap = data;
                if (RaceManager.Instance != null)
                {
                    RaceManager.Instance.CurrentMapId = mapId;
                }
            }
        }

        public bool UnlockMap(string mapId)
        {
            MapData data = AvailableMaps.Find(m => m.id.Equals(mapId, System.StringComparison.OrdinalIgnoreCase));
            if (data != null && SaveManager.Instance != null)
            {
                if (SaveManager.Instance.DeductCoins(data.unlockCostCoins))
                {
                    SaveManager.Instance.UnlockMap(mapId);
                    return true;
                }
            }
            return false;
        }

        public bool IsMapUnlocked(string mapId)
        {
            MapData data = AvailableMaps.Find(m => m.id.Equals(mapId, System.StringComparison.OrdinalIgnoreCase));
            if (data != null && data.defaultUnlocked) return true;
            return SaveManager.Instance != null && SaveManager.Instance.IsMapUnlocked(mapId);
        }
    }
}
