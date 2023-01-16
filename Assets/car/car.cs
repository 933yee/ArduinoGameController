using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class car : MonoBehaviour
{
    private SerialPort arduinoStream;
    public float endpoint;
    float startpoint;
    float length;
    public Slider hint;
    public string port;
    private Thread readThread;
    public string readMessage;
    public Transform simulate;
    bool isNewMessage;
    public Vector3 newGravity;
    public Text death_;
    public Text record_;
    public Text record_now;
    public Text speed_;
    static int death = 0;
    static float record = 0;
    static float speed = 0;
    Vector3 prePos = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        startpoint = transform.position.x;
        length = endpoint - startpoint;
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
            ArduinoWrite("R");
        }
        catch
        {
            Debug.Log("Connect failed");
        }
        death_.text = death.ToString();
        record_.text = record.ToString("0.00") + " %";
    }
    public Rigidbody rb;
    public float forwardForce = 100f;
    public float sidewaysForce = 100f;

    // Update is called once per frame
    void FixedUpdate()
    {
        record_now.text = (hint.value * 100).ToString("0.00") + " %";
        if (hint.value * 100 > record)
        {
            record = hint.value * 100;
            record_.text = record.ToString("0.00") + " %";
        }
        ArduinoWrite("R");
        if (transform.position.y <= -3)
        {
            death++;
            death_.text = death.ToString();
            OnApplicationQuit();
            SceneManager.LoadScene(8);
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
            else if (msg[0] == "Angle")
            {
                Debug.Log(msg[1]);
                rb.AddForce(transform.forward * forwardForce * Time.deltaTime);
                string[] angles = msg[1].Split(" ");
                float x = float.Parse(angles[0]);
                float y = float.Parse(angles[1]);
                float z = float.Parse(angles[2]);
                // if (z >= 270f) z -= 360f;
                // Debug.Log(z);
                transform.rotation = Quaternion.Euler(0, z + (90), 0);
                // if (float.Parse(angles[1]) >= 0f && float.Parse(angles[1]) <= 180f)
                //     rb.AddForce(forwardForce * Time.deltaTime * ((90f - float.Parse(angles[1])) / 360f), 0, 0);
                // Debug.Log((90f - float.Parse(angles[1])) / 360f);
                // if (float.Parse(angles[2]) >= 270f)
                // {
                //     rb.AddForce(0, 0, forwardForce * Time.deltaTime * ((360f - float.Parse(angles[2])) / 360f));
                // }

                // else if (float.Parse(angles[2]) <= 90f)
                // {
                //     rb.AddForce(0, 0, forwardForce * Time.deltaTime * ((float.Parse(angles[2]) - 180f) / 360f));
                // }
                simulate.rotation = Quaternion.Euler(180, 90, z * 0.9f);
            }

        }
        // isNewMessage = false;

        hint.value = (transform.position.x - startpoint) / length;
        // Debug.Log((transform.position.x - startpoint) / length);
        if (hint.value >= 1)
        {
            OnApplicationQuit();
            SceneManager.LoadScene(0);
            return;
        }
        speed_.text = ((Vector3.Distance(prePos, transform.position) * 3600 / 1000 / Time.deltaTime).ToString("0.0")) + " km/hr";
        prePos = transform.position;
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
