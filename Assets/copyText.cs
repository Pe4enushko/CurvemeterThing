using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class copyText : MonoBehaviour
{
    public GameObject Source;
    TextMeshProUGUI SourceText;
    TextMeshPro Target;
    // Start is called before the first frame update
    void Start()
    {
        SourceText = Source.GetComponent<TextMeshProUGUI>();
        Target = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        Target.text = SourceText.text;
        Target.pageToDisplay = Target.textInfo.pageCount - 1;
    }
}
