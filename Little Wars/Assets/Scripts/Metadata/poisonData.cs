using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poisonData : Metadata
{

    public override void onAtk(int targetIndex)
    {
        if (myEnemies[targetIndex].myUnit != null)
        {
            myEnemies[targetIndex].myUnit.setHealth(0);
        }
    }

    public override void onHitBy(int attackerIndex)
    {
        if (myEnemies[attackerIndex].myUnit != null)
        {
            myEnemies[attackerIndex].myUnit.setHealth(0);
        }
    }
}
