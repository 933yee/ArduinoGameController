using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using System.Threading;
public class moveCube : MonoBehaviour
{
    private SerialPort arduinoStream;
    public string port = "COM4";
    private Thread readThread;
    public Transform man;
    public string readMessage;
    bool isNewMessage;
    public float forwardForce = 10000f;
    public Rigidbody rb;
    public Animator human;
    public GameObject cameraPivot;
    private float bas = 0f;
    private void Start()
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
            ArduinoWrite("C");
        }
        catch
        {
            Debug.Log("Connect failed");
        }
    }
    void Update()
    {

        ArduinoWrite("C");
        if (transform.position.y <= -10)
        {
            ArduinoWrite("H");
            OnApplicationQuit();
            SceneManager.LoadScene(9);
            return;
        }
        if (isNewMessage)
        {
            string[] msg = readMessage.Split(":");
            if (msg[0] == "Exit" && msg[1] == "1")
            {
                OnApplicationQuit();
                SceneManager.LoadScene(0);
                return;
            }
            if (msg[0] == "CameraMovement")
            {
                string[] action = msg[1].Split(" ");
                int x = int.Parse(action[0]);
                int y = int.Parse(action[1]);
                if (x <= 100 && (y >= 400 || y <= 600))
                {
                    man.transform.rotation *= Quaternion.Euler(0, 1f, 0);
                }
                //right
                else if (x >= 900 && (y >= 400 || y <= 600))
                {
                    man.transform.rotation *= Quaternion.Euler(0, -1f, 0);
                }
                transform.rotation = man.rotation;
                transform.position = man.position + new Vector3(0, 5.18f, 0f) + man.forward * 0.7f;
            }
            else if (msg[0] == "Movement")
            {
                // transform.rotation = man.rotation;
                string[] actions = msg[1].Split(" ");
                int x = int.Parse(actions[0]);
                int y = int.Parse(actions[1]);

                //right down
                if (x >= 900 && y >= 900)
                {
                    // human.SetBool("idle_run", true);
                    rb.AddForce((man.right - man.forward) * forwardForce);
                    // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 135, 0);
                }
                //right up
                else if (x >= 900 && y <= 100)
                {
                    // human.SetBool("idle_run", true);
                    rb.AddForce((man.right + man.forward) * forwardForce);
                    // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 45, 0);
                }
                //left down
                else if (x <= 100 && y >= 900)
                {
                    // human.SetBool("idle_run", true);
                    rb.AddForce((-man.right - man.forward) * forwardForce);
                    // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 225, 0);
                }
                //left up
                else if (x <= 100 && y <= 100)
                {
                    // human.SetBool("idle_run", true);
                    rb.AddForce((-man.right + man.forward) * forwardForce);
                    // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 315, 0);
                }
                //left
                else if (x <= 100 && (y >= 400 || y <= 600))
                {
                    // human.SetBool("idle_run", true);
                    // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 270, 0);
                    rb.AddForce(-man.right * forwardForce);
                }
                //right
                else if (x >= 900 && (y >= 400 || y <= 600))
                {
                    // human.SetBool("idle_run", true);
                    // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 90, 0);
                    rb.AddForce(man.right * forwardForce);
                }
                //down
                else if ((x >= 400 || x <= 600) && y >= 900)
                {
                    // human.SetBool("idle_run", true);
                    // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
                    rb.AddForce(-man.forward * forwardForce);
                }
                //up
                else if ((x >= 400 || x <= 600) && y <= 100)
                {
                    // human.SetBool("idle_run", true);
                    // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 0, 0);
                    rb.AddForce(man.forward * forwardForce);
                }
                transform.position = man.position + new Vector3(0, 5.18f, 0f) + man.forward * 0.7f;

                // Debug.Log(bas);
                // transform.rotation = man.rotation;
            }
            else
            {
                // Debug.Log(msg[1]);
                transform.position = man.position + new Vector3(0, 5.18f, 0f) + man.forward * 0.7f;
                // human.SetBool("idle_run", false);
                if (msg[0] == "Angle")
                {
                    string[] angles = msg[1].Split(" ");
                    // transform.position = new Vector2((float.Parse(angles[0]) * 20 / 90), (float.Parse(angles[1]) * 10 / 90));
                    transform.rotation = man.rotation * Quaternion.Euler(float.Parse(angles[0]), -float.Parse(angles[1]), 0);
                }
                // man.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                // bas = transform.rotation.eulerAngles.y / 100f;
            }
            isNewMessage = false;
        }
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
}
