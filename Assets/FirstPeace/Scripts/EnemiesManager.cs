using System.Collections;
using System.Collections.Generic;
using FirstPeace.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This class is responsible for managing the enemies in the game. It will spawn the enemies at an decreasing rate
/// and make them move towards the player. When an enemy reached a certain distance from the player, it gives
/// the enemy the EnemyController component and stop managing it.
/// </summary>
public class EnemiesManager : MonoBehaviour
{
    //Singleton pattern
    public static EnemiesManager Instance { get; private set; }
    [Header("Enemy Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float EnemySpeed = 1f;
    [SerializeField] private float EnemyHealth = 5f;
    [SerializeField] private float _zDistanceToPlayer = 5f;
    
    [Header("Spawn Rate Settings")]
    [SerializeField] private float _initialMeanSpawnRate = 1f;
    [SerializeField] private float _spawnRateDecrease = 0.01f;
    [SerializeField] private float _minSpawnRate = 0.1f;
    [SerializeField] private float _initialStdDev = 0.1f;
    [SerializeField] private float _stdDevDecrease = 0.01f;
    [SerializeField] private float _minStdDev = 0.01f;
    
    [Header("Pool settings")]
    [SerializeField] private int _initialPoolSize = 10;
    [SerializeField] private float _deathRatio = 0.5f;
    
    [Header("Spawn Position Settings")]
    [SerializeField] private float zSpawnPosition = 50f;
    [SerializeField] private float xSPawnAmplitude = 5f;
    //The enemy container
    [SerializeField] private GameObject _enemiesContainer;
    
    

    
    
    [SerializeField] private GameObject _player;
    
    //The list of supervised enemies that are currently on the map
    private List<GameObject> _supervisedEnemiesList = new List<GameObject>();
    //The list of autonomous enemies that are currently on the map
    private List<GameObject> _autonomousEnemiesList = new List<GameObject>();

    
    private CustomRandom.GaussianRandom _gaussianRandom;
    
    private float _currentSpawnRate;
    private float _currentStdDev;
    
    private float _timeSinceLastSpawn;
    private float _timeToNextSpawn;
    
    private float zPlayerPosition;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _currentSpawnRate = _initialMeanSpawnRate;
        _gaussianRandom = new CustomRandom.GaussianRandom(_initialMeanSpawnRate, _initialStdDev);
        zPlayerPosition = _player.transform.position.z;
        Initialize();
    }

    private void Initialize()
    {
        //Fill the pool with enemies
        for (int i = 0; i < _initialPoolSize; i++)
        {
            SpawnEnemy();
        }
    }

    private void Update()
    {
        HandleEnemySpawn();
        MoveEnemiesForward();
        CheckEnemyProximity();
    }

    private void HandleEnemySpawn()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn >= _timeToNextSpawn)
        {
            _timeSinceLastSpawn = 0;
            _timeToNextSpawn = GetNextSpawnTime();
            
            SpawnEnemy();
        }
    }
    
    private float GetNextSpawnTime()
    {
        float spawnTime = _gaussianRandom.Next(_currentSpawnRate, _currentStdDev);
        
        _currentSpawnRate -= _spawnRateDecrease;
        if (_currentSpawnRate < _minSpawnRate)
        {
            _currentSpawnRate = _minSpawnRate;
        }
        
        _currentStdDev -= _stdDevDecrease;
        if (_currentStdDev < _minStdDev)
        {
            _currentStdDev = _minStdDev;
        }
        
        return spawnTime;
    }

    /// <summary>
    /// Spawn an enemy at the spawn position
    /// </summary>
    private void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-xSPawnAmplitude, xSPawnAmplitude), 0, zSpawnPosition);
        GameObject enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity,_enemiesContainer.transform);
        enemy.transform.forward = -transform.forward;
        EnemyController enemyController = enemy.AddComponent<EnemyController>();
        enemyController.Initialize(EnemyHealth, this);
        enemyController.enabled = true;
        
        AutonomousEnemy autonomousEnemy = enemy.AddComponent<AutonomousEnemy>();
        autonomousEnemy.enabled = false;
        _supervisedEnemiesList.Add(enemy);
    }

    /// <summary>
    /// Goes through the list of supervised enemies and moves them forward by the given speed.
    /// </summary>
    private void MoveEnemiesForward()
    {
        Debug.Log("Moving enemies forward!");
        for (int i = 0; i < _supervisedEnemiesList.Count; i++)
        {
            _supervisedEnemiesList[i].transform.Translate(Vector3.forward * (EnemySpeed * Time.deltaTime));
        }
    }
    
    /// <summary>
    /// Goes through the list of supervised enemies and checks if they are close to the player. If they are, it will give them the
    /// AutonomousEnemy component and stop managing them by removing them from the supervised list and add them to the autonomous list.
    /// </summary>
    private void CheckEnemyProximity()
    {
        for (int i = 0; i < _supervisedEnemiesList.Count; i++)
        {
            //If the i-th enemy is close to the player
            if (_supervisedEnemiesList[i].transform.position.z - zPlayerPosition < _zDistanceToPlayer)
            {   
                MakeEnemyAutonomous(i);
            }
            else //Else, we can leave the loop because the list is ordered by distance
            {
                return;
            }
        }
    }
    
    public void KilledEnemy(GameObject enemy)
    {
        if (_autonomousEnemiesList.Contains(enemy))
        {
            _autonomousEnemiesList.Remove(enemy);
        }
        else
        {
            _supervisedEnemiesList.Remove(enemy);
        }
        //Reset or kill the enemy depending on the death ratio
        if (Random.Range(0f, 1f) < _deathRatio)
        {
            Destroy(enemy);
        }
        else
        {
            ResetEnemy(enemy);
        }
    }
    
    private void MakeEnemyAutonomous(int i)
    {
        AutonomousEnemy autonomousEnemy = _supervisedEnemiesList[i].GetComponent<AutonomousEnemy>();
        autonomousEnemy.enabled = true;
        autonomousEnemy.Initialize(EnemySpeed, _player);
        _autonomousEnemiesList.Add(_supervisedEnemiesList[i]);
        _supervisedEnemiesList.RemoveAt(i);
    }
    public void ResetEnemy(GameObject enemy)
    {
        enemy.GetComponent<AutonomousEnemy>().enabled = false;
        
        //Then realoccates the enemy to the pool
        enemy.transform.position = new Vector3(Random.Range(-xSPawnAmplitude, xSPawnAmplitude), 0, zSpawnPosition);
        _supervisedEnemiesList.Add(enemy);
    }

    public void UpdateKillScore(int changed)
    {
        
    }

}

    
