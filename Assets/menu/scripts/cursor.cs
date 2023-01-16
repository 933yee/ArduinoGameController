using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using UnityEngine.SceneManagement;
public class cursor : MonoBehaviour
{
    private SerialPort arduinoStream;
    public string port;
    private Thread readThread;
    public string readMessage;
    bool isNewMessage;
    public GameObject Cursor;
    public Animator rotate;

    public float leftMost, rightMost, upMost, downMost;
    public float keyboard_cursor_speed = 0.01f, arduino_cursor_speed = 0.5f;

    public enum GameStatus
    {
        MENU,
        CUBE_RUN_MENU,
        THREEDHUMAN,
        RACE,
    }
    public static GameStatus gameStatus;
    // Start is called before the first frame update
    void Start()
    {
        if (port != "")
        {
            arduinoStream = new SerialPort(port, 115200);
            arduinoStream.ReadTimeout = 10;
        }
        try
        {
            arduinoStream.Open();
            readThread = new Thread(new ThreadStart(ArduinoRead));
            readThread.Start();
            Debug.Log("Connect successfully");
            ArduinoWrite("M");
        }
        catch
        {
            Debug.Log("Connect failed");
        }
    }

    // Update is called once per frame
    void Update()
    {
        ArduinoWrite("M");
        // Keyboard controller
        if (Input.GetKey(KeyCode.Space))
        {
            if (gameStatus == GameStatus.MENU)
            {
                if (FindObjectOfType<tetris>().state)
                {
                    OnApplicationQuit();
                    LoadGameScene(1);
                }
                else if (FindObjectOfType<cuberun>().state)
                {
                    OnApplicationQuit();
                    gameStatus = GameStatus.CUBE_RUN_MENU;
                    LoadGameScene(2);
                }
                else if (FindObjectOfType<race>().state)
                {
                    ArduinoWrite("R");
                    OnApplicationQuit();
                    LoadGameScene(8);
                    return;
                }
                else if (FindObjectOfType<threeDhuman>().state)
                {
                    ArduinoWrite("H");
                    OnApplicationQuit();
                    LoadGameScene(9);
                    return;
                }
            }
            else if (gameStatus == GameStatus.CUBE_RUN_MENU)
            {
                if (FindObjectOfType<level>().state)
                {
                    OnApplicationQuit();
                    gameStatus = GameStatus.MENU;
                    LoadGameScene(3);
                }
                else if (FindObjectOfType<endless>().state)
                {
                    OnApplicationQuit();
                    gameStatus = GameStatus.MENU;
                    LoadGameScene(7);
                }
            }
        }
        if (Input.GetKey(KeyCode.Escape) && gameStatus == GameStatus.CUBE_RUN_MENU)
        {
            gameStatus = GameStatus.MENU;
            LoadGameScene(0);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            Vector2 pos = Cursor.transform.position;
            if (pos.x >= leftMost)
                Cursor.transform.position = pos + keyboard_cursor_speed * Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Vector2 pos = Cursor.transform.position;
            if (pos.x <= rightMost)
                Cursor.transform.position = pos + keyboard_cursor_speed * Vector2.right;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            Vector2 pos = Cursor.transform.position;
            if (pos.y <= upMost)
                Cursor.transform.position = pos + keyboard_cursor_speed * Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Vector2 pos = Cursor.transform.position;
            if (pos.y >= downMost)
                Cursor.transform.position = pos + keyboard_cursor_speed * Vector2.down;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            rotate.SetBool("rotate", true);
        }
        else
        {
            rotate.SetBool("rotate", false);
        }


        // Arduino controller
        if (isNewMessage)
        {
            // Debug.Log(readMessage);
            string[] msg = readMessage.Split(" ");
            Debug.Log(msg[0]);
            if (msg[0] == "Button:" && msg[1] == "1")
            {
                Debug.Log("hit");
                if (gameStatus == GameStatus.MENU)
                {
                    if (FindObjectOfType<tetris>().state)
                    {
                        OnApplicationQuit();
                        LoadGameScene(1);
                    }
                    else if (FindObjectOfType<cuberun>().state)
                    {
                        OnApplicationQuit();
                        gameStatus = GameStatus.CUBE_RUN_MENU;
                        LoadGameScene(2);
                    }
                    else if (FindObjectOfType<race>().state)
                    {
                        ArduinoWrite("R");
                        OnApplicationQuit();
                        LoadGameScene(8);
                        return;
                    }
                    else if (FindObjectOfType<threeDhuman>().state)
                    {
                        ArduinoWrite("H");
                        OnApplicationQuit();
                        LoadGameScene(9);
                        return;
                    }
                }
                else if (gameStatus == GameStatus.CUBE_RUN_MENU)
                {
                    if (FindObjectOfType<level>().state)
                    {
                        OnApplicationQuit();
                        gameStatus = GameStatus.MENU;
                        LoadGameScene(3);
                    }
                    else if (FindObjectOfType<endless>().state)
                    {
                        OnApplicationQuit();
                        gameStatus = GameStatus.MENU;
                        LoadGameScene(7);
                    }
                }

            }
            if (msg[0] == "Movement:")
            {
                int x = int.Parse(msg[1]);
                int y = int.Parse(msg[2]);

                //right down
                if (x >= 1000 && y >= 1000)
                {
                    Vector2 pos = Cursor.transform.position;
                    if (pos.x <= rightMost && pos.y >= downMost)
                        Cursor.transform.position = pos + arduino_cursor_speed * (Vector2.right + Vector2.down);
                }
                //right up
                else if (x >= 1000 && y <= 100)
                {
                    Vector2 pos = Cursor.transform.position;
                    if (pos.x <= rightMost && pos.y <= upMost)
                        Cursor.transform.position = pos + arduino_cursor_speed * (Vector2.right + Vector2.up);
                }
                //left down
                else if (x <= 100 && y >= 1000)
                {
                    Vector2 pos = Cursor.transform.position;
                    if (pos.x >= leftMost && pos.y >= downMost)
                        Cursor.transform.position = pos + arduino_cursor_speed * (Vector2.left + Vector2.down);
                }
                //left up
                else if (x <= 100 && y <= 100)
                {
                    Vector2 pos = Cursor.transform.position;
                    if (pos.x >= leftMost && pos.y <= upMost)
                        Cursor.transform.position = pos + arduino_cursor_speed * (Vector2.left + Vector2.up);
                }
                //left
                else if (x <= 100 && (y >= 400 || y <= 600))
                {
                    Vector2 pos = Cursor.transform.position;
                    if (pos.x >= leftMost)
                        Cursor.transform.position = pos + arduino_cursor_speed * Vector2.left;
                }
                //right
                else if (x >= 1000 && (y >= 400 || y <= 600))
                {
                    Vector2 pos = Cursor.transform.position;
                    if (pos.x <= rightMost)
                        Cursor.transform.position = pos + arduino_cursor_speed * Vector2.right;
                }
                //down
                else if ((x >= 400 || x <= 600) && y >= 1000)
                {
                    Vector2 pos = Cursor.transform.position;
                    if (pos.y >= downMost)
                        Cursor.transform.position = pos + arduino_cursor_speed * Vector2.down;
                }
                //up
                else if ((x >= 400 || x <= 600) && y <= 100)
                {
                    Vector2 pos = Cursor.transform.position;
                    if (pos.y <= upMost)
                        Cursor.transform.position = pos + arduino_cursor_speed * Vector2.up;
                }
            }
        }
        isNewMessage = false;
    }
    private void ArduinoRead()
    {
        while (arduinoStream.IsOpen)
        {
            try
            {
                readMessage = arduinoStream.ReadLine();
                isNewMessage = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
    public void ArduinoWrite(string message)
    {
        // Debug.Log(message);
        try
        {
            arduinoStream.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    private void OnApplicationQuit()
    {
        if (arduinoStream != null)
        {
            if (arduinoStream.IsOpen)
            {
                arduinoStream.Close();
                // readThread.Abort();
            }
        }
    }

    void LoadGameScene(int GameSceneID)
    {
        SceneManager.LoadScene(GameSceneID);
    }
}
