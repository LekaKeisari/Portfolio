using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResetter : MonoBehaviour
{

    [SerializeField] private ScenarioManagerSO scenarioManagerSO;

    private Vector3 startingPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        scenarioManagerSO.PlayerPosResetter = this;
    }


    public void ResetPosition()
    {
        transform.position = startingPosition;
    }
}
