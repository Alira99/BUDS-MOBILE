using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class JobHiringTry : MonoBehaviour
{
    public Text BusinessName;
    public Text BusinessID;
    public Sprite defaultIcon;
    public Image LogoImage;
    public GameObject buttonTemplate;
    public Transform buttonParent;

    [System.Serializable]
    public struct BusinessDetails
    {
        public string ID;
        public string Position;
        public string Requirements;
        public string APP_ID;
        public string ImageURL;
    }

    void Start()
    {
        // Get the selected object's ID from PlayerPrefs
        int selectedObjectID = PlayerPrefs.GetInt("SelectedObjectID");
        string businessID = selectedObjectID.ToString(); // Convert selectedObjectID to string

        // Get the business name from PlayerPrefs
        string businessName = PlayerPrefs.GetString("BusinessName");

        BusinessID.text = businessID;
        BusinessName.text = businessName;

        // Fetch business details using the selected business ID
        StartCoroutine(GetBusinessDetails(businessID));
    }


    IEnumerator GetBusinessDetails(string businessID)
    {
        string url = "http://buds.ucc-bscs.com/BuDS_Mobile/jobhiringtry.php?id=" + businessID;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                List<BusinessDetails> detailsList = JsonConvert.DeserializeObject<List<BusinessDetails>>(jsonResponse);

                foreach (BusinessDetails details in detailsList)
                {
                    SetupButton(details);
                    StartCoroutine(LoadLogoImage(details.ImageURL));
                }

                Destroy(buttonTemplate); // Destroy the button template after all buttons are created
            }
            else
            {
                Debug.LogError("Empty response received.");
            }
        }
        else
        {
            Debug.LogError("Failed to fetch business details. Error: " + request.error);
        }
    }

    void SetupButton(BusinessDetails details)
    {
        GameObject button = Instantiate(buttonTemplate, buttonParent);
        button.SetActive(true);

        Text businessPositionText = button.transform.Find("Text_gamePosition").GetComponent<Text>();
        Text businessRequirementsText = button.transform.Find("Text_gameRequirements").GetComponent<Text>();

        businessPositionText.text = details.Position;
        businessRequirementsText.text = details.Requirements;

        Text appIdText = button.transform.Find("APP_ID").GetComponent<Text>();
        appIdText.text = details.APP_ID;

        Button buttonComponent = button.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => ShowDetailsAndTransition(details));
    }
    IEnumerator LoadLogoImage(string imageURL)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
            LogoImage.sprite = sprite;
        }
        else
        {
            LogoImage.sprite = defaultIcon;
            Debug.LogError("Failed to load logo. Error: " + request.error);
        }
    }

    void ShowDetailsAndTransition(BusinessDetails details)
    {
        // Debug.Log("Business Position: " + details.Position);
        // Debug.Log("Business Requirements: " + details.Requirements);
        Debug.Log("APP ID: " + details.APP_ID);

        // Additional actions if needed

        // Set the APP_ID to PlayerPrefs for use in the next scene
        PlayerPrefs.SetString("APP_ID", details.APP_ID);

        // Load the "APPLYNOW" scene
        SceneManager.LoadScene("APPLYNOW");
    }
}
