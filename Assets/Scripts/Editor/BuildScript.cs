using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

namespace VehicleCoinCollector.Editor
{
    /// <summary>
    /// Automated Unity Build Script to assemble the Release Android APK for ApniAlto / Car Rush.
    /// </summary>
    public static class BuildScript
    {
        public static void BuildAndroidAPK()
        {
            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            string apkPath = Path.Combine(outputDir, "ApniAlto_CarRush.apk");
            string[] scenes = { "Assets/Scenes/VehicleCoinCollector.unity" };

            Debug.Log("[BuildScript] Switching active build target to Android...");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            PlayerSettings.companyName = "ParveenEngg";
            PlayerSettings.productName = "ApniAlto Car Rush";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.parveenengg.apnialto");

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Debug.Log("[BuildScript] Starting Android APK build at: " + apkPath);
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("[BuildScript] APK BUILD SUCCEEDED! Total size: " + summary.totalSize + " bytes. Output path: " + apkPath);
            }
            else
            {
                Debug.LogError("[BuildScript] APK BUILD FAILED with result: " + summary.result + " Errors: " + summary.totalErrors);
            }
        }
    }
}
