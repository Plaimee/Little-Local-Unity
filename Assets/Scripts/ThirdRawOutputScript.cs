using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ThirdRawOutputScript : MonoBehaviour
{
    public static ThirdRawOutputScript instance;
    public RawImage locationImage;
    public RawImage annotatedImg;
    public RawImage annotatedImgDup;
    public string annotatedImgPath;
    public string oldAnnotatedImgPath;
    public RawImage secOutputBgColor;
    public RawImage colorBg;
    public Vector2 uvCoordinates;
    public Camera rawOutput;
    public string capDir = "C:\\BKKDW2025\\photo\\element_output_04\\thirdOutput\\";
    public string capImagePath;
    public GameObject[] cloud;
    public bool check = true;

    void Start()
    {
        instance = this;
        annotatedImgPath = SetupScript.instance.annotatedImg;
        ActivateRandomCloud();
    }

    // Update is called once per frame
    void Update()
    {
        if(SetupScript.instance.annotatedImg != null && !string.IsNullOrEmpty(SetupScript.instance.annotatedImg)) {
            annotatedImgPath = SetupScript.instance.annotatedImg;
            if (check && annotatedImgPath != oldAnnotatedImgPath)
            {
                oldAnnotatedImgPath = annotatedImgPath;
                ShowImage(annotatedImg, annotatedImgPath);
                ShowImage(annotatedImgDup, annotatedImgPath);
                PickColor();
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

    public void PickColor()
    {
        if (annotatedImg.texture is Texture2D texture2D && locationImage.texture is Texture2D locationTexture)
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

            if (SecRawOutputScript.instance.cloudIndex >= 0 && cloud.Length > SecRawOutputScript.instance.cloudIndex)
            {
                cloud[SecRawOutputScript.instance.cloudIndex]?.SetActive(true);
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
