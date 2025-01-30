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

    public Camera rawOutput;
    public string capDir = "E:\\work\\BKKDW2024\\photo\\element_output_04\\firstOutput\\";
    public string capImagePath;

    public List<KeypointsData> jsonObjList;
    public string kpObj;
    public GameObject[] leftEyes;
    public GameObject[] rightEyes;
    public GameObject[] noses;
    public GameObject[] mouths;

    private bool check = true;

    [System.Serializable]
    public class KeypointsData
    {
        public int face_id;
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
        locationSelected = SelectLocationScript.instance.locationImage;
    }

    private void Update()
    {
        kpObj = SetupScript.instance.keyPointsData;
        if (check || !string.IsNullOrEmpty(kpObj) || !string.IsNullOrEmpty(locationSelected))
        {
            UpdateKeypointData(kpObj);
            if (jsonObjList != null && jsonObjList.Count > 0)
            {
                for (int i = 0; i < jsonObjList.Count; i++)
                {
                    UpdateKeypointPositions(jsonObjList[i], i);
                }
                ShowImage(locationImage, locationSelected);
                SaveRawOutput();
                check = false;
            }
            else
            {
                Debug.LogError("Parsed keypoints data is null!");
            }
        }
    }

    // Update the positions of the keypoints based on new data
    public void UpdateKeypointPositions(KeypointsData keypoints, int index)
    {
        if (index >= leftEyes.Length || index >= rightEyes.Length || index >= noses.Length || index >= mouths.Length)
        {
            Debug.LogError($"Index {index} is out of range for keypoint objects.");
            return;
        }

        if (keypoints.left_eye != null && leftEyes[index] != null)
            leftEyes[index].GetComponent<RectTransform>().anchoredPosition = new Vector3(keypoints.left_eye.x, -keypoints.left_eye.y, 0);

        if (keypoints.right_eye != null && rightEyes[index] != null)
            rightEyes[index].GetComponent<RectTransform>().anchoredPosition = new Vector3(keypoints.right_eye.x, -keypoints.right_eye.y, 0);

        if (keypoints.nose != null && noses[index] != null)
            noses[index].GetComponent<RectTransform>().anchoredPosition = new Vector3(keypoints.nose.x, -keypoints.nose.y, 0);

        if (keypoints.mouth != null && mouths[index] != null)
            mouths[index].GetComponent<RectTransform>().anchoredPosition = new Vector3(keypoints.mouth.x, -keypoints.mouth.y, 0);
    }

    // Deserialize keypoints data from JSON string and store it
    public void UpdateKeypointData(string keypointsJson)
    {
        try
        {
            if (string.IsNullOrEmpty(keypointsJson) || keypointsJson == "[]")
            {
                Debug.LogError("Received empty or invalid JSON data.");
                return;
            }

            // Deserialize JSON safely
            jsonObjList = JsonConvert.DeserializeObject<List<KeypointsData>>(keypointsJson);

            if (jsonObjList == null || jsonObjList.Count == 0)
            {
                Debug.LogError("Parsed keypoints data is null or empty!");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing keypoints data: {ex.Message}\nJSON Data: {keypointsJson}");
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
