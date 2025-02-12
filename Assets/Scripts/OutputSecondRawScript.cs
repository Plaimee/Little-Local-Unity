using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class OutputSecondRawScript : MonoBehaviour
{
    public static OutputSecondRawScript instance;
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
    public GameObject[] leftEyes, rightEyes, noses, mouths;
    public List<KeypointsData> jsonObjList;
    public string kpObj;
    public RawImage locationImage;
    public RawImage orgImage;
    public string orgImagePath;
    public string oldOrgImagePath;
    public RawImage colorBg;
    public Vector2 uvCoordinates;
    public Camera rawOutput;
    public string capDir = "C:\\BKKDW2025\\photo\\element_output_04\\secondOutput\\";
    public string capImagePath;
    public GameObject[] cloud;
    public int cloudIndex;
    private bool check = true;

    void Start()
    {
        instance = this;
        ActivateRandomCloud();
    }

    void Update()
    {
        if(!string.IsNullOrEmpty(SetupScript.instance.keyPointsData) && !string.IsNullOrEmpty(SetupScript.instance.removedOrgBg) && File.Exists(SetupScript.instance.removedOrgBg)){
            kpObj = SetupScript.instance.keyPointsData;
            orgImagePath = SetupScript.instance.removedOrgBg;
            if (check && !string.IsNullOrEmpty(kpObj) && !string.IsNullOrEmpty(orgImagePath) && kpObj == OutputFirstRawScript.instance.kpObj && locationImage != null)
            {
                UpdateKeypointData(kpObj);
                if (jsonObjList != null && jsonObjList.Count > 0)
                {
                    for (int i = 0; i < jsonObjList.Count; i++)
                    {
                        UpdateKeypointPositions(jsonObjList[i], i);
                    }
                    ShowImage(orgImage, orgImagePath);
                    PickColor();
                    SaveRawOutput();
                    check = false;
                }
                else
                {
                    Debug.LogError("Parsed keypoints data is null!");
                }
            }
        }
    }

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

    public void PickColor()
    {
        if (orgImage.texture is Texture2D texture2D && locationImage.texture is Texture2D locationTexture)
        {
            if (!texture2D.isReadable || !locationTexture.isReadable)
            {
                Debug.LogError("One or both textures are not readable. Enable 'Read/Write Enabled' in the texture's import settings.");
                return;
            }

            // Generate random UV coordinates
            Vector2 uv1 = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
            Vector2 uv2 = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);

            // Convert UV to pixel coordinates for orgImage
            int x1 = Mathf.Clamp((int)(uv1.x * texture2D.width), 0, texture2D.width - 1);
            int y1 = Mathf.Clamp((int)(uv1.y * texture2D.height), 0, texture2D.height - 1);

            // Convert UV to pixel coordinates for locationImage
            int x2 = Mathf.Clamp((int)(uv2.x * locationTexture.width), 0, locationTexture.width - 1);
            int y2 = Mathf.Clamp((int)(uv2.y * locationTexture.height), 0, locationTexture.height - 1);

            // Pick two colors: one from orgImage, one from locationImage
            Color color1 = texture2D.GetPixel(x1, y1);
            Color color2 = locationTexture.GetPixel(x2, y2);

            // Generate a gradient texture
            Texture2D gradientTexture = CreateGradientTexture(color1, color2, 256);

            // Apply the gradient texture to colorBg
            colorBg.texture = gradientTexture;

            Debug.Log($"Picked Colors: {color1} from orgImage, {color2} from locationImage");
        }
        else
        {
            Debug.LogError("One or both source textures are not a Texture2D or are missing.");
        }
    }

    private Texture2D CreateGradientTexture(Color color1, Color color2, int height)
    {
        int width = 1; // Single-column texture for vertical gradient
        Texture2D gradientTexture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            float t = y / (float)(height - 1);
            Color gradientColor = Color.Lerp(color1, color2, t); // Top to Bottom
            gradientTexture.SetPixel(0, y, gradientColor);
        }

        gradientTexture.Apply();
        return gradientTexture;
    }

    public void ActivateRandomCloud()
    {
        if(cloud != null) {
            DisableAllObjects(cloud);

            cloudIndex = ActivateRandomCloud(cloud);
        }
    }

    private int ActivateRandomCloud(GameObject[] objects)
    {
        if (objects == null || objects.Length == 0) return -1;

        int randomIndex = UnityEngine.Random.Range(0, objects.Length);

        if (objects[randomIndex] != null)
        {
            objects[randomIndex].SetActive(true);
            Debug.Log("Activated: " + objects[randomIndex].name);
        }

        return randomIndex;
    }

    private void DisableAllObjects(GameObject[] objects)
    {
        if (objects == null) return;
        foreach (GameObject obj in objects)
        {
            if (obj != null) obj.SetActive(false);
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
            capImagePath = Path.Combine(capDir, $"{DateTime.Now:yyyyMMdd_HHmmss}.png");
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
}
