using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Paroxe.PdfRenderer;

public class PDFLoader : MonoBehaviour
{
    public PDFViewer pdfViewer;
    private string pdfUrl;

    public static string profileID;

    public void Start()
    {
        if (Profile.profileID != null)
        {
            Debug.LogWarning("ID RECEIVED: " + Profile.profileID); // Access profileID using Profile.profileID
            SetAppId(Profile.profileID);
        }
        else
        {
            Debug.LogWarning("No profile ID found.");
        }
    }



    public void SetAppId(string appId)
    {
        Debug.Log("SetAppId method called with appId: " + appId);
        Debug.Log("Profile ID: " + profileID);
        pdfUrl = "http://buds.ucc-bscs.com/BuDS_Mobile/get_resume.php?app_id=" + appId;

        // Start loading PDF after setting the URL
        StartCoroutine(LoadPDF());
    }

    IEnumerator LoadPDF()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(pdfUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to load PDF: " + www.error);
            }
            else if (www.result == UnityWebRequest.Result.Success)
            {
                // Load the PDF document from the web URL
                pdfViewer.LoadDocumentFromWeb(pdfUrl, "");
                Debug.Log("PDF loaded successfully.");
            }
        }
    }

}
