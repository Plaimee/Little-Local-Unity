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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
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
        webCamTexture = new WebCamTexture(cam, 1920, 1080, 30);
        webCamImage.texture = webCamTexture;

        webCamTexture.Play();
        SceneManager.LoadScene("startScene", LoadSceneMode.Single);
    }

    void Update()
    {
        if(check && !string.IsNullOrEmpty(nfdText)){
            nfdText = null;
            SceneManager.LoadScene("noFaceScene", LoadSceneMode.Single);
            check = false;
        }

        CheckAndUpdate(ref keyPointsData, ref oldKeyPointsData);
        CheckAndUpdate(ref outputId, ref oldOutputId);
        CheckAndUpdate(ref removedOrgBg, ref oldRemovedOrgBg);
        CheckAndUpdate(ref annotatedImg, ref oldAnnotatedImg);

        if (OutputScript.instance != null &&
                !string.IsNullOrEmpty(OutputScript.instance.saveImagePath) &&
                SecOutputScript.instance != null &&
                !string.IsNullOrEmpty(SecOutputScript.instance.saveImagePath) &&
                FourthOutputScript.instance != null &&
                !string.IsNullOrEmpty(FourthOutputScript.instance.saveImagePath))
            {
                characterOutput = OutputScript.instance.saveImagePath;
                secondOutput = SecOutputScript.instance.saveImagePath;
                finalOutput = FourthOutputScript.instance.saveImagePath;

                if (CheckAndUpdate(ref characterOutput, ref oldCharacterOutput) &&
                    CheckAndUpdate(ref secondOutput, ref oldSecondOutput) &&
                    CheckAndUpdate(ref finalOutput, ref oldFinalOutput))
                {
                    SendData(characterOutput, secondOutput, finalOutput);
                }
            }
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
