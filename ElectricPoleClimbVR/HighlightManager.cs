using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HighlightedObject
{
    public GameObject obj;
    public List<MeshRenderer> meshes = new List<MeshRenderer>();
    public List<Material> defaultMaterials = new List<Material>();
}

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager instance = null;

    public Material defaultMaterial;

    public Material highlightMaterial;

    public bool highlighted = false;

    [SerializeField] private List<HighlightedObject> highlightedObjects = new List<HighlightedObject>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void HighlightToggle(GameObject objectToHighlight)
    {
        bool contains = false;

        //Check if the object is already highlighted
        foreach (HighlightedObject obj in highlightedObjects)
        {
            if (obj.obj == objectToHighlight)
            {
                contains = true;

                for (int i = 0; i < obj.meshes.Count; i++)
                    obj.meshes[i].material = obj.defaultMaterials[i]; 

                highlightedObjects.Remove(obj);
                break;
            }
            else
                contains = false;
        }

        if (contains == false)
        {
            HighlightedObject newHighlightedObj = new HighlightedObject();
            newHighlightedObj.obj = objectToHighlight;
            newHighlightedObj.meshes.AddRange(objectToHighlight.GetComponentsInChildren<MeshRenderer>());

            foreach (MeshRenderer mesh in newHighlightedObj.meshes)
            {
                newHighlightedObj.defaultMaterials.Add(mesh.material);
                mesh.material = highlightMaterial;
            }

            highlightedObjects.Add(newHighlightedObj);
        }
    }

    public Material HighlightObjectOnOff()
    {

        if (highlighted)
        {
            //highlighted = false;
            return defaultMaterial;
        }

        else
        {
            //highlighted = true;
            return highlightMaterial;
        }
    }

    //public void CheckHighlightStatus(Material material)
    //{
    //    if (material.name == highlightMaterial.name)
    //    {
    //    Debug.Log("Funktio toimii!");
    //        highlighted = true;
    //    }

    //    else
    //    {
    //        highlighted = false;
    //    }
    //}
    
}
