using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverlayController : MonoBehaviour
{
    [Tooltip("The player manager object")]
    [SerializeField] private PlayerManager _playerManager;
    
    private EnemiesManager _enemiesManager;
    [Tooltip("The game overlay view component")]
    [SerializeField] private GameOverlayView _gameOverlayView;
    
    private int _killCount;
    private int _enemyCount;
    private void OnEnable()
    {
        _enemiesManager = EnemiesManager.Instance;
        
        _enemiesManager.OnEnemyKilled += OnEnemyKilled;
        _enemiesManager.OnEnemySpawn += OnEnemySpawned;
    }
    
    private void OnEnemyKilled()
    {
        ChangeKillCount(1);
        ChangeEnemyCount(-1);
    }
    
    private void OnEnemySpawned()
    {
        ChangeEnemyCount(1);
    }

    private void ChangeEnemyCount(int change)
    {
        _enemyCount += change;
        _gameOverlayView.UpdateEnemyCount(_enemyCount);
    }
    
    private void ChangeKillCount(int change)
    {
        _killCount += change;
        _gameOverlayView.UpdateKillCount(_killCount);
    }
    
    
}
