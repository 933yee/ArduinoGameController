using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerMovement movement;
    private void OnCollisionEnter(Collision collisionInfo) {
        string tg = collisionInfo.collider.tag;
        if(tg == "Obstacle" || tg == "SingleHalfCuboid" || tg == "SingleHalfWall" || 
        tg == "SingleQuarterCuboid" || tg == "DoubleLeft" || tg == "DoubleRight"
        || tg == "LongWall"){
            movement.enabled = false;
            FindObjectOfType<GameManager>().NormalEndGame();
        }
    }
}
