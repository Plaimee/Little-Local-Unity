using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LittleCtrlScript : MonoBehaviour
{
    public static LittleCtrlScript instance;
    public GameObject[] pdHead;
    public GameObject[] pdEyes;
    public GameObject pdBody;
    public GameObject[] gtwHead;
    public GameObject[] gtwEyes;
    public GameObject gtwBody;
    public GameObject[] nose;
    public GameObject[] mouth;

    // second raw output
    public GameObject[] pdEyes2;
    public GameObject[] gtwEyes2;
    public GameObject[] nose2;
    public GameObject[] mouth2;
    public GameObject secFace2Element;

    public GameObject ppGroup;
    public GameObject ggGroup;
    public GameObject pgGroup;
    public GameObject face2Element;
    public string littleSelect;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        littleSelect = CharacterScript.instance.btnName;
        SelectLittleTemplate();
        ActivateRandomGameObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectLittleTemplate()
    {
        switch (littleSelect)
        {
            case "Pundaow":
                pdBody.SetActive(true);
                gtwBody.SetActive(false);
                ppGroup.SetActive(false);
                ggGroup.SetActive(false);
                pgGroup.SetActive(false);
                face2Element.SetActive(false);
                secFace2Element.SetActive(false);
                break;

            case "Gaewtawan":
                gtwBody.SetActive(true);
                pdBody.SetActive(false);
                ppGroup.SetActive(false);
                ggGroup.SetActive(false);
                pgGroup.SetActive(false);
                face2Element.SetActive(false);
                secFace2Element.SetActive(false);
                break;

            case "PP":
                ppGroup.SetActive(true);
                gtwBody.SetActive(false);
                pdBody.SetActive(false);
                ggGroup.SetActive(false);
                pgGroup.SetActive(false);
                face2Element.SetActive(true);
                secFace2Element.SetActive(true);
                break;

            case "GG":
                ggGroup.SetActive(true);
                ppGroup.SetActive(false);
                gtwBody.SetActive(false);
                pdBody.SetActive(false);
                pgGroup.SetActive(false);
                face2Element.SetActive(true);
                secFace2Element.SetActive(true);
                break;

            case "PG":
                pgGroup.SetActive(true);
                ggGroup.SetActive(false);
                ppGroup.SetActive(false);
                gtwBody.SetActive(false);
                pdBody.SetActive(false);
                face2Element.SetActive(true);
                secFace2Element.SetActive(true);
                break;

            default:
                Debug.LogError("Unknown littleSelect value: " + littleSelect);
                break;
        }
    }

    public void ActivateRandomGameObject()
    {
        try
        {
            if (!string.IsNullOrEmpty(littleSelect) && (littleSelect == "Pundaow" || littleSelect == "Gaewtawan"))
            {
                GameObject[] headArray = pdBody.activeSelf ? pdHead : gtwHead;
                GameObject[] eyesArrayToEnable = pdBody.activeSelf ? pdEyes : gtwEyes;
                GameObject[] eyesArrayToDisable = pdBody.activeSelf ? gtwEyes : pdEyes;

                // Deactivate all eyes and mouth in the "other" group
                foreach (GameObject obj in eyesArrayToDisable)
                {
                    if (obj != null) obj.SetActive(false);
                }

                // Deactivate all second output arrays (eyes2, mouth2)
                foreach (GameObject obj in pdEyes2) { if (obj != null) obj.SetActive(false); }
                foreach (GameObject obj in gtwEyes2) { if (obj != null) obj.SetActive(false); }
                foreach (GameObject obj in mouth2) { if (obj != null) obj.SetActive(false); }
                foreach (GameObject obj in nose2) { if (obj != null) obj.SetActive(false); }

                // Enable eyes in the active group
                foreach (GameObject obj in eyesArrayToEnable)
                {
                    if (obj != null)
                    {
                        obj.SetActive(true);

                        // Activate the corresponding second eye if exists
                        int index = Array.IndexOf(eyesArrayToEnable, obj);
                        if (pdBody.activeSelf)
                        {
                            pdEyes2[index]?.SetActive(true);
                        }
                        else
                        {
                            gtwEyes2[index]?.SetActive(true);
                        }
                    }
                }

                // Validate arrays
                if (headArray == null || headArray.Length == 0 || mouth == null || mouth.Length == 0 || nose == null || nose.Length == 0)
                {
                    Debug.LogError("No GameObjects assigned to head or nose or mouth arrays, or the arrays are empty!");
                    return;
                }

                // Deactivate all objects in headArray and mouthArray
                foreach (GameObject obj in headArray)
                {
                    if (obj != null) obj.SetActive(false);
                }

                foreach (GameObject obj in nose)
                {
                    if (obj != null) obj.SetActive(false);
                }

                foreach (GameObject obj in mouth)
                {
                    if (obj != null) obj.SetActive(false);
                }

                // Randomly activate one object in each array
                int randomHeadIndex = Random.Range(0, headArray.Length);
                int randomNoseIndex = Random.Range(0, nose.Length);
                int randomMouthIndex = Random.Range(0, mouth.Length);
                GameObject activatedHead = headArray[randomHeadIndex];
                GameObject activatedNose = nose[randomNoseIndex];
                GameObject activatedMouth = mouth[randomMouthIndex];

                // Activate head
                if (activatedHead != null)
                {
                    activatedHead.SetActive(true);
                }

                if (activatedNose != null)
                {
                    activatedNose.SetActive(true);
                    nose2[randomNoseIndex]?.SetActive(true);
                }
                
                // Activate mouth
                if (activatedMouth != null)
                {
                    activatedMouth.SetActive(true);
                    mouth2[randomMouthIndex]?.SetActive(true);
                }

                Debug.Log($"Activated: {headArray[randomHeadIndex]?.name}, {nose[randomNoseIndex]?.name} and {mouth[randomMouthIndex]?.name}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in ActivateRandomGameObject: {ex.Message}");
        }
    }

    public GameObject GetActiveMouth()
    {
        foreach (GameObject m in mouth)
        {
            if (m != null && m.activeSelf)
            {
                return m;
            }
        }
        return null;
    }

    public GameObject[] GetActiveEyes()
    {
        List<GameObject> activeEyes = new List<GameObject>();
        GameObject[] eyesArray = pdBody.activeSelf ? pdEyes : gtwEyes;

        foreach (GameObject eye in eyesArray)
        {
            if (eye != null && eye.activeSelf)
            {
                activeEyes.Add(eye);
            }
        }
        return activeEyes.ToArray();
    }

    private void DisableAllObjects(GameObject[] objects)
    {
        if (objects == null) return;
        foreach (GameObject obj in objects)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}
