using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PPCtrlScript : MonoBehaviour
{
    public GameObject ppGroup;
    public GameObject[] ppHead1;
    public GameObject[] ppHead2;
    public GameObject[] gtwEyes1;
    public GameObject[] gtwEyes2;
    public GameObject[] ppNose1;
    public GameObject[] ppNose2;
    public GameObject[] ppMouth1;
    public GameObject[] ppMouth2;

    // second raw output
    public GameObject[] secGtwEyes1;
    public GameObject[] secGtwEyes2;
    public GameObject[] secPpNose1;
    public GameObject[] secPpNose2;
    public GameObject[] secPpMouth1;
    public GameObject[] secPpMouth2;

    void Start()
    {
        ActivateRandomGameObject();
    }

    public void ActivateRandomGameObject()
    {
        if (CharacterScript.instance.btnName == "PP")
        {
            DisableAllObjects(gtwEyes1);
            DisableAllObjects(gtwEyes2);
            DisableAllObjects(ppHead1);
            DisableAllObjects(ppHead2);
            DisableAllObjects(ppNose1);
            DisableAllObjects(ppNose2);
            DisableAllObjects(ppMouth1);
            DisableAllObjects(ppMouth2);
            DisableAllObjects(secGtwEyes1);
            DisableAllObjects(secGtwEyes2);
            DisableAllObjects(secPpNose1);
            DisableAllObjects(secPpNose2);
            DisableAllObjects(secPpMouth1);
            DisableAllObjects(secPpMouth2);

            ActivateRandomFromArray(ppHead1);
            ActivateRandomFromArray(ppHead2);
            int randomMouth1Index = ActivateRandomFromArray(ppMouth1);
            int randomMouth2Index = ActivateRandomFromArray(ppMouth2);

            ActivateRandomFromArray(ppNose1);
            ActivateRandomFromArray(ppNose2);
            int randomNose1Index = ActivateRandomFromArray(ppNose1);
            int randomNose2Index = ActivateRandomFromArray(ppNose1);

            // Set secPpMouth1 and secPpMouth2 based on the activated indexes
            if (randomMouth1Index >= 0 && secPpMouth1.Length > randomMouth1Index)
            {
                secPpMouth1[randomMouth1Index]?.SetActive(true);
            }
            if (randomMouth2Index >= 0 && secPpMouth2.Length > randomMouth2Index)
            {
                secPpMouth2[randomMouth2Index]?.SetActive(true);
            }

            if (randomNose1Index >= 0 && secPpNose1.Length > randomNose1Index)
            {
                secPpNose1[randomNose1Index]?.SetActive(true);
            }
            if (randomNose2Index >= 0 && secPpNose2.Length > randomNose2Index)
            {
                secPpNose2[randomNose2Index]?.SetActive(true);
            }
        }
    }

    private int ActivateRandomFromArray(GameObject[] objects)
    {
        if (objects == null || objects.Length == 0) return -1;

        int randomIndex = Random.Range(0, objects.Length);

        if (objects[randomIndex] != null)
        {
            objects[randomIndex].SetActive(true);
            Debug.Log("Activated: " + objects[randomIndex].name);
        }

        return randomIndex;
    }


    private void DisableAllObjects(GameObject[] objects)
    {
        if (objects == null) return;
        foreach (GameObject obj in objects)
        {
            if (obj != null) obj.SetActive(false);
        }
        
    }

    public GameObject GetActiveMouth()
    {
        return FindActiveObject(ppMouth1) ?? FindActiveObject(ppMouth2);
    }

    private GameObject FindActiveObject(GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null && obj.activeSelf)
            {
                return obj;
            }
        }
        return null;
    }
}
