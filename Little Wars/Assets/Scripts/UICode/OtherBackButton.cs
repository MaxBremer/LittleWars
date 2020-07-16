using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherBackButton : MonoBehaviour
{
    public void pressed()
    {
        GameObject.Find("Options").SetActive(false);
    }
}
