using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Bolt.EntityBehaviour<IBullet>
{
    // Start is called before the first frame update
    public Vector3 shootDirection;
    public ParticleSystem projectileParticle;
    public float power;
    private float powerRate = 3f;
    private Rigidbody rg;
    
    void Start()
    {
        rg = GetComponent<Rigidbody>();
        rg.AddForce(( shootDirection + transform.up) * power/1.5f , ForceMode.Impulse);
    }
    public override void Attached()
    {
        state.SetTransforms(state.BulletTransform, transform);
        state.BulletDamage = power;
    }

   
    // Update is called once per frames
    void Update()
    {
        
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        IUnit iunit = collision.transform.GetComponent<IUnit>();
        if(iunit != null)
        {
            BoltNetwork.Destroy(collision.gameObject);
        }
        
    }
    */
}
