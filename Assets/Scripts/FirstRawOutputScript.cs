using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FirstRawOutputScript : MonoBehaviour
{
    public static FirstRawOutputScript instance;
    public string locationSelected;
    public RawImage locationImage;
    public string[] locationName = { "location003.png" };
    public string imgPath = Path.Combine(Application.dataPath, "Assets/location/");
    public string selectedPath;

    public Camera rawOutput;
    public string capDir = "E:\\work\\BKKDW2024\\photo\\element_output_04\\firstOutput\\";
    public string capImagePath;

    public KeypointsData jsonObj;
    public string kpObj;
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject nose;
    public GameObject mouth;

    private bool check = true;

    [System.Serializable]
    public class KeypointsData
    {
        public Keypoint left_eye;
        public Keypoint right_eye;
        public Keypoint nose;
        public Keypoint mouth;
    }

    [System.Serializable]
    public class Keypoint
    {
        public float x;
        public float y;
    }

    private void Start()
    {
        instance = this;
        kpObj = SetupScript.instance.keyPointsData;
        locationSelected = SelectLocationScript.instance.currentLocationName;
    }

    private void Update()
    {
        if (check || string.IsNullOrEmpty(kpObj) || string.IsNullOrEmpty(locationSelected))
        {
            kpObj = SetupScript.instance.keyPointsData;
            if (!string.IsNullOrEmpty(kpObj))
            {
                UpdateKeypointData(kpObj);
                if (jsonObj != null)
                {
                    UpdateKeypointPositions(jsonObj);
                    DisplayImages();
                    SaveRawOutput();
                }
                else
                {
                    Debug.LogError("Parsed keypoints data is null!");
                }
            }
            check = false;
        }
    }

    // Update the positions of the keypoints based on new data
    public void UpdateKeypointPositions(KeypointsData keypoints)
    {
        // Update the position for each keypoint directly
        if (keypoints.left_eye != null && leftEye != null)
        {
            Vector3 newPos = new Vector3(keypoints.left_eye.x, -keypoints.left_eye.y, 0);
            leftEye.GetComponent<RectTransform>().anchoredPosition = newPos;
        }

        if (keypoints.right_eye != null && rightEye != null)
        {
            Vector3 newPos = new Vector3(keypoints.right_eye.x, -keypoints.right_eye.y, 0);
            rightEye.GetComponent<RectTransform>().anchoredPosition = newPos;
        }

        if (keypoints.nose != null && nose != null)
        {
            Vector3 newPos = new Vector3(keypoints.nose.x, -keypoints.nose.y, 0);
            nose.GetComponent<RectTransform>().anchoredPosition = newPos;
        }

        if (keypoints.mouth != null && mouth != null)
        {
            Vector3 newPos = new Vector3(keypoints.mouth.x, -keypoints.mouth.y, 0);
            mouth.GetComponent<RectTransform>().anchoredPosition = newPos;
        }
    }

    // Deserialize keypoints data from JSON string and store it
    public void UpdateKeypointData(string keypointsJson)
    {
        try
        {
            // If the JSON is an array, deserialize it as a List of Keypoints.
            var keypointsList = JsonConvert.DeserializeObject<List<KeypointsData>>(keypointsJson);

            // Log the keypoints for debugging
            /*            foreach (var keypoint in keypointsList)
                        {
                            Debug.Log($"Left Eye: {keypoint.left_eye.x}, {keypoint.left_eye.y}");
                            Debug.Log($"Right Eye: {keypoint.right_eye.x}, {keypoint.right_eye.y}");
                            Debug.Log($"Nose: {keypoint.nose.x}, {keypoint.nose.y}");
                            Debug.Log($"Mouth: {keypoint.mouth.x}, {keypoint.mouth.y}");
                        }*/

            // Update your data (take the first element of the list, or implement logic for handling multiple items)
            jsonObj = keypointsList[0];  // assuming you only want the first object in the array
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing keypoints data: {ex.Message}");
        }
    }

    public void DisplayImages()
    {
        try
        {
            if (!string.IsNullOrEmpty(locationSelected) && locationSelected == "003")
            {
                selectedPath = $"{imgPath}{locationName[0]}";
                ShowImage(locationImage, selectedPath);
            }
            else
            {
                Debug.LogError("Location Name is null or empty.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error displaying images: {ex.Message}");
        }
    }

    public void SaveRawOutput()
    {
        try
        {
            if (rawOutput == null)
            {
                Debug.LogError("Target camera is not assigned!");
                return;
            }

            // Set the camera resolution
            int width = Screen.width;
            int height = Screen.height;

            // Create a RenderTexture for the camera
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            rawOutput.targetTexture = renderTexture;

            // Render the camera to the RenderTexture
            RenderTexture.active = renderTexture;
            rawOutput.Render();

            // Create a Texture2D to save the camera's output
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            // Encode the texture to PNG
            capImagePath = Path.Combine(capDir, $"{DateTime.Now:dd_MM_yyyy_HHmmss}.png");
            Directory.CreateDirectory(capDir);
            File.WriteAllBytes(capImagePath, texture.EncodeToPNG());

            Debug.Log($"Camera view saved to: {capImagePath}");

            // Cleanup
            RenderTexture.active = null;
            rawOutput.targetTexture = null;
            renderTexture.Release();
            Destroy(renderTexture);
            Destroy(texture);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving camera view: {ex.Message}");
        }
    }

    public void ShowImage(RawImage imageContainer, string imagePath)
    {
        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                imageContainer.texture = texture;
                imageContainer.GetComponent<AspectRatioFitter>().aspectRatio = texture.width / (float)texture.height;
                Debug.Log("Image loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to load image from file: " + imagePath);
            }
        }
        else
        {
            Debug.LogError("Image path is null, empty, or file does not exist.");
        }
    }
}
