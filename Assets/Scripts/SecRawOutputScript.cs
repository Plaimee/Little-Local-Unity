using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static FirstRawOutputScript;

public class SecRawOutputScript : MonoBehaviour
{
    public static SecRawOutputScript instance;
    public GameObject[] pdEyes;
    public GameObject[] gtwEyes;
    public GameObject[] matchMouth;
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject nose;
    public GameObject mouth;

    public RawImage orgImage;
    public string orgImagePath;

    public RawImage colorBg;
    public Vector2 uvCoordinates;

    public Camera rawOutput;
    public string capDir = "E:\\work\\BKKDW2024\\photo\\element_output_04\\secondOutput\\";
    public string capImagePath;

    private bool check = true;

    void Start()
    {
        instance = this;
        orgImagePath = WebcamScript.instance.imgPath;
        ShowImage(orgImage, orgImagePath);
    }

    void Update()
    {
        LittleCtrlScript littleCtrl = FindObjectOfType<LittleCtrlScript>();
        if (littleCtrl != null)
        {
            MatchActiveGameObjects(littleCtrl);
        }
        else
        {
            Debug.LogError("LittleCtrlScript instance not found in the scene.");
        }

        if (check || string.IsNullOrEmpty(orgImagePath))
        {
            orgImagePath = WebcamScript.instance.imgPath;
            if (!string.IsNullOrEmpty(orgImagePath))
            {
                MatchPosition();
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

    public void MatchPosition()
    {
        if (FirstRawOutputScript.instance == null || FirstRawOutputScript.instance.jsonObjList == null)
        {
            Debug.LogError("FirstRawOutputScript instance or jsonObjList is null.");
            return;
        }

        // Iterate over all keypoint data sets
        foreach (KeypointsData keypoints in FirstRawOutputScript.instance.jsonObjList)
        {
            UpdatePosition(leftEye, keypoints.left_eye);
            UpdatePosition(rightEye, keypoints.right_eye);
            UpdatePosition(nose, keypoints.nose);
            UpdatePosition(mouth, keypoints.mouth);
        }
    }

    private void UpdatePosition(GameObject target, Keypoint keypoint)
    {
        if (target != null && keypoint != null)
        {
            // Invert the Y position to match the coordinates
            Vector3 newPos = new Vector3(keypoint.x, -keypoint.y, target.transform.position.z);
            target.transform.position = newPos;
        }
        else
        {
            Debug.LogError("Target GameObject or keypoint is null.");
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
        if (orgImage.texture is Texture2D texture2D)
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

    public void MatchActiveGameObjects(LittleCtrlScript littleCtrl)
    {
        if (littleCtrl == null)
        {
            Debug.LogError("LittleCtrlScript instance is null.");
            return;
        }

        // Get active objects from LittleCtrlScript
        GameObject[] activeEyes = littleCtrl.GetActiveEyes();
        GameObject activeMouth = littleCtrl.GetActiveMouth();

        // Deactivate all pdEyes objects
        foreach (GameObject eye in pdEyes)
        {
            if (eye != null)
                eye.SetActive(false);
        }

        // Activate matching pdEyes objects
        foreach (GameObject activeEye in activeEyes)
        {
            for (int i = 0; i < pdEyes.Length; i++)
            {
                if (pdEyes[i] == activeEye && pdEyes[i] != null)
                {
                    pdEyes[i].SetActive(true);
                    Debug.Log($"Activated pdEye: {pdEyes[i].name}");
                }
            }
        }

        // Deactivate all matchMouth objects
        foreach (GameObject mouthObj in matchMouth)
        {
            if (mouthObj != null)
                mouthObj.SetActive(false);
        }

        // Activate the matching mouth object
        if (activeMouth != null)
        {
            bool mouthFound = false;
            for (int i = 0; i < matchMouth.Length; i++)
            {
                if (matchMouth[i] != null && matchMouth[i].name == activeMouth.name)
                {
                    matchMouth[i].SetActive(true);
                    mouthFound = true;
                    break;
                }
            }

            if (!mouthFound)
            {
                Debug.LogWarning($"Active mouth '{activeMouth.name}' not found in matchMouth array.");
            }
        }
        else
        {
            Debug.LogWarning("No active mouth detected from LittleCtrlScript.");
        }
    }
}
