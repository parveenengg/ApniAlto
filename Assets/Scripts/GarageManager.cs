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
        public float topSpeed;
        public float acceleration;
        public float handling;
        public float braking;
        public Color bodyColor;
        public Color roofColor;
    }

    /// <summary>
    /// Garage Manager — single default vehicle (BLAZE GT).
    /// More vehicles can be added later.
    /// </summary>
    public class GarageManager : MonoBehaviour
    {
        public static GarageManager Instance { get; private set; }

        public VehicleData DefaultVehicle = new VehicleData
        {
            id = "mustang",
            name = "BLAZE GT",
            rarity = "EPIC",
            topSpeed = 6.5f,
            acceleration = 5.0f,
            handling = 6.0f,
            braking = 5.5f,
            bodyColor = new Color(0.85f, 0.46f, 0.02f),
            roofColor = Color.black
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
            SelectedVehicle = DefaultVehicle;
        }
    }
}
