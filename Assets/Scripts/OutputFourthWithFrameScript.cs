using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class OutputFourthWithFrameScript : MonoBehaviour
{
    public static OutputFourthWithFrameScript instance;
    public Camera fourthCamera;
    public string saveDir = "C:\\BKKDW2025\\photo\\output_04\\";
    public string saveImagePath;

    public RawImage first;
    public RawImage sec;
    public RawImage third;
    public RawImage fourth;
    public string firstOutput;
    public string secOutput;
    public string thirdOutput;
    public string fouthPath;
    public string fouthDir = Path.Combine(Application.dataPath, "Assets/location/");
    public RawImage stamp;
    public string stampPath;

    public GameObject[] stampRand;

    public TextMeshProUGUI locationThaiName;
    public string thaiName;

    public TextMeshProUGUI locationLandmark;
    public string landmark;
    public TextMeshProUGUI outputIdText;
    public string outputId;
    public bool check = true;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        fouthPath = Path.Combine(fouthDir, "output4.png");
        thaiName = SelectLocationScript.instance.thaiName;
        landmark = SelectLocationScript.instance.landmark;

        if (thaiName != null) locationThaiName.text = thaiName;
        if (landmark != null) locationLandmark.text = landmark;
        ActivateRandomGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrEmpty(OutputFirstRawScript.instance.capImagePath) && File.Exists(OutputFirstRawScript.instance.capImagePath) && !string.IsNullOrEmpty(OutputSecondRawScript.instance.capImagePath) && File.Exists(OutputSecondRawScript.instance.capImagePath) && !string.IsNullOrEmpty(OutputThirdRawScript.instance.capImagePath) && File.Exists(OutputThirdRawScript.instance.capImagePath) && !string.IsNullOrEmpty(SelectLocationScript.instance.stampImage) && File.Exists(SelectLocationScript.instance.stampImage) && !string.IsNullOrEmpty(SetupScript.instance.outputId)) {
            firstOutput = OutputFirstRawScript.instance.capImagePath;
            secOutput = OutputSecondRawScript.instance.capImagePath;
            thirdOutput = OutputThirdRawScript.instance.capImagePath;
            stampPath = SelectLocationScript.instance.stampImage;
            outputId = SetupScript.instance.outputId;
            if (check && !string.IsNullOrEmpty(firstOutput) && File.Exists(firstOutput) && !string.IsNullOrEmpty(secOutput) && File.Exists(secOutput) && !string.IsNullOrEmpty(thirdOutput) && File.Exists(thirdOutput) && !string.IsNullOrEmpty(stampPath) && !string.IsNullOrEmpty(outputId))
            {
                outputIdText.text = outputId;
                DisplayImages();
                ShowImage(first, firstOutput);
                ShowImage(sec, secOutput);
                ShowImage(third, thirdOutput);
                ShowImage(fourth, fouthPath);
                SaveCamera();
                check = false;
            }
        }  
    }

    public void ActivateRandomGameObject()
    {
        if (stampRand.Length == 0)
        {
            Debug.LogError("No GameObjects assigned to the array!");
            return;
        }

        // Deactivate all GameObjects first
        foreach (GameObject obj in stampRand)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Randomly pick one GameObject to activate
        int randomIndex = UnityEngine.Random.Range(0, stampRand.Length);
        if (stampRand[randomIndex] != null)
        {
            stampRand[randomIndex].SetActive(true);
            Debug.Log($"Activated GameObject: {stampRand[randomIndex].name}");
        }
        else
        {
            Debug.LogError("GameObject at the selected random index is null!");
        }
    }

    public void DisplayImages()
    {
        try
        {
            if (!string.IsNullOrEmpty(stampPath))
            {
                ShowImage(stamp, stampPath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error displaying images: {ex.Message}");
        }
    }

    public void SaveCamera()
    {
        try
        {
            if (fourthCamera == null)
            {
                Debug.LogError("Target camera is not assigned!");
                return;
            }

            // Set the camera resolution
            int width = Screen.width;
            int height = Screen.height;

            // Create a RenderTexture for the camera
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            fourthCamera.targetTexture = renderTexture;

            // Render the camera to the RenderTexture
            RenderTexture.active = renderTexture;
            fourthCamera.Render();

            // Create a Texture2D to save the camera's output
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            // Encode the texture to PNG
            saveImagePath = Path.Combine(saveDir, $"{DateTime.Now:yyyyMMdd_HHmmss}.png");
            Directory.CreateDirectory(saveDir); // Ensure the directory exists
            File.WriteAllBytes(saveImagePath, texture.EncodeToPNG());

            Debug.Log($"Camera view saved to: {saveImagePath}");

            // Cleanup
            RenderTexture.active = null;
            fourthCamera.targetTexture = null;
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
