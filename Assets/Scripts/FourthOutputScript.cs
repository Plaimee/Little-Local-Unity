using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FourthOutputScript : MonoBehaviour
{

    public Camera fourthCamera;
    public string saveDir = "E:\\work\\BKKDW2024\\photo\\output_04\\";
    public string saveImagePath;

    public RawImage first;
    public RawImage sec;
    public RawImage third;
    public RawImage firstCopy;
    public string firstOutput;
    public string secOutput;
    public string thirdOutput;
    public string firstOutputCopy;

    public RawImage stamp;
    public string stampPath;

    public GameObject[] stampRand;

    public bool check = true;
    // Start is called before the first frame update
    void Start()
    {
        firstOutput = FirstRawOutputScript.instance.capImagePath;
        secOutput = SecRawOutputScript.instance.capImagePath;
        thirdOutput = ThirdRawOutputScript.instance.capImagePath;
        firstOutputCopy = FirstRawOutputScript.instance.capImagePath;
        stampPath = OutputScript.instance.stampImagePath;
        ActivateRandomGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        firstOutput = FirstRawOutputScript.instance.capImagePath;
        secOutput = SecRawOutputScript.instance.capImagePath;
        thirdOutput = ThirdRawOutputScript.instance.capImagePath;
        firstOutputCopy = FirstRawOutputScript.instance.capImagePath;
        stampPath = OutputScript.instance.stampImagePath;
        if (check && !string.IsNullOrEmpty(firstOutput) && !string.IsNullOrEmpty(secOutput) && !string.IsNullOrEmpty(thirdOutput) && !string.IsNullOrEmpty(firstOutputCopy) && !string.IsNullOrEmpty(stampPath))
        {
            DisplayImages();
            ShowImage(first, firstOutput);
            ShowImage(sec, secOutput);
            ShowImage(third, thirdOutput);
            ShowImage(firstCopy, firstOutputCopy);
            SaveCamera();
            check = false;
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
            saveImagePath = Path.Combine(saveDir, $"{DateTime.Now:dd_MM_yyyy_HHmmss}.png");
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
