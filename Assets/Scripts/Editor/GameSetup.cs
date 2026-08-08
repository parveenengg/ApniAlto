using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace VehicleCoinCollector.Editor
{
    public class GameSetup
    {
        [MenuItem("Vehicle Game/Build Complete Coin Collector Level")]
        public static void BuildCoinCollectorLevel()
        {
            // 1. Create a fresh scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 2. Lighting & Sky
            GameObject dirLight = new GameObject("Directional Light");
            Light lightComponent = dirLight.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.intensity = 1.25f;
            lightComponent.shadows = LightShadows.Soft;
            dirLight.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            // 3. Create Materials
            Material grassMat = CreateMaterial("Mat_Grass", new Color(0.35f, 0.75f, 0.35f));
            Material roadMat = CreateMaterial("Mat_Road", new Color(0.2f, 0.22f, 0.25f));
            Material wallMat = CreateMaterial("Mat_Wall", new Color(0.6f, 0.65f, 0.7f));
            Material carBodyMat = CreateMaterial("Mat_CarBody", new Color(0.95f, 0.35f, 0.15f));
            Material carRoofMat = CreateMaterial("Mat_CarRoof", new Color(0.15f, 0.55f, 0.9f));
            Material wheelMat = CreateMaterial("Mat_Wheel", new Color(0.12f, 0.12f, 0.14f));
            Material headlightMat = CreateMaterial("Mat_Headlight", new Color(1.0f, 0.95f, 0.6f));
            Material coinMat = CreateMaterial("Mat_Coin", new Color(1.0f, 0.85f, 0.1f));
            Material obstacleMat = CreateMaterial("Mat_Obstacle", new Color(0.9f, 0.2f, 0.2f));
            Material movingObstacleMat = CreateMaterial("Mat_MovingObstacle", new Color(0.85f, 0.45f, 0.1f));
            Material treeTrunkMat = CreateMaterial("Mat_TreeTrunk", new Color(0.45f, 0.3f, 0.15f));
            Material treeFoliageMat = CreateMaterial("Mat_TreeFoliage", new Color(0.2f, 0.6f, 0.25f));
            Material finishArchMat = CreateMaterial("Mat_FinishArch", new Color(0.9f, 0.9f, 0.95f));
            Material finishBannerMat = CreateMaterial("Mat_FinishBanner", new Color(0.1f, 0.8f, 0.4f));

            // 4. Ground Platform & Road Track
            GameObject groundObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            groundObj.name = "Ground Platform";
            groundObj.transform.position = new Vector3(0, -0.5f, 60f);
            groundObj.transform.localScale = new Vector3(60f, 1f, 160f);
            groundObj.GetComponent<Renderer>().sharedMaterial = grassMat;

            GameObject roadObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roadObj.name = "Driving Road Track";
            roadObj.transform.position = new Vector3(0, -0.4f, 60f);
            roadObj.transform.localScale = new Vector3(20f, 1f, 150f);
            roadObj.GetComponent<Renderer>().sharedMaterial = roadMat;

            GameObject startPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            startPlatform.name = "Start Area";
            startPlatform.transform.position = new Vector3(0, -0.38f, -10f);
            startPlatform.transform.localScale = new Vector3(24f, 1f, 15f);
            startPlatform.GetComponent<Renderer>().sharedMaterial = finishBannerMat;

            CreateWall("Wall Left", new Vector3(-29f, 2.5f, 60f), new Vector3(2f, 6f, 160f), wallMat);
            CreateWall("Wall Right", new Vector3(29f, 2.5f, 60f), new Vector3(2f, 6f, 160f), wallMat);
            CreateWall("Wall Back", new Vector3(0f, 2.5f, -19f), new Vector3(60f, 6f, 2f), wallMat);
            CreateWall("Wall Front", new Vector3(0f, 2.5f, 139f), new Vector3(60f, 6f, 2f), wallMat);

            // 5. Stylized Low-Poly Cartoon Vehicle Setup
            GameObject vehicleRoot = new GameObject("Player Vehicle");
            vehicleRoot.tag = "Player";
            vehicleRoot.transform.position = new Vector3(0f, 0.5f, -10f);

            Rigidbody carRb = vehicleRoot.AddComponent<Rigidbody>();
            carRb.mass = 1200f;
            carRb.linearDamping = 0.5f;
            carRb.angularDamping = 2.0f;
            carRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            BoxCollider carCollider = vehicleRoot.AddComponent<BoxCollider>();
            carCollider.center = new Vector3(0f, 0.7f, 0f);
            carCollider.size = new Vector3(2.2f, 1.4f, 4.0f);

            PlayerVehicleController vehicleController = vehicleRoot.AddComponent<PlayerVehicleController>();

            GameObject carBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carBody.name = "Body Base";
            carBody.transform.SetParent(vehicleRoot.transform, false);
            carBody.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            carBody.transform.localScale = new Vector3(2.2f, 0.8f, 4.0f);
            carBody.GetComponent<Renderer>().sharedMaterial = carBodyMat;

            GameObject carCabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carCabin.name = "Cabin Roof";
            carCabin.transform.SetParent(vehicleRoot.transform, false);
            carCabin.transform.localPosition = new Vector3(0f, 1.15f, -0.2f);
            carCabin.transform.localScale = new Vector3(1.8f, 0.7f, 2.2f);
            carCabin.GetComponent<Renderer>().sharedMaterial = carRoofMat;

            vehicleController.bodyBase = carBody.transform;
            vehicleController.cabinRoof = carCabin.transform;

            CreateHeadlight(vehicleRoot.transform, new Vector3(-0.75f, 0.6f, 1.98f), headlightMat);
            CreateHeadlight(vehicleRoot.transform, new Vector3(0.75f, 0.6f, 1.98f), headlightMat);

            Transform[] wheelTransforms = new Transform[4];
            wheelTransforms[0] = CreateWheel(vehicleRoot.transform, "Wheel FL", new Vector3(-1.15f, 0.35f, 1.2f), wheelMat);
            wheelTransforms[1] = CreateWheel(vehicleRoot.transform, "Wheel FR", new Vector3(1.15f, 0.35f, 1.2f), wheelMat);
            wheelTransforms[2] = CreateWheel(vehicleRoot.transform, "Wheel RL", new Vector3(-1.15f, 0.35f, -1.2f), wheelMat);
            wheelTransforms[3] = CreateWheel(vehicleRoot.transform, "Wheel RR", new Vector3(1.15f, 0.35f, -1.2f), wheelMat);

            vehicleController.wheels = wheelTransforms;

            // 6. Spawn Coins
            GameObject coinGroup = new GameObject("Collectible Coins");
            Vector3[] coinPositions = new Vector3[]
            {
                new Vector3(-4f, 1.2f, 5f),
                new Vector3(0f, 1.2f, 12f),
                new Vector3(4f, 1.2f, 19f),
                new Vector3(-5f, 1.2f, 32f),
                new Vector3(5f, 1.2f, 38f),
                new Vector3(0f, 1.2f, 48f),
                new Vector3(-6f, 1.2f, 62f),
                new Vector3(0f, 1.2f, 72f),
                new Vector3(6f, 1.2f, 80f),
                new Vector3(-4f, 1.2f, 95f),
                new Vector3(4f, 1.2f, 102f),
                new Vector3(0f, 1.2f, 115f)
            };

            foreach (var pos in coinPositions)
            {
                CreateCoin(coinGroup.transform, pos, coinMat);
            }

            // 7. Spawn Obstacles
            GameObject obstacleGroup = new GameObject("Obstacles");

            CreateObstacle(obstacleGroup.transform, "Static Box 1", PrimitiveType.Cube, new Vector3(-5f, 1f, 25f), new Vector3(2.5f, 2f, 2.5f), obstacleMat, ObstacleBehaviour.ObstacleType.Stationary);
            CreateObstacle(obstacleGroup.transform, "Static Box 2", PrimitiveType.Cube, new Vector3(5f, 1f, 45f), new Vector3(3f, 2f, 3f), obstacleMat, ObstacleBehaviour.ObstacleType.Stationary);
            CreateObstacle(obstacleGroup.transform, "Static Cylinder 1", PrimitiveType.Cylinder, new Vector3(0f, 1.2f, 68f), new Vector3(2.5f, 1.2f, 2.5f), obstacleMat, ObstacleBehaviour.ObstacleType.Stationary);
            CreateObstacle(obstacleGroup.transform, "Static Box 3", PrimitiveType.Cube, new Vector3(-6f, 1f, 88f), new Vector3(3f, 2f, 3f), obstacleMat, ObstacleBehaviour.ObstacleType.Stationary);

            GameObject movingOb1 = CreateObstacle(obstacleGroup.transform, "Moving Obstacle 1", PrimitiveType.Cube, new Vector3(0f, 1.2f, 35f), new Vector3(4f, 1.8f, 2f), movingObstacleMat, ObstacleBehaviour.ObstacleType.OscillatingSideToSide);
            ObstacleBehaviour obScript1 = movingOb1.GetComponent<ObstacleBehaviour>();
            obScript1.moveDistance = 6f;
            obScript1.moveSpeed = 2.5f;

            GameObject movingOb2 = CreateObstacle(obstacleGroup.transform, "Moving Obstacle 2", PrimitiveType.Cylinder, new Vector3(0f, 1.5f, 105f), new Vector3(3.5f, 1.5f, 3.5f), movingObstacleMat, ObstacleBehaviour.ObstacleType.OscillatingSideToSide);
            ObstacleBehaviour obScript2 = movingOb2.GetComponent<ObstacleBehaviour>();
            obScript2.moveDistance = 7f;
            obScript2.moveSpeed = 3.0f;

            // 8. Low-Poly Decorative Trees & Rocks
            GameObject environmentGroup = new GameObject("Environment Decorative Objects");
            for (float z = 0f; z <= 120f; z += 15f)
            {
                CreateTree(environmentGroup.transform, new Vector3(-16f, 0f, z + Random.Range(-2f, 2f)), treeTrunkMat, treeFoliageMat);
                CreateTree(environmentGroup.transform, new Vector3(16f, 0f, z + Random.Range(-2f, 2f)), treeTrunkMat, treeFoliageMat);

                CreateRock(environmentGroup.transform, new Vector3(-22f, 0f, z + 7f), wallMat);
                CreateRock(environmentGroup.transform, new Vector3(22f, 0f, z + 7f), wallMat);
            }

            // 9. Finish Zone & Archway
            GameObject finishRoot = new GameObject("Finish Area");
            finishRoot.transform.position = new Vector3(0f, 0f, 125f);

            GameObject leftPillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftPillar.transform.SetParent(finishRoot.transform, false);
            leftPillar.transform.localPosition = new Vector3(-8f, 4f, 0f);
            leftPillar.transform.localScale = new Vector3(1.5f, 8f, 1.5f);
            leftPillar.GetComponent<Renderer>().sharedMaterial = finishArchMat;

            GameObject rightPillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightPillar.transform.SetParent(finishRoot.transform, false);
            rightPillar.transform.localPosition = new Vector3(8f, 4f, 0f);
            rightPillar.transform.localScale = new Vector3(1.5f, 8f, 1.5f);
            rightPillar.GetComponent<Renderer>().sharedMaterial = finishArchMat;

            GameObject topBanner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topBanner.transform.SetParent(finishRoot.transform, false);
            topBanner.transform.localPosition = new Vector3(0f, 7.5f, 0f);
            topBanner.transform.localScale = new Vector3(17.5f, 1.8f, 1.2f);
            topBanner.GetComponent<Renderer>().sharedMaterial = finishBannerMat;

            GameObject finishTrigger = new GameObject("Finish Trigger Zone");
            finishTrigger.transform.SetParent(finishRoot.transform, false);
            finishTrigger.transform.localPosition = new Vector3(0f, 2.5f, 0f);

            BoxCollider finishCol = finishTrigger.AddComponent<BoxCollider>();
            finishCol.isTrigger = true;
            finishCol.size = new Vector3(16f, 5f, 4f);
            finishTrigger.AddComponent<FinishZone>();

            // 10. Managers & Audio Setup
            GameObject managersObj = new GameObject("Game Managers");
            ScoreManager scoreMgr = managersObj.AddComponent<ScoreManager>();
            scoreMgr.totalCoinsInLevel = coinPositions.Length;

            GameManager gameMgr = managersObj.AddComponent<GameManager>();
            AudioManager audioMgr = managersObj.AddComponent<AudioManager>();

            // 11. UI Canvas & HUD Setup
            GameObject uiCanvasObj = BuildUICanvas();

            // 12. Main Camera & Trailing Follow
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            cameraObj.transform.position = new Vector3(0f, 7f, -20f);

            Camera camComponent = cameraObj.AddComponent<Camera>();
            camComponent.clearFlags = CameraClearFlags.SolidColor;
            camComponent.backgroundColor = new Color(0.55f, 0.8f, 0.95f);

            cameraObj.AddComponent<AudioListener>();
            CameraFollow camFollow = cameraObj.AddComponent<CameraFollow>();
            camFollow.target = vehicleRoot.transform;

            // 13. Save Scene
            if (!Directory.Exists("Assets/Scenes"))
            {
                Directory.CreateDirectory("Assets/Scenes");
            }
            string scenePath = "Assets/Scenes/VehicleCoinCollector.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            Debug.Log($"[VehicleCoinCollector] Level successfully created and saved to: {scenePath}");
        }

        private static Material CreateMaterial(string name, Color color)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            mat.name = name;
            string path = $"Assets/Materials/{name}.mat";
            if (!Directory.Exists("Assets/Materials"))
            {
                Directory.CreateDirectory("Assets/Materials");
            }
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static void CreateWall(string name, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void CreateHeadlight(Transform parent, Vector3 localPos, Material mat)
        {
            GameObject hl = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hl.name = "Headlight";
            hl.transform.SetParent(parent, false);
            hl.transform.localPosition = localPos;
            hl.transform.localScale = new Vector3(0.4f, 0.4f, 0.2f);
            hl.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(hl.GetComponent<Collider>());
        }

        private static Transform CreateWheel(Transform parent, string name, Vector3 localPos, Material mat)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = name;
            wheel.transform.SetParent(parent, false);
            wheel.transform.localPosition = localPos;
            wheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            wheel.transform.localScale = new Vector3(0.8f, 0.3f, 0.8f);
            wheel.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(wheel.GetComponent<Collider>());
            return wheel.transform;
        }

        private static void CreateCoin(Transform parent, Vector3 pos, Material mat)
        {
            GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coin.name = "Coin";
            coin.transform.SetParent(parent, false);
            coin.transform.position = pos;
            coin.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            coin.transform.localScale = new Vector3(1.2f, 0.15f, 1.2f);
            coin.GetComponent<Renderer>().sharedMaterial = mat;

            SphereCollider triggerCol = coin.AddComponent<SphereCollider>();
            triggerCol.isTrigger = true;
            triggerCol.radius = 1.2f;

            coin.AddComponent<Coin>();
        }

        private static GameObject CreateObstacle(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat, ObstacleBehaviour.ObstacleType mode)
        {
            GameObject ob = GameObject.CreatePrimitive(type);
            ob.name = name;
            ob.transform.SetParent(parent, false);
            ob.transform.position = pos;
            ob.transform.localScale = scale;
            ob.GetComponent<Renderer>().sharedMaterial = mat;

            ObstacleBehaviour script = ob.AddComponent<ObstacleBehaviour>();
            script.obstacleType = mode;
            script.damageToPlayer = 10;
            return ob;
        }

        private static void CreateTree(Transform parent, Vector3 pos, Material trunkMat, Material foliageMat)
        {
            GameObject tree = new GameObject("LowPoly Tree");
            tree.transform.SetParent(parent, false);
            tree.transform.position = pos;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(tree.transform, false);
            trunk.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            trunk.transform.localScale = new Vector3(0.6f, 1.5f, 0.6f);
            trunk.GetComponent<Renderer>().sharedMaterial = trunkMat;

            GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage.transform.SetParent(tree.transform, false);
            foliage.transform.localPosition = new Vector3(0f, 4.0f, 0f);
            foliage.transform.localScale = new Vector3(3.2f, 3.5f, 3.2f);
            foliage.GetComponent<Renderer>().sharedMaterial = foliageMat;
        }

        private static void CreateRock(Transform parent, Vector3 pos, Material rockMat)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "LowPoly Rock";
            rock.transform.SetParent(parent, false);
            rock.transform.position = pos;
            rock.transform.localScale = new Vector3(Random.Range(2f, 3.5f), Random.Range(1.5f, 2.5f), Random.Range(2f, 3.5f));
            rock.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            rock.GetComponent<Renderer>().sharedMaterial = rockMat;
        }

        private static GameObject BuildUICanvas()
        {
            GameObject canvasObj = new GameObject("UI Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            UIManager uiManager = canvasObj.AddComponent<UIManager>();

            // Score HUD Text
            GameObject scoreTextObj = new GameObject("Score HUD Text");
            scoreTextObj.transform.SetParent(canvasObj.transform, false);
            RectTransform scoreRect = scoreTextObj.AddComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0f, 1f);
            scoreRect.anchorMax = new Vector2(0f, 1f);
            scoreRect.pivot = new Vector2(0f, 1f);
            scoreRect.anchoredPosition = new Vector2(30f, -30f);
            scoreRect.sizeDelta = new Vector2(300f, 50f);

            Text scoreText = scoreTextObj.AddComponent<Text>();
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            scoreText.fontSize = 32;
            scoreText.color = Color.yellow;
            scoreText.text = "Coins: 0 / 12";

            uiManager.scoreLegacyText = scoreText;

            // Integrity / Health HUD Container & Fill Bar
            GameObject healthBarContainer = new GameObject("Integrity HUD Container");
            healthBarContainer.transform.SetParent(canvasObj.transform, false);
            RectTransform healthContainerRect = healthBarContainer.AddComponent<RectTransform>();
            healthContainerRect.anchorMin = new Vector2(0f, 1f);
            healthContainerRect.anchorMax = new Vector2(0f, 1f);
            healthContainerRect.pivot = new Vector2(0f, 1f);
            healthContainerRect.anchoredPosition = new Vector2(30f, -85f);
            healthContainerRect.sizeDelta = new Vector2(250f, 24f);

            Image healthBg = healthBarContainer.AddComponent<Image>();
            healthBg.color = new Color(0.2f, 0.2f, 0.25f, 0.8f);

            GameObject healthFillObj = new GameObject("Fill");
            healthFillObj.transform.SetParent(healthBarContainer.transform, false);
            RectTransform fillRect = healthFillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image healthFillImage = healthFillObj.AddComponent<Image>();
            healthFillImage.color = new Color(0.2f, 0.85f, 0.4f);
            healthFillImage.type = Image.Type.Filled;
            healthFillImage.fillMethod = Image.FillMethod.Horizontal;
            healthFillImage.fillAmount = 1.0f;

            uiManager.healthBarFill = healthFillImage;

            // Integrity Text Label
            GameObject healthTextObj = new GameObject("Integrity Text");
            healthTextObj.transform.SetParent(canvasObj.transform, false);
            RectTransform healthTextRect = healthTextObj.AddComponent<RectTransform>();
            healthTextRect.anchorMin = new Vector2(0f, 1f);
            healthTextRect.anchorMax = new Vector2(0f, 1f);
            healthTextRect.pivot = new Vector2(0f, 1f);
            healthTextRect.anchoredPosition = new Vector2(30f, -115f);
            healthTextRect.sizeDelta = new Vector2(300f, 40f);

            Text healthText = healthTextObj.AddComponent<Text>();
            healthText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            healthText.fontSize = 22;
            healthText.color = Color.white;
            healthText.text = "Integrity: 100 / 100";

            uiManager.healthLegacyText = healthText;

            // Top Right Settings Gear Button
            GameObject settingsBtnObj = new GameObject("Top Right Settings Button");
            settingsBtnObj.transform.SetParent(canvasObj.transform, false);
            RectTransform settingsBtnRect = settingsBtnObj.AddComponent<RectTransform>();
            settingsBtnRect.anchorMin = new Vector2(1f, 1f);
            settingsBtnRect.anchorMax = new Vector2(1f, 1f);
            settingsBtnRect.pivot = new Vector2(1f, 1f);
            settingsBtnRect.anchoredPosition = new Vector2(-25f, -25f);
            settingsBtnRect.sizeDelta = new Vector2(50f, 50f);

            Image settingsBtnBg = settingsBtnObj.AddComponent<Image>();
            settingsBtnBg.color = new Color(0.15f, 0.2f, 0.3f, 0.9f);
            Button settingsBtn = settingsBtnObj.AddComponent<Button>();

            GameObject settingsIconObj = new GameObject("IconText");
            settingsIconObj.transform.SetParent(settingsBtnObj.transform, false);
            RectTransform iconRect = settingsIconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            Text iconText = settingsIconObj.AddComponent<Text>();
            iconText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            iconText.fontSize = 30;
            iconText.alignment = TextAnchor.MiddleCenter;
            iconText.color = Color.white;
            iconText.text = "⚙";

            uiManager.topRightSettingsButton = settingsBtn;

            // Settings Modal Panel
            GameObject settingsPanelObj = new GameObject("Settings Modal Panel");
            settingsPanelObj.transform.SetParent(canvasObj.transform, false);
            RectTransform settingsPanelRect = settingsPanelObj.AddComponent<RectTransform>();
            settingsPanelRect.anchorMin = new Vector2(0.25f, 0.25f);
            settingsPanelRect.anchorMax = new Vector2(0.75f, 0.75f);
            settingsPanelRect.offsetMin = Vector2.zero;
            settingsPanelRect.offsetMax = Vector2.zero;

            Image settingsBg = settingsPanelObj.AddComponent<Image>();
            settingsBg.color = new Color(0.12f, 0.16f, 0.24f, 0.95f);

            GameObject setHeaderObj = new GameObject("Settings Title");
            setHeaderObj.transform.SetParent(settingsPanelObj.transform, false);
            RectTransform setHeaderRect = setHeaderObj.AddComponent<RectTransform>();
            setHeaderRect.anchorMin = new Vector2(0.1f, 0.7f);
            setHeaderRect.anchorMax = new Vector2(0.9f, 0.95f);
            setHeaderRect.offsetMin = Vector2.zero;
            setHeaderRect.offsetMax = Vector2.zero;

            Text setHeaderText = setHeaderObj.AddComponent<Text>();
            setHeaderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            setHeaderText.fontSize = 32;
            setHeaderText.alignment = TextAnchor.MiddleCenter;
            setHeaderText.color = Color.white;
            setHeaderText.text = "⚙ GAME SETTINGS ⚙\n\nControls: WASD Drive • Space Jump";

            GameObject closeBtnObj = new GameObject("Close Button");
            closeBtnObj.transform.SetParent(settingsPanelObj.transform, false);
            RectTransform closeBtnRect = closeBtnObj.AddComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(0.35f, 0.1f);
            closeBtnRect.anchorMax = new Vector2(0.65f, 0.3f);
            closeBtnRect.offsetMin = Vector2.zero;
            closeBtnRect.offsetMax = Vector2.zero;

            Image closeBtnBg = closeBtnObj.AddComponent<Image>();
            closeBtnBg.color = new Color(0.2f, 0.6f, 0.9f);
            Button closeBtn = closeBtnObj.AddComponent<Button>();

            GameObject closeTextObj = new GameObject("Text");
            closeTextObj.transform.SetParent(closeBtnObj.transform, false);
            RectTransform closeTextRect = closeTextObj.AddComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            Text closeText = closeTextObj.AddComponent<Text>();
            closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeText.fontSize = 24;
            closeText.alignment = TextAnchor.MiddleCenter;
            closeText.color = Color.white;
            closeText.text = "Close";

            uiManager.settingsModalPanel = settingsPanelObj;
            uiManager.closeSettingsButton = closeBtn;

            // Level Complete Modal Panel
            GameObject levelCompletePanelObj = new GameObject("Level Complete Panel");
            levelCompletePanelObj.transform.SetParent(canvasObj.transform, false);
            RectTransform panelRect = levelCompletePanelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.2f, 0.2f);
            panelRect.anchorMax = new Vector2(0.8f, 0.8f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelBg = levelCompletePanelObj.AddComponent<Image>();
            panelBg.color = new Color(0.1f, 0.15f, 0.25f, 0.92f);

            GameObject modalTextObj = new GameObject("Modal Summary Text");
            modalTextObj.transform.SetParent(levelCompletePanelObj.transform, false);
            RectTransform modalTextRect = modalTextObj.AddComponent<RectTransform>();
            modalTextRect.anchorMin = new Vector2(0.1f, 0.5f);
            modalTextRect.anchorMax = new Vector2(0.9f, 0.9f);
            modalTextRect.offsetMin = Vector2.zero;
            modalTextRect.offsetMax = Vector2.zero;

            Text modalText = modalTextObj.AddComponent<Text>();
            modalText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            modalText.fontSize = 36;
            modalText.alignment = TextAnchor.MiddleCenter;
            modalText.color = Color.white;
            modalText.text = "Level Complete!\nCoins Collected: 12 / 12";

            uiManager.finalScoreLegacyText = modalText;

            // Restart Button
            GameObject btnRestartObj = new GameObject("Restart Button");
            btnRestartObj.transform.SetParent(levelCompletePanelObj.transform, false);
            RectTransform btnRestRect = btnRestartObj.AddComponent<RectTransform>();
            btnRestRect.anchorMin = new Vector2(0.15f, 0.15f);
            btnRestRect.anchorMax = new Vector2(0.45f, 0.35f);
            btnRestRect.offsetMin = Vector2.zero;
            btnRestRect.offsetMax = Vector2.zero;

            Image btnRestBg = btnRestartObj.AddComponent<Image>();
            btnRestBg.color = new Color(0.2f, 0.7f, 0.3f);
            Button btnRestart = btnRestartObj.AddComponent<Button>();

            GameObject btnRestTextObj = new GameObject("Text");
            btnRestTextObj.transform.SetParent(btnRestartObj.transform, false);
            RectTransform restTextRect = btnRestTextObj.AddComponent<RectTransform>();
            restTextRect.anchorMin = Vector2.zero;
            restTextRect.anchorMax = Vector2.one;
            Text restText = btnRestTextObj.AddComponent<Text>();
            restText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            restText.fontSize = 24;
            restText.alignment = TextAnchor.MiddleCenter;
            restText.color = Color.white;
            restText.text = "Restart";

            // Quit Button
            GameObject btnQuitObj = new GameObject("Quit Button");
            btnQuitObj.transform.SetParent(levelCompletePanelObj.transform, false);
            RectTransform btnQuitRect = btnQuitObj.AddComponent<RectTransform>();
            btnQuitRect.anchorMin = new Vector2(0.55f, 0.15f);
            btnQuitRect.anchorMax = new Vector2(0.85f, 0.35f);
            btnQuitRect.offsetMin = Vector2.zero;
            btnQuitRect.offsetMax = Vector2.zero;

            Image btnQuitBg = btnQuitObj.AddComponent<Image>();
            btnQuitBg.color = new Color(0.8f, 0.25f, 0.25f);
            Button btnQuit = btnQuitObj.AddComponent<Button>();

            GameObject btnQuitTextObj = new GameObject("Text");
            btnQuitTextObj.transform.SetParent(btnQuitObj.transform, false);
            RectTransform quitTextRect = btnQuitTextObj.AddComponent<RectTransform>();
            quitTextRect.anchorMin = Vector2.zero;
            quitTextRect.anchorMax = Vector2.one;
            Text quitText = btnQuitTextObj.AddComponent<Text>();
            quitText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            quitText.fontSize = 24;
            quitText.alignment = TextAnchor.MiddleCenter;
            quitText.color = Color.white;
            quitText.text = "Quit / Menu";

            uiManager.levelCompletePanel = levelCompletePanelObj;
            uiManager.restartButton = btnRestart;
            uiManager.quitButton = btnQuit;

            // Game Over Modal Panel
            GameObject gameOverPanelObj = new GameObject("Game Over Panel");
            gameOverPanelObj.transform.SetParent(canvasObj.transform, false);
            RectTransform goPanelRect = gameOverPanelObj.AddComponent<RectTransform>();
            goPanelRect.anchorMin = new Vector2(0.2f, 0.2f);
            goPanelRect.anchorMax = new Vector2(0.8f, 0.8f);
            goPanelRect.offsetMin = Vector2.zero;
            goPanelRect.offsetMax = Vector2.zero;

            Image goPanelBg = gameOverPanelObj.AddComponent<Image>();
            goPanelBg.color = new Color(0.25f, 0.1f, 0.1f, 0.95f);

            GameObject goTextObj = new GameObject("Game Over Text");
            goTextObj.transform.SetParent(gameOverPanelObj.transform, false);
            RectTransform goTextRect = goTextObj.AddComponent<RectTransform>();
            goTextRect.anchorMin = new Vector2(0.1f, 0.4f);
            goTextRect.anchorMax = new Vector2(0.9f, 0.9f);
            goTextRect.offsetMin = Vector2.zero;
            goTextRect.offsetMax = Vector2.zero;

            Text goText = goTextObj.AddComponent<Text>();
            goText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            goText.fontSize = 36;
            goText.alignment = TextAnchor.MiddleCenter;
            goText.color = Color.red;
            goText.text = "VEHICLE BLASTED! 💥\nIntegrity reached 0!";

            uiManager.gameOverLegacyText = goText;

            GameObject goBtnObj = new GameObject("Game Over Restart Button");
            goBtnObj.transform.SetParent(gameOverPanelObj.transform, false);
            RectTransform goBtnRect = goBtnObj.AddComponent<RectTransform>();
            goBtnRect.anchorMin = new Vector2(0.3f, 0.15f);
            goBtnRect.anchorMax = new Vector2(0.7f, 0.35f);
            goBtnRect.offsetMin = Vector2.zero;
            goBtnRect.offsetMax = Vector2.zero;

            Image goBtnBg = goBtnObj.AddComponent<Image>();
            goBtnBg.color = new Color(0.8f, 0.2f, 0.2f);
            Button goBtn = goBtnObj.AddComponent<Button>();

            GameObject goBtnTextObj = new GameObject("Text");
            goBtnTextObj.transform.SetParent(goBtnObj.transform, false);
            RectTransform goBtnTextRect = goBtnTextObj.AddComponent<RectTransform>();
            goBtnTextRect.anchorMin = Vector2.zero;
            goBtnTextRect.anchorMax = Vector2.one;
            Text goBtnText = goBtnTextObj.AddComponent<Text>();
            goBtnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            goBtnText.fontSize = 24;
            goBtnText.alignment = TextAnchor.MiddleCenter;
            goBtnText.color = Color.white;
            goBtnText.text = "Restart Level";

            uiManager.gameOverPanel = gameOverPanelObj;
            uiManager.gameOverRestartButton = goBtn;

            return canvasObj;
        }
    }
}
