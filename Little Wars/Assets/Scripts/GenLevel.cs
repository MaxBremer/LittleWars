using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenLevel
{
    public Unit[] enemiesToFace;
    public int numEnemySlots;
    public int numFriendlySlots;

    public float difficulty;

    public Unit[] availableInMarket;
    public int[] marketUnitChances;
    public int numShopSlots;

    public int ctrlIncrease;

    public GenLevel() { }

}
