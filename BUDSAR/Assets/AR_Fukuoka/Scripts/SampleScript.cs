using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using UnityEngine.UI;

namespace Budsar
{
    public class SampleScript : MonoBehaviour
    {
        public AREarthManager EarthManager;
        public VpsInitializer Initializer;
        public Text OutputText;
        public double HeadingThreshold = 25;
        public double HorizontalThreshold = 20;
        public GameObject ContentPrefab;
        public ARAnchorManager AnchorManager;
        public double Altitude;
        public double Heading;

        private List<GameObject> displayObjects = new List<GameObject>();
        private CoordinateData[] dynamicCoordinates;
        public Dictionary<int, string> nameMap = new Dictionary<int, string>();

        public string jsonURL = "http://buds.ucc-bscs.com/get_coordinates.php";

        private int currentIndex = 0;

        void Start()
        {
            StartCoroutine(FetchJSONData());
        }

        void Update()
        {
            string status = "";
            if (!Initializer.IsReady || EarthManager.EarthTrackingState != TrackingState.Tracking)
            {
                return;
            }

            GeospatialPose pose = EarthManager.CameraGeospatialPose;

            if (pose.OrientationYawAccuracy > HeadingThreshold || pose.HorizontalAccuracy > HorizontalThreshold)
            {
                status = "Low Tracking Accuracy";
            }
            else
            {
                status = "High Tracking Accuracy";

                if (dynamicCoordinates != null && currentIndex < dynamicCoordinates.Length)
                {
                    CoordinateData coordinate = dynamicCoordinates[currentIndex];
                    double latitude = coordinate.Latitude;
                    double longitude = coordinate.Longitude;

                    Altitude = pose.Altitude - 1.5f;

                    Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);
                    ARGeospatialAnchor anchor = AnchorManager.AddAnchor(latitude, longitude, Altitude, quaternion);

                    if (anchor != null)
                    {
                        GameObject displayObject = Instantiate(ContentPrefab, anchor.transform);
                        displayObject.name = "Object_" + coordinate.Id;
                        displayObjects.Add(displayObject);
                    }

                    currentIndex++;
                }
            }
            ShowTrackingInfo(status, pose);
        }

        void ShowTrackingInfo(string status, GeospatialPose pose)
        {
            OutputText.text = "";

            if (status == "High Tracking Accuracy")
            {
                // Display only the status in the OutputText
                OutputText.text = status;
            }
            else if (status == "Low Tracking Accuracy")
            {
                // Display the status in the OutputText
                OutputText.text = status;
            }
        }




        private IEnumerator FetchJSONData()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(jsonURL))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    ParseAndProcessJSON(jsonResponse);
                }
                else
                {
                    Debug.LogError("Failed to fetch JSON data: " + webRequest.error);
                }
            }
        }

        private void ParseAndProcessJSON(string jsonResponse)
        {
            Debug.Log("Received JSON Response: " + jsonResponse);

            try
            {
                CoordinateDataArrayWrapper wrapper = JsonUtility.FromJson<CoordinateDataArrayWrapper>(jsonResponse);
                dynamicCoordinates = wrapper.Coordinates;

                if (dynamicCoordinates != null && dynamicCoordinates.Length > 0)
                {
                    foreach (var coordinate in dynamicCoordinates)
                    {
                        nameMap.Add(coordinate.Id, coordinate.Name);
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse dynamic coordinates from JSON.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("JSON parsing error: " + e.Message);
            }
        }
        private class CoordinateDataArrayWrapper
        {
            public CoordinateData[] Coordinates;
        }
    }

    [Serializable]
    public class CoordinateData
    {
        public int Id;
        public double Latitude;
        public double Longitude;
        public string Name;
    }
}
