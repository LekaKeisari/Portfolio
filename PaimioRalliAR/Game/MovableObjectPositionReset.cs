using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovableObjectPositionReset : MonoBehaviour
{
    [SerializeField] private List<GameObject> movableObjects = new List<GameObject>();
    [SerializeField] private List<Vector3> positions = new List<Vector3>();
    [SerializeField] private List<Quaternion> rotations = new List<Quaternion>();



    public void Reposition()
    {
        for (int index = 0; index < movableObjects.Count; index++)
        {
            //movableObjects[index].transform.localPosition = new Vector3(positions[index].x, positions[index].y, positions[index].z);
            //movableObjects[index].transform.localRotation = Quaternion.Euler(rotations[index].x, rotations[index].y, rotations[index].z);
            Rigidbody rg = movableObjects[index].GetComponent<Rigidbody>();
            rg.isKinematic = true;
            movableObjects[index].transform.localPosition = positions[index];
            rg.transform.localRotation = Quaternion.Euler(rotations[index].x, rotations[index].y, rotations[index].z);
            rg.isKinematic = false;
        }
    }
}
