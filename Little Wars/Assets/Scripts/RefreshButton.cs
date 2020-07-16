using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshButton : MonoBehaviour
{
    GameController gc;
    public Material defaultMat;
    public Material overMat;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        gameObject.GetComponent<MeshRenderer>().material = overMat;
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = defaultMat;
    }

    void OnMouseDown()
    {
        //gc.mk.emptyStoredMarket();
        if (gc.ctrl >= 1)
        {
            gc.ctrl -= 1;
            gc.mk.resetMarket();
            if (gc.st.isInfinite)
            {
                gc.mk.initMarketPhase(null);
            }
            else
            {
                gc.mk.initMarketPhase(gc.levels[gc.levelCount]);
            }
        }
    }
}
