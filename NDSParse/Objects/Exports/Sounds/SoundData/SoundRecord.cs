using System.Diagnostics;
using NDSParse.Data;
using Serilog;

namespace NDSParse.Objects.Exports.Sounds.SoundData;

public abstract class SoundRecord<T> : NDSExport
{
    public Dictionary<SoundFileType, List<T>> Records = new();
    
    private Dictionary<SoundFileType, uint> RecordOffsets = new();

    public abstract T RecordHandler(BaseReader reader, SoundFileType type);
    
    public override void Deserialize(BaseReader reader)
    {
        base.Deserialize(reader);
        
        for (var recordIndex = 0; recordIndex < 8; recordIndex++)
        {
            var type = (SoundFileType) recordIndex;
            RecordOffsets[type] = reader.Read<uint>();
        }

        foreach (var (type, offset) in RecordOffsets)
        {
            Records[type] = GetRecords(reader, type, offset);
        }
        
    }
    
    public List<T> GetRecords(BaseReader reader, SoundFileType type, uint offset)
    {
        reader.Position = offset;
        
        var count = reader.Read<uint>();
        var offsets = new List<uint>();
        for (var index = 0; index < count; index++)
        {
            var entryOffset = reader.Read<uint>();
            if (entryOffset != 0)
            {
                offsets.Add(entryOffset);
            }
        }
        
        var records = new List<T>();
        foreach (var entryOffset in offsets)
        {
            reader.Position = entryOffset;
            records.Add(RecordHandler(reader, type));
        }

        return records;
    }
}