using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    
    public TextMeshProUGUI playerScoreTxt;                                          //Score text that is shown during game

    public TextMeshProUGUI finalScoreTxt;                                           //Score to be shown in game over screen
    public TextMeshProUGUI highScoreText;                                           // Highscore to be shown in game over screen
    public TextMeshProUGUI startCounterTxt;                                         //Counter at race start

    public GameObject pauseScreen;
    public GameObject gameOverScreen;

    public Slider batteryBar;
    public Slider speedBar;

    private float playerScore;

    public Animator playerScoreAnimator;

    private SceneLoader sceneLoader;

    [SerializeField] private float speedbarMax = 85;



    // Start is called before the first frame update
    void Start()
    {
        //Sets sliders maximum values to current maximum values
        batteryBar.maxValue = GameManager.instance.batteryLife;
        speedBar.maxValue = speedbarMax;

        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
    }


    // Update is called once per frame
    void Update()
    {
        //Handles escape key: Opens pause menu. If already open, closes pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.instance.gameOver)
            {
                if (pauseScreen.activeInHierarchy)
                {
                    ContinueGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }


    private void FixedUpdate()
    {
        batteryBar.value = GameManager.instance.batteryLife;                    //Sets battrylife value to same as in UI batterybar
        speedBar.value = GameManager.instance.velocity;                         //Sets UI speedbar to same as speed
    }


    //Function for pause menu: sets pause screen active, stops time and ground movement, saves current speed to temporary variable
    public void PauseGame()
    {
        if (!GameManager.instance.gameOver)
        {
            GameSoundManager.instance.SetPause(true);
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }        
    }


    //Function for continue game: deactivates pause screen, sets speed and time back to same as before pausing
    public void ContinueGame()
    {
        GameSoundManager.instance.SetPause(false);
        pauseScreen.SetActive(false);       
        Time.timeScale = 1f;
    }


    //Function for restarting the game: set time to normal and reload current scene
    public void Retry()
    {        
        Time.timeScale = 1f;
        sceneLoader.LoadSceneAsyncByIndex(3);
    }


    //Function for returning to main menu: set time to normal and load previous scene
    public void backToMenu()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
        sceneLoader.LoadSceneAsyncByIndex(1);
    }


    //Function for game over: sets game over screen active, stops time and displays final score
    public void GameOver()
    {
        if (!GameManager.instance.gameOver)
        {
            GameManager.instance.gameOver = true;
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;
            playerScore = GameManager.instance.PlayerScore;
            finalScoreTxt.text = "Pisteet: " + playerScore.ToString("0");
            highScoreText.text = "Ennätys: " + StaticVariables.PlayerHighScore.ToString();

            if (playerScore > StaticVariables.PlayerHighScore)                  //Updates high score if new record
            {
                finalScoreTxt.text = "Pisteet: " + playerScore.ToString("0") + " Uusi ennätys!";
                highScoreText.text = "Ennätys: " + playerScore.ToString("0");
                StaticVariables.PlayerHighScore = playerScore;
            }
        }
    }

    public void HighlightScoreOn()
    {
        if (!playerScoreAnimator.GetBool("ScoreEffectsOn"))
        {
            playerScoreAnimator.SetBool("ScoreEffectsOn", true);
        }
        
    }

    public void HighlightScoreOff()
    {
        if (playerScoreAnimator.GetBool("ScoreEffectsOn"))
        {
            playerScoreAnimator.SetBool("ScoreEffectsOn", false);
        }
    }
}
