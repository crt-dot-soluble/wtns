namespace WTNS;

public static class BinaryProtocol
{
    public static byte[] SerializeTransmission(int state, Tuple<int, byte[]>[] objects, int userId)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Write the size later
                writer.Seek(4, SeekOrigin.Begin);

                // Write state
                writer.Write(state);

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

    public static Tuple<int, Tuple<int, byte[]>[], int> DeserializeTransmission(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // Read size
                int size = reader.ReadInt32();

                // Read state
                int state = reader.ReadInt32();

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

                return new Tuple<int, Tuple<int, byte[]>[], int>(state, objects.ToArray(), userId);
            }
        }
    }
}
