using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public bool levelStatus;
    public event Action OnLevelComplete;

    void Start()
    {
        //set level status to active
        levelStatus = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //check if object is player
        if (other.CompareTag("Player"))
        {
            //set level status to inactive
            levelStatus = false;
            //call level complete event
            OnLevelComplete?.Invoke();
        }
    }
}
