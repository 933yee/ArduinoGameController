using UnityEngine;

public class ObstacleDel : MonoBehaviour
{
    private Transform player;
    private void Start() {
        player = FindObjectOfType<PlayerMovement>().transform;
    }
    void Update()
    {    
        if(tag == "PairWide"){
            Transform[] children = gameObject.GetComponentsInChildren<Transform>();
            if (children[1].position.z + 10f <= player.position.z){
                Destroy(gameObject);
                transform.parent.GetComponent<ObstacleSpawn>().SpawnObstacle();
            } 
        }else if(tag == "LongWall"){
            if (transform.position.z + 50f <= player.position.z){
                Destroy(gameObject);
                transform.parent.GetComponent<ObstacleSpawn>().SpawnObstacle();
            } 
        }else{
            if (transform.position.z + 10f <= player.position.z){
                Destroy(gameObject);
                transform.parent.GetComponent<ObstacleSpawn>().SpawnObstacle();
            } 
        }
    }
}
