// See https://aka.ms/new-console-template for more information
using StackExchange.Redis;

Console.WriteLine("Garnet Client");

ConfigurationOptions configurationOptions = new ConfigurationOptions {
    AbortOnConnectFail = false, // For High Availability we must let StackExchange.Redis try to reconnect itself.
    AllowAdmin = true,
    EndPoints = { "localhost:6379" },
};

ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync(configurationOptions);
IDatabase db = connection.GetDatabase();

var generateRandomArrays = GenerateRandomArrays(20000);
string[] keys = new string[generateRandomArrays.Length];

for(int i = 0; i < generateRandomArrays.Length; i++) {
    var serializedArray = MemoryPack.MemoryPackSerializer.Serialize(generateRandomArrays[i]);
    var key = $"key{i}";
    keys[i] = key;

    await db.StringSetAsync(key, serializedArray);
}

RedisResult result = await db.ExecuteAsync("VECTOR.SUM", keys);

byte[] resultMemory = (byte[])result!;
double[] resultArray = MemoryPack.MemoryPackSerializer.Deserialize<double[]>(resultMemory)!;

Console.WriteLine($"length: {resultArray.Length}");

static double[][] GenerateRandomArrays(int n) {
    Random random = new();
    double[][] arrays = new double[n][];

    for (int i = 0; i < n; i++) {
        arrays[i] = new double[10000];
        for (int j = 0; j < 10000; j++) {
            arrays[i][j] = random.NextDouble() * 1000;
        }
    }

    return arrays;
}
