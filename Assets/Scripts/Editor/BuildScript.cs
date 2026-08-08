using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

namespace VehicleCoinCollector.Editor
{
    /// <summary>
    /// Automated Unity Build Script for Mac Standalone, WebGL, and Android.
    /// </summary>
    public static class BuildScript
    {
        public static void BuildMacApp()
        {
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string appPath = Path.Combine(outputDir, "ApniAlto_CarRush.app");
            string[] scenes = { "Assets/Scenes/VehicleCoinCollector.unity" };

            Debug.Log("[BuildScript] Starting Standalone macOS App build at: " + appPath);
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = appPath,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("[BuildScript] MAC APP BUILD SUCCEEDED! Path: " + appPath);
            }
            else
            {
                Debug.LogError("[BuildScript] MAC APP BUILD FAILED: " + summary.result);
            }
        }

        public static void BuildAndroidAPK()
        {
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            string apkPath = Path.Combine(outputDir, "ApniAlto_CarRush.apk");
            string[] scenes = { "Assets/Scenes/VehicleCoinCollector.unity" };

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            PlayerSettings.companyName = "ParveenEngg";
            PlayerSettings.productName = "ApniAlto Car Rush";

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("[BuildScript] APK BUILD SUCCEEDED! Path: " + apkPath);
            }
            else
            {
                Debug.LogError("[BuildScript] APK BUILD FAILED: " + summary.result);
            }
        }
    }
}
