using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicSlider : MonoBehaviour
{
    gStats gs;

    private void Start()
    {
        gs = GameObject.Find("gStatsHolder").GetComponent<gStats>();
    }

    public void valChanged()
    {
        gs.musicVolume = gameObject.GetComponent<Slider>().value;
    }
}
