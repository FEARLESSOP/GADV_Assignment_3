using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public bool levelStatus;

    // Start is called before the first frame update
    void Start()
    {
        levelStatus = true;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            levelStatus = false;
        }
    }
}
