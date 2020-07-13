using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bobData : Metadata
{

    public override void onAtk(int targetIndex)
    {
        if(targetIndex > 0 && myEnemies[targetIndex-1].containing != null)
        {
            Unit potUnit = myEnemies[targetIndex - 1].containing.GetComponent<Unit>();
            potUnit.atkDamage(me.curAtk);
        }
        if(targetIndex < myEnemies.Length-1 && myEnemies[targetIndex+1].containing != null)
        {
            Unit potUnit = myEnemies[targetIndex + 1].containing.GetComponent<Unit>();
            potUnit.atkDamage(me.curAtk);
        }
    }
}
