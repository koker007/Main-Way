using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldCTRL : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField inputField;

    [SerializeField]
    string keyPlaceholder;
    [SerializeField]
    TextMeshProUGUI textPlaceholder;

    public string text { 
        get { return inputField.text; }
        set { inputField.text = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        IniKeyPlaceholder();
    }

    void IniKeyPlaceholder()
    {
        textPlaceholder.text = Language.GetTextFromKey(keyPlaceholder, textPlaceholder.text);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
