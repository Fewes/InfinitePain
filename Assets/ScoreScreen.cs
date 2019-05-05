using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    Text m_Text;

    void OnEnable()
    {
        m_Text = GetComponent<Text>();
        int score = 10;
        m_Text.text = "Score: " + score;
    }

}
