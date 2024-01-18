using System.Globalization;
using NDSParse.Data;
using NDSParse.Objects;

namespace NDSParse.Games.Pokemon.BW2;

public class BW2Texts : Deserializable
{
    public List<string> TextList = [];

    
    public override void Deserialize(BaseReader reader)
    {
        var numSections = reader.Read<ushort>();
        var numNames = reader.Read<ushort>();
        
        reader.Position += sizeof(uint) * 2;

        var sectionOffsets = new List<uint>();
        for (var sectionIndex = 0; sectionIndex < numSections; sectionIndex++)
        {
            sectionOffsets.Add(reader.Read<uint>());
        }

        foreach (var sectionOffset in sectionOffsets)
        {
            reader.Position = sectionOffset;
            
            var sectionSize = reader.Read<uint>();
            var origKey = 0x7C89;
            for (var nameIndex = 0; nameIndex < numNames; nameIndex++)
            {
                var stringOffset = reader.Read<uint>();
                var stringSize = reader.Read<ushort>();
                reader.Position += sizeof(ushort); // unknown

                reader.Peek(() =>
                {
                    reader.Position = stringOffset + sectionOffset; 
                    var key = origKey;
                    var finalText = string.Empty;
                    for (var charIndex = 0; charIndex < stringSize; charIndex++)
                    {
                        var character = Convert.ToUInt16(reader.Read<ushort>() ^ key);
                        switch (character)
                        {
                            case 0xFFFE:
                                finalText += '\n';
                                break;
                            case 0xFFFF:
                            case 0xF100:
                                break;
                            case > 20 and <= 0xFFF0 and not 0xF000:
                                var converted = Convert.ToChar(character);
                                if (char.GetUnicodeCategory(converted) != UnicodeCategory.OtherNotAssigned) finalText += converted;
                                break;
                        }
                        
                        key = ((key << 3) | (key >> 13)) & 0xFFFF;
                    }
                    
                    origKey += 0x2983;
                    if (origKey > 0xFFFF) origKey -= 0x10000;
                    
                    TextList.Add(finalText);
                });
            }
        }
    }
}