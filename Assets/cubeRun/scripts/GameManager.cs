using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    bool gameHasEnded = false;
    public float restartDelay = 0.0f;
    public GameObject compeleteLevelUI;
    public void CompeleteLevel(){
        compeleteLevelUI.SetActive(true);
    }
    public void NormalEndGame(){
        if(!gameHasEnded){
            gameHasEnded = true;
            Debug.Log("Game Over");
            Invoke("Restart", restartDelay);
        }
    }
    void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
