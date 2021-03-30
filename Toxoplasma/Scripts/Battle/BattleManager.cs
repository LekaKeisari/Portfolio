using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;

    public GameObject player;
   
    private GameObject enemy;
    [SerializeField]
    private GameObject battleOverCanvas;
    [SerializeField]
    private GameObject damageCanvas;

    public float attackDistance = 1.3f;

    [HideInInspector]
    public GameObject mouseOverEnemy;
        
    public BattleUIManager battleUIManager;

    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private Button attackButton;
    [SerializeField]
    private Button magicButton;
    [SerializeField]
    private Button firstAidButton;
    [SerializeField]
    private Button escapeButton;

    private TMP_Text damageTxt;

    private PlayerStats playerStats;
    private EnemyStats enemyStats;

    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField]
    private List<Transform> enemyPositions = new List<Transform>();
    [SerializeField]
    private List<GameObject> battleZonePrefabs = new List<GameObject>();
    
    private List<GameObject> battleParticipants = new List<GameObject>();
    private List<string> namesSorted = new List<string>();
    
    private int battleParticipantsLeft = 0;

    [HideInInspector]
    public int battleXpValue = 0;

    private int turnNumber;
    private float distanceMoved;
    private float fractionOfJourney;
    private float startTime;
    private float healMultiplier = 0.7f;
        
    [HideInInspector]
    public bool playersTurn = false;
    [HideInInspector]
    public bool turnInProgress = false;
    [HideInInspector]
    public bool attackButtonPressed = false;

    [HideInInspector]
    public bool battleStartEventDone = false;
    //private bool battleOver = false;

    private GameManager gameManager;

    private Animator playerAnimator;
    private Animator enemyAnimator;


    public Texture2D cursorImage;

    private void Awake()
    {
        //If this script does not exit already, use this current instance
        if (instance == null)
            instance = this;

        //If this script already exit, DESTROY this current instance
        else if (instance != this)
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        playerStats = player.GetComponent<PlayerStats>();
        playerAnimator = player.GetComponentInChildren<Animator>();
        //enemyStats = enemy.GetComponent<EnemyStats>();
        damageTxt = damageCanvas.GetComponentInChildren<TMP_Text>();
        SetupBattle();

        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!playersTurn && !turnInProgress)
        {            
            turnInProgress = true;
            StartCoroutine(MeleeAttack(enemy, player));
        }  
        //if (playersTurn && !turnInProgress)
        //{
            //if (Input.GetKeyDown(KeyCode.Mouse0) && mouseOverEnemy)
            //{
            //    turnInProgress = true;
            //    battleUIManager.targetIndicator.SetActive(false);
            //    StartCoroutine(MeleeAttack(player, mouseOverEnemy));
            //}
            //if (Input.GetKeyDown(KeyCode.Mouse1))
            //{
            //    turnInProgress = true;
            //    battleUIManager.targetIndicator.SetActive(false);
            //    HealYourself();
            //}
        //}
    }

    public IEnumerator MeleeAttack(GameObject attacker, GameObject target)
    {
        playerStats = player.GetComponent<PlayerStats>();
        enemyStats = enemy.GetComponent<EnemyStats>();
        Vector3 originalPosition = attacker.transform.position;
        float distanceToTarget = Vector3.Distance(originalPosition, target.transform.position);
        yield return new WaitUntil(() => battleStartEventDone);
        startTime = Time.time;

        if (attacker.name == "Player")
        {
            playerAnimator.SetBool("isRunning", true);
        }

        else
        {
            enemyAnimator = attacker.GetComponentInChildren<Animator>();
            enemyAnimator.SetBool("isRunning", true);
        }

        while (Vector3.Distance(attacker.transform.position, target.transform.position) > attackDistance)
        {
            distanceMoved = (Time.time - startTime);
            fractionOfJourney = (distanceMoved / distanceToTarget) * movementSpeed;
            attacker.transform.position = Vector3.Lerp(originalPosition, new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z + 1f), fractionOfJourney);
            if (attacker.name == "Player")
            {
                attacker.transform.LookAt(target.transform);
            }
            else
            {
                attacker.transform.LookAt(originalPosition);
            }
            yield return new WaitForSeconds(0.01f);
        }
        if (attacker.name == "Player")
        {
            playerAnimator.SetBool("isRunning", false);

        }
        else
        {            
            enemyAnimator.SetBool("isRunning", false);
            //attacker.transform.rotation *= Quaternion.Euler(0, 180, 0);
        }

        yield return new WaitForSeconds(.5f);
        //Debug.Log("Attack");

        //yield return new WaitForSeconds(0.5f);

        if (attacker.name == "Player")
        {
            playerAnimator.SetBool("hasKicked", true);

            yield return new WaitForSeconds(1.5f);

            playerAnimator.SetBool("hasKicked", false);
            //float targetHealth = target.GetComponent<EnemyStats>().enemyCurrentHealth;
            damageTxt.text = playerStats.playerDamage.ToString();

            StartCoroutine(DisplayDamage(target));

            enemyStats.enemyCurrentHealth -= playerStats.playerDamage;

            if (enemyStats.enemyCurrentHealth <= 0)
            {                
                enemyStats.enemyDead = true;
                target.SetActive(false);
                mouseOverEnemy = null;
                battleParticipantsLeft = 0;
                for (int i = 0; i < battleParticipants.Count; i++)
                {                    
                    if (battleParticipants[i].activeInHierarchy)
                    {
                        battleParticipantsLeft++;
                    }
                }
                if (battleParticipantsLeft == 1 && !gameManager.restarted)
                {
                    yield return new WaitForSeconds(2f);
                    playerStats.CurrentXP += battleXpValue;
                    StartCoroutine(BackToWorld());
                    turnInProgress = false;
                    if (gameManager.dangerZone)
                    {
                        gameManager.ToggleRandomBattle(true);
                        yield return null;
                    }
                }
            }
            
        }
        else
        {
            //float playerHealth = playerStats.playerCurrentHealth;
            //damageTxt.text = attacker.GetComponent<EnemyStats>().enemyDamage.ToString();
            enemyAnimator.SetBool("hasAttacked", true);
            yield return new WaitForSeconds(1f);
            enemyAnimator.SetBool("hasAttacked", false);
            damageTxt.text = enemyStats.enemyDamage.ToString();

            StartCoroutine(DisplayDamage(target));

            //playerStats.PlayerCurrentHealth -= attacker.GetComponent<EnemyStats>().enemyDamage;
            playerStats.PlayerCurrentHealth -= enemyStats.enemyDamage;

            if (playerStats.PlayerCurrentHealth <= 0)
            {
                Debug.Log("Player dead");
            }            
        }

        startTime = Time.time;

        if (attacker.name == "Player")
        {
            playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            enemyAnimator.SetBool("isRunning", true);

        }

        while (Vector3.Distance(attacker.transform.position, originalPosition) > 0)
        {
            distanceMoved = (Time.time - startTime);
            fractionOfJourney = (distanceMoved / distanceToTarget) * movementSpeed;
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, originalPosition, fractionOfJourney);
            if (attacker.name == "Player")
            {
                attacker.transform.LookAt(originalPosition);
            }
            else
            {
                attacker.transform.LookAt(target.transform);
            }            
            yield return new WaitForSeconds(0.01f);
        }

        if (attacker.name == "Player")
        {
            attacker.transform.rotation = Quaternion.identity;
            playerAnimator.SetBool("isRunning", false);
        }
        else
        {
            enemyAnimator.SetBool("isRunning", false);
            attacker.transform.rotation = Quaternion.identity;

        }

        yield return new WaitForSeconds(1f);
                    
        StartCoroutine(CheckWhoseTurn());
        
    }

    public void HealYourself()
    {
        if (playerStats.PlayerCurrentHealth < playerStats.PlayerCurrentMaxHealth * healMultiplier)
        {
            turnInProgress = true;
            Debug.Log("Heal");
            player.GetComponent<ParticleSystem>().Play();
            playerStats.PlayerCurrentMaxHealth = playerStats.PlayerCurrentMaxHealth * healMultiplier;
            playerStats.PlayerCurrentHealth = playerStats.PlayerCurrentMaxHealth;
            StartCoroutine(CheckWhoseTurn());
        }
        else
        {
            Debug.Log("No heal");
            turnNumber--;
            StartCoroutine(CheckWhoseTurn());
        }
    }

    public IEnumerator MagicAttack(GameObject attacker)
    {
        playerStats = player.GetComponent<PlayerStats>();
        enemyStats = enemy.GetComponent<EnemyStats>();

        yield return new WaitUntil(() => battleStartEventDone);

        if (attacker.tag == "Player")
        {

            playerAnimator.SetBool("hasSliced", true);

            yield return new WaitForSeconds(3f);

            playerAnimator.SetBool("hasSliced", false);

            foreach (GameObject battleCharacter in battleParticipants)
            {
                if (battleCharacter.tag == "Enemy" && battleCharacter.activeInHierarchy)
                {
                    enemyStats = battleCharacter.GetComponent<EnemyStats>();
                    damageTxt.text = playerStats.playerDamage.ToString();

                    StartCoroutine(DisplayDamage(battleCharacter));

                    enemyStats.enemyCurrentHealth -= playerStats.playerDamage;

                    if (enemyStats.enemyCurrentHealth <= 0)
                    {
                        enemyStats.enemyDead = true;
                        battleCharacter.SetActive(false);
                        mouseOverEnemy = null;
                        battleParticipantsLeft = 0;
                        for (int i = 0; i < battleParticipants.Count; i++)
                        {
                            if (battleParticipants[i].activeInHierarchy)
                            {
                                battleParticipantsLeft++;
                            }
                        }
                        if (battleParticipantsLeft == 1 && !gameManager.restarted)
                        {
                            yield return new WaitForSeconds(2f);
                            playerStats.CurrentXP += battleXpValue;
                            StartCoroutine(BackToWorld());
                            turnInProgress = false;
                            if (gameManager.dangerZone)
                            {
                                gameManager.ToggleRandomBattle(true);
                                yield return null;
                            }
                        }
                    }
                }
            }

            //animator.SetBool("hasSliced", false);
        }

        else
        {
            damageTxt.text = enemyStats.enemyDamage.ToString();

            StartCoroutine(DisplayDamage(player));

            playerStats.PlayerCurrentHealth -= enemyStats.enemyDamage;

            if (playerStats.PlayerCurrentHealth <= 0)
            {
                Debug.Log("Player dead");
            }
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(CheckWhoseTurn());
    }

    private IEnumerator EscapeFromBattle()
    {
        int randomEscapeDamage = Random.Range(0, 5);

        yield return new WaitUntil(() => battleStartEventDone);

        damageTxt.text = randomEscapeDamage.ToString();
        StartCoroutine(DisplayDamage(player));
        playerStats.PlayerCurrentHealth -= randomEscapeDamage;

        playerAnimator.SetBool("isRunning", true);

        Vector3 startPosition = player.transform.position;
        player.transform.rotation = Quaternion.Euler(0, 180, 0);
        float escapeDistance = Vector3.Distance(startPosition, new Vector3(startPosition.x, startPosition.y, 3f));


        while (Vector3.Distance(startPosition, player.transform.position) <= escapeDistance)
        {
            distanceMoved = (Time.time - startTime);
            fractionOfJourney = (distanceMoved / escapeDistance) * movementSpeed;
            player.transform.position = Vector3.Lerp(player.transform.position, new Vector3(startPosition.x, startPosition.y, player.transform.position.z -0.5f), fractionOfJourney);
            yield return new WaitForSeconds(0.01f);
        }
        playerAnimator.SetBool("isRunning", false);
        yield return new WaitForSeconds(1f);

        StartCoroutine(BackToWorld());

    }

    private IEnumerator CheckWhoseTurn()
    {
        if (turnNumber > namesSorted.Count -1)
        {
            turnNumber = 0;
        }
        string turn = namesSorted[turnNumber];
        
            for (int i = 0; i < battleParticipants.Count; i++)
            {
            //Debug.Log("turn" + turnNumber);
            //Debug.Log("i " + i);
                if (battleParticipants[i].name == turn && battleParticipants[i].gameObject.activeInHierarchy)
                {
                    if (turn == "Player")
                    {
                        playersTurn = true;
                        turnInProgress = false;
                        turnNumber++;
                        EnableInterfaceButtons();
                    break;
                    }
                    else
                    {
                        enemy = battleParticipants[i];
                        playersTurn = false;
                        turnInProgress = false;
                        turnNumber++;

                    break;
                    }
                }                
                if (!battleParticipants[i].gameObject.activeInHierarchy && battleParticipants[i].name == turn)
                {
                    //Debug.Log("ei löydy " + turn);
                    turnNumber++;
                    StartCoroutine(CheckWhoseTurn());
                    yield return null;                
                }

            }
        
        //turnNumber++;        
        yield return new WaitForSeconds(0.1f);
}

    public IEnumerator BackToWorld()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            battleOverCanvas.SetActive(true);
            yield return new WaitForSeconds(3f);
        }
        if (gameManager.returnCameraMovement)
        {
            gameManager.cameraFollowDisabled = false;
        }
        if (playerAnimator)
        {
            playerAnimator.SetBool("isBattling", false);

        }
        SceneManager.LoadScene(0);
        yield return null;
    }

    private IEnumerator DisplayDamage(GameObject target)
    {
        damageCanvas.transform.position = (target.transform.position + Vector3.up * 3);
        damageCanvas.SetActive(true);
        damageCanvas.transform.LookAt(mainCamera.transform);

        yield return new WaitForSeconds(2f);

        damageCanvas.SetActive(false);
    }

    public void UpdateHPText() 
    {
        battleUIManager.hPText.text = playerStats.PlayerCurrentHealth.ToString("F0") + " / " + playerStats.PlayerCurrentMaxHealth.ToString("F0");
        battleUIManager.hPSlider.value = playerStats.PlayerCurrentHealth / playerStats.PlayerCurrentMaxHealth;
    }

    public void UpdateXP()
    {
        battleUIManager.xPText.text = playerStats.CurrentXP.ToString() + " / " + playerStats.NextLevelXP.ToString();
        battleUIManager.xPSlider.value = playerStats.CurrentXP / playerStats.NextLevelXP;
        battleUIManager.levelText.text = "Level: " + playerStats.PlayerLevel.ToString();
    }       

    private void SortByInitiative()
    {
        Dictionary<float, string> initiativeNameDict = new Dictionary<float, string>();

        foreach (GameObject gameObject in battleParticipants)
        {
            float initiative;
            if (gameObject.name == "Player")
            {
                initiative = gameObject.GetComponent<PlayerStats>().playerInitiative;
            }
            else
            {
                initiative = gameObject.GetComponent<EnemyStats>().enemyInitiative;
            }

            while(initiativeNameDict.ContainsKey(initiative))
            {
                initiative += 0.01f;
            }

            initiativeNameDict.Add(initiative, gameObject.name);
        }

        List<float> initiatives = initiativeNameDict.Keys.ToList();

        initiatives.Sort();        

        for (int i = 0; i < initiativeNameDict.Count; i++)
        {
            //Debug.Log(initiatives[i]);
            namesSorted.Add(initiativeNameDict[initiatives[i]]);
        }
        //Debug.Log(initiativeNameDict[initiatives[turnNumber]]);        
    }

    private void SetupBattle()
    {
        gameManager.scriptedEvents.RemoveAll(item => item == null);

        SelectBattleGround();
        int numberOfEnemies = Random.Range(1, 4);

        battleParticipants.Add(player);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)].gameObject, enemyPositions[i]);
            enemy.name = "enemy" + i;
            battleParticipants.Add(enemy);
        }
        foreach (GameObject go in battleParticipants)
        {
            if (go.name != "Player")
            {
                battleXpValue += go.GetComponent<EnemyStats>().xpValue;
            }
        }
        enemy = battleParticipants[1];
        turnNumber = 0;
        SortByInitiative();
        UpdateHPText();
        StartCoroutine(CheckWhoseTurn());
    }

    private void SelectBattleGround()
    {
        switch (gameManager.battleZoneNumber)
        {
            case 1:
                Instantiate(battleZonePrefabs[0]);
                break;
            case 2:
                Instantiate(battleZonePrefabs[1]);
                break;
            case 3:
                Instantiate(battleZonePrefabs[2]);
                break;
            default:
                break;
        }
    }

    public void PlayerAttack()
    {
        if (mouseOverEnemy)
        {
            turnInProgress = true;
            battleUIManager.targetIndicator.SetActive(false);
            attackButtonPressed = false;
            DisableInterfaceButtons();
            StartCoroutine(MeleeAttack(player, mouseOverEnemy));
        }
    }

    public void PlayerMagic()
    {
        turnInProgress = true;
        battleUIManager.targetIndicator.SetActive(false);
        DisableInterfaceButtons();
        StartCoroutine(MagicAttack(player));
    }

    public void PlayerEscape()
    {
        turnInProgress = true;
        battleUIManager.targetIndicator.SetActive(false);
        DisableInterfaceButtons();
        StartCoroutine(EscapeFromBattle());

    }
    private void DisableInterfaceButtons()
    {
            attackButton.enabled = false;
            magicButton.enabled = false;
            firstAidButton.enabled = false;
            escapeButton.enabled = false;
    }
    private void EnableInterfaceButtons()
    {
            attackButton.enabled = true;
            magicButton.enabled = true;
            escapeButton.enabled = true;
        if (playerStats.PlayerCurrentHealth / playerStats.PlayerCurrentMaxHealth <= .7f)
        {
            firstAidButton.enabled = true;
        }
    }

    public void EnableAttack()
    {
        if (attackButtonPressed)
        {
            attackButtonPressed = false;
            battleUIManager.targetIndicator.SetActive(false);
        }
        else
        {
            attackButtonPressed = true;
        }
    }
}
