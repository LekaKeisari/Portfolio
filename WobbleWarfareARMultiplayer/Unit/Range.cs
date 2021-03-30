using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Range : Bolt.EntityBehaviour<ICustomCapsule>, IUnit
{
    [SerializeField] UnitSelectorSO unitSelector = null;

    [SerializeField] private GameObject bullet;

    [SerializeField] private Transform hand;

    [SerializeField] private Animator anim;

    public Vector3 shootDirection;

    private Button chargeButton = null;
    public Button ChargeButton { get => chargeButton; set => chargeButton = value; }
    [SerializeField]
    private MeshRenderer selectionIndicator = null;
    [SerializeField]
    private NavMeshAgent agent = null;

    [SerializeField] private int maxHealth;
    public int MaxHealth { get => maxHealth; }

    [SerializeField] private int currentHealth;
    public int CurrentHealth { get => currentHealth; set =>  currentHealth = value; }

    [SerializeField] private int damage;
    public int Damage { get => damage; }

    [SerializeField] private int speed;
    public int Speed { get => speed; }

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

    public float powerCharge;

    private float maxPowerCharge =10f;

    private float chargeRate = 3f;

    private bool canShot = true;

    public void Attack(IUnit target)
    {
        target.CurrentHealth -= damage;
        
    }

    public void Move()
    {
        agent.speed = speed;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            agent.destination = new Vector3(transform.position.x, transform.position.y, hit.point.z);

            switch (unitType)
            {
                case UnitType.Infantry:
                    break;
                case UnitType.Cavalry:
                    break;
                case UnitType.Archer:
                    AudioManager.PlaySound(AudioManager.Sound.ArcherMove, transform.position);
                    break;
                case UnitType.Catapult:
                    AudioManager.PlaySound(AudioManager.Sound.CataMove, transform.position);
                    break;
                default:
                    break;
            }

            state.Running = true;
        }
    }

    public void Charge()
    {
        charging = true;
        canShot = true;
        
        switch (unitType)
        {
            case UnitType.Infantry:
                break;
            case UnitType.Cavalry:                
                break;
            case UnitType.Archer:
                AudioManager.PlaySound(AudioManager.Sound.ArcherAttack, transform.position);
                break;
            case UnitType.Catapult:
                AudioManager.PlaySound(AudioManager.Sound.CataAttack, transform.position);
                break;
            default:
                break;
        }
    }

    public void ReleaseCharge()
    {              
        state.Shoot();    
        /*
        GameObject newBullet = BoltNetwork.Instantiate(bullet, hand.position, Quaternion.identity);
            //Bolt.Instantiate(bullet, hand.position, hand.rotation);
        newBullet.GetComponent<Bullet>().power = powerCharge;
        newBullet.GetComponent<Bullet>().shootDirection = this.shootDirection;
        charging = false;
        powerCharge = damage;
        */
        
    }

    public void Shoot()
    {
        if (canShot)
        {
            StartCoroutine(DelayAnimationShoot(1f));
            canShot = false;
        }    
    }

    IEnumerator DelayAnimationShoot(float delay)
    {
        state.Animator.SetTrigger("shoot");
        yield return new WaitForSeconds(delay);
        if (entity.IsOwner)
        {
            GameObject newBullet = BoltNetwork.Instantiate(bullet, hand.position, Quaternion.identity);
            //Bolt.Instantiate(bullet, hand.position, hand.rotation);
            newBullet.GetComponent<Bullet>().power = powerCharge;
            newBullet.GetComponent<Bullet>().shootDirection = this.shootDirection;           
            powerCharge = damage;
            charging = false;
        }
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
    public override void Attached()
    {
        state.SetTransforms(state.CapsuleTransform, transform);
        state.SetAnimator(anim);
        if (entity.IsOwner)
        {
            state.Health = CurrentHealth;            
        }
        state.AddCallback("Health", HealthCallback);
        state.OnShoot += Shoot;
        state.OnDead += Dead;        
    }

    public override void SimulateOwner()
    {
        if (selected && !charging && Input.GetMouseButton(0))
        {
            Move();
        }
        if(agent.remainingDistance < 0.5f)
        {
            state.Running = false;
        }
        else
        {
            state.Running = true;           
        }
    }

    private void OnMouseDown()
    {
        if (!selected && entity.IsOwner)
        {
            ToggleSelected(true);
            //chargeButton.onClick.AddListener(Charge);
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //selectionIndicator = GetComponentsInChildren<MeshRenderer>()[1];
        selectionIndicator.gameObject.SetActive(false);
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        powerCharge = damage;
    }

    // Update is called once per frame
    void Update()
    {
        if (charging)
        {
            if (powerCharge < maxPowerCharge)
            {
                powerCharge += Time.deltaTime * chargeRate;
            }
            
        }
        if (state.Running)
        {
            state.Animator.SetBool("running", true);
        }
        else
        {
            state.Animator.SetBool("running", false);
        }
        
        
    }
    public void Dead()
    {
        agent.isStopped = true;
        StartCoroutine(DelayAnimationDead(1));
        switch (unitType)
        {
            case UnitType.Infantry:
                break;
            case UnitType.Cavalry:
                break;
            case UnitType.Archer:
                AudioManager.PlaySound(AudioManager.Sound.ArcherDie, transform.position);
                break;
            case UnitType.Catapult:
                AudioManager.PlaySound(AudioManager.Sound.CataDie, transform.position);
                break;
            default:
                break;
        }
    }
    public void TakeDamage(int damage)
    {
        if (entity.IsOwner)
        {
            state.Health -= damage;           
        }
        
        state.AddCallback("Health", HealthCallback);
        Debug.Log("Callback");

    }
    private void HealthCallback()
    {
        currentHealth = state.Health;

        if (currentHealth <= 0)
        {
            BoltNetwork.Destroy(gameObject);
        }
    }

    public void ToggleSelected(bool selectedState)
    {
            EventTrigger trigger = chargeButton.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
        if (selectedState && entity.IsOwner)
        {
            selected = true;

            if (unitSelector.selectedUnit != null)
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
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => { Charge(); });
            trigger.triggers.Add(entry);
            entry2.eventID = EventTriggerType.PointerUp;
            entry2.callback.AddListener((data) => { ReleaseCharge(); });
            trigger.triggers.Add(entry2);
        }

        else if (!selectedState && entity.IsOwner)
        {
            selected = false;

            selectionIndicator.gameObject.SetActive(false);
            entry.callback.RemoveAllListeners();
            entry2.callback.RemoveAllListeners();
            trigger.triggers.Remove(entry);
            trigger.triggers.Remove(entry2);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Hit");
        if (!other.GetComponentInParent<BoltEntity>().IsOwner)
        {
            state.Dead();
            //Dead();
        }
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (entity.IsOwner)
        {
            if (!collision.transform.GetComponent<BoltEntity>().IsOwner)
            {
                BoltNetwork.Destroy(gameObject);
            }
        }
        
    }
    */
}
