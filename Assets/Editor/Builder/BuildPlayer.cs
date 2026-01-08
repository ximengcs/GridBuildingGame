using System;
using Builder;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildPlayer
{
    public static void BuildApk(BuildConfig config)
    {
        var now = DateTimeOffset.Now;
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Boot/launcher.unity" },
            locationPathName = $"Build/{config.BuildTarget}/{config.Version}_{now:yyyyMMdd_hhmmss}.apk",
            target = config.BuildTarget,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            EditorUtility.RevealInFinder(buildPlayerOptions.locationPathName);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
    public static void BuildWin64(BuildConfig config)
    {
        var now = DateTimeOffset.Now;
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Boot/launcher.unity" },
            locationPathName = $"Build/{config.BuildTarget}/{config.Version}_{now:yyyyMMdd_hhmmss}/sg.exe",
            target = config.BuildTarget,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            EditorUtility.RevealInFinder(buildPlayerOptions.locationPathName);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}