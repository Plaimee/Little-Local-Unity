using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PGCtrlScript : MonoBehaviour
{
    public GameObject pgGroup;
    public GameObject[] pdHead;
    public GameObject[] eyes1;
    public GameObject[] pdNose;
    public GameObject[] pdMouth;

    public GameObject[] gtwHead;
    public GameObject[] eyes2;
    public GameObject[] gtwNose;
    public GameObject[] gtwMouth;

    // second raw output
    public GameObject[] secEyes1;
    public GameObject[] secEyes2;
    public GameObject[] secPdNose;
    public GameObject[] secPdMouth;
    public GameObject[] secGtwNose;
    public GameObject[] secGtwMouth;

    void Start()
    {
        ActivateRandomGameObject();
    }

    public void ActivateRandomGameObject()
    {
        if (CharacterScript.instance.btnName == "PG")
        {
            DisableAllObjects(pdHead);
            DisableAllObjects(eyes1);
            DisableAllObjects(pdNose);
            DisableAllObjects(pdMouth);
            DisableAllObjects(gtwHead);
            DisableAllObjects(eyes2);
            DisableAllObjects(gtwNose);
            DisableAllObjects(gtwMouth);
            DisableAllObjects(secEyes1);
            DisableAllObjects(secEyes2);
            DisableAllObjects(secPdNose);
            DisableAllObjects(secPdMouth);
            DisableAllObjects(secGtwNose);
            DisableAllObjects(secGtwMouth);

            ActivateRandomFromArray(pdHead);
            ActivateRandomFromArray(gtwHead);
            int randomMouth1Index = ActivateRandomFromArray(pdMouth);
            int randomMouth2Index = ActivateRandomFromArray(gtwMouth);

            ActivateRandomFromArray(pdNose);
            ActivateRandomFromArray(gtwNose);
            int randomPdNoseIndex = ActivateRandomFromArray(pdNose);
            int randomGtwNoseIndex = ActivateRandomFromArray(gtwNose);

            if (randomMouth1Index >= 0 && secPdMouth.Length > randomMouth1Index)
            {
                secPdMouth[randomMouth1Index]?.SetActive(true);
            }
            if (randomMouth2Index >= 0 && secGtwMouth.Length > randomMouth2Index)
            {
                secGtwMouth[randomMouth2Index]?.SetActive(true);
            }

            if (randomPdNoseIndex >= 0 && secPdNose.Length > randomPdNoseIndex)
            {
                secPdNose[randomPdNoseIndex]?.SetActive(true);
            }
            if (randomGtwNoseIndex >= 0 && secGtwNose.Length > randomGtwNoseIndex)
            {
                secGtwNose[randomGtwNoseIndex]?.SetActive(true);
            }
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

    public GameObject GetActiveMouth()
    {
        return FindActiveObject(pdMouth) ?? FindActiveObject(gtwMouth);
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
