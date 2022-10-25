using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonCTRL : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI[] text;

    public void SetText(string newText) {
        foreach (TextMeshProUGUI text in this.text) {
            text.text = newText;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
