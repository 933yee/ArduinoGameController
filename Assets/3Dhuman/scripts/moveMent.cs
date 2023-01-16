using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using System.Threading;
public class moveMent : MonoBehaviour
{
    //Arduino
    private SerialPort arduinoStream;
    public string port;
    private Thread readThread;
    public string readMessage;
    bool isNewMessage;


    public Rigidbody rb;
    public float forwardForce = 2000f;
    public Animator human;
    public GameObject cameraPivot;
    public Vector3 newGravity;
    private bool stand = false;
    private void Start()
    {
        Physics.gravity = newGravity;
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
            ArduinoWrite("H");
        }
        catch
        {
            Debug.Log("Connect failed");
        }
    }

    // public GameObject Camera;
    private void OnCollisionEnter(Collision other)
    {
        stand = true;
        // Debug.Log("stand");
    }
    private void OnCollisionExit(Collision other)
    {
        stand = false;
        // Debug.Log("jump");
    }
    void Update()
    {
        if (transform.position.y <= -10)
        {
            ArduinoWrite("C");
            OnApplicationQuit();
            SceneManager.LoadScene(10);
            return;
        }
        ArduinoWrite("H");
        // Camera.transform.position = transform.position;
        // Camera.transform.position += new Vector3(0, 15, -10);
        // if (Input.GetKey(KeyCode.Space) && stand)
        // {
        //     human.SetBool("idle_jump", true);
        //     // transform.rotation = Quaternion.Euler(0, 45, 0);
        // }
        // else
        // {
        //     human.SetBool("idle_jump", false);
        // }
        // if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        // {
        //     human.SetBool("idle_run", true);
        //     rb.AddForce(transform.forward * forwardForce);
        //     transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 45, 0);
        // }
        // else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        // {
        //     human.SetBool("idle_run", true);
        //     rb.AddForce(transform.forward * forwardForce);
        //     transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 135, 0);
        // }
        // else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        // {
        //     human.SetBool("idle_run", true);
        //     rb.AddForce(transform.forward * forwardForce);
        //     transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 315, 0);
        // }
        // else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        // {
        //     human.SetBool("idle_run", true);
        //     rb.AddForce(transform.forward * forwardForce);
        //     transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 225, 0);
        // }
        // else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        // {
        //     human.SetBool("idle_run", true);
        //     transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 0, 0);
        //     rb.AddForce(transform.forward * forwardForce);
        // }
        // else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        // {
        //     human.SetBool("idle_run", true);
        //     transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 180, 0);
        //     rb.AddForce(transform.forward * forwardForce);
        // }
        // else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        // {
        //     human.SetBool("idle_run", true);
        //     transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 270, 0);
        //     rb.AddForce(transform.forward * forwardForce);
        // }
        // else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        // {
        //     human.SetBool("idle_run", true);
        //     transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 90, 0);
        //     rb.AddForce(transform.forward * forwardForce);
        // }
        if (isNewMessage)
        {
            // Debug.Log(readMessage);
            string[] msg = readMessage.Split(" ");
            if (msg[0] == "Exit:" && msg[1] == "1")
            {
                OnApplicationQuit();
                SceneManager.LoadScene(0);
                return;
            }
            //Zoom
            if (msg[0] == "ZoomIn:" && msg[1] == "1")
            {
                FindObjectOfType<cameraOrbit>().ZoomState = 1;
            }
            else if (msg[0] == "ZoomOut:" && msg[1] == "1")
            {
                FindObjectOfType<cameraOrbit>().ZoomState = -1;
            }
            else
            {
                FindObjectOfType<cameraOrbit>().ZoomState = 0;
            }

            //Actions
            if (msg[0] == "Space:" && msg[1] == "1")
            {
                human.SetBool("idle_jump", true);
                // transform.rotation = Quaternion.Euler(0, 45, 0);
            }
            else
            {
                human.SetBool("idle_jump", false);
            }

            if (msg[0] == "CameraMovement:")
            {
                int x = int.Parse(msg[1]);
                int y = int.Parse(msg[2]);
                //right down
                if (x >= 900 && y >= 900)
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = 1;
                    FindObjectOfType<cameraOrbit>().CameraStateY = -1;
                }
                //right up
                else if (x >= 900 && y <= 100)
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = 1;
                    FindObjectOfType<cameraOrbit>().CameraStateY = 1;
                }
                //left down
                else if (x <= 100 && y >= 900)
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = -1;
                    FindObjectOfType<cameraOrbit>().CameraStateY = -1;
                }
                //left up
                else if (x <= 100 && y <= 100)
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = -1;
                    FindObjectOfType<cameraOrbit>().CameraStateY = 1;
                }
                //left
                else if (x <= 100 && (y >= 400 || y <= 600))
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = -1;
                    FindObjectOfType<cameraOrbit>().CameraStateY = 0;
                }
                //right
                else if (x >= 900 && (y >= 400 || y <= 600))
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = 1;
                    FindObjectOfType<cameraOrbit>().CameraStateY = 0;
                }
                //down
                else if ((x >= 400 || x <= 600) && y >= 900)
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = 0;
                    FindObjectOfType<cameraOrbit>().CameraStateY = -1;
                }
                //up
                else if ((x >= 400 || x <= 600) && y <= 100)
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = 0;
                    FindObjectOfType<cameraOrbit>().CameraStateY = 1;
                }
                else
                {
                    FindObjectOfType<cameraOrbit>().CameraStateX = 0;
                    FindObjectOfType<cameraOrbit>().CameraStateY = 0;
                }
            }
            else
            {
                FindObjectOfType<cameraOrbit>().CameraStateX = 0;
                FindObjectOfType<cameraOrbit>().CameraStateY = 0;
            }

            if (msg[0] == "Movement:")
            {
                int x = int.Parse(msg[1]);
                int y = int.Parse(msg[2]);

                //right down
                if (x >= 900 && y >= 900)
                {
                    human.SetBool("idle_run", true);
                    rb.AddForce(transform.forward * forwardForce);
                    transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 135, 0);
                }
                //right up
                else if (x >= 900 && y <= 100)
                {
                    human.SetBool("idle_run", true);
                    rb.AddForce(transform.forward * forwardForce);
                    transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 45, 0);
                }
                //left down
                else if (x <= 100 && y >= 900)
                {
                    human.SetBool("idle_run", true);
                    rb.AddForce(transform.forward * forwardForce);
                    transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 225, 0);
                }
                //left up
                else if (x <= 100 && y <= 100)
                {
                    human.SetBool("idle_run", true);
                    rb.AddForce(transform.forward * forwardForce);
                    transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 315, 0);
                }
                //left
                else if (x <= 100 && (y >= 400 || y <= 600))
                {
                    human.SetBool("idle_run", true);
                    transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 270, 0);
                    rb.AddForce(transform.forward * forwardForce);
                }
                //right
                else if (x >= 900 && (y >= 400 || y <= 600))
                {
                    human.SetBool("idle_run", true);
                    transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 90, 0);
                    rb.AddForce(transform.forward * forwardForce);
                }
                //down
                else if ((x >= 400 || x <= 600) && y >= 900)
                {
                    human.SetBool("idle_run", true);
                    transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 180, 0);
                    rb.AddForce(transform.forward * forwardForce);
                }
                //up
                else if ((x >= 400 || x <= 600) && y <= 100)
                {
                    human.SetBool("idle_run", true);
                    transform.rotation = Quaternion.Euler(0, cameraPivot.transform.rotation.eulerAngles.y + 0, 0);
                    rb.AddForce(transform.forward * forwardForce);
                }
            }
            else
            {
                human.SetBool("idle_run", false);
            }
        }

        isNewMessage = false;
    }
    //Arduino
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
