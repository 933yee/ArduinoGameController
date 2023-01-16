using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{   
    public Rigidbody rb;
    public float forwardForce = 2000f;
    public float sidewaysForce = 500f;
    private bool flagFirst = false, flagSecond = false, flagThird = false, flagForth = false;
    void FixedUpdate()
    {
        rb.AddForce(0, 0, forwardForce*Time.deltaTime);
        if(Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow)){
            rb.AddForce(sidewaysForce*Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        if(Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow)){
            rb.AddForce(-sidewaysForce*Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }
        if(Input.GetKey(KeyCode.Escape)){
            SceneManager.LoadScene(0);
        }
        if(transform.position.y < -1f){
            FindObjectOfType<GameManager>().NormalEndGame();
        }
        if(transform.position.z >= 1000f && !flagFirst){
            flagFirst = true;
            forwardForce += 800f;
        }
        if(transform.position.z >= 2000f && !flagSecond){
            flagSecond = true;
            forwardForce += 700f;
        }
        if(transform.position.z >= 3000f && !flagThird){
            flagThird = true;
            forwardForce += 600f;
        }
        if(transform.position.z >= 4000f && !flagForth){
            flagThird = true;
            forwardForce += 500f;
        }
    }
}
