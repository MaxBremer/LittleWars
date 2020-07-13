using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    gStats stats;
    // Start is called before the first frame update
    void Start()
    {
        stats = GameObject.FindGameObjectWithTag("stats").GetComponent<gStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pressed()
    {
        SceneManager.LoadScene(1);
        stats.isInfinite = false;
    }
}
