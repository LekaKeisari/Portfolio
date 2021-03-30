using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UnityEngine.AI;
using UnityEngine.UI;

public class Infantry : Bolt.EntityBehaviour<ICustomCapsule>, IUnit
{
    [SerializeField] UnitSelectorSO unitSelector = null;

    private Button chargeButton = null;
    public Button ChargeButton { get => chargeButton; set => chargeButton = value; }
    [SerializeField]
    private MeshRenderer selectionIndicator = null;

    private NavMeshAgent agent = null;

    [SerializeField] Animator anim;

    [SerializeField] private int maxHealth = 3;
    public int MaxHealth { get => maxHealth; }

    [SerializeField] private int currentHealth = 3;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }

    [SerializeField] private int damage = 1;
    public int Damage { get => damage; }

    [SerializeField] private int speed = 1;
    public int Speed { get => speed; }

    [SerializeField] private int chargeSpeed = 3;

    private bool selected = false;
    public bool Selected { get => selected; set => selected = value; }

    [SerializeField] private bool killInfantry;
    public bool KillInfantry { get => killInfantry; }

    [SerializeField] private bool killCavalry;
    public bool KillCavalry { get => killCavalry; }

    [SerializeField] private bool killArcher;
    public bool KillArcher { get => KillArcher; }

    [SerializeField] private bool killCatapult;
    public bool KillCatapult { get => killCatapult; }

    [SerializeField] private UnitType unitType;
    public UnitType UnitType { get => unitType; }

    private bool charging = false;

    public void Move()
    {
        agent.speed = speed;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            agent.destination = new Vector3(transform.position.x, transform.position.y, hit.point.z);
            state.Running = true;
            switch (unitType)
            {
                case UnitType.Infantry:
                    AudioManager.PlaySound(AudioManager.Sound.KnightMove, transform.position);
                    break;
                case UnitType.Cavalry:
                    AudioManager.PlaySound(AudioManager.Sound.HorseMove, transform.position);
                    break;
                case UnitType.Archer:
                    break;
                case UnitType.Catapult:
                    break;
                default:
                    break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    public void Attack(IUnit target)
    {
        target.CurrentHealth -= damage;
    }

    public void Charge()
    {
        charging = true;
        agent.speed = chargeSpeed;
        state.Running = true;
        ToggleSelected(false);

        Vector3 basePosition = transform.position;
        Quaternion baseRotation = transform.rotation;

        float edgeOfScreen = 5f;

        switch (unitType)
        {
            case UnitType.Infantry:
                AudioManager.PlaySound(AudioManager.Sound.KnightAttack, transform.position);
                break;
            case UnitType.Cavalry:
                AudioManager.PlaySound(AudioManager.Sound.HorseAttack, transform.position);
                break;
            case UnitType.Archer:
                break;
            case UnitType.Catapult:
                break;
            default:
                break;
        }

        if (BoltNetwork.IsServer)
        {
            agent.destination = new Vector3(edgeOfScreen, transform.position.y, transform.position.z);
            
        }
        else
        {
            agent.destination = new Vector3(-edgeOfScreen, transform.position.y, transform.position.z);
            
        }
        if(entity.IsOwner)
            StartCoroutine(ReturnToBase(basePosition,baseRotation));
    }

    private IEnumerator ReturnToBase(Vector3 basePosition,Quaternion baseRotation)
    {
        yield return new WaitUntil(() => GetComponent<NavMeshAgent>().remainingDistance < .1f);

        if(unitType == UnitType.Infantry)
            yield return new WaitForSeconds(3f);
        else
            yield return new WaitForSeconds(6f);

        agent.ResetPath();
        transform.position = basePosition;
        agent.velocity = new Vector3(0,0,0);
        transform.rotation = baseRotation;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        charging = false;
    }

    private void OnMouseDown()
    {
        if (!selected && entity.IsOwner && !charging)
        {
            ToggleSelected(true);
        }
    }
    public override void Attached()
    {
        state.SetTransforms(state.CapsuleTransform, transform);
        state.SetAnimator(anim);
        state.OnDead += Dead;
    }

    public override void SimulateOwner()
    {
        if (selected && !charging && Input.GetMouseButton(0))
        {
            Move();
        }
        if(agent.remainingDistance < 0.5)
        {
            state.Running = false;
        }
        else
        {
            state.Running = true;
        }

    }
    void Update()
    {
        if (state.Running)
        {
            state.Animator.SetBool("running", true);
        }
        else
        {
            state.Animator.SetBool("running", false);
        }
    }

    void Start()
    {
        //selectionIndicator = GetComponentsInChildren<MeshRenderer>()[1];
        selectionIndicator.gameObject.SetActive(false);
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();               
    }

    public void ToggleSelected(bool selectedState)
    {
        if (selectedState)
        {
            selected = true;

            if (unitSelector.selectedUnit != null && unitSelector.selectedUnit != this.gameObject)
            {
                unitSelector.selectedUnit.GetComponent<IUnit>().ToggleSelected(false);
                unitSelector.selectedUnit = this.gameObject;
            }
            else
            {
                unitSelector.selectedUnit = this.gameObject;

            }

            selectionIndicator.gameObject.SetActive(true);
            chargeButton.gameObject.SetActive(true);
            chargeButton.onClick.AddListener(Charge);
        }

        else
        {
            selected = false;
            selectionIndicator.gameObject.SetActive(false);
            chargeButton.onClick.RemoveListener(Charge);
        }
    }/*

    private void OnTriggerEnter(Collider other)
    {
        IUnit iunit = other.transform.GetComponent<IUnit>();
        BoltEntity ent = other.transform.GetComponent<BoltEntity>();
        
        if (entity.IsOwner)
        {
        
            if (iunit != null && !ent.IsOwner)
            {
                Debug.Log("Collide");
                switch (iunit.UnitType)
                {
                    case UnitType.Infantry:
                        if (killInfantry)
                        {
                            var request = DestroyRequest.Create();
                            request.Entity = ent;
                            request.Send();
                            
                        }
                        BoltNetwork.Destroy(gameObject);
                        break;
                    case UnitType.Cavalry:
                        if (killCavalry)
                        {
                            var request = DestroyRequest.Create();
                            request.Entity = ent;
                            request.Send();
                            if (unitType == UnitType.Cavalry)
                            {
                                BoltNetwork.Destroy(gameObject);
                            }

                        }
                        break;
                    case UnitType.Archer:
                        if (killArcher)
                        {
                            var request = DestroyRequest.Create();
                            request.Entity = ent;
                            request.Send();
                        }
                        break;
                    case UnitType.Catapult:
                        if (killCatapult)
                        {

                            var request = DestroyRequest.Create();
                            request.Entity = ent;
                            request.Send();

                        }
                        break;
                }
            }

        }
    }
    */
    private void OnCollisionEnter(Collision collision)
    {
        IUnit iunit = collision.transform.GetComponent<IUnit>();
        BoltEntity ent = collision.transform.GetComponent<BoltEntity>();
        
        if (entity.IsOwner)
        {
            if (iunit != null && !ent.IsOwner)
            {
                Debug.Log("Collide");
                //GetComponent<Rigidbody>().velocity = Vector3.zero;
                agent.isStopped = true;
                switch (iunit.UnitType)
                {
                    case UnitType.Infantry:
                        if (killInfantry)
                        {
                            var request = DestroyRequest.Create();
                            request.Entity = ent;
                            request.Send();
                            BoltNetwork.Destroy(gameObject);
                        }
                        break;
                    case UnitType.Cavalry:
                        if (killCavalry)
                        {
                            var request = DestroyRequest.Create();
                            request.Entity = ent;
                            request.Send();
                            if (unitType == UnitType.Cavalry)
                            {
                                BoltNetwork.Destroy(gameObject);
                            }

                        }
                        break;
                    case UnitType.Archer:
                        if (killArcher)
                        {
                            var request = DestroyRequest.Create();
                            request.Entity = ent;
                            request.Send();
                        }
                        break;
                    case UnitType.Catapult:
                        if (killCatapult)
                        {

                            var request = DestroyRequest.Create();
                            request.Entity = ent;
                            request.Send();

                        }
                        break;
                }
            }

        }
    }
    
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Hit");
        if (!other.GetComponentInParent<BoltEntity>().IsOwner)
        {
            state.Dead();
            agent.isStopped = true;
            //Dead();
        }
        
    }


    void Dead()
    {
        //agent.enabled = false;
        StartCoroutine(DelayAnimationDead(2.33f)); switch (unitType)
        {
            case UnitType.Infantry:
                AudioManager.PlaySound(AudioManager.Sound.KnightDie, transform.position);
                break;
            case UnitType.Cavalry:
                AudioManager.PlaySound(AudioManager.Sound.HorseDie, transform.position);
                break;
            case UnitType.Archer:
                break;
            case UnitType.Catapult:
                break;
            default:
                break;
        }

        //BoltNetwork.Destroy(this.gameObject);
    }
    IEnumerator DelayAnimationDead(float delay)
    {
        state.Animator.SetTrigger("dead");
        yield return new WaitForSeconds(delay);
        if (entity.IsOwner)
        {
            BoltNetwork.Destroy(gameObject);
        }

    }

}
