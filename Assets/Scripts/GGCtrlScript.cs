using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGCtrlScript : MonoBehaviour
{
    public GameObject ggGroup;
    public GameObject[] ggHead1;
    public GameObject[] ggHead2;
    public GameObject[] pdEyes1;
    public GameObject[] pdEyes2;
    public GameObject[] ggNose1;
    public GameObject[] ggNose2;
    public GameObject[] ggMouth1;
    public GameObject[] ggMouth2;

    // second raw output
    public GameObject[] secPdEyes1;
    public GameObject[] secPdEyes2;
    public GameObject[] secGgNose1;
    public GameObject[] secGgNose2;
    public GameObject[] secGgMouth1;
    public GameObject[] secGgMouth2;

    void Start()
    {
        ActivateRandomGameObject();
    }

    public void ActivateRandomGameObject()
    {
        if (CharacterScript.instance.btnName == "GG")
        {
            DisableAllObjects(pdEyes1);
            DisableAllObjects(pdEyes2);
            DisableAllObjects(ggHead1);
            DisableAllObjects(ggHead2);
            DisableAllObjects(ggNose1);
            DisableAllObjects(ggNose2);
            DisableAllObjects(ggMouth1);
            DisableAllObjects(ggMouth2);
            DisableAllObjects(secPdEyes1);
            DisableAllObjects(secPdEyes2);
            DisableAllObjects(secGgNose1);
            DisableAllObjects(secGgNose2);
            DisableAllObjects(secGgMouth1);
            DisableAllObjects(secGgMouth2);

            ActivateRandomFromArray(ggHead1);
            ActivateRandomFromArray(ggHead2);
            int randomMouth1Index = ActivateRandomFromArray(ggMouth1);
            int randomMouth2Index = ActivateRandomFromArray(ggMouth2);

            ActivateRandomFromArray(ggNose1);
            ActivateRandomFromArray(ggNose2);
            int randomNose1Index = ActivateRandomFromArray(ggNose1);
            int randomNose2Index = ActivateRandomFromArray(ggNose2);

            // Set secPpMouth1 and secPpMouth2 based on the activated indexes
            if (randomMouth1Index >= 0 && secGgMouth1.Length > randomMouth1Index)
            {
                secGgMouth1[randomMouth1Index]?.SetActive(true);
            }
            if (randomMouth2Index >= 0 && secGgMouth2.Length > randomMouth2Index)
            {
                secGgMouth2[randomMouth2Index]?.SetActive(true);
            }

            if (randomNose1Index >= 0 && secGgNose1.Length > randomNose1Index)
            {
                secGgNose1[randomNose1Index]?.SetActive(true);
            }
            if (randomNose2Index >= 0 && secGgNose2.Length > randomNose2Index)
            {
                secGgNose2[randomNose2Index]?.SetActive(true);
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
        return FindActiveObject(ggMouth1) ?? FindActiveObject(ggMouth2);
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
