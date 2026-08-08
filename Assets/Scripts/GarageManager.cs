using UnityEngine;
using System.Collections.Generic;

namespace VehicleCoinCollector
{
    [System.Serializable]
    public class VehicleData
    {
        public string id;
        public string name;
        public string rarity;
        public int costCoins;
        public bool isUnlocked;
        public float topSpeed;      // Base 1-10 scale
        public float acceleration;  // Base 1-10 scale
        public float handling;      // Base 1-10 scale
        public float braking;       // Base 1-10 scale
        public Color bodyColor;
        public Color roofColor;
    }

    /// <summary>
    /// Garage & Vehicle Selection Manager with stat upgrade support (+5% for coins).
    /// </summary>
    public class GarageManager : MonoBehaviour
    {
        public static GarageManager Instance { get; private set; }

        public List<VehicleData> Vehicles = new List<VehicleData>
        {
            new VehicleData { id = "mustang", name = "BLAZE GT", rarity = "EPIC", costCoins = 0, isUnlocked = true, topSpeed = 6.5f, acceleration = 5.0f, handling = 6.0f, braking = 5.5f, bodyColor = new Color(0.85f, 0.46f, 0.02f), roofColor = Color.black },
            new VehicleData { id = "verna", name = "BLUE THUNDER", rarity = "RARE", costCoins = 15, isUnlocked = true, topSpeed = 7.2f, acceleration = 6.0f, handling = 6.5f, braking = 6.0f, bodyColor = new Color(0.14f, 0.38f, 0.92f), roofColor = new Color(0.12f, 0.16f, 0.23f) },
            new VehicleData { id = "lamborghini", name = "VIPER R", rarity = "LEGENDARY", costCoins = 50, isUnlocked = true, topSpeed = 9.5f, acceleration = 9.0f, handling = 8.5f, braking = 8.0f, bodyColor = new Color(0.91f, 0.70f, 0.03f), roofColor = new Color(0.06f, 0.09f, 0.16f) },
            new VehicleData { id = "alto", name = "WHITE SEDAN", rarity = "COMMON", costCoins = 2, isUnlocked = true, topSpeed = 6.0f, acceleration = 5.5f, handling = 6.0f, braking = 5.5f, bodyColor = new Color(0.97f, 0.98f, 0.99f), roofColor = new Color(0.12f, 0.16f, 0.23f) }
        };

        public VehicleData SelectedVehicle { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SelectedVehicle = Vehicles[0];
        }

        private void Start()
        {
            if (SaveManager.Instance != null && !string.IsNullOrEmpty(SaveManager.Instance.Data.selectedCarId))
            {
                SelectVehicle(SaveManager.Instance.Data.selectedCarId);
            }
        }

        public void SelectVehicle(string id)
        {
            VehicleData v = Vehicles.Find(x => x.id.Equals(id, System.StringComparison.OrdinalIgnoreCase));
            if (v != null)
            {
                SelectedVehicle = v;
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.Data.selectedCarId = id;
                    SaveManager.Instance.Save();
                }
            }
        }

        public bool UpgradeSelectedVehicleStat(string statName, int cost = 2000)
        {
            if (SaveManager.Instance == null || SelectedVehicle == null) return false;

            if (SaveManager.Instance.DeductCoins(cost))
            {
                if (statName.Equals("speed", System.StringComparison.OrdinalIgnoreCase))
                {
                    SelectedVehicle.topSpeed = Mathf.Min(10.0f, SelectedVehicle.topSpeed + 0.5f);
                    SaveManager.Instance.Data.speedUpgradeLevel++;
                }
                else if (statName.Equals("accel", System.StringComparison.OrdinalIgnoreCase))
                {
                    SelectedVehicle.acceleration = Mathf.Min(10.0f, SelectedVehicle.acceleration + 0.5f);
                    SaveManager.Instance.Data.accelUpgradeLevel++;
                }
                else if (statName.Equals("handling", System.StringComparison.OrdinalIgnoreCase))
                {
                    SelectedVehicle.handling = Mathf.Min(10.0f, SelectedVehicle.handling + 0.5f);
                    SaveManager.Instance.Data.handlingUpgradeLevel++;
                }
                else if (statName.Equals("braking", System.StringComparison.OrdinalIgnoreCase))
                {
                    SelectedVehicle.braking = Mathf.Min(10.0f, SelectedVehicle.braking + 0.5f);
                    SaveManager.Instance.Data.brakingUpgradeLevel++;
                }

                SaveManager.Instance.Save();

                // Apply immediately if in-game
                PlayerVehicleController player = FindFirstObjectByType<PlayerVehicleController>();
                if (player != null) player.ApplyUpgradedStats();

                return true;
            }
            return false;
        }

        public void SetSelectedVehicleColor(Color color)
        {
            if (SelectedVehicle != null)
            {
                SelectedVehicle.bodyColor = color;
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.Data.carColorHex = "#" + ColorUtility.ToHtmlStringRGB(color);
                    SaveManager.Instance.Save();
                }
            }
        }
    }
}
