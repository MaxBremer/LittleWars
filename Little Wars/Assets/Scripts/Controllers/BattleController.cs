using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController
{
    public Slot[] friendlySlots;
    public Slot[] enemySlots;

    public bUnit[] friendlyUnitsOnBoard;
    public bUnit[] enemyUnitsOnBoard;

    /*public UnitMesh[] friendlyUnitMeshes;
    public UnitMesh[] enemyUnitMeshes;*/

    public bool isFight;

    public BattleController() {
        isFight = false;
    }

    public void initLevel(BaseLevel level) { }

    public void initLevel(GenLevel level) { }

    public void placeFriendlyAt(int index, bUnit unit)
    {
        if(index < 0 || index > friendlyUnitsOnBoard.Length)
        {
            Debug.Log("ERROR: Bad place index " + index + " for friendly unit. Crash imminent.");
            return;
        }
        friendlyUnitsOnBoard[index] = unit;
    }
}
