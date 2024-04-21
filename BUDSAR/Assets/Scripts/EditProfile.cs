using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class EditProfile : MonoBehaviour
{
    public string baseUrl = "http://buds.ucc-bscs.com/BuDS_Mobile/editprofile.php";
    public InputField updateFirstName;
    public InputField updateMiddleName;
    public InputField updateLastName;
    public Text updateEmail;
    public Text info;

    [System.Serializable]
    public class RegistrationResponse
    {
        public bool success;
        public string message;
        // Add other properties as needed to match your JSON response
    }

    private string userApiUrl = "http://buds.ucc-bscs.com/BuDS_Mobile/profile.php";

    void Start()
    {
        // Fetch user data and populate input fields when the EditProfile scene is loaded
        StartCoroutine(FetchUserDataForEditProfile());
    }

    IEnumerator FetchUserDataForEditProfile()
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
                    string jsonResponse = userRequest.downloadHandler.text;
                    ProfileResponse profileResponse = JsonConvert.DeserializeObject<ProfileResponse>(jsonResponse);
                    SetUserInfoForEditProfile(profileResponse.data.Surname, profileResponse.data.Firstname, profileResponse.data.MiddleName, profileResponse.data.Email);
                }
                catch (JsonException ex)
                {
                    Debug.LogError("Error parsing JSON: " + ex.Message);
                    // Handle JSON parsing error gracefully
                }
            }
        }
    }

    void SetUserInfoForEditProfile(string surname, string firstname, string middleName, string userEmail)
    {
        // Set the input field values with the user's current information
        updateFirstName.text = firstname;
        updateMiddleName.text = middleName;
        updateLastName.text = surname;
        updateEmail.text = userEmail;
    }

    public void AccountUpdate()
    {
        string Firstname = updateFirstName.text;
        string MiddleName = updateMiddleName.text;
        string Surname = updateLastName.text;
        string email = updateEmail.text;

        // Basic client-side validation
        if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(MiddleName) || string.IsNullOrWhiteSpace(Surname))
        {
            info.text = "Please fill in all required fields.";
            return;
        }

        StartCoroutine(UpdateAccount(Firstname, MiddleName, Surname, email));
    }

    IEnumerator UpdateAccount(string Firstname, string MiddleName, string Surname, string email)
    {
        WWWForm form = new WWWForm();
        form.AddField("UpdateFirstName", Firstname);
        form.AddField("UpdateMiddleName", MiddleName);
        form.AddField("UpdateLastName", Surname);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                HandleNetworkError(www.error);
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                HandleHTTPError(www.error);
            }
            else if (www.responseCode == 500)
            {
                HandleServerError();
            }
            else
            {
                HandleSuccessResponse(www.downloadHandler.text);
            }
        }
    }

    void HandleNetworkError(string errorMessage)
    {
        Debug.LogError("Network error: " + errorMessage);
        info.text = "Network error. Please check your connection.";
    }

    void HandleHTTPError(string errorMessage)
    {
        Debug.LogError("HTTP error: " + errorMessage);
        info.text = "HTTP error. Please try again later.";
    }

    void HandleServerError()
    {
        Debug.LogError("Server error 500: Internal Server Error");
        info.text = "Server error. Please contact support.";
    }

    void HandleSuccessResponse(string responseText)
    {
        responseText = responseText.Trim();
        Debug.Log("Response JSON: " + responseText);

        // Deserialize the JSON response using JsonUtility
        RegistrationResponse data = JsonUtility.FromJson<RegistrationResponse>(responseText);

        if (data.success)
        {
            info.text = "UPDATE SUCCESSFULLY";
        }
        else
        {
            info.text = data.message; // Display the error message from the JSON response
        }
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
        public string Surname;
        public string Firstname;
        public string MiddleName;
        public string Email;
    }

    public void ChangeProfile()
    {
        SceneManager.LoadScene("PROFILE");
    }
}
