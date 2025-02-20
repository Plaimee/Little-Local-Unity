using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SetupScript : MonoBehaviour
{
    public SocketIOUnity socket;
    public static SetupScript instance;
    public float lastInteractionTime;
    public float inactivityTimeout = 300f; 
    private bool isStartScene = false;
    public GameObject noInternetMessage;
    public float checkInterval = 5f;
    public WebCamTexture webCamTexture;
    public RawImage webCamImage;
    public string keyPointsData;
    public string oldKeyPointsData;
    public string annotatedImg;
    public string oldAnnotatedImg;
    public string removedOrgBg;
    public string oldRemovedOrgBg;
    private string characterOutput;
    private string oldCharacterOutput;
    private string secondOutput;
    private string oldSecondOutput;
    private string finalOutput;
    private string oldFinalOutput;
    public string nfdText;
    public string outputId;
    public string oldOutputId;
    public bool check;
    private bool isQRSceneActive = false;
    public bool hasResetAfterQR = false;

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        noInternetMessage.SetActive(false);
        StartCoroutine(CheckInternetConnection());
        ResetInteractionTimer();
        
        // Connect Socket
        socket = new SocketIOUnity("http://localhost:3001/");
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("connect to server");
            socket.On("keypointUpdate", (data) =>
            {
                keyPointsData = FixJsonStructure(data.ToString());
                Debug.Log("Received Cleaned JSON: " + keyPointsData);
                check = true;
            });

            socket.On("outputId", (data) => {
                outputId = data.ToString().Trim('[', '"', '"', ']');
                Debug.Log("Received output ID: "+ outputId);
                check = true;
            });

            socket.On("removedOrgBg", (data) => {
                removedOrgBg = data.ToString().Trim('[', '"', '"', ']');
                Debug.Log(removedOrgBg);
                check = true;
            });
            
            socket.On("imageToUnity", (data) =>
            {
                annotatedImg = data.ToString().Trim('[', '"', '"', ']');
                Debug.Log(annotatedImg);
                check = true;
            });

            socket.On("noFaceDetected", (data) => {
                nfdText = data.ToString().Trim('[', '"', '"', ']');
                Debug.Log(nfdText);
                check = true;
            });
        };
        socket.Connect();

        WebCamDevice[] camDevices = WebCamTexture.devices;
        string cam = camDevices.Length > 1 ? camDevices[1].name : camDevices[0].name;
        // string cam = camDevices[0].name;
        webCamTexture = new WebCamTexture(cam, 1920, 1080, 30);
        webCamImage.texture = webCamTexture;

        webCamTexture.Play();
        SceneManager.LoadScene("startScene", LoadSceneMode.Single);
    }

    void Update()
    {
        CheckForInteraction();
        CheckInactivityTimeout();

        if(check && !string.IsNullOrEmpty(nfdText)){
            SceneManager.LoadScene("noFaceScene", LoadSceneMode.Single);
            check = false;
        }

        CheckAndUpdate(ref keyPointsData, ref oldKeyPointsData);
        CheckAndUpdate(ref outputId, ref oldOutputId);
        CheckAndUpdate(ref removedOrgBg, ref oldRemovedOrgBg);
        CheckAndUpdate(ref annotatedImg, ref oldAnnotatedImg);

        if (OutputFirstWithFrameScript.instance != null &&
            !string.IsNullOrEmpty(OutputFirstWithFrameScript.instance.saveImagePath) &&
            OutputSecondWithFrameScript.instance != null &&
            !string.IsNullOrEmpty(OutputSecondWithFrameScript.instance.saveImagePath) &&
            OutputFourthWithFrameScript.instance != null &&
            !string.IsNullOrEmpty(OutputFourthWithFrameScript.instance.saveImagePath))
        {
            characterOutput = OutputFirstWithFrameScript.instance.saveImagePath;
            secondOutput = OutputSecondWithFrameScript.instance.saveImagePath;
            finalOutput = OutputFourthWithFrameScript.instance.saveImagePath;

            if (CheckAndUpdate(ref characterOutput, ref oldCharacterOutput) &&
                CheckAndUpdate(ref secondOutput, ref oldSecondOutput) &&
                CheckAndUpdate(ref finalOutput, ref oldFinalOutput))
            {
                SendData(characterOutput, secondOutput, finalOutput);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isQRSceneActive = scene.name == "qrCodeScene";
        isStartScene = scene.name == "startScene";
        Debug.Log($"Scene loaded: {scene.name}, isQRSceneActive: {isQRSceneActive}, isStartScene: {isStartScene}");
    }

    public void ResetSocketData()
    {
        keyPointsData = null;
        oldKeyPointsData = null;
        annotatedImg = null;
        oldAnnotatedImg = null;
        removedOrgBg = null;
        oldRemovedOrgBg = null;
        characterOutput = null;
        oldCharacterOutput = null;
        secondOutput = null;
        oldSecondOutput = null;
        finalOutput = null;
        oldFinalOutput = null;
        nfdText = null;

        if (hasResetAfterQR)
        {
            outputId = null;
            oldOutputId = null;
        }

        check = true;
        Debug.Log("Reset socket data completed");
    }

    bool CheckAndUpdate(ref string newValue, ref string oldValue)
    {
        if (!string.IsNullOrEmpty(newValue) && newValue != oldValue)
        {
            oldValue = newValue;
            check = false;
            return true;
        }
        return false;
    }

    public void SendData(string characterOutput, string secondOutput, string finalOutput)
    {
        try
        {
            if (string.IsNullOrEmpty(outputId))
            {
                Debug.LogError("❌ No ID not received yet!");
                return;
            }
            var data = new
            {
                outputId,
                characterOutput,
                secondOutput,
                finalOutput,
            };

            if(File.Exists(characterOutput) && File.Exists(secondOutput) && File.Exists(finalOutput)) {
                socket.Emit("sendData", new { noId = outputId, chaop = characterOutput, secop = secondOutput, fiop = finalOutput });
                Debug.Log("📤 Sent Data to Server: " + JsonConvert.SerializeObject(data));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("🚨 Error sending data: " + ex.Message);
        }
    }

    IEnumerator CheckInternetConnection()
    {
        while (true)
        {
            // Check if we can reach Google's DNS server
            bool isConnected = false;
            
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://8.8.8.8"))
            {
                // Send the request and wait for response
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    isConnected = true;
                }
                else
                {
                    isConnected = false;
                }
            }

            // Update UI based on connection status
            noInternetMessage.SetActive(!isConnected);

            // Wait before next check
            yield return new WaitForSeconds(checkInterval);
        }
    }

     private void CheckForInteraction()
    {
        if (isStartScene) return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            ResetInteractionTimer();
        }
    }

    private void ResetInteractionTimer()
    {
        lastInteractionTime = Time.time;
    }

    private void CheckInactivityTimeout()
    {
        if (isStartScene) return;

        if (Time.time - lastInteractionTime >= inactivityTimeout)
        {
            Debug.Log("No interaction detected for 5 minutes. Returning to start scene...");
            ReturnToStartScene();
        }
    }

    private void ReturnToStartScene()
    {
        ResetSocketData();
        ResetInteractionTimer();
        SceneManager.LoadScene("startScene", LoadSceneMode.Single);
    }

    public string FixJsonStructure(string jsonString)
    {
        jsonString = jsonString.Trim();

        if (jsonString.StartsWith("\"") && jsonString.EndsWith("\""))
        {
            jsonString = jsonString.Substring(1, jsonString.Length - 2);
        }

        jsonString = jsonString.Replace("\\\"", "\"");

        if (jsonString.StartsWith("[[") && jsonString.EndsWith("]]"))
        {
            jsonString = jsonString.Substring(1, jsonString.Length - 2);
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
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }

    private void OnDestroy()
    {
        if (socket != null)
        {
            socket.Disconnect();
            socket.Dispose();
        }
    }
}
