using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private BattleManager battleManager;
    private GameManager gameManager;

    public float playerInitiative = 4;
    public float playerDamage = 4;
    public float playerMaxHealth = 100;

    private static float playerCurrentMaxHealth = 50;
    private static float playerCurrentHealth = 50;
    private static float currentXP = 0;
    private static float nextLevelXP = 10;
    private static int playerLevel = 1;


    public float PlayerCurrentHealth 
    {
        get { return playerCurrentHealth; }
        set 
        { 
            playerCurrentHealth = value;
            battleManager.UpdateHPText();
        } 
    }
    public float PlayerCurrentMaxHealth 
    {
        get { return playerCurrentMaxHealth; }
        set 
        { 
            playerCurrentMaxHealth = value;
            battleManager.UpdateHPText();
        } 
    }

    public float CurrentXP 
    {
        get { return currentXP; }
        set {
            if (battleManager.battleXpValue + currentXP >= nextLevelXP)
            {
                currentXP = battleManager.battleXpValue - (nextLevelXP - currentXP);
                playerLevel++;
                NextLevelXP *= 2;
            }
            else
            {
                currentXP = value;                    
            }

            battleManager.UpdateXP();
        } 
    }
    public float NextLevelXP 
    {
        get { return nextLevelXP; }
        set { 
            nextLevelXP = value; 
            
        } 
    }
    public int PlayerLevel 
    {
        get { return playerLevel; }
        set {
            playerLevel = value; 
            
        } 
    }

    //public bool playerTurnComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.instance;
        gameManager = GameManager.instance;

        if (!gameManager.player)
        {
            gameManager.player = this.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
