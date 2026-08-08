using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Auto-starts Free Play when VehicleCoinCollector scene loads.
    /// Applies map environment colors and vehicle colors.
    /// Attach this to a GameObject in the VehicleCoinCollector scene.
    /// </summary>
    public class GameStartup : MonoBehaviour
    {
        [Header("Environment References")]
        public Renderer groundRenderer;

        [Header("Vehicle Body References")]
        public Renderer[] vehicleRenderers;

        private void Start()
        {
            ApplyMapEnvironment();
            ApplyVehicleColors();

            // Start Free Play
            if (RaceManager.Instance != null)
            {
                RaceManager.Instance.StartFreePlay();
            }

            Debug.Log("[GameStartup] Free Play started on Green Valley.");
        }

        private void ApplyMapEnvironment()
        {
            if (MapManager.Instance == null) return;

            if (groundRenderer != null)
            {
                groundRenderer.material.color = MapManager.Instance.GroundColor;
            }

            RenderSettings.fog = true;
            RenderSettings.fogColor = MapManager.Instance.FogColor;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 40f;
            RenderSettings.fogEndDistance = 120f;

            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.backgroundColor = MapManager.Instance.FogColor;
            }
        }

        private void ApplyVehicleColors()
        {
            if (GarageManager.Instance == null || GarageManager.Instance.SelectedVehicle == null) return;

            VehicleData vehicle = GarageManager.Instance.SelectedVehicle;

            if (vehicleRenderers != null && vehicleRenderers.Length > 0 && vehicleRenderers[0] != null)
            {
                vehicleRenderers[0].material.color = vehicle.bodyColor;
            }
            if (vehicleRenderers != null && vehicleRenderers.Length > 1 && vehicleRenderers[1] != null)
            {
                vehicleRenderers[1].material.color = vehicle.roofColor;
            }
        }
    }
}
