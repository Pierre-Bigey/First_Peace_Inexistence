using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverOverlayController : MonoBehaviour
{
    public void Restart()
    {
        GameManager.Instance.Restart();
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
