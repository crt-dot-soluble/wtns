
# WTNS Binary Protocol

This document outlines the standard for a language-agnostic protocol designed for efficient and flexible communication between clients and servers. The protocol supports the transmission of multiple different objects within a single message.

## Protocol Transmission Format

Transmission Format:

- Size (4 bytes)
- Command (4 bytes)
- Object1 Type (4 bytes)
- Size of Object1 (4 bytes)
- Serialized Object1
- Object2 Type (4 bytes)
- Size of Object2 (4 bytes)
- Serialized Object2
- ...
- Sending User's ID (4 bytes)

### Fields

- **Size (Integer - 4 bytes):** Total size in bytes of the transmission, including all fields.
- **Command (Integer - 4 bytes):** Specifies the command or action to be performed on the server.
- **Object1 Type (Integer - 4 bytes):** Specifies the type of the first serialized object being sent.
- **Size of Object1 (Integer - 4 bytes):** Size in bytes of the serialized Object1.
- **Serialized Object1:** Actual serialized data of the first object being sent.
- **Object2 Type (Integer - 4 bytes):** Specifies the type of the second serialized object being sent.
- **Size of Object2 (Integer - 4 bytes):** Size in bytes of the serialized Object2.
- **Serialized Object2:** Actual serialized data of the second object being sent.
- ...
- **Sending User's ID (Integer - 4 bytes):** Positive integer value representing the user ID starting at one.

### Reasoning

- **Size (Integer - 4 bytes):** Broad range of transmission sizes without unnecessary overhead.
- **Command (Integer - 4 bytes):** Clear instruction to the server on how to handle the incoming data.
- **Object Type (Integer - 4 bytes):** Diverse set of object types without excessive space consumption.
- **Size of Object (Integer - 4 bytes):** Substantial size representation without sacrificing efficiency.
- **Sending User's ID (Integer - 4 bytes):** Consistent and efficient representation for a user ID.

### Flexibility and Scalability

- Allows transmission of multiple different objects with variable sizes in a single message.
- Command field enhances flexibility, enabling different actions based on the specified command.
- Scalable design accommodates new functionalities involving efficient transmission of multiple different objects.

## C# Implementation

```cs
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class BinaryProtocol
{
    public static byte[] CreateTransmission(int command, Tuple<int, byte[]>[] objects, int userId)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Write the size later
                writer.Seek(4, SeekOrigin.Begin);

                // Write command
                writer.Write(command);

                foreach (var obj in objects)
                {
                    // Write object type
                    writer.Write(obj.Item1);

                    // Write size of object
                    writer.Write(obj.Item2.Length);

                    // Write serialized object
                    writer.Write(obj.Item2);
                }

                // Write user ID
                writer.Write(userId);
            }

            // Write the actual size now
            BitConverter.GetBytes((int)stream.Length).CopyTo(stream.GetBuffer(), 0);

            return stream.ToArray();
        }
    }

    public static Tuple<int, Tuple<int, byte[]>[], int> ReadTransmission(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // Read size
                int size = reader.ReadInt32();

                // Read command
                int command = reader.ReadInt32();

                // Read objects
                var objects = new List<Tuple<int, byte[]>>();
                while (stream.Position < size - 4 - 4) // Subtract size and user ID
                {
                    int objType = reader.ReadInt32();
                    int objSize = reader.ReadInt32();
                    byte[] objData = reader.ReadBytes(objSize);

                    objects.Add(new Tuple<int, byte[]>(objType, objData));
                }

                // Read user ID
                int userId = reader.ReadInt32();

                return new Tuple<int, Tuple<int, byte[]>[], int>(command, objects.ToArray(), userId);
            }
        }
    }
}
```
