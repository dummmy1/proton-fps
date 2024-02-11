using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;



public class ScoreManager : MonoBehaviour
{

    public static int score;

    Text text;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<Text>();
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Kills: " + score;
    }
}
