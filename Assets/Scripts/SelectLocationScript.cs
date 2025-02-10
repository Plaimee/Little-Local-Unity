using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectLocationScript : MonoBehaviour
{
    public static SelectLocationScript instance;
    public GameObject layoutLocation;
    public Button nextLocation;
    public Button previousLocation;
    public Button checkCurrent;

    private int currentIndex = 0;

    public string currentLocationName;
    public string locationImage;
    public string[] locationName = { "TigerGodShrine.png", "TaisunBar.png", "PrinceSommotAmornphanPalace.png", "MahannoparamTemple.png" };
    public string locationDir = Path.Combine(Application.dataPath, "Assets/location/");
    public TextMeshProUGUI placename;
    public string thaiName;
    public string[] locationThaiName = { "ศาลเจ้าพ่อเสือ", "ไท่ขุน บาร์", "วังกรมพระสมมตอมรพันพธ์ุ", "วัดมหรรณพาราม" };
    public string landmark;
    public string[] locationLandmarks = { "13.7538779,100.4969511", "13.752885,100.5036473", "13.7525207,100.5019529", "13.754736,100.4993281" };
    public string stampImage;
    public string[] stampName = { "stamp001.png", "stamp002.png", "stamp003.png", "stamp004.png" };
    public string stampDir = Path.Combine(Application.dataPath, "Assets/stamp/");

    private RectTransform layoutRectTransform;

    void Start()
    {
        instance = this;
        
        if (layoutLocation != null)
        {
            layoutRectTransform = layoutLocation.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Horizontal Layout Group is not assigned.");
        }

        UpdateCurrentLocation();

        if (nextLocation != null && previousLocation != null && checkCurrent != null)
        {
            nextLocation.onClick.AddListener(NextLocation);
            previousLocation.onClick.AddListener(PreviousLocation);
            checkCurrent.onClick.AddListener(CheckCurrentLocation);
        }
        else
        {
            Debug.LogError("Buttons are not assigned in the Inspector.");
        }
    }

    void NextLocation()
    {
        if (layoutLocation != null)
        {
            Transform parentTransform = layoutLocation.transform;

            if (currentIndex < parentTransform.childCount - 1)
            {
                currentIndex++;
                SnapTo(currentIndex);
                UpdateCurrentLocation();
            }
            else
            {
                Debug.Log("Already at the last location.");
            }
        }
    }

    void PreviousLocation()
    {
        if (layoutLocation != null)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                SnapTo(currentIndex);
                UpdateCurrentLocation();
            }
            else
            {
                Debug.Log("Already at the first location.");
            }
        }
    }

    void SnapTo(int index)
    {
        if (layoutRectTransform != null)
        {
            Transform child = layoutLocation.transform.GetChild(index);
            Vector3 targetPosition = -child.localPosition;
            StartCoroutine(SmoothSnap(targetPosition));
        }
    }

    IEnumerator SmoothSnap(Vector3 targetPosition)
    {
        float duration = 0.3f; // Smooth transition time
        float elapsedTime = 0f;
        Vector3 startPosition = layoutRectTransform.anchoredPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            layoutRectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        layoutRectTransform.anchoredPosition = targetPosition; // Ensure it snaps precisely
    }

    void CheckCurrentLocation()
    {
        if (layoutLocation != null)
        {
            Transform parentTransform = layoutLocation.transform;
            if (parentTransform.childCount > 0)
            {
                currentLocationName = parentTransform.GetChild(currentIndex).gameObject.name;
                MatchDetail();
                Debug.Log($"Current Location: {currentLocationName}");
            }
            else
            {
                Debug.LogError("No children in the layout.");
            }
        }
    }

    void UpdateCurrentLocation()
    {
        currentLocationName = layoutLocation.transform.GetChild(currentIndex).gameObject.name;
        MatchDetail();
        placename.text = thaiName;
    }

    public void MatchDetail()
    {
        int index = -1;

        switch (currentLocationName)
        {
            case "TigerGodShrine":
                index = 0;
                break;
            case "TaisunBar":
                index = 1;
                break;
            case "PrinceSommotAmornphanPalace":
                index = 2;
                break;
            case "MahannoparamTemple":
                index = 3;
                break;
        }

        if (index >= 0)
        {
            locationImage = Path.Combine(locationDir, locationName[index]);
            stampImage = Path.Combine(stampDir, stampName[index]);
            thaiName = locationThaiName[index];
            landmark = locationLandmarks[index];
            Debug.Log($"Location Image: {locationImage}, Stamp image is : {stampImage}, Thai Name: {thaiName}, Landmark: {landmark}");
        }
        else
        {
            Debug.LogError("Location name not found in list.");
        }
    }
}
