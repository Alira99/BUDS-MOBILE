using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class Login : MonoBehaviour
{
    public string baseUrl = "http://buds.ucc-bscs.com/BuDS_Mobile/login.php"; // 
    public InputField accountEmail;
    public InputField accountPassword;
    public Text info;

    void Start()
    {
        // Initialize your UI elements or other setup if needed
    }

    void Update()
    {
        // Add any update-related logic here if needed
    }

    public void AccountLogin()
    {
        string email = accountEmail.text;
        string password = accountPassword.text;

        // Basic client-side validation
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            info.text = "Please enter both email and password.";
            return;
        }

        StartCoroutine(LogInAccount(email, password));
    }

   IEnumerator LogInAccount(string email, string password)
{
    WWWForm form = new WWWForm();
    form.AddField("LoginEmail", email);
    form.AddField("LoginPassword", password);

    using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
    {
        www.timeout = 10; // Set a timeout value (in seconds)

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            HandleError("Network error. Please check your connection.");
        }
        else if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            HandleError("HTTP error. Please try again later.");
        }
        else
        {
            HandleResponse(www.downloadHandler.text);
        }
    }
}

 void HandleResponse(string responseText)
{
    // Trim any leading/trailing whitespace from the response text
    responseText = responseText.Trim();

    // Log the response text for debugging purposes
    Debug.Log("Response JSON: " + responseText);

    try
    {
        // Deserialize JSON response
        LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(responseText);

        if (response != null)
        {
            if (response.success)
            {
                // Login successful; handle the success scenario here
                info.text = "Login successful.";

                // Store the user ID for later use
                PlayerPrefs.SetString("UserId", response.userId);
                PlayerPrefs.SetString("ownerId", response.ownerId);

                    SceneManager.LoadScene("HOMESCREEN");
            }
            else
            {
                info.text = response.message; // Show the error message to the user.
            }
        }
        else
        {
            Debug.LogError("Invalid JSON response format");
            info.text = "Invalid server response format. Please try again later.";
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError("JSON deserialization error: " + e.Message);
        info.text = "JSON deserialization error. Please try again later.";
    }
}

    void HandleError(string errorMessage)
    {
        Debug.LogError("Error: " + errorMessage);
        info.text = errorMessage;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("REGISTER");
    }
}

[System.Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public string userId; // Add this line to include the user ID
    public string ownerId;
}