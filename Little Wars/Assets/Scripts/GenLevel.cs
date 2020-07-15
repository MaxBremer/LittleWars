using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenLevel
{
    public bUnit[] enemyUnits;
    public int enemySlotNum;
    public int friendlySlotNum;

    public float difficulty;

    public BaseUnit[] availableInMarket;
    public int[] marketUnitChances;
    public int numShopSlots;

    public int startingCtrl;

    public GenLevel() { }

}
