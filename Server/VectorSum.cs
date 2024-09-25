using Garnet.common;
using Garnet.server;

namespace Server;

public class VectorSum : CustomProcedure {
    public override bool Execute(IGarnetApi garnetApi, ArgSlice input, ref MemoryResult<byte> output) {
        Console.WriteLine("Executing VECTOR.SUM");
        int offset = 0;
        double[] sum = new double[10000]; // We just fix the length to 10000.
        double[] marketScenarioPricesBuffer = new double[sum.Length];
        ArgSlice array;
        ArgSlice key;

        while ((key = GetNextArg(input, ref offset)).Length > 0) {
            if (garnetApi.GET(key, out array) == GarnetStatus.OK) {
                MemoryPack.MemoryPackSerializer.Deserialize(array.Span, ref marketScenarioPricesBuffer!);

                for(int i = 0; i < marketScenarioPricesBuffer.Length; i++) {
                    sum[i] += marketScenarioPricesBuffer[i];
                }
            }
        }

        byte[] serializedResult = MemoryPack.MemoryPackSerializer.Serialize(sum);
        WriteByteArray(ref output, serializedResult.AsSpan());

        return true;
    }

    protected static void WriteByteArray(ref MemoryResult<byte> output, Span<byte> bytes) {
        var output1 = (output.MemoryOwner, output.Length);
        WriteBulkString(ref output1, bytes);
        output.MemoryOwner = output1.MemoryOwner;
        output.Length = output1.Length;
    }
}