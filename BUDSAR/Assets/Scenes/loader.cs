using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class loader : MonoBehaviour
{
    [SerializeField]
    private string imageURL;

    [SerializeField]
    private RawImage uiRawImage;

    public string TRY;

    public void LoadImage()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            uiRawImage.texture = DownloadHandlerTexture.GetContent(request);
            Debug.Log("Image Loaded Successfully.");

            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene(TRY);
        }
        else
        {
            Debug.LogError("Failed to load image: " + request.error);
        }
    }
}
