using UnityEngine;
using System.Collections.Generic;

namespace VehicleCoinCollector
{
    [System.Serializable]
    public class CarItem
    {
        public string carID;
        public string displayName;
        public int cost;
        public bool isUnlocked;
        public Color bodyColor;
        public Color roofColor;
        public float topSpeed;
        public float acceleration;
        public bool isHoverCar;
    }

    /// <summary>
    /// Garage Manager: manages vehicle catalog (Alto, Verna, Lamborghini, Mustang, Flying Car),
    /// coin purchasing, and equipping active vehicle for gameplay.
    /// </summary>
    public class GarageManager : MonoBehaviour
    {
        public static GarageManager Instance { get; private set; }

        [Header("Car Catalog")]
        public List<CarItem> carCatalog = new List<CarItem>();
        public int activeCarIndex = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            InitializeCatalog();
        }

        private void InitializeCatalog()
        {
            carCatalog.Clear();

            // 1. Alto (Free)
            carCatalog.Add(new CarItem
            {
                carID = "alto",
                displayName = "Alto",
                cost = 0,
                isUnlocked = true,
                bodyColor = new Color(0.1f, 0.7f, 0.8f), // Cyan
                roofColor = Color.white,
                topSpeed = 11f,
                acceleration = 12f,
                isHoverCar = false
            });

            // 2. Verna (15 Coins)
            carCatalog.Add(new CarItem
            {
                carID = "verna",
                displayName = "Verna",
                cost = 15,
                isUnlocked = false,
                bodyColor = new Color(0.1f, 0.4f, 0.9f), // Cobalt Blue
                roofColor = new Color(0.1f, 0.2f, 0.4f),
                topSpeed = 13f,
                acceleration = 14f,
                isHoverCar = false
            });

            // 3. Lamborghini (50 Coins)
            carCatalog.Add(new CarItem
            {
                carID = "lamborghini",
                displayName = "Lamborghini",
                cost = 50,
                isUnlocked = false,
                bodyColor = new Color(1.0f, 0.85f, 0.0f), // Yellow Supercar
                roofColor = new Color(0.15f, 0.15f, 0.15f),
                topSpeed = 16f,
                acceleration = 18f,
                isHoverCar = false
            });

            // 4. Mustang (150 Coins)
            carCatalog.Add(new CarItem
            {
                carID = "mustang",
                displayName = "Mustang",
                cost = 150,
                isUnlocked = false,
                bodyColor = new Color(0.85f, 0.1f, 0.1f), // Crimson Red
                roofColor = Color.black,
                topSpeed = 15f,
                acceleration = 16f,
                isHoverCar = false
            });

            // 5. Flying Car (2 Coins)
            carCatalog.Add(new CarItem
            {
                carID = "flying_car",
                displayName = "Flying Car",
                cost = 2,
                isUnlocked = false,
                bodyColor = new Color(0.0f, 0.9f, 0.95f), // Neon Cyan Hover Car
                roofColor = new Color(0.6f, 0.2f, 0.9f), // Purple
                topSpeed = 18f,
                acceleration = 20f,
                isHoverCar = true
            });
        }

        public CarItem GetActiveCar()
        {
            if (activeCarIndex >= 0 && activeCarIndex < carCatalog.Count)
            {
                return carCatalog[activeCarIndex];
            }
            return carCatalog[0];
        }

        public bool BuyCar(int index)
        {
            if (index < 0 || index >= carCatalog.Count) return false;
            CarItem car = carCatalog[index];

            if (car.isUnlocked) return true;

            int coins = (ScoreManager.Instance != null) ? ScoreManager.Instance.currentCoins : 12850;
            if (coins >= car.cost)
            {
                car.isUnlocked = true;
                EquipCar(index);
                return true;
            }
            return false;
        }

        public void EquipCar(int index)
        {
            if (index >= 0 && index < carCatalog.Count && carCatalog[index].isUnlocked)
            {
                activeCarIndex = index;
            }
        }
    }
}
