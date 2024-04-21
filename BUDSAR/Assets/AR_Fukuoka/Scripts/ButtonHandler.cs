using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public void OnButtonClick()
    {
        // Get the selected object's ID
        int selectedObjectID = MarkerInteraction.selectedObjectID;

        // Store the selectedObjectID in PlayerPrefs
        PlayerPrefs.SetInt("SelectedObjectID", selectedObjectID);

        // Load the BusinessProfile scene
        SceneManager.LoadScene("BusinessProfile");
    }
}
