using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject panelMenuPause;
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!panelMenuPause.gameObject.activeSelf)
            {
                ActivateMenuPause();
            }
            else if (panelMenuPause.gameObject.activeSelf)
            {
                DeactivateMenuPause();
            }
        }
    }

    public void ActivateMenuPause()
    {
        panelMenuPause.gameObject.SetActive(true);
        
        StopAllCoroutines();

        Time.timeScale = 0;
    }

    public void DeactivateMenuPause()
    {
        panelMenuPause.gameObject.SetActive(false);

        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        Time.timeScale = 1;
    }
}
