using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private BattleManager battleManager;
    private BattleUIManager battleUIManager;

    private Vector3 targetIndicatorPos = new Vector3(0, -0.95f, 0);

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.instance;
        battleUIManager = battleManager.battleUIManager;
    }

    // Update is called once per frame
    private void OnMouseOver()
    {
        GameObject targetIndicator = battleUIManager.targetIndicator;
        if (battleManager.playersTurn && !battleManager.turnInProgress && battleManager.attackButtonPressed)
        {
            battleManager.mouseOverEnemy = this.gameObject;
            targetIndicator.SetActive(true);
            targetIndicator.transform.position = transform.position + targetIndicatorPos;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                battleManager.PlayerAttack();
            }
        }
    }
    

}
