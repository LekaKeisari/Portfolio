using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas settingsCanvas;                                              //Canvas for settings-menu
    [SerializeField]
    private Canvas pauseCanvas;                                                 //Canvas for pause-menu

   
    //Loads next scene by build index
   public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Quits application
    public void QuitGame()
    {
        Application.Quit();
    }

    //Opens and closes settings-menu
    public void HandleMenu(Canvas menuCanvas)
    {
        if (menuCanvas.isActiveAndEnabled)
        {
            menuCanvas.gameObject.SetActive(false);                         //If menu is open close it
        }
        else
        {
            menuCanvas.gameObject.SetActive(true);                          //If menu is closed open it
        }
    }

    //Opens and closes pause-menu and freezes time
    public void PauseGame()
    {
        if (pauseCanvas.isActiveAndEnabled)
        {
            pauseCanvas.gameObject.SetActive(false);                            //If menu is open close it
            Time.timeScale = 1f;                                                //Set time to move normally
        }
        else
        {
            pauseCanvas.gameObject.SetActive(true);                             //If menu is closed open it
            Time.timeScale = 0f;                                                //Stops time
        }
    }
}
