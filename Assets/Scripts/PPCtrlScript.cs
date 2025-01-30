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
    public GameObject[] ppMouth1;
    public GameObject[] ppMouth2;

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
            DisableAllObjects(ppMouth1);
            DisableAllObjects(ppMouth2);

            ActivateRandomFromArray(ppHead1);
            ActivateRandomFromArray(ppHead2);
            ActivateRandomFromArray(ppMouth1);
            ActivateRandomFromArray(ppMouth2);
        }
       
    }

    private void DisableAllObjects(GameObject[] objects)
    {
        if (objects == null) return;
        foreach (GameObject obj in objects)
        {
            if (obj != null) obj.SetActive(false);
        }
        
    }

    private void ActivateRandomFromArray(GameObject[] objects)
    {
        if (objects == null || objects.Length == 0) return;

        int randomIndex = Random.Range(0, objects.Length);

        if (objects[randomIndex] != null)
        {
            objects[randomIndex].SetActive(true);
            Debug.Log("Activated: " + objects[randomIndex].name);
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
