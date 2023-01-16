using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Transform player;
    public Text scoreText;
    private void Update() {
        scoreText.text = player.position.z.ToString("0");
    }
}
