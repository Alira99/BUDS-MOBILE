using UnityEngine;

public class RaycastHandler : MonoBehaviour
{
    public Camera arCamera;
    public LayerMask raycastLayer;
    public GameObject marker;
    public GameObject panel;

    void Update()
    {
        // Update the position of the marker to be in the center of the screen
        marker.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Perform raycast from the center of the screen
        Ray ray = arCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Check if the ray hits an object on the specified layer
        if (Physics.Raycast(ray, out hit, float.MaxValue, raycastLayer))
        {
            // Trigger the display of the UI Panel
            ShowPanel();
        }
    }

    void ShowPanel()
    {
        // Activate the panel
        panel.SetActive(true);
    }
}