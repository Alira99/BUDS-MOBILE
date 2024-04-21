using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BusinessNameFetcher : MonoBehaviour
{
    public Text textField;
    private string apiUrl = "https://www.ucc-buds.com/get_coordinates.php"; // Replace this with your API URL

    void Start()
    {
        StartCoroutine(GetJsonData());
    }

    IEnumerator GetJsonData() // Change the return type to IEnumerator
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error retrieving JSON data: " + webRequest.error);
            }
            else
            {
                // Parse JSON data
                string jsonData = webRequest.downloadHandler.text;
                Debug.Log("JSON Data: " + jsonData); // Log the JSON data received

                BusinessData businessData = JsonUtility.FromJson<BusinessData>(jsonData);

                // Display the business names in the UI or associate them with 3D objects as needed
                if (businessData != null && businessData.Coordinates != null && businessData.Coordinates.Count > 0)
                {
                    // Iterate through each coordinate and display its corresponding business name
                    foreach (var coordinate in businessData.Coordinates)
                    {
                        textField.text += coordinate.Name + "\n";

                        // Debug log to verify the name for each coordinate
                        Debug.Log("Coordinate (" + coordinate.Latitude + ", " + coordinate.Longitude + ") has name: " + coordinate.Name);
                    }
                }
                else
                {
                    textField.text = "No business names found.";
                }
            }
        }
    }
}

[System.Serializable]
public class CoordinateData
{
    public float Latitude;
    public float Longitude;
    public string Name;
}

[System.Serializable]
public class BusinessData
{
    public List<CoordinateData> Coordinates;
}