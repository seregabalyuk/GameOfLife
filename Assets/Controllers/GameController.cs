using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField]
    FlagWatcher[] flags;
    [SerializeField]
    CellAutomation automation;
    [SerializeField]
    Texture2D playerBase;
    [SerializeField] 
    int moneyPerStep = 1;
    [SerializeField]
    int factorCost = 3;
    [SerializeField]
    int startMoney = 100;
    [SerializeField]
    TextMeshProUGUI[] moneyText;
    [SerializeField]
    float radiusBase;
    [SerializeField]
    float speedMin = 1;
    [SerializeField]
    float speedMax = 1000;
    [SerializeField]
    TextMeshProUGUI textWin;
    [SerializeField]
    RunPause runPause;
    
    
    
    float timeForStep = 1;
    bool isPause = true;

    int []moneyPlayers;
    float time = 0;
    

    void Start() {
        CreateBases();
        SetSpeed(0.5f);
    }

    void CreateBases() {
        var points = ShapeFromImage.CreatePoints(playerBase);
        foreach (var flag in flags) {
            var pos = flag.posInScreen;
            foreach (var point in points) {
                automation[pos + point] = true;
            }
        }
        moneyPlayers = new int[flags.Length];
        for (int i = 0; i < flags.Length; ++ i) {
            moneyPlayers[i] = startMoney;
        }
    }
    void UpdateCells() {
        while (time > timeForStep) {
            automation.NextStep();
            time -= timeForStep;
            for (int i = 0; i < moneyPlayers.Length; ++ i) {
                if (moneyPlayers[i] != -1) {
                    moneyPlayers[i] += moneyPerStep;
                }
            }
        }
    }

    void UpdatePause() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            isPause = !isPause;
            runPause.ChangeState(isPause);
        }
    }

    void CheckWin() {
        int countLive = 0;
        int numberLive = -1;
        for (int i = 0; i < flags.Length; ++ i) {
            var pos = flags[i].posInScreen;
            if(!automation[pos]) {
                moneyPlayers[i] = -1;
            }
            if(moneyPlayers[i] != -1) {
                countLive += 1;
                numberLive = i;
            }
        }
        if (countLive == 1) {
            textWin.text = "WIN " + (numberLive  + 1).ToString();
            textWin.color = flags[numberLive].GetColor();
        }
    }

    void Update() {
        time += Time.deltaTime;
        UpdatePause();
        if (!isPause) {
            UpdateCells();
            CheckWin();
        } else {
            time = 0;
        }
        for (int i = 0; i < moneyPlayers.Length; ++ i) {
            if (moneyPlayers[i] != -1) {
                moneyText[i].text = (moneyPlayers[i] / factorCost).ToString();
            } else {
                moneyText[i].text = "Dead";
            }
            
        }
    }
 

    public void SetSpeed(float percent) {
        timeForStep = 1.0f / (speedMin + (speedMax - speedMin) * percent);
    }

    public void LimitClick(MouseControler controler) {
        if (!isPause) {
            controler.CanDraw(false);
            return;
        }
        bool can = false;
        for (int i = 0; i < flags.Length; ++ i) {
            var pos = flags[i].posInScreen;
            var clickPos = controler.GetPosPixel();
            var points = controler.GetPoints();
            if ((pos - clickPos).magnitude <= radiusBase &&
                moneyPlayers[i] >= points.Count * factorCost) {
                can = true;
                moneyPlayers[i] -= points.Count;
                break;
            }
        }
        controler.CanDraw(can);
    }

    public void Restart() {
        automation.Clear();
        CreateBases();
        textWin.text = "";
    }
}
