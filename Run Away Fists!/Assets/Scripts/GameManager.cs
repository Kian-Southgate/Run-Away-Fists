using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject subMenu;

    //THIS CODE WILL BE USED LATER DO NOT DELETE - KIAN
    /* private void Awake()
    {
        DontDestroyOnLoad(this);
    }*/

    // Use this for initialization
    void Start()
    {


        //if (SceneManager.GetActiveScene() != Scen)
    }

    // Update is called once per frame
    void Update()
    {

        //ATTEMPT TO ADD SUBMENU IN EACH SCENE EXCEPT MAIN MENU VIA CODE IS IN PROGRESS - KIAN
        /* if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            subMenu = new GameObject("subMenu");

        };*/


    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadLevel4()
    {
        SceneManager.LoadScene(4);
    }

    public void LoadLevel5()
    {
        SceneManager.LoadScene(5);
    }

    public void LoadTutorialLevel()
    {
        SceneManager.LoadScene(6);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
