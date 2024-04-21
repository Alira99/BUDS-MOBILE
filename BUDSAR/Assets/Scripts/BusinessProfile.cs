using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class BusinessProfile : MonoBehaviour
{
    public Text BusinessName;
    public Text BusinessAddress;
    public Text BusinessDescrip;
    public Text BusinessNumber;
    public Text BusinessEmail;
    public Text BusinessID;
    public Image LogoImage;
    public Sprite defaultIcon;
    public Button viewJobHiringButton;

    [System.Serializable]
    public struct BusinessDetails
    {
        public string Name;
        public string Address;
        public string Descrip;
        public string Number;
        public string Email;
        public string ImageURL;
    }

    void Start()
    {
        string businessName = PlayerPrefs.GetString("BusinessName");
        string businessAddress = PlayerPrefs.GetString("BusinessAddress");

        BusinessName.text = businessName;

        // Get the selected object's ID from PlayerPrefs
        int selectedObjectID = PlayerPrefs.GetInt("SelectedObjectID");

        // Dynamically set the businessID based on the selectedObjectID
        string businessID = selectedObjectID.ToString(); // Convert selectedObjectID to string

        // Assign the dynamically set businessID to BusinessID text
        BusinessID.text = businessID;

        // Fetch business details using the dynamically set businessID and selectedObjectID
        StartCoroutine(GetBusinessDetails(businessID, selectedObjectID));
    }

    IEnumerator GetBusinessDetails(string businessID, int selectedObjectID)
    {
        string url = "http://buds.ucc-bscs.com/BuDS_Mobile/businessprofile.php?id=" + businessID + "&selectedObjectID=" + selectedObjectID;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            BusinessDetails details = JsonUtility.FromJson<BusinessDetails>(request.downloadHandler.text);
            BusinessName.text = details.Name;
            BusinessAddress.text = details.Address;
            BusinessDescrip.text = details.Descrip;
            BusinessNumber.text = details.Number;
            BusinessEmail.text = details.Email;

            StartCoroutine(LoadLogoImage(details.ImageURL));

            StartCoroutine(CheckJobHirings(businessID, selectedObjectID));
        }
        else
        {
            Debug.LogError("Failed to fetch business details. Error: " + request.error);
        }
    }

    IEnumerator CheckJobHirings(string businessID, int selectedObjectID)
    {
        string url = "http://buds.ucc-bscs.com/BuDS_Mobile/jobhiringtry.php?id=" + businessID + "&selectedObjectID=" + selectedObjectID;
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            List<BusinessDetails> detailsList = JsonConvert.DeserializeObject<List<BusinessDetails>>(request.downloadHandler.text);

            viewJobHiringButton.gameObject.SetActive(detailsList.Count > 0);
        }
        else
        {
            Debug.LogError("Failed to fetch job hirings. Error: " + request.error);
        }
    }

    IEnumerator LoadLogoImage(string ImageURL)
    {
        UnityWebRequest w = UnityWebRequestTexture.GetTexture(ImageURL);
        yield return w.SendWebRequest();

        if (w.result == UnityWebRequest.Result.Success)
        {
            Texture2D tx = DownloadHandlerTexture.GetContent(w);
            Sprite sprite = Sprite.Create(tx, new Rect(0f, 0f, tx.width, tx.height), Vector2.zero, 10f);

            Debug.Log("Sprite size: " + sprite.rect.size);

            LogoImage.sprite = sprite;
        }
        else
        {
            LogoImage.sprite = defaultIcon;
            Debug.LogError("Failed to load logo. Error: " + w.error);
        }
    }
}
