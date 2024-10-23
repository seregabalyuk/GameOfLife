using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Sandbox : MonoBehaviour {
    [SerializeField]
    CellAutomation automation;
    [SerializeField]
    float speedMin = 1;
    [SerializeField]
    float speedMax = 1000;
    
    float timeForStep = 1;
    bool isPause = false;

    int []moneyPlayers;
    float time = 0;
    

    void Start() {
        SetSpeed(0.5f);
    }

    void UpdateCells() {
        while (time > timeForStep) {
            automation.NextStep();
            time -= timeForStep;
        }
    }

    void UpdatePause() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            isPause = !isPause;
        }
    }

    void Update() {
        time += Time.deltaTime;
        UpdatePause();
        if (!isPause) {
            UpdateCells();
        } else {
            time = 0;
        }
    }
 

    public void SetSpeed(float percent) {
        timeForStep = 1.0f / (speedMin + (speedMax - speedMin) * percent);
    }

    public void Restart() {
        automation.Clear();
    }
}
