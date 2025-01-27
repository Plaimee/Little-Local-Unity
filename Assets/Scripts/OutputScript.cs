using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public string checkStamp;
    public RawImage stamp;
    public string[] stampName = { "stamp003.png" };
    public string stampPath = Path.Combine(Application.dataPath, "Assets/stamp/");
    public string stampImagePath;

    private bool check = true;

    public void Start()
    {
        instance = this;
        checkStamp = SelectLocationScript.instance.currentLocationName;
        rawOutputPath = FirstRawOutputScript.instance.capImagePath;
    }

    public void Update()
    {
        rawOutputPath = FirstRawOutputScript.instance.capImagePath;
        if (check && !string.IsNullOrEmpty(checkStamp) && !string.IsNullOrEmpty(rawOutputPath))
        {
            DisplayImages();
            ShowImage(firstOutput, rawOutputPath);
            SaveImage();
            check = false;
        }
    }

    public void DisplayImages()
    {
        try
        {
            if (!string.IsNullOrEmpty(checkStamp) && checkStamp == "003")
            {
                stampImagePath = $"{stampPath}{stampName[0]}";
                ShowImage(stamp, stampImagePath); 
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
