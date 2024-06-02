using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    
    [Tooltip("The speed at which the player moves")]
    [SerializeField] private float _speed = 3;
    [Tooltip("The threshold for the player to move")]
    [SerializeField] private float _moveThreshold = 0.1f;
    
    public void MovePlayerToXPosition(float xPosition)
    {
        //Apply the threshold to the x position
        if (Mathf.Abs(xPosition - transform.position.x) > _moveThreshold)
        {
            //Translate the player to the x position with the given speed
            Vector3 Direction = new Vector3(xPosition-transform.position.x, 0, 0).normalized;
            transform.Translate(Direction * (_speed * Time.deltaTime));
        }
    }

    /// <summary>
    /// Function called when an enemy hits the player
    /// </summary>
    public void Die()
    {
        //Set the time speed to 0
        Time.timeScale = 0;
        //Show the game over panel
        
    }
    
}
