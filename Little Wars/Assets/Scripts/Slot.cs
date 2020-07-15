using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{

    public int slotIndex;
    //public GameObject containing;
    public bUnit myUnit;
    public UnitMesh myUnitMesh;
    GameController gc;

    public Transform pos;

    // Start is called before the first frame update
    void Start()
    {
        pos = gameObject.transform.GetChild(0).transform;
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void OnMouseEnter()
    {
        gc.mouseOverSlotFunc(this);
    }

    void OnMouseExit()
    {
        gc.mouseExitSlotFunc(this);

    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gc.rightMouseSlotFunc(this);
        }
    }

    void OnMouseDown()
    {
        gc.leftMouseSlotFunc(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void assignUnit(bUnit unit)
    {
        if (myUnitMesh == null)
        {
            myUnitMesh = transform.GetChild(0).GetComponent<UnitMesh>();
            //myUnitMesh.transform.localScale = new Vector3(.5f, .5f, .5f);
        }
        myUnitMesh.gameObject.SetActive(true);

        myUnit = unit;

        myUnitMesh.myUnit = unit;
        myUnitMesh.initUnitRenderComponents();
        myUnitMesh.mySlot = this;

    }

    public void emptyUnit()
    {
        myUnitMesh.gameObject.SetActive(false);
        myUnit = null;
    }
}
