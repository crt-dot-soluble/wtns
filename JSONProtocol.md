
# JSONProtocol

This document outlines the standard for a language-agnostic protocol designed for efficient and flexible communication between clients and servers using JSON serialization. The protocol supports the transmission of multiple different objects within a single message.

## Protocol Transmission Format

Transmission Format:

```json
Transmission (JSON Object):
{
    "Size": <Integer>,
    "Command": "<String>",
    "Objects": [<JSON Object1>, <JSON Object2>, ...],
    "Sender": "<String>"
}
```

### Fields

- **Size (Integer):** Total size in bytes of the transmission, including all fields.
- **Command (String):** Specifies the command or action to be performed on the server.
- **Objects (Array of JSON Objects):** Array of objects being sent in the transmission.
- **Sender (String):** The identifier of the sender.

### Reasoning

- **Size (Integer):** Broad range of transmission sizes without unnecessary overhead.
- **Command (String):** Clear instruction to the server on how to handle the incoming data.
- **Objects (Array of JSON Objects):** Flexibility to include multiple different objects.
- **Sender (String):** Identification of the sender for logging or authentication purposes.

### Flexibility and Scalability

- Allows transmission of multiple different objects with variable sizes in a single message.
- Command field enhances flexibility, enabling different actions based on the specified command.
- Scalable design accommodates new functionalities involving efficient transmission of multiple different objects.

## C# Implementation

```cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

public class JSONProtocol
{
    public static byte[] CreateTransmission(string command, object[] objects, string sender)
    {
        var transmission = new Dictionary<string, object>
        {
            { "Command", command },
            { "Objects", objects },
            { "Sender", sender }
        };

        string json = JsonConvert.SerializeObject(transmission);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        int size = jsonBytes.Length;
        byte[] sizeBytes = BitConverter.GetBytes(size);

        return CombineByteArrays(sizeBytes, jsonBytes);
    }

    public static Tuple<string, object[], string> ReadTransmission(byte[] data)
    {
        byte[] sizeBytes = new byte[4];
        Array.Copy(data, 0, sizeBytes, 0, 4);
        int size = BitConverter.ToInt32(sizeBytes, 0);

        string json = Encoding.UTF8.GetString(data, 4, size);
        var transmission = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        string command = (string)transmission["Command"];
        object[] objects = ((List<object>)transmission["Objects"]).ToArray();
        string sender = (string)transmission["Sender"];

        return new Tuple<string, object[], string>(command, objects, sender);
    }

    private static byte[] CombineByteArrays(byte[] arr1, byte[] arr2)
    {
        byte[] combined = new byte[arr1.Length + arr2.Length];
        Buffer.BlockCopy(arr1, 0, combined, 0, arr1.Length);
        Buffer.BlockCopy(arr2, 0, combined, arr1.Length, arr2.Length);
        return combined;
    }
}
```
