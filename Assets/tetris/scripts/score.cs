using UnityEngine;
using UnityEngine.UI;
public class score : MonoBehaviour
{
    public Text scoreText;
    void Update()
    {
        int Score = FindObjectOfType<Control>().score;
        scoreText.text = Score > 9999999 ? "9999999" : Score.ToString();
        // scoreText.text = .ToString();
    }
}
