using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component will be set on an enemy when it reaches a certain distance from the player and will
/// make it move towards the player. It contains a Initialize method that will be called by the EnemiesManager with the given
/// speed.
/// </summary>
public class AutonomousEnemy : MonoBehaviour
{
    private float _speed;
    private GameObject _player;

    public void Initialize(float speed, GameObject player)
    {
        _speed = speed;
        _player = player;
    }
    
    private void Update()
    {
        Vector3 direction = (_player.transform.position - transform.position ).normalized;
        transform.Translate(direction * (_speed * Time.deltaTime), Space.World);
    }
}
    
