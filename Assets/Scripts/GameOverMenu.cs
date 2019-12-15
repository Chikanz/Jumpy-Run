using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameOverMenu : MonoBehaviour
{
    private float StartScale;
    public float timeToScale = 1;
    
    private void Start()
    {
        StartScale = transform.localScale.x;
        
        //Scale this menu in when game is over
        GameManager.Instance.OnStateChanged += state =>
        {
            switch (state)
            {
                case GameManager.eGameState.RUNNING:
                    toggleMenu(false);
                    break;
                case GameManager.eGameState.GAMEOVER:
                    toggleMenu(true);
                    StartCoroutine(scale(timeToScale));
                    break;
            }
        };
    }
    
    IEnumerator scale(float timeToScale)
    {
        float newScale = 0;
        float timeElapsed = 0;
        while (timeElapsed <= timeToScale)
        {
            newScale = Mathf.Lerp(0.0f, StartScale, timeElapsed / timeToScale);
            transform.localScale = Vector3.one * newScale;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    void toggleMenu(bool enabled)
    {
        transform.GetChild(0).gameObject.SetActive(enabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
