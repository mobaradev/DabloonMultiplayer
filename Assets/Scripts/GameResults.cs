using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResults : MonoBehaviour
{
    private TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        this.text = this.GetComponent<TextMeshPro>();
        this.text.SetText("Game result:\n " + PlayerPrefs.GetInt("Player1Score") + " - " + PlayerPrefs.GetInt("Player2Score"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
