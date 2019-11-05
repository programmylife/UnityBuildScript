using TMPro;
using UnityEngine;

public class TextUpdateScript : MonoBehaviour
{
    public BuildSO buildSO;
    public TextMeshProUGUI InstitutionText;
    public TextMeshProUGUI PlatformText;
    public TextMeshProUGUI APIURLText;
    public TextMeshProUGUI SemesterText;
    public TextMeshProUGUI GameVersionText;
    // Start is called before the first frame update
    void Awake()
    {
        //institution
        InstitutionText.text = "Institution: " + buildSO.institution;

        //platform
        PlatformText.text = "Platform: " + buildSO.targetPlatform;

        //apiurl
        APIURLText.text = "APIURL: " + buildSO.baseAPIURL;

        //semester
        SemesterText.text = " semester: " + buildSO.semester;

        //gameversion
        GameVersionText.text = " GameVersion: " + buildSO.GameVersion;
    }
}
