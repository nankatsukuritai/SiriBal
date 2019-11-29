﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Generic;
using Generic.Manager;

public class ResultSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartButtonClicked()
    {
        GameSceneManager.ChangeScene(GameScenes.SeriousBalloon);
    }

    public void RankingButtonClicked()
    {
        GameSceneManager.ChangeScene(GameScenes.ScoreBoard);
    }
    public void ToTopButtonClicked()
    {
        GameSceneManager.ChangeScene(GameScenes.Top);
    }
}
