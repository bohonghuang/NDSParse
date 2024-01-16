using NDSParse.Data;
using NDSParse.Objects.Exports.Sounds.SoundData;

namespace NDSParse.Objects.Exports.Sounds;

public class STRM : SoundTypeBase<STRMInfo> 
{
    public HEAD Header;
    public DATA Data;
    
    public override string Magic => "STRM";
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        Header = Construct<HEAD>(reader);
        Data = Construct<DATA>(reader);

    }
    
    public class HEAD : NDSExport
    {
        public WaveType Type;
        public bool Looping;
        public ushort NumChannels;
        public ushort SampleRate;
        public ushort Time;
        public uint LoopOffset; // In Samples
        public uint NumSamples; 
        public uint DataOffset;
        public uint NumBlocks;
        
        // Per-Channel
        public uint BlockLength;
        public uint SamplesPerBlock;
        public uint LastBlockLength;
        public uint SamplesPerLastBlock;
        
        public override string Magic => "HEAD";

        public override void Deserialize(BaseReader reader)
        {
            base.Deserialize(reader);
            
            Type = reader.ReadEnum<WaveType, byte>();
            Looping = reader.ReadByte() == 1;
            NumChannels = reader.Read<ushort>();
            SampleRate = reader.Read<ushort>();
            Time = reader.Read<ushort>();
            LoopOffset = reader.Read<uint>();
            NumSamples = reader.Read<uint>();
            DataOffset = reader.Read<uint>();
            NumBlocks = reader.Read<uint>();
            BlockLength = reader.Read<uint>();
            SamplesPerBlock = reader.Read<uint>();
            LastBlockLength = reader.Read<uint>();
            SamplesPerLastBlock = reader.Read<uint>();

            reader.Position += 32; // reserved
        }
    }

    public class DATA : NDSExport
    {
        public DataBlock Data;
    
        public override string Magic => "DATA";

        public override void Deserialize(BaseReader reader)
        {
            base.Deserialize(reader);

            Data = CreateDataBlock(reader);
        }
    }
    
    public enum WaveType : byte
    {
        PCM8 = 0,
        PCM16 = 1,
        ADPCM = 2
    }
}

public class STRMInfo : SoundInfoTypeBase
{
    public byte Volume;
    public byte Priority;
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);

        reader.Position += sizeof(ushort); // unknown
        
        Volume = reader.ReadByte();
        Priority = reader.ReadByte();
    }
}