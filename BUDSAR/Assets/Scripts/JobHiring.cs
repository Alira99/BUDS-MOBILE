using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class JobHiring : MonoBehaviour
{
    public Text BusinessName;
    public Text BusinessID;
    public Sprite defaultIcon;
    public Image LogoImage;
    public GameObject detailsPanelPrefab;
    public Transform detailsPanelParent;

    [Serializable]
    public struct BusinessDetails
    {
        public string APP_ID; // Added APP_ID field
        public string Position;
        public string Requirements;
        public string ImageURL;
    }

    void Start()
    {
        string businessID = PlayerPrefs.GetString("BusinessID");
        string businessName = PlayerPrefs.GetString("BusinessName");

        BusinessID.text = businessID;
        BusinessName.text = businessName;

        StartCoroutine(GetBusinessDetails(businessID));
    }

    IEnumerator GetBusinessDetails(string businessID)
    {
        string url = "http://buds.ucc-bscs.com/BuDS_Mobile/jobhiring.php?id=" + businessID;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            List<BusinessDetails> detailsList = JsonConvert.DeserializeObject<List<BusinessDetails>>(request.downloadHandler.text);

            foreach (BusinessDetails details in detailsList)
            {
                CreateDetailsPanel(details);
                StartCoroutine(LoadLogoImage(details.ImageURL));
            }
        }
        else
        {
            Debug.LogError("Failed to fetch business details. Error: " + request.error);
        }
    }

    void CreateDetailsPanel(BusinessDetails details)
    {
        GameObject panel = Instantiate(detailsPanelPrefab, detailsPanelParent);
        panel.SetActive(true);

        Text positionText = panel.transform.Find("BusinessPosition").GetComponent<Text>();
        Text requirementsText = panel.transform.Find("BusinessRequirements").GetComponent<Text>();
        Text appIdText = panel.transform.Find("APP_ID").GetComponent<Text>(); // Add this line to get the AppID text component

        positionText.text = details.Position;
        requirementsText.text = details.Requirements;
        appIdText.text = details.APP_ID; // Set the text for AppID
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
}
