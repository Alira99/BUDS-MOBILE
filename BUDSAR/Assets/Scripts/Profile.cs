using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class Profile : MonoBehaviour
{

    public PDFLoader pdfLoader; // Reference to the PDFLoader script

    public string userApiUrl = "http://buds.ucc-bscs.com/BuDS_Mobile/profile.php";
    public Text NameText;
    public Text EmailText;
    public Text IdText;

    public static string profileID; // Static variable to store the ID
    private ProfileResponse profileResponse; // Declare at the class level
    void Start()
    {
        GetUserData();
    }

    void GetUserData()
    {
        StartCoroutine(FetchUserData());
    }

    IEnumerator FetchUserData()
    {
        using (UnityWebRequest userRequest = UnityWebRequest.Get(userApiUrl))
        {
            yield return userRequest.SendWebRequest();

            if (userRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error while fetching user data: " + userRequest.error);
                // Handle error gracefully, e.g., display an error message to the user
            }
            else
            {
                try
                {
                    Debug.Log("Pdf Loader: " + pdfLoader);

                    string jsonResponse = userRequest.downloadHandler.text;
                    ProfileResponse profileResponse = JsonConvert.DeserializeObject<ProfileResponse>(jsonResponse);
                    Debug.Log("Profile Script: ID from profileResponse: " + profileResponse.data.ID);
                    SetUserInfo(profileResponse.data.ID, profileResponse.data.Surname, profileResponse.data.Firstname, profileResponse.data.MiddleName, profileResponse.data.Email);

                    // Pass the ID to PDFLoader
                    Debug.Log("Calling SetAppId with appId: " + profileResponse.data.ID);
                    pdfLoader.SetAppId(profileResponse.data.ID);

                }
                catch (JsonException ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                }
            }
        }
    }

    void SetUserInfo(string id, string surname, string firstname, string middleName, string userEmail)
    {
        IdText.text = $"ID: {id}";
        NameText.text = $"{surname}, {firstname} {middleName}";
        EmailText.text = $"Email: {userEmail}";
    }

    [System.Serializable]
    public class ProfileResponse
    {
        public bool success;
        public UserData data;
    }

    [System.Serializable]
    public class UserData
    {
        public string ID;
        public string Surname;
        public string Firstname;
        public string MiddleName;
        public string Email;
    }

    public void ChangeProfile()
    {
        SceneManager.LoadScene("EDIT PROFILE");
    }

    public void ChangeHome()
    {
        SceneManager.LoadScene("HOMESCREEN");
    }

    public void ChangeLogout()
    {
        SceneManager.LoadScene("WELCOME SCREEN");
    }

    public void ChangePDFViewer()
    {
        profileID = IdText.text.Replace("ID: ", ""); // Extract ID from the UI text
        Debug.Log("Extracted Profile ID: " + profileID); // Debug log to verify extracted ID
        SceneManager.LoadScene("PDFViewer_CanvasScaler_Mobile");
    }

}
