using NDSParse.Objects.Exports.Sounds;

namespace NDSParse.Conversion.Sounds;

public static class SoundExtensions
{
    public static WAV ToWave(this STRM stream)
    {
        var data = stream.Data.Data.CreateAssetReader().ReadAllBytes();
        if (stream.Header.NumChannels == 2)
        {
            var (left, right) = SplitChannels(data, stream.Header.NumBlocks, stream.Header.BlockLength, stream.Header.LastBlockLength, stream.Header.Type);
            if (stream.Header.Looping)
            {
                data = stream.Header.Type switch
                {
                    STRM.WaveType.PCM16 => MergeChannels(left, right, (int) stream.Header.LoopOffset * 2),
                    _ => throw new NotSupportedException()
                };
            }
            else
            {
                data = MergeChannels(left, right);
            }
        }
        
        return new WAV(data, stream.Header.NumChannels, stream.Header.SampleRate, 16);
    }
    
    private static (byte[], byte[]) SplitChannels(byte[] data, uint numBlocks, uint blockSize, uint lastBlockSize, STRM.WaveType waveType)
    {
        var listData = data.ToList();

        byte[] GetChannel(bool left)
        {
            var result = new List<byte>();
            void CopyBlock(int index, int blockLength)
            {
                var offset = left ? 0 : blockLength;
                var blockData = new byte[blockLength];
                listData.CopyTo(index * blockLength * 2 + offset, blockData, 0, blockLength);

                switch (waveType)
                {
                    case STRM.WaveType.PCM16:
                        result.AddRange(blockData);
                        break;
                }
            }

            for (var i = 0; i < numBlocks; i++)
            {
                if (i < numBlocks - 1) CopyBlock(i, (int) blockSize);
                else CopyBlock(i, (int) lastBlockSize);
            }
        
            return result.ToArray();
        }
        
        return (GetChannel(true), GetChannel(false));
    }
    
    private static byte[] MergeChannels(byte[] leftChannel, byte[] rightChannel, int loopSample = 0)
    {
        var result = new List<byte>();

        for (var i = loopSample; i < leftChannel.Length; i += 2)
        {
            result.Add(leftChannel[i]);
            if (i + 1 < leftChannel.Length)
                result.Add(leftChannel[i + 1]);

            result.Add(rightChannel[i]);
            if (i + 1 < leftChannel.Length)
                result.Add(rightChannel[i + 1]);
        }

        return result.ToArray();
    }
}