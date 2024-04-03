namespace WTNS.Net;

using Newtonsoft.Json;

public static class JSONProtocol
{
    public struct Transmission
    {
        [JsonProperty("State")]
        public int State { get; set; }

        [JsonProperty("Sender")]
        public int Sender { get; set; }

        [JsonProperty("Objects")]
        public Tuple<int, object>[] Objects { get; set; }
    }

    public static byte[] SerializeTransmission(Transmission transmission)
    {
        string json = JsonConvert.SerializeObject(transmission);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    public static Transmission DeserializeTransmission(byte[] data)
    {
        string json = System.Text.Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<Transmission>(json);
    }
}
