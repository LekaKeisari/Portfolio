using UnityEngine;
using Bolt;
using UnityEngine.UI;

public class NetworkCallbacks : GlobalEventListener
{
    public GameObject testCapsule;
    public GameObject hostPivot;
    public GameObject clientPivot;
    [SerializeField]
    private Button chargeButton = null;

    [SerializeField]
    private UnitCollectionSO unitCollection = null;

    public override void SceneLoadLocalDone(string scene)
    {
        //Vector3 spawnPos = Vector3.zero;

        //if (BoltNetwork.IsClient)
        //{
        //    spawnPos = new Vector3(30f, 1f, Random.Range(-8, 8));
        //    GameObject unit = BoltNetwork.Instantiate(testCapsule, spawnPos, clientPivot.transform.rotation);
        //    unitCollection.units.Add(unit);
        //    if (unit.GetComponent<Infantry>() != null)
        //    {
        //        unit.GetComponent<Infantry>().ChargeButton = chargeButton;
        //    }
        //    if (unit.GetComponent<Range>() != null)
        //    {
        //        unit.GetComponent<Range>().ChargeButton = chargeButton;
        //        unit.GetComponent<Range>().shootDirection = new Vector3(1, 0, 0);

        //    }
        //}
        //if (BoltNetwork.IsServer)
        //{
        //    spawnPos = new Vector3(-30f, 1f, Random.Range(-8, 8));
        //    GameObject unit = BoltNetwork.Instantiate(testCapsule, spawnPos, hostPivot.transform.rotation);
        //    unitCollection.units.Add(unit);
        //    if (unit.GetComponent<Infantry>() != null)
        //    {
        //        unit.GetComponent<Infantry>().ChargeButton = chargeButton;
        //    }
        //    if (unit.GetComponent<Range>() != null)
        //    {
        //        unit.GetComponent<Range>().ChargeButton = chargeButton;
        //        unit.GetComponent<Range>().shootDirection = new Vector3(1, 0, 0);
        //    }           
        //}
    }

    public override void OnEvent(DestroyRequest evnt)
    {
        if (evnt.Entity.IsOwner)
        {
            //evnt.Entity.GetState<ICustomCapsule>().OnDead();
            BoltNetwork.Destroy(evnt.Entity.gameObject);
        }
            
    }
}
