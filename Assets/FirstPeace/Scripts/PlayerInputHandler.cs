using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerManager))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerManager _playerManager;
    
    private void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;
            
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
            {
                Vector3 worldPosition = hit.point;
                
                _playerManager.MovePlayerToXPosition(worldPosition.x);
            }
        }
    }
}
