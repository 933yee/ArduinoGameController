using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
public class hi : MonoBehaviour
{
    private SerialPort arduinoStream;
    public string port = "COM4";
    private Thread readThread;
    public string readMessage;
    bool isNewMessage;

    private void Start()
    {
        if (port != "")
        {
            arduinoStream = new SerialPort(port, 9600);
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
    void Update()
    {

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
