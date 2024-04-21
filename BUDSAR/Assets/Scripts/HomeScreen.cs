using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;
using SFB;

public class HomeScreen : MonoBehaviour
{
    public Button uploadButton;
    private string baseURL = "http://buds.ucc-bscs.com/BuDS_Mobile/resume.php";
    private bool isUploading = false;

    void Start()
    {
        if (uploadButton != null)
        {
            uploadButton.onClick.AddListener(UploadResume);
        }
        else
        {
            Debug.LogError("Upload button not assigned.");
        }
    }

    public void UploadResume()
    {
        if (isUploading)
        {
            Debug.LogWarning("Upload process already in progress.");
            return;
        }

        isUploading = true;

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select PDF File", "", "pdf", false);
        if (paths != null && paths.Length > 0)
        {
            StartCoroutine(UploadResumeCoroutine(paths[0]));
        }
        else
        {
            Debug.LogError("No file selected.");
            isUploading = false;
        }
    }

    IEnumerator UploadResumeCoroutine(string filePath)
    {
        byte[] fileData;

        try
        {
            fileData = File.ReadAllBytes(filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading file: " + e.Message);
            isUploading = false;
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, Path.GetFileName(filePath), "application/pdf");

        using (UnityWebRequest www = UnityWebRequest.Post(baseURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("File upload error: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response: " + responseText);

                if (!responseText.Contains("SUCCESS"))
                {
                    SceneManager.LoadScene("SUCCESS APPLY");
                }
            }
        }

        isUploading = false;
    }
    public void ChangeProfile()
    {
        // Pass user ID to Profile.cs when changing scenes
        PlayerPrefs.SetString("ProfileUserId", PlayerPrefs.GetString("UserId", ""));
        SceneManager.LoadScene("PROFILE");
    }

    public void ChangeResume()
    {
        SceneManager.LoadScene("VIEW RESUME");
    }

    public void ChangeBusinessList()
    {
        SceneManager.LoadScene("TRY");
    }
    public void ChangeBusinessAR()
    {
        SceneManager.LoadScene("BUDSAR");
    }
}
