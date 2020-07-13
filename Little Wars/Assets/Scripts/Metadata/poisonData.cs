using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poisonData : Metadata
{

    public override void onAtk(int targetIndex)
    {
        if (myEnemies[targetIndex].containing != null)
        {
            myEnemies[targetIndex].containing.GetComponent<Unit>().setHealth(0);
        }
    }

    public override void onHitBy(int attackerIndex)
    {
        if (myEnemies[attackerIndex].containing != null)
        {
            myEnemies[attackerIndex].containing.GetComponent<Unit>().setHealth(0);
        }
    }
}
