using UnityEngine;
public class ObstacleSpawn : MonoBehaviour
{
    public GameObject[] ObstaclePrefabs;
    public int StartSpawnNum = 30;
    public int SpawnInterval = 30;
    private float CurrentSpawnZ = 57f;
    void Start()
    {
        for(int i=0; i<StartSpawnNum; i++){
            int r = Random.Range(0, 4);
            GameObject Obstacle = Instantiate(ObstaclePrefabs[r], transform);
            if(Obstacle.tag == "SingleHalfCuboid"){
                Obstacle.transform.position = new Vector3(Random.Range(-3.75f, 3.75f), 1f, CurrentSpawnZ);
            }else if(Obstacle.tag == "SingleQuarterCuboid"){
                Obstacle.transform.position = new Vector3(Random.Range(-5.625f, 5.625f), 1f, CurrentSpawnZ);
            }else if(Obstacle.tag == "SingleHalfWall"){
                Obstacle.transform.position = new Vector3(Random.Range(-3.75f, 3.75f), 1f, CurrentSpawnZ);
            }else if(Obstacle.tag == "PairWide"){
                Transform[] children;
                children = Obstacle.GetComponentsInChildren<Transform>();
                float left_scale_X = Random.Range(3f, 9f); 
                float right_scale_X = 12f - left_scale_X;
                float left_pos_X = -7.5f + left_scale_X/2f;
                float right_pos_X = 7.5f - right_scale_X/2f;
                children[1].transform.position = new Vector3(left_pos_X, 1f, CurrentSpawnZ);
                children[1].transform.localScale = new Vector3(left_scale_X, 1f, 1f);
                children[2].transform.position = new Vector3(right_pos_X, 1f, CurrentSpawnZ);
                children[2].transform.localScale = new Vector3(right_scale_X, 1f, 1f);
            }
            CurrentSpawnZ += SpawnInterval;
        }      
    }

    // Update is called once per frame
    public void SpawnObstacle()
    {
        int r = Random.Range(0, ObstaclePrefabs.Length);
        GameObject Obstacle = Instantiate(ObstaclePrefabs[r], transform);
        if(Obstacle.tag == "SingleHalfCuboid"){
            Obstacle.transform.position = new Vector3(Random.Range(-3.75f, 3.75f), 1f, CurrentSpawnZ);
            CurrentSpawnZ += SpawnInterval;
        }else if(Obstacle.tag == "SingleQuarterCuboid"){
            Obstacle.transform.position = new Vector3(Random.Range(-5.625f, 5.625f), 1f, CurrentSpawnZ);
            CurrentSpawnZ += SpawnInterval;
        }else if(Obstacle.tag == "SingleHalfWall"){
            Obstacle.transform.position = new Vector3(Random.Range(-3.75f, 3.75f), 1f, CurrentSpawnZ);
            CurrentSpawnZ += SpawnInterval;
        }else if(Obstacle.tag == "PairWide"){
            Transform[] children;
            children = Obstacle.GetComponentsInChildren<Transform>();
            float left_scale_X = Random.Range(3f, 9f); 
            float right_scale_X = 12f - left_scale_X;
            float left_pos_X = -7.5f + left_scale_X/2f;
            float right_pos_X = 7.5f - right_scale_X/2f;
            children[1].transform.position = new Vector3(left_pos_X, 1f, CurrentSpawnZ);
            children[1].transform.localScale = new Vector3(left_scale_X, 1f, 1f);
            children[2].transform.position = new Vector3(right_pos_X, 1f, CurrentSpawnZ);
            children[2].transform.localScale = new Vector3(right_scale_X, 1f, 1f);
            CurrentSpawnZ += SpawnInterval;
        }else if(Obstacle.tag == "LongWall"){
            Obstacle.transform.position = new Vector3(0f, 1f, CurrentSpawnZ+SpawnInterval);
            CurrentSpawnZ += 2*SpawnInterval;
        }
    }
}
