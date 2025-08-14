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
        levelStatus = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            levelStatus = false;
            OnLevelComplete?.Invoke();
        }
    }
}
