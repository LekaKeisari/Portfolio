using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objectPrefabs;

    private List<GameObject> pooledObjects = new List<GameObject>();

    public GameObject GetObject(string type)
    {

        //Check if game object exist in pool and is not active, activates it if true
        foreach (GameObject go in pooledObjects)                
        {
            if (go.name == type && !go.activeInHierarchy)
            {
                go.SetActive(true);
                return go;
            }
        }

        //Check if game object exist in prefabs list and spawns it if true
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            if (objectPrefabs[i].name == type)
            {
                GameObject newObject = Instantiate(objectPrefabs[i]);
                pooledObjects.Add(newObject);
                newObject.name = type;
                return newObject;
            }
        }
        return null;
    }


    public GameObject SpawnPoolObject(string type)
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            if (objectPrefabs[i].name == type)
            {
                GameObject newObject = Instantiate(objectPrefabs[i]);
                newObject.name = type;
                return newObject;
            }
        }
        return null;
    }

    public void ReleaseObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
