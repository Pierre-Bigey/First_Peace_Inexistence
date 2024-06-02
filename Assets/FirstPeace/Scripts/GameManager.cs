using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private UIManager _uiManager;

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        _uiManager = UIManager.Instance;
        _uiManager.ShowGameOverlayPanel();
        
    }

    public void GameOver()
    {
        //Pausing the game
        Time.timeScale = 0;
        //Showing game over panel
        _uiManager.ShowGameOverOverlayPanel();
    }
    
    public void Restart()
    {
        //Resuming the game
        Time.timeScale = 1;
        //Reloading the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
