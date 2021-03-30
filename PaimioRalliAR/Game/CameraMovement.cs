using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    public Vector3 offsetNormal = new Vector3(0f, 10.7f, -7f);
    public Vector3 offsetBoostMobe = new Vector3(0f, 10.7f, -15f);
    private Vector3 offSet;

    [Range(0, 10)]
    [SerializeField]
    private float smoothFollowing;

    private Vector3 startPos;

    private float distance;
       


    private void Start()
    {
        offSet = offsetNormal;
        transform.position = player.transform.position + offSet;
        startPos = transform.position;
    }


    private void LateUpdate()
    {
        distance = (Mathf.Abs(player.transform.position.x) + smoothFollowing) * 0.1f;
        startPos = new Vector3(0f, 0f, player.transform.position.z) + offSet;
        transform.position = Vector3.Lerp(startPos, player.transform.position + offSet, distance);
    }


    // Move camera farther from car when player get speed boost or move camera back to its original position if speed boost has ended
    public IEnumerator MoveCameraZAxis(float duration)
    {
        float elapsed = 0.0f;
        Vector3 temp = offSet;
        while (elapsed < duration)
        {
            if (GameManager.instance.boostMode == true)
            {
                offSet = Vector3.Lerp(temp, offsetBoostMobe, 0.6f * elapsed);
            }
            else
            {
                offSet = Vector3.Lerp(temp, offsetNormal, 0.6f * elapsed);
            }
            elapsed = elapsed + Time.deltaTime;
            yield return null;
        }
    }
}
