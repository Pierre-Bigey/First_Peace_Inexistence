using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Singleton instance
    public static UIManager Instance { get; private set; }
    
    
    [Tooltip("The game overlay panel")]
    [SerializeField] private GameObject _gameOverlayPanel;
    
    [Tooltip("The game overlay view component")]
    private GameOverlayController _gameOverlayController;
    
    [Tooltip("The game over panel")]
    [SerializeField] private GameObject _gameOverOverlayPanel;
    [Tooltip("The game over overlay controller")]
    private GameOverOverlayController _gameOverOverlayController;
    
    //Singleton instanciation
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
        _gameOverlayController = _gameOverlayPanel.GetComponent<GameOverlayController>();
        _gameOverOverlayController = _gameOverOverlayPanel.GetComponent<GameOverOverlayController>();
    }
    
    public void ShowGameOverOverlayPanel()
    {
        _gameOverlayPanel.SetActive(false);
        _gameOverOverlayPanel.SetActive(true);
    }
    
    public void ShowGameOverlayPanel()
    {
        _gameOverlayPanel.SetActive(true);
        _gameOverOverlayPanel.SetActive(false);
    }
    
}

