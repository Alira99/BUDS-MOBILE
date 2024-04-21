using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class Register : MonoBehaviour
{
    public string baseUrl = "http://buds.ucc-bscs.com/BuDS_Mobile/register.php";
    public InputField accountFirstName;
    public InputField accountMiddleName;
    public InputField accountLastName;
    public InputField accountEmail;
    public InputField accountPassword;
    public InputField accountCPassword;
    public Text info;

    [System.Serializable]
    public class RegistrationResponse
    {
        public bool success;
        public string message;
        // Add other properties as needed to match your JSON response
    }

    public void AccountRegister()
    {
        string Firstname = accountFirstName.text;
        string MiddleName = accountMiddleName.text;
        string Surname = accountLastName.text;
        string email = accountEmail.text;
        string password = accountPassword.text;
        string confirmPassword = accountCPassword.text;

        // Basic client-side validation
        if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(MiddleName) || string.IsNullOrWhiteSpace(Surname) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            info.text = "Please fill in all required fields.";
            return;
        }

        StartCoroutine(RegisterNewAccount(Firstname, MiddleName, Surname, email, password, confirmPassword));
    }

    IEnumerator RegisterNewAccount(string Firstname, string MiddleName, string Surname, string email, string password, string confirmPassword)
    {
        WWWForm form = new WWWForm();
        form.AddField("NewAccountFirstName", Firstname);
        form.AddField("NewAccountMiddleName", MiddleName);
        form.AddField("NewAccountLastName", Surname);
        form.AddField("NewAccountEmail", email);
        form.AddField("NewAccountPassword", password);
        form.AddField("NewAccountConfirmpassword", confirmPassword);

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
            // Registration was successful.
            // Optionally, you can navigate to the login scene here.
            SceneManager.LoadScene("LOGIN");
        }
        else
        {
            info.text = data.message; // Display the error message from the JSON response
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("LOGIN");
    }
}