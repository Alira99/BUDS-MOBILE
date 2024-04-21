using UnityEngine;
using UnityEngine.UI;
using Budsar;

public class MarkerInteraction : MonoBehaviour
{
    public Camera arCamera; // Reference to the AR camera
    public LayerMask raycastLayer; // Layer mask for raycasting
    public Text textField;
    private SampleScript sampleScript;

    public static int selectedObjectID = -1; // Store the ID of the selected object

    void Start()
    {
        sampleScript = FindObjectOfType<SampleScript>(); // Find the SampleScript component in the scene
    }

    void Update()
    {
        // Create a ray from the AR camera through the center of the screen
        Ray ray = arCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, raycastLayer))
        {
            // Check if the hit object is a 3D object
            if (hit.collider != null)
            {
                // Get the ID from the hit object
                string objectName = hit.collider.gameObject.name;
                int id;
                if (int.TryParse(objectName.Split('_')[1], out id))
                {
                    // Fetch the name from the SampleScript using the ID
                    string name;
                    if (sampleScript.nameMap.TryGetValue(id, out name))
                    {
                        // Store the ID of the selected object
                        selectedObjectID = id;

                        // Check if the business name is already set in PlayerPrefs
                        if (!PlayerPrefs.HasKey("BusinessName"))
                        {
                            // Set the business name in PlayerPrefs only if it's not already set
                            PlayerPrefs.SetString("BusinessName", name);
                        }

                        // Display the name in the text field
                        textField.text = name;

                        // Set the business ID in PlayerPrefs
                        PlayerPrefs.SetInt("SelectedObjectID", selectedObjectID);
                    }
                    else
                    {
                        Debug.LogError("Name not found for ID: " + id);
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse ID from object name.");
                }

                // Do nothing if the marker hits a 3D object
                return;
            }
        }

        // Hide the panel if the marker doesn't hit any 3D object
        HidePanel();
    }

    void HidePanel()
    {
        // Deactivate the panel
        GameObject panel = GameObject.Find("Panel"); // Adjust the name if necessary
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}
