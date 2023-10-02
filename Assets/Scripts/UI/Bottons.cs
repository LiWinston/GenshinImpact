using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Bottons : MonoBehaviour
{
    public void Start(){
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("WinScene");
    }
}
