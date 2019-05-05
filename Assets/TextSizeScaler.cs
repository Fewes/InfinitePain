using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSizeScaler : MonoBehaviour
{
    public int maxFontSize = 20;
    public int minFontSize = 12;

    Text m_Text;

    void Start()
    {
        m_Text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Mathf.Sin(Time.time * 8.0f) * 0.5f + 1);
        m_Text.fontSize = (int)Mathf.Lerp(minFontSize, maxFontSize, t);
    }
}
