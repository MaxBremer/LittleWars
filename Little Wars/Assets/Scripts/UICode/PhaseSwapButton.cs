using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseSwapButton : MonoBehaviour
{
    GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pressed()
    {
        if (gc.curPhase == GamePhase.Prep)
        {
            GameObject.Find("TurnPhaseText").GetComponent<Text>().text = "Fight!";
            gc.initFight(true);
        }
    }
}
