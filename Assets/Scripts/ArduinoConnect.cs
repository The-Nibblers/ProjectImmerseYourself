using System.Collections;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArduinoController : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM8", 9600);

    void Start()
    {
        try
        {
            sp.Open();
            sp.ReadTimeout = 50;
            Debug.Log("Serial port opened!");
        }
        catch
        {
            Debug.LogError("Could not open serial port!");
        }
    }

    void Update()
    {
        // Listen for messages from Arduino
        if (sp.IsOpen)
        {
            try
            {
                string message = sp.ReadLine();
                if (message.Contains("ALL_ON"))
                {
                    Debug.Log("🔥 ALL LEDs ON! Puzzle solved!");
                }
                else if (message.Contains("NOT_ALL"))
                {
                    Debug.Log("💡 Not all LEDs on anymore...");
                }
            }
            catch (System.TimeoutException) { }
        }

        // Example: Press B to buzz 2.5s
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            Buzz(1.5f);
        }
    }

    /// <summary>
    /// Send buzzer command to Arduino for given seconds
    /// </summary>
    public void Buzz(float seconds)
    {
        if (sp.IsOpen)
        {
            string command = $"BUZZ:{seconds}\n";
            sp.Write(command);
            Debug.Log($"Sent buzzer command: {command}");
        }
    }
    

    void OnApplicationQuit()
    {
        if (sp.IsOpen)
            sp.Close();
    }
}
