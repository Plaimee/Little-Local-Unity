using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ThirdOutputScript : MonoBehaviour
{
    public RawImage stamp;
    public string stampPath;

    public Camera thirdCamera;
    public RawImage thirdOutput;
    public string saveDir = "E:\\work\\BKKDW2024\\photo\\output_03\\";
    public string saveImagePath;

    public string rawOutputPath;

    private bool check = true;

    void Start()
    {
        rawOutputPath = ThirdRawOutputScript.instance.capImagePath;
        stampPath = OutputScript.instance.stampImagePath;
    }

    // Update is called once per frame
    void Update()
    {
        rawOutputPath = ThirdRawOutputScript.instance.capImagePath;
        stampPath = OutputScript.instance.stampImagePath;
        if (check && !string.IsNullOrEmpty(stampPath) && !string.IsNullOrEmpty(rawOutputPath))
        {
            DisplayImages();
            ShowImage(thirdOutput, rawOutputPath);
            SaveImage();
            check = false;
        }
        
    }
    public void DisplayImages()
    {
        try
        {
            if(!string.IsNullOrEmpty(stampPath))
            {
                ShowImage(stamp, stampPath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error displaying images: {ex.Message}");
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

    public void SaveImage()
    {
        try
        {
            if (thirdCamera == null)
            {
                Debug.LogError("Target camera is not assigned!");
                return;
            }

            // Set the camera resolution
            int width = Screen.width;
            int height = Screen.height;

            // Create a RenderTexture for the camera
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            thirdCamera.targetTexture = renderTexture;

            // Render the camera to the RenderTexture
            RenderTexture.active = renderTexture;
            thirdCamera.Render();

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
            thirdCamera.targetTexture = null;
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
