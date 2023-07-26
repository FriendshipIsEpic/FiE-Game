using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ReloadLevelStart : MonoBehaviour
{

    public AudioMixer masterMixer;

    // Start is called before the first frame update
    void Start()
    {


        masterMixer.SetFloat("masterVol", 0);

        Application.LoadLevel(1);
    }

    
}
