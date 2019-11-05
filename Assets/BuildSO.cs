using UnityEngine;

[CreateAssetMenu(menuName = "SOs/BuildSOs")]
public class BuildSO : ScriptableObject
{
    public string institution;
    public string semester;
    public string targetPlatform;
    public string apiTarget;
    public string baseAPIURL = "localhost:8000/";
    public string GameVersion;
}
