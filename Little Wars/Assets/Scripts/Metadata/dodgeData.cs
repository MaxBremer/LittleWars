using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dodgeData : Metadata
{
    int myOldHealth;

    public override void onStartFight()
    {
        /*base.onStartFight();*/
        myOldHealth = me.curHealth;
    }

    public override void onHitBy(int attackerIndex)
    {
        //base.onHitBy(attackerIndex);
        if(Random.Range(1, 5) <= 3)
        {
            me.curHealth = myOldHealth;
        }
    }
}
