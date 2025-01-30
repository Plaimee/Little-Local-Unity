using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleCtrlScript : MonoBehaviour
{
    public GameObject[] pdHead;
    public GameObject[] pdEyes;
    public GameObject pdBody;
    public GameObject[] gtwHead;
    public GameObject[] gtwEyes;
    public GameObject gtwBody;
    public GameObject[] mouth;

    public GameObject ppGroup;
    public GameObject ggGroup;
    public GameObject pgGroup;
    public GameObject face2Element;


    public string littleSelect;
    // Start is called before the first frame update
    void Start()
    {
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
                break;

            case "Gaewtawan":
                gtwBody.SetActive(true);
                pdBody.SetActive(false);
                ppGroup.SetActive(false);
                ggGroup.SetActive(false);
                pgGroup.SetActive(false);
                face2Element.SetActive(false);
                break;

            case "PP":
                ppGroup.SetActive(true);
                gtwBody.SetActive(false);
                pdBody.SetActive(false);
                ggGroup.SetActive(false);
                pgGroup.SetActive(false);
                face2Element.SetActive(true);
                break;

            case "GG":
                ggGroup.SetActive(true);
                ppGroup.SetActive(false);
                gtwBody.SetActive(false);
                pdBody.SetActive(false);
                pgGroup.SetActive(false);
                face2Element.SetActive(true);
                break;

            case "PG":
                pgGroup.SetActive(true);
                ggGroup.SetActive(false);
                ppGroup.SetActive(false);
                gtwBody.SetActive(false);
                pdBody.SetActive(false);
                face2Element.SetActive(true);
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
            if(!string.IsNullOrEmpty(littleSelect) && littleSelect == "Pundaow" || littleSelect == "Gaewtawan")
            {
                GameObject[] headArray = pdBody.activeSelf ? pdHead : gtwHead;
                GameObject[] eyesArrayToEnable = pdBody.activeSelf ? pdEyes : gtwEyes;
                GameObject[] eyesArrayToDisable = pdBody.activeSelf ? gtwEyes : pdEyes;

                // Deactivate all eyes in the "other" group
                foreach (GameObject obj in eyesArrayToDisable)
                {
                    if (obj != null) obj.SetActive(false);
                }

                // Enable all eyes in the active group
                foreach (GameObject obj in eyesArrayToEnable)
                {
                    if (obj != null) obj.SetActive(true);
                }

                // Validate arrays
                if (headArray == null || headArray.Length == 0 || mouth == null || mouth.Length == 0)
                {
                    Debug.LogError("No GameObjects assigned to head or mouth arrays, or the arrays are empty!");
                    return;
                }

                // Deactivate all objects in headArray and mouth
                foreach (GameObject obj in headArray)
                {
                    if (obj != null) obj.SetActive(false);
                }

                foreach (GameObject obj in mouth)
                {
                    if (obj != null) obj.SetActive(false);
                }

                // Randomly activate one object in each array
                int randomHeadIndex = Random.Range(0, headArray.Length);
                int randomMouthIndex = Random.Range(0, mouth.Length);

                if (headArray[randomHeadIndex] != null)
                {
                    headArray[randomHeadIndex].SetActive(true);
                }

                if (mouth[randomMouthIndex] != null)
                {
                    mouth[randomMouthIndex].SetActive(true);
                }

                Debug.Log($"Activated: {headArray[randomHeadIndex]?.name} and {mouth[randomMouthIndex]?.name}");
            }
        }
        catch
        {

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
