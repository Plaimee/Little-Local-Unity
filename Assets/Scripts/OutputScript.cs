using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class OutputScript : MonoBehaviour
{
    public static OutputScript instance;
    
    public Camera firstCamera;
    public RawImage firstOutput;
    public string saveDir = "E:\\work\\BKKDW2024\\photo\\output_01\\";
    public string saveImagePath;
    public string rawOutputPath;

    public RawImage stamp;
    public string stampImage;

    public TextMeshProUGUI locationThaiName;
    public string thaiName;
    public TextMeshProUGUI locationLandmark;
    public string landmark;

    private bool check = true;

    public void Start()
    {
        instance = this;
        stampImage = SelectLocationScript.instance.stampImage;
        thaiName = SelectLocationScript.instance.thaiName;
        landmark = SelectLocationScript.instance.landmark;
        rawOutputPath = FirstRawOutputScript.instance.capImagePath;

        if (locationThaiName != null) locationThaiName.text = thaiName;
        if (locationLandmark != null) locationLandmark.text = landmark;
    }

    public void Update()
    {
        rawOutputPath = FirstRawOutputScript.instance.capImagePath;
        if (check && !string.IsNullOrEmpty(stampImage) && !string.IsNullOrEmpty(rawOutputPath))
        {
            ShowImage(firstOutput, rawOutputPath);
            ShowImage(stamp, stampImage);
            SaveImage();
            check = false;
        }
    }

    public void SaveImage()
    {
        try
        {
            if (firstCamera == null)
            {
                Debug.LogError("Target camera is not assigned!");
                return;
            }

            // Set the camera resolution
            int width = Screen.width;
            int height = Screen.height;

            // Create a RenderTexture for the camera
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            firstCamera.targetTexture = renderTexture;

            // Render the camera to the RenderTexture
            RenderTexture.active = renderTexture;
            firstCamera.Render();

            // Create a Texture2D to save the camera's output
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            // Encode the texture to PNG
            saveImagePath = Path.Combine(saveDir, $"{DateTime.Now:dd_MM_yyyy_HHmmss}.png");
            Directory.CreateDirectory(saveDir);
            File.WriteAllBytes(saveImagePath, texture.EncodeToPNG());

            Debug.Log($"Camera view saved to: {saveImagePath}");

            // Cleanup
            RenderTexture.active = null;
            firstCamera.targetTexture = null;
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
