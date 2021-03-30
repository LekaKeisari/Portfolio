using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;

    private bool canLaunch = true;
    private FileInfo fileInfo = new FileInfo(Environment.CurrentDirectory);
    private DirectoryInfo directoryInfo;
    private string fileName = "Muuntamo.exe";
    private string folderName = "Muuntamo";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        directoryInfo = fileInfo.Directory;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SwitchApp()
    {
        // Check if file exist
        if (File.Exists(directoryInfo + "\\" + folderName + "\\" + fileName) && canLaunch == true)
        {
            canLaunch = false;
            // Start launching the other application
            Process.Start(directoryInfo + "\\" + folderName + "\\" + fileName);
            // Quit current application
            Application.Quit();
        }
    }
}
