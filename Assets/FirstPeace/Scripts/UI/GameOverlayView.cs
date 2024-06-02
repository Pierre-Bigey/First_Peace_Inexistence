using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverlayView : MonoBehaviour
{
    [Tooltip("The game over panel")]
    [SerializeField] private GameObject _gameOverPanel;
    [Tooltip("The kill count text")]
    [SerializeField] private TMP_Text killCountText;
    [Tooltip("The enemy count text")]
    [SerializeField] private TMP_Text EnemyCountText;
    
    public void ShowGameOverPanel()
    {
        _gameOverPanel.SetActive(true);
    }
    
    public void UpdateKillCount(int killCount)
    {
        killCountText.text = "Kills: " + killCount;
    }
    
    public void UpdateEnemyCount(int enemyCount)
    {
        EnemyCountText.text = "Enemies: " + enemyCount;
    }
    
}
