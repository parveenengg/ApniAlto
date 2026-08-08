using UnityEngine;
using System;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Persistent data structure for saving player progression and settings.
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        public int coins = 100000000;
        public int gems = 100000000;
        public float masterVolume = 1.0f;
        public int playerLevel = 5;
        public int playerXP = 750;
        public int gamesPlayed = 0;
    }

    /// <summary>
    /// Persistent SaveManager singleton — reads/writes PlayerSaveData to PlayerPrefs.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private const string SAVE_KEY = "ApniAlto_SaveData_v2";
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
    }
}
