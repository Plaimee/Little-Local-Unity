using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class OutputThirdRawScript : MonoBehaviour
{
    public static OutputThirdRawScript instance;
    public RawImage locationImage;
    public RawImage annotatedImg;
    public RawImage annotatedImgDup;
    public string annotatedImgPath;
    public string oldAnnotatedImgPath;
    public RawImage secOutputBgColor;
    public Camera rawOutput;
    public string capDir = "C:\\BKKDW2025\\photo\\element_output_04\\thirdOutput\\";
    public string capImagePath;
    public GameObject[] cloud;
    public bool check = true;

    void Start()
    {
        instance = this;
        ActivateRandomCloud();
    }

    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrEmpty(SetupScript.instance.annotatedImg) && File.Exists(SetupScript.instance.annotatedImg)) {
            annotatedImgPath = SetupScript.instance.annotatedImg;
            if (check && annotatedImgPath != oldAnnotatedImgPath)
            {
                oldAnnotatedImgPath = annotatedImgPath;
                ShowImage(annotatedImg, annotatedImgPath);
                ShowImage(annotatedImgDup, annotatedImgPath);
                SaveRawOutput();
                check = false;
            }
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

            if (OutputSecondRawScript.instance.cloudIndex >= 0 && cloud.Length > OutputSecondRawScript.instance.cloudIndex)
            {
                cloud[OutputSecondRawScript.instance.cloudIndex]?.SetActive(true);
            }
        }
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
