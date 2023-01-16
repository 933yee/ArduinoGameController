using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    // Start is called before the first frame update
    public void Quit(){
        Debug.Log("QUIT");
        Application.Quit();
    }
    public void Menu(){
        Debug.Log("MENU");
        SceneManager.LoadScene(0);
    }
}

