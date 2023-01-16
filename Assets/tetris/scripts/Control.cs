using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Threading;
using UnityEngine.SceneManagement;
public class Control : MonoBehaviour
{
    public ParticleSystem deleteEffect;
    private float previousTime = 0f, leftTime = 0f, rightTime = 0f;
    static float spaceTime = 0f, clockwiseTime = 0f, counterclockwiseTime = 0f;
    public float fallTime = 0.8f;
    public float left_right_speed = 0.08f;
    public static int height = 20, width = 10;
    public Transform[,] grid = new Transform[width + 5, height + 5];
    public int score = 0;
    //Arduino
    private SerialPort arduinoStream;
    public string port;
    private Thread readThread;
    public string readMessage;
    bool isNewMessage;
    GameObject current_block;
    spawner Spawner;
    [SerializeField] Slider volumeSlider;
    public Animator clockwise, counterclockwise, space, control;
    void Start()
    {
        Spawner = FindObjectOfType<spawner>();
        current_block = Spawner.NewBlock();

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
        }
        catch
        {
            Debug.Log("Connect failed");
        }
        ArduinoWrite("M");
    }

    // Update is called once per frame
    void Update()
    {
        ArduinoWrite("M");
        if (Input.GetKey(KeyCode.Escape))
        {
            OnApplicationQuit();
            SceneManager.LoadScene(0);
        }
        if ((Input.GetKey(KeyCode.LeftArrow)) && Time.time - leftTime > left_right_speed)
        {
            current_block.transform.position += new Vector3(-1, 0, 0);
            // Debug.Log(current_block.transform.position.x);
            if (!current_block.GetComponent<T>().validMove()) current_block.transform.position -= new Vector3(-1, 0, 0);
            leftTime = Time.time;
        }
        if ((Input.GetKey(KeyCode.RightArrow)) && Time.time - rightTime > left_right_speed)
        {
            current_block.transform.position += new Vector3(1, 0, 0);
            if (!current_block.GetComponent<T>().validMove()) current_block.transform.position -= new Vector3(1, 0, 0);
            rightTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), -90);
            if (!current_block.GetComponent<T>().validMove())
                current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), 90);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), 90);
            if (!current_block.GetComponent<T>().validMove())
                current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), -90);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            current_block.GetComponent<T>().moveToBottom();
            current_block.GetComponent<T>().shadowDestroy();
            current_block.GetComponent<T>().enabled = false;
            AddToGrid();
            if (LoseGame())
            {
                OnApplicationQuit();
                SceneManager.LoadScene(1);
            }
            else current_block = Spawner.NewBlock();
            checkLines();
            return;
        }

        if (Time.time - previousTime > (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
        {
            current_block.transform.position += new Vector3(0, -1, 0);
            if (!current_block.GetComponent<T>().validMove())
            {
                current_block.GetComponent<T>().shadowDestroy();
                current_block.transform.position -= new Vector3(0, -1, 0);
                current_block.GetComponent<T>().enabled = false;
                AddToGrid();
                if (LoseGame()) SceneManager.LoadScene(1);
                else current_block = Spawner.NewBlock();
                checkLines();
            }
            previousTime = Time.time;
            return;

        }
        // if (isNewMessage)
        // {
        //     // Debug.Log(readMessage);
        //     if (readMessage == "left" && Time.time - leftTime > left_right_speed)
        //     {
        //         current_block.transform.position += new Vector3(-1, 0, 0);
        //         if (!current_block.GetComponent<T>().validMove()) current_block.transform.position -= new Vector3(-1, 0, 0);
        //         leftTime = Time.time;
        //     }
        //     if (readMessage == "right" && Time.time - rightTime > left_right_speed)
        //     {
        //         current_block.transform.position += new Vector3(1, 0, 0);
        //         if (!current_block.GetComponent<T>().validMove()) current_block.transform.position -= new Vector3(1, 0, 0);
        //         rightTime = Time.time;
        //     }
        //     if (readMessage == "up" && Time.time - clockwiseTime > left_right_speed * 3)
        //     {
        //         clockwise.SetBool("clockwise", true);
        //         current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), -90);
        //         if (!current_block.GetComponent<T>().validMove())
        //             current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), 90);
        //         clockwiseTime = Time.time;
        //     }
        //     else if (Time.time - clockwiseTime < left_right_speed * 3)
        //     {
        //         clockwise.SetBool("clockwise", true);
        //     }
        //     else
        //     {
        //         clockwise.SetBool("clockwise", false);
        //     }
        //     if (readMessage == "counterclockwise" && Time.time - counterclockwiseTime > left_right_speed * 3)
        //     {
        //         counterclockwise.SetBool("counterclockwise", true);
        //         current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), 90);
        //         if (!current_block.GetComponent<T>().validMove())
        //             current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), -90);
        //         counterclockwiseTime = Time.time;
        //     }
        //     else if (Time.time - counterclockwiseTime < left_right_speed * 3)
        //     {
        //         counterclockwise.SetBool("counterclockwise", true);
        //     }
        //     else
        //     {
        //         counterclockwise.SetBool("counterclockwise", false);
        //     }
        //     if (readMessage == "down")
        //     {
        //         current_block.transform.position += new Vector3(0, -1, 0);
        //         if (!current_block.GetComponent<T>().validMove())
        //         {
        //             current_block.GetComponent<T>().shadowDestroy();
        //             current_block.transform.position -= new Vector3(0, -1, 0);
        //             current_block.GetComponent<T>().enabled = false;
        //             AddToGrid();
        //             if (LoseGame())
        //             {
        //                 OnApplicationQuit();
        //                 SceneManager.LoadScene(1);
        //             }
        //             else current_block = Spawner.NewBlock();
        //             checkLines();
        //         }
        //         previousTime = Time.time;
        //         return;
        //     }

        //     if (readMessage == "space" && Time.time - spaceTime > left_right_speed * 3)
        //     {
        //         space.SetBool("space", true);
        //         current_block.GetComponent<T>().moveToBottom();
        //         current_block.GetComponent<T>().shadowDestroy();
        //         current_block.GetComponent<T>().enabled = false;
        //         AddToGrid();
        //         if (LoseGame())
        //         {
        //             OnApplicationQuit();
        //             SceneManager.LoadScene(1);
        //         }
        //         else current_block = Spawner.NewBlock();
        //         checkLines();
        //         spaceTime = Time.time;
        //         return;
        //     }
        //     else if (Time.time - spaceTime < left_right_speed * 3)
        //     {
        //         space.SetBool("space", true);
        //     }
        //     else
        //     {
        //         space.SetBool("space", false);
        //     }
        //     if (readMessage.Length >= 6 && readMessage.Substring(0, 6) == "volume")
        //     {
        //         volumeSlider.value = float.Parse(readMessage.Substring(6));
        //         // Debug.Log(readMessage.Substring(6));
        //     }

        // }
        // isNewMessage = false;
        if (isNewMessage)
        {
            // Debug.Log(readMessage);
            string[] msg = readMessage.Split(" ");

            //exit
            if (msg[0] == "Exit:" && msg[1] == "1")
            {
                OnApplicationQuit();
                SceneManager.LoadScene(0);
                return;
            }

            //space->hard drop
            if (msg[0] == "Space:" && msg[1] == "1" && Time.time - spaceTime > left_right_speed * 3)
            {
                space.SetBool("space", true);
                current_block.GetComponent<T>().moveToBottom();
                current_block.GetComponent<T>().shadowDestroy();
                current_block.GetComponent<T>().enabled = false;
                AddToGrid();
                if (LoseGame())
                {
                    OnApplicationQuit();
                    SceneManager.LoadScene(1);
                    return;
                }
                else
                    current_block = Spawner.NewBlock();
                checkLines();
                spaceTime = Time.time;
                return;
            }
            else if (Time.time - spaceTime > left_right_speed * 3)
            {
                space.SetBool("space", false);
            }


            //movement
            if (msg[0] == "Movement:")
            {
                int x = int.Parse(msg[1]);
                int y = int.Parse(msg[2]);
                //right
                if (x >= 1000 && (y >= 400 || y <= 600) && Time.time - rightTime > left_right_speed)
                {
                    current_block.transform.position += new Vector3(1, 0, 0);
                    if (!current_block.GetComponent<T>().validMove()) current_block.transform.position -= new Vector3(1, 0, 0);
                    rightTime = Time.time;
                    control.SetBool("right", true);
                }
                //left
                else if (x <= 100 && (y >= 400 || y <= 600) && Time.time - leftTime > left_right_speed)
                {
                    current_block.transform.position += new Vector3(-1, 0, 0);
                    if (!current_block.GetComponent<T>().validMove()) current_block.transform.position -= new Vector3(-1, 0, 0);
                    leftTime = Time.time;
                    control.SetBool("left", true);
                }
            }
            else
            {
                control.SetBool("right", false);
                control.SetBool("left", false);
            }



            // clockwise
            if (msg[0] == "Clockwise:" && Time.time - clockwiseTime > left_right_speed * 3)
            {
                clockwise.SetBool("clockwise", true);
                current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), -90);
                if (!current_block.GetComponent<T>().validMove())
                    current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), 90);
                clockwiseTime = Time.time;
            }
            else if (Time.time - clockwiseTime > left_right_speed * 3)
            {
                clockwise.SetBool("clockwise", false);
            }


            //counterclockwise
            if (msg[0] == "Counterclockwise:" && Time.time - counterclockwiseTime > left_right_speed * 3)
            {
                counterclockwise.SetBool("counterclockwise", true);
                current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), 90);
                if (!current_block.GetComponent<T>().validMove())
                    current_block.transform.RotateAround(current_block.transform.TransformPoint(current_block.GetComponent<T>().rotationPoint), new Vector3(0, 0, 1), -90);
                counterclockwiseTime = Time.time;
            }
            else if (Time.time - counterclockwiseTime > left_right_speed * 3)
            {
                counterclockwise.SetBool("counterclockwise", false);
            }

            //volume
            if (msg[0] == "Volume:")
            {
                volumeSlider.value = float.Parse(msg[1]);
            }
        }
        else
        {
            if (Time.time - spaceTime > left_right_speed * 3) space.SetBool("space", false);
            if (Time.time - clockwiseTime > left_right_speed * 3) clockwise.SetBool("clockwise", false);
            if (Time.time - counterclockwiseTime > left_right_speed * 3) counterclockwise.SetBool("counterclockwise", false);
        }
        isNewMessage = false;
    }
    // Arduino
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
        Debug.Log(message);
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
            }
        }
    }
    public bool LoseGame()
    {
        foreach (Transform children in current_block.transform)
        {
            if (children.position.y >= height) return true;
        }
        return false;
    }

    public void checkLines()
    {
        int deleteed_lines = 0;
        for (int i = height - 1; i >= 0; i--)
        {
            if (hasLine(i))
            {
                GameObject empty = new GameObject();
                empty.transform.position = new Vector3(width, i, 0);
                Instantiate(deleteEffect, empty.transform, false);
                // Debug.Log(i);
                DeleteLine(i);
                deleteed_lines++;
                RowDown(i);
            }
        }
        if (deleteed_lines == 4) score += 1000;
        if (deleteed_lines == 3) score += 400;
        if (deleteed_lines == 2) score += 200;
        if (deleteed_lines == 1) score += 100;
    }
    public bool hasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null) return false;
        }
        return true;
    }

    public void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);

            grid[j, i] = null;
        }
    }

    public void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }
    public void AddToGrid()
    {
        foreach (Transform children in current_block.transform)
        {
            int x = Mathf.RoundToInt(children.transform.position.x);
            int y = Mathf.RoundToInt(children.transform.position.y);
            grid[x, y] = children;
        }
    }

}
