using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> onClick)
    {
        button.onClick.AddListener(delegate ()
        {
            onClick(param);
        });
    }
}

public class BusinessList : MonoBehaviour
{
    [Serializable]
    public struct Game
    {
        public string Name;
        public string Address;
        public string ImageURL;
        public Sprite Image;
        public string ID;
    }

    Game[] allGames;

    [SerializeField] Sprite defaultIcon;

    public GameObject buttonTemplate;

    void Start()
    {
        StartCoroutine (GetGames());
    }

    void DrawUI ()
    {
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        int N = allGames.Length;

        for (int i = 0; i < N; i++) {
            g = Instantiate(buttonTemplate, transform);
            g.transform.GetChild (0).GetComponent <Image> ().sprite = allGames [i].Image;
            g.transform.GetChild (1).GetComponent <Text> ().text = allGames [i].Name;
            g.transform.GetChild (2).GetComponent <Text> ().text = allGames [i].Address;
            g.transform.GetChild (3).GetComponent <Text> ().text = allGames [i].ID;

            g.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }

        Destroy(buttonTemplate);
    }

    void ItemClicked(int itemIndex)
    {
        // Set the selected object's ID
        PlayerPrefs.SetInt("SelectedObjectID", int.Parse(allGames[itemIndex].ID));

        // Load the BUSINESSPROFILE scene
        SceneManager.LoadScene("BUSINESSPROFILE");
    }


    // ********************************************************************************************************************************

    IEnumerator GetGames()
    {
        string url = "http://buds.ucc-bscs.com/BuDS_Mobile/businesslist.php";

        UnityWebRequest request = UnityWebRequest.Get (url);
        request.chunkedTransfer = false;
        yield return request.Send ();

        if (request.isNetworkError){
         }else{
            if (request.isDone){
                allGames = JsonHelper.GetArray<Game>(request.downloadHandler.text);
                StartCoroutine (GetGamesIcones ());
            }
        }

    }

    IEnumerator GetGamesIcones()
    {
        for (int i = 0; i < allGames.Length; i++) {
            WWW w = new WWW (allGames [i].ImageURL);
            yield return w;

            if (w.error != null)
            {
                allGames [i].Image = defaultIcon;
            }
            else{
                if(w.isDone){
                    Texture2D tx = w.texture;
                    allGames [i].Image = Sprite.Create(tx, new Rect(0f,0f,tx.width,tx.height),Vector2.zero, 10f);
                }
            }

        }

        DrawUI ();
    }

}
