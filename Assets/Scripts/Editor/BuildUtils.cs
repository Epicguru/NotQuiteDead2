using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class BuildUtils
{
    public const string RUN_LATEST_BUILD = "File/Builds/Run Latest Build #F5";
    public const string BUILD_AGAIN = "File/Builds/Build #F6";
    public const string SELECT_BUILD_PATH = "File/Builds/Set Build Path...";
    public const string BUILD_FOLDER_NAME = "Builds";
    public static string FinalBuildPath
    {
        get
        {
            string bp = BuildPath;
            bool absolute = Path.IsPathRooted(bp) && !bp.TrimStart().StartsWith(@"\");
            if (absolute)
            {
                return bp;
            }
            else
            {
                string a = Directory.GetParent(Application.dataPath).ToString();
                string b = bp.Substring(1); // Remove the fist \ character.
                string final = Path.Combine(a, b);
                return final;
            }
        }
    }

    [MenuItem(RUN_LATEST_BUILD)]
    public static void RunLatestBuild()
    {
        string path = BuildPath;

        // Check to see if the file exists.
        if (!CanRunLatest())
        {
            Debug.LogError("Could not find build at '{0}'".Form(BuildPath));
            return;
        }

        // Start process.
        System.Diagnostics.Process.Start(FinalBuildPath);
    }

    [MenuItem(BUILD_AGAIN)]
    public static void BuildAgain()
    {
        EditorUtility.DisplayProgressBar("Building", "Building to '{0}'".Form(BuildPath), 0f);
        string[] scenes = new string[]
        {
            "Assets/Scenes/Loading Scene.unity",
            "Assets/Scenes/Dev.unity"
        };

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.locationPathName = FinalBuildPath;
        options.scenes = scenes;
        options.targetGroup = BuildTargetGroup.Standalone;
        options.target = BuildTarget.StandaloneWindows64;

        var report = BuildPipeline.BuildPlayer(options);
        EditorUtility.ClearProgressBar();

        const float MB = (1024 * 1024);
        Debug.Log("Built in {0} seconds, {1} MB total (over {2} files).".Form(report.summary.totalTime.TotalSeconds, (report.summary.totalSize / MB).ToString("N1"), report.files.Length));
    }

    [MenuItem(RUN_LATEST_BUILD, validate = true)]
    public static bool CanRunLatest()
    {
        return File.Exists(FinalBuildPath);
    }

    [MenuItem(SELECT_BUILD_PATH)]
    public static void SetBuildPath()
    {
        string initialPath = Directory.GetParent(Application.dataPath).ToString();
        string path = EditorUtility.OpenFilePanelWithFilters("Select Build Path", initialPath, new string[] { "Executable", "exe" });
        path = path.Replace("/", "\\").Replace(initialPath.Replace("/", "\\"), "");

        BuildPath = path;

        Debug.Log("Set relative build path to {0}.".Form(path));
    }

    public const string BUILD_PATH_KEY = "BuildUtils.BuildPath";

    public static string BuildPath
    {
        get
        {
            return EditorPrefs.GetString(BUILD_PATH_KEY, "Builds/Game.exe");
        }
        set
        {
            EditorPrefs.SetString(BUILD_PATH_KEY, value);
        }
    }
}
