using UnityEditor;
using UnityEngine;
using System;

//------------------------------------------------------------------------------------------------------------------------------

public class BuildWindow : EditorWindow
{
    BuildOptions buildOptions = BuildOptions.None;
    static BuildSO buildSO;
    const string localURL = "localhost:8000/";
    const string stagingURL = "https://stagingURL.com/";
    const string prodURL = "https://productionAPIURL.com/";
    const string appName = "NameOfApp";
    const string buildPath = @"../../Builds/";
    string gitHash = null;

    const string readmeMacWebFileName = "READ_ME_-_Mac.txt";
    const string readmeWindowsWebFileName = "READ_ME_-_Windows.txt";
    const string readmeLinuxWebFileName = "READ_ME_-_Linux.txt";

    string[] allLevels = {
        "Assets/Scenes/main.unity",
    };
    //------------------------------------------------------------------
    // adds a Unity Editor option named "Build Window" to the Window menu
    //------------------------------------------------------------------
    [MenuItem("Build/Build Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BuildWindow window = (BuildWindow)EditorWindow.GetWindow(typeof(BuildWindow));
        //Get the reference to the build scriptable object.
        buildSO = (BuildSO)AssetDatabase.LoadAssetAtPath("Assets/Editor/NewBuildSO.asset", typeof(BuildSO));
    }

    public enum BUILD_PLATFORM
    {
        WINDOWS = 0,
        MAC = 1,
        LINUX = 2
    }
    public BUILD_PLATFORM build_platform;
    public string[] platform_strings = { "windows", "mac", "linux" };

    public enum BUILD_API
    {
        LOCAL = 0,
        STAGING = 1,
        PRODUCTION = 2
    }
    public BUILD_API build_api;
    public string[] target_api_strings = { "local", "staging", "production" };
    public string[] target_api_URL_strings = { localURL, stagingURL, prodURL };

    public enum BUILD_SEMESTER
    {
        FALL2019 = 0,
        SPRING2020 = 1
    }
    public BUILD_SEMESTER build_semester;
    public string[] semester_strings = { "fall2019", "spring2020" };

    public enum BUILD_INSTITUTION
    {
        WEBSITE = 0,
        DEMO = 1
    }
    public BUILD_INSTITUTION build_institution;
    public string[] institution_strings = { "Website", "Demo" };

    void OnGUI()
    {
        //If the project recompiles, it loses the buildSO reference, so we grab it again if it is null.
        if (buildSO == null)
        {
            buildSO = (BuildSO)AssetDatabase.LoadAssetAtPath("Assets/Editor/BuildSO.asset", typeof(BuildSO));
        }
        EditorGUILayout.LabelField("Current platform target is: " + buildSO.targetPlatform);
        build_platform = (BUILD_PLATFORM)EditorGUILayout.EnumPopup("select target platform:", build_platform);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Current api target is: " + buildSO.apiTarget);
        build_api = (BUILD_API)EditorGUILayout.EnumPopup("select target platform:", build_api);

        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Current Semester target is: " + buildSO.semester);
        build_semester = (BUILD_SEMESTER)EditorGUILayout.EnumPopup("select target platform:", build_semester);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Current Build type is: " + buildSO.institution);
        build_institution = (BUILD_INSTITUTION)EditorGUILayout.EnumPopup("select target platform:", build_institution);

        EditorGUILayout.Space();

        if (GUILayout.Button(" Build Game "))
        {
            Debug.Log("Building game");
            SetSOValues();
            BuildSetup();
        }
        if (GUILayout.Button(" Just Change BuildSO "))
        {
            Debug.Log("Changing buildSO");
            SetSOValues();
        }
    }

    BuildSO SetSOValues()
    {
        gitHash = GetGitCommitHash();

        buildSO.apiTarget = target_api_strings[(int)build_api];
        buildSO.baseAPIURL = target_api_URL_strings[(int)build_api];
        buildSO.semester = semester_strings[(int)build_semester];
        buildSO.institution = institution_strings[(int)build_institution];
        buildSO.targetPlatform = platform_strings[(int)build_platform];
        buildSO.GameVersion = buildSO.institution + "." + buildSO.semester + "." + buildSO.apiTarget[0] + "." + gitHash;

        /* This saves the scriptable object.
        Without it, the settings aren't saved before the build.
        */
        EditorUtility.SetDirty(buildSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (buildSO.apiTarget == null)
        {
            Debug.LogError("Server target is null");
        }
        if (buildSO.semester == null)
        {
            Debug.LogError("Semester is null");
        }
        if (buildSO.institution == null)
        {
            Debug.LogError("Institution is null");
        }
        if (buildSO.targetPlatform == null)
        {
            Debug.LogError("No build target platform set");
        }

        Debug.Log("institution: " + buildSO.institution);
        Debug.Log("apiurl: " + buildSO.baseAPIURL);
        Debug.Log("semester: " + buildSO.semester);
        Debug.Log("buildOptions: " + buildOptions);
        return buildSO;
    }

    void BuildSetup()
    {

        string today = DateTime.Now.ToString("M-d-yyyy");
        string macFilePath = buildPath + buildSO.institution + "_" + today + "_" + buildSO.apiTarget + @"_Mac/" + appName;
        string macFileName = macFilePath + @"/" + appName + @".app";
        string windowsFilePath = buildPath + buildSO.institution + "_" + today + "_" + buildSO.apiTarget + @"_Windows/" + appName;
        string windowsFileName = windowsFilePath + @"/" + appName + @".exe";
        string linuxFilePath = buildPath + buildSO.institution + "_" + today + "_" + buildSO.apiTarget + @"_Linux/" + appName;
        string linuxFileName = linuxFilePath + @"/" + appName;

        string readmeMacFileName = readmeMacWebFileName;
        string readmeWindowsFileName = readmeWindowsWebFileName;
        string readmeLinuxFileName = readmeLinuxWebFileName;

        if (buildSO.targetPlatform == "mac")
        {
            BuildGame(macFilePath, macFileName, readmeMacFileName, allLevels, BuildTarget.StandaloneOSX);
        }
        if (buildSO.targetPlatform == "windows")
        {
            BuildGame(windowsFilePath, windowsFileName, readmeWindowsFileName, allLevels, BuildTarget.StandaloneWindows);
        }
        if (buildSO.targetPlatform == "linux")
        {
            BuildGame(linuxFilePath, linuxFileName, readmeLinuxFileName, allLevels, BuildTarget.StandaloneLinux64);
        }
    }

    private string GetGitCommitHash()
    {
        string gitHashCommand = "git rev-parse HEAD";
        System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + gitHashCommand);
        procStartInfo.RedirectStandardError = procStartInfo.RedirectStandardInput = procStartInfo.RedirectStandardOutput = true;
        procStartInfo.UseShellExecute = false;

        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo = procStartInfo;
        p.Start();
        return p.StandardOutput.ReadToEnd();
    }

    void BuildGame(string appPath, string fileName, string readmeFileName, string[] curLevels, BuildTarget buildTarget)
    {
        BuildPipeline.BuildPlayer(curLevels, fileName, buildTarget, buildOptions);
        //copy the readme file
        FileUtil.CopyFileOrDirectory(Application.dataPath + @"/README/" + readmeFileName, appPath + @"/" + readmeFileName);
    }
}
