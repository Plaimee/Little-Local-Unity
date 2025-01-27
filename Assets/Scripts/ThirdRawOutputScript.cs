using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ThirdRawOutputScript : MonoBehaviour
{
    public static ThirdRawOutputScript instance;
    public RawImage annotatedImg;
    public string annotatedImgPath;

    public RawImage colorBg;
    public Vector2 uvCoordinates;

    public Camera rawOutput;
    public string capDir = "E:\\work\\BKKDW2024\\photo\\element_output_04\\thirdOutput\\";
    public string capImagePath;

    private bool check = true;

    void Start()
    {
        instance = this;
        annotatedImgPath = SetupScript.instance.annotatedImg;
        ShowImage(annotatedImg, annotatedImgPath);
    }

    // Update is called once per frame
    void Update()
    {
        if (check|| string.IsNullOrEmpty(annotatedImgPath))
        {
            annotatedImgPath = SetupScript.instance.annotatedImg;
            if (!string.IsNullOrEmpty(annotatedImgPath))
            {
                PickColor();
                SaveRawOutput();
            }
            else
            {
                Debug.LogError("stampPath or annotatedImgPath is null");
            }
            check = false;
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
        if (annotatedImg.texture is Texture2D texture2D)
        {
            if (!texture2D.isReadable)
            {
                Debug.LogError("Texture is not readable. Enable 'Read/Write Enabled' in the texture's import settings.");
                return;
            }

            // Convert UV coordinates (0-1) to pixel coordinates
            int x = Mathf.Clamp((int)(uvCoordinates.x * texture2D.width), 0, texture2D.width - 1);
            int y = Mathf.Clamp((int)(uvCoordinates.y * texture2D.height), 0, texture2D.height - 1);

            // Get the color from the pixel
            Color pickedColor = texture2D.GetPixel(x, y);

            // Apply the color to the colorBg RawImage
            colorBg.color = pickedColor;

            Debug.Log($"Picked Color: {pickedColor}");
        }
        else
        {
            Debug.LogError("Source texture is not a Texture2D or is missing.");
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
}
