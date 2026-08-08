using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Map Manager — single default map (Green Valley).
    /// More maps can be added later.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        public string DefaultMapName = "GREEN VALLEY";
        public Color GroundColor = new Color(0.35f, 0.74f, 0.35f);
        public Color FogColor = new Color(0.53f, 0.81f, 0.92f);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
