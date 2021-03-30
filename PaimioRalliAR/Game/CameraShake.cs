using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    // Choose which axis, how long and how much camera will shake
    public IEnumerator ShakeCamera(bool moveX, bool moveY, bool moveZ ,float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0.0f;
        int xRange;
        int yRange;
        int zRange;

        if (moveX)
        {
            xRange = 1;
        }
        else
        {
            xRange = 0;
        }
        if (moveY)
        {
            yRange = 1;
        }
        else
        {
            yRange = 0;
        }
        if (moveZ)
        {
            zRange = 1;
        }
        else
        {
            zRange = 0;
        }

        while (elapsed < duration)
        {
            float x = Random.Range(-xRange, xRange) * magnitude;
            float y = Random.Range(-yRange, yRange) * magnitude;
            float z = Random.Range(-zRange, zRange) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z + z);

            elapsed = elapsed + Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
