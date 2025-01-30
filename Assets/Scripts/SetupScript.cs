using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SocketIOClient;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupScript : MonoBehaviour
{
    public SocketIOUnity socket;
    public static SetupScript instance;
    public WebCamTexture webCamTexture;
    public RawImage webCamImage;
    public string keyPointsData;
    public string oldKeyPointsData;
    public string annotatedImg;
    public string oldAnnotatedImg;
    public bool check;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        // Connect Socket
        socket = new SocketIOUnity("http://localhost:3000/");
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("connect to server");

            socket.On("keypointUpdate", (data) =>
            {
                keyPointsData = FixJsonStructure(data.ToString());
                Debug.Log("Received Cleaned JSON: " + keyPointsData);
                check = true;
            });
            
            socket.On("imageToUnity", (data) =>
            {
                annotatedImg = data.ToString().Trim('[', '"', '"', ']');
                Debug.Log(annotatedImg);
                check = true;
            });
        };
        socket.Connect();

        ////Save get the camera devices, in case you have more than 1 camera.
        WebCamDevice[] camDevices = WebCamTexture.devices;
        //Get the used camera name for the WebCamTexture initialization.
        string cam = camDevices[0].name;
        webCamTexture = new WebCamTexture(cam);
        webCamImage.texture = webCamTexture;

        webCamTexture.Play();
        SceneManager.LoadScene("startScene", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrEmpty(keyPointsData))
        {
            if(check && keyPointsData != oldKeyPointsData)
            {
                oldKeyPointsData = keyPointsData;
                check = false;
            }
        }

        if(!string.IsNullOrEmpty(annotatedImg))
        {
            if(check && annotatedImg != oldAnnotatedImg)
            {
                oldAnnotatedImg = annotatedImg;
                check = false;

            }
        }
    }

    public string FixJsonStructure(string jsonString)
    {
        jsonString = jsonString.Trim();

        if (jsonString.StartsWith("\"") && jsonString.EndsWith("\""))
        {
            jsonString = jsonString.Substring(1, jsonString.Length - 2); // Remove surrounding quotes
        }

        jsonString = jsonString.Replace("\\\"", "\""); // Fix escaped quotes

        // Check if JSON starts with double brackets [[ ... ]]
        if (jsonString.StartsWith("[[") && jsonString.EndsWith("]]"))
        {
            jsonString = jsonString.Substring(1, jsonString.Length - 2); // Remove one layer of brackets
        }

        return jsonString;
    }

    public void StartCamera()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Play();
        }
    }

    public void PauseCamera()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Pause();
        }
    }

    public void StopCamera()
    {
        // หยุดการทำงานของกล้อง
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }
}
