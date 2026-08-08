using UnityEngine;
using System;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Persistent data structure for saving player progression, settings, currency, and best times.
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        public int coins = 100000000;
        public int gems = 100000000;
        public string selectedCarId = "mustang";
        public string ownedCarIds = "mustang,verna,lamborghini,alto";
        public string unlockedMapIds = "green_valley,desert_run,coastal_drive";
        
        // Upgrades stored as "carId_stat" -> level
        public int speedUpgradeLevel = 0;
        public int accelUpgradeLevel = 0;
        public int handlingUpgradeLevel = 0;
        public int brakingUpgradeLevel = 0;
        public string carColorHex = "#D97706";

        // Settings
        public int controlType = 0; // 0 = Steering Wheel, 1 = Buttons, 2 = Tilt
        public float sensitivity = 0.7f;
        public bool vibrationOn = true;
        public bool autoAccelerateOn = false;
        public float masterVolume = 1.0f;

        // Progression & Stats
        public int playerLevel = 5;
        public int playerXP = 750;
        public int gamesPlayed = 12;
        public float bestTimeCheckpoints = 42.3f;
        public float bestTimeSprint = 35.8f;
    }

    /// <summary>
    /// Persistent SaveManager singleton that reads/writes PlayerSaveData to PlayerPrefs.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SAVE_KEY = "ApniAlto_SaveData_v1";
        public PlayerSaveData Data { get; private set; } = new PlayerSaveData();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSaveData();
        }

        public void LoadSaveData()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                try
                {
                    string json = PlayerPrefs.GetString(SAVE_KEY);
                    Data = JsonUtility.FromJson<PlayerSaveData>(json) ?? new PlayerSaveData();
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[SaveManager] Error loading save data, using defaults: " + e.Message);
                    Data = new PlayerSaveData();
                }
            }
            else
            {
                Data = new PlayerSaveData();
                Save();
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(Data);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError("[SaveManager] Failed to save data: " + e.Message);
            }
        }

        public bool IsCarOwned(string carId)
        {
            if (string.IsNullOrEmpty(Data.ownedCarIds)) return false;
            string[] owned = Data.ownedCarIds.Split(',');
            return Array.Exists(owned, id => id.Trim().Equals(carId, StringComparison.OrdinalIgnoreCase));
        }

        public void UnlockCar(string carId)
        {
            if (!IsCarOwned(carId))
            {
                Data.ownedCarIds += "," + carId;
                Save();
            }
        }

        public bool IsMapUnlocked(string mapId)
        {
            if (string.IsNullOrEmpty(Data.unlockedMapIds)) return false;
            string[] unlocked = Data.unlockedMapIds.Split(',');
            return Array.Exists(unlocked, id => id.Trim().Equals(mapId, StringComparison.OrdinalIgnoreCase));
        }

        public void UnlockMap(string mapId)
        {
            if (!IsMapUnlocked(mapId))
            {
                Data.unlockedMapIds += "," + mapId;
                Save();
            }
        }

        public void AddCoins(int amount)
        {
            Data.coins += amount;
            Save();
        }

        public void AddGems(int amount)
        {
            Data.gems += amount;
            Save();
        }

        public bool DeductCoins(int amount)
        {
            if (Data.coins >= amount)
            {
                Data.coins -= amount;
                Save();
                return true;
            }
            return false;
        }

        public bool DeductGems(int amount)
        {
            if (Data.gems >= amount)
            {
                Data.gems -= amount;
                Save();
                return true;
            }
            return false;
        }
    }
}
