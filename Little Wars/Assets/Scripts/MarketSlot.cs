using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketSlot : MonoBehaviour
{
    public int buyPrice;
    public Unit storedUnit;
    public Material altMaterial;
    public Vector3 unitPlacement;
    public bool isInvSlot;
    Material defaultMaterial;

    GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        unitPlacement = gameObject.transform.GetChild(0).transform.position;
        defaultMaterial = gameObject.GetComponent<MeshRenderer>().material;

        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    /*void OnMouseEnter()
    {
        mouseOverFunc();
    }*/

    void OnMouseOver()
    {
        mouseOverFunc();
    }

    public void mouseOverFunc()
    {
        gameObject.GetComponent<MeshRenderer>().material = altMaterial;
        if (Input.GetMouseButtonDown(1))
        {
            rightMouseDownFunc();
        }
    }

    public void rightMouseDownFunc()
    {
        if (isInvSlot)
        {
            int theIndex = gc.mk.invSlots.IndexOf(this);
            gc.ih.removeFromInv(theIndex - gc.mk.removedBeforeMe(theIndex));
            gc.mk.emptyMarketSlot(this);
        }
    }

    void OnMouseExit()
    {
        mouseExitFunc();
    }

    public void mouseExitFunc()
    {
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    void OnMouseDown()
    {
        mouseDownFunc();
    }

    public void mouseDownFunc()
    {
        if (!isInvSlot && storedUnit != null && storedUnit.curBuyCost <= gc.ctrl && gc.ih.unitInventory.Count < InventoryHandler.maxInvSlots)
        {
            gc.ctrl -= storedUnit.curBuyCost;
            //gc.ih.addToInv(storedUnit);
            gc.mk.addToInv(storedUnit);
            
            gc.ih.addToInv(gc.mk.invSlots[gc.mk.curInvCount-1].storedUnit);
            gc.mk.emptyMarketSlot(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
