using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gStats : MonoBehaviour
{
    public bool isInfinite;
    public float curDifficulty;

    public float musicVolume;

    // Start is called before the first frame update
    void Start()
    {
        curDifficulty = 1.0f;
        musicVolume = .8f;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.GetComponent<AudioSource>().volume = musicVolume;
    }
}
