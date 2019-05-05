using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreen : MonoBehaviour
{
    Text m_Text;
    public GameObject restart;

    void Player_OnDeath(object sender)
    {
        restart.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Text = GetComponent<Text>();

        Player.local.killable.OnDeath += Player_OnDeath;
    }

    void Update()
    {
        int score = Player.score;
        m_Text.text = "Score: " + score;
    }

}
