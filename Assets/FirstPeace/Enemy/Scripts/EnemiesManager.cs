using System;
using System.Collections;
using System.Collections.Generic;
using FirstPeace.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    [Tooltip("The prefab of the enemy that will be spawned")]
    [SerializeField] private GameObject _enemyPrefab;
    [Tooltip("The speed at which the enemies will move")]
    [SerializeField] private float EnemySpeed = 1f;
    [Tooltip("The health of the enemies")]
    [SerializeField] private float EnemyHealth = 5f;
    [Tooltip("The distance from the player at which the enemy will be given the AutonomousEnemy component")]
    [SerializeField] private float _zDistanceToPlayer = 5f;
    
    [Header("Spawn Rate Settings")]
    [Tooltip("The initial mean spawn rate")]
    [SerializeField] private float _initialMeanSpawnRate = 1f;
    [Tooltip("The rate at which the spawn rate will decrease")]
    [SerializeField] private float _spawnRateDecrease = 0.01f;
    [Tooltip("The minimum spawn rate")]
    [SerializeField] private float _minSpawnRate = 0.1f;
    [Tooltip("The initial standard deviation of the spawn rate")]
    [SerializeField] private float _initialStdDev = 0.1f;
    [Tooltip("The rate at which the standard deviation will decrease")]
    [SerializeField] private float _stdDevDecrease = 0.01f;
    [Tooltip("The minimum standard deviation")]
    [SerializeField] private float _minStdDev = 0.05f;
    
    [Header("Pool settings")]
    [Tooltip("The initial size of the pool")]
    [SerializeField] private int _initialPoolSize = 10;
    [Tooltip("The ratio of enemies that will be reset instead of killed")]
    [SerializeField] private float _deathRatio = 0.5f;
    [Tooltip("The maximum size of the pool")]
    [SerializeField] private int _maxPoolSize = 40;
    
    [Header("Spawn Position Settings")]
    [Tooltip("The z position at which the enemies will spawn")]
    [SerializeField] private float zSpawnPosition = 50f;
    [Tooltip("The x amplitude at which the enemies will spawn")]
    [SerializeField] private float xSPawnAmplitude = 5f;
    //The enemy container
    [Tooltip("The container in which the enemies will be spawned")]
    [SerializeField] private GameObject _enemiesContainer;
    [Tooltip("The player object")]
    [SerializeField] private GameObject _player;
    
    
    public Action OnEnemyKilled;
    public Action OnEnemySpawn;
    
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

    /// <summary>
    /// Will decide if an enemy should be spawned or not
    /// </summary>
    private void HandleEnemySpawn()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn >= _timeToNextSpawn)
        {
            _timeSinceLastSpawn = 0;
            _timeToNextSpawn = GetNextSpawnTime();
            if (_supervisedEnemiesList.Count + _autonomousEnemiesList.Count < _maxPoolSize)
            {
                SpawnEnemy();
            }
        }
    }
    
    /// <summary>
    /// Return the next spawn time using a gaussian distribution
    /// </summary>
    /// <returns></returns>
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
        
        OnEnemySpawn.Invoke();
    }

    /// <summary>
    /// Goes through the list of supervised enemies and moves them forward by the given speed.
    /// </summary>
    private void MoveEnemiesForward()
    {
        //Debug.Log("Moving enemies forward!");
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
        OnEnemyKilled.Invoke();
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
        
        OnEnemySpawn?.Invoke();
    }

}

    
