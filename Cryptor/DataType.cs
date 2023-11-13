using System.Text;

namespace RHTableTool.Cryptor
{
    public static class DataType
    {
        public static string GetColumnType(int value, bool repstring2)
        {
            return value switch
            {
                0 => "int32",
                1 => "float",
                2 => repstring2 ? "string" : "string2",
                3 => "string",
                4 => "int64",
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unexpected type: {value}"),
            };
        }


        public static int GetColumnTypeByValue(string value)
        {
            return value switch
            {
                "int32" => 0,
                "float" => 1,
                "string2" => 2,
                "string" => 3,
                "int64" => 4,
                _ => 0,
            };
        }

        public static string GetValueByType(int type, BinaryReader reader, bool isXml)
        {
            switch (type)
            {
                case 0:
                    return Convert.ToString(reader.ReadInt32());
                case 1:
                    return Convert.ToString(reader.ReadSingle());
                case 2:
                case 3:
                    {
                        int numStrLen = reader.ReadInt16();
                        if (isXml)
                        {
                            return XMLEncode(Encoding.Unicode.GetString(reader.ReadBytes(numStrLen * 2)));
                        }
                        return Encoding.Unicode.GetString(reader.ReadBytes(numStrLen * 2));
                    }
                case 4:
                    return Convert.ToString(reader.ReadInt64());
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unexpected type: {type}");
            }
        }

        public static void WriteValueByType(BinaryWriter writer, string value, int type, bool isXml)
        {
            switch (type)
            {
                case 0:
                    writer.Write(Convert.ToInt32(value));
                    break;
                case 1:
                    writer.Write(Convert.ToSingle(value));
                    break;
                case 2:
                case 3:
                    {
                        if (isXml)
                        {
                            byte[] strByte = Encoding.Unicode.GetBytes(XMLDecode(value));
                            short numStrLen = (short)(strByte.Length / 2);
                            writer.Write(numStrLen);
                            writer.Write(strByte);
                        }
                        else
                        {
                            byte[] strByte = Encoding.Unicode.GetBytes(value);
                            short numStrLen = (short)(strByte.Length / 2);
                            writer.Write(numStrLen);
                            writer.Write(strByte);
                        }
                    }
                    break;
                case 4:
                    writer.Write(Convert.ToInt64(value));
                    break;
            }
        }

        public static bool ValidateAttributes(string type)
        {
            string[] allowedTypes = { "int32", "float", "string", "string2", "int64" };
            return allowedTypes.Contains(type);
        }

        public static string XMLEncodeAttribute(string str)
        {
            str = str.Replace("(", "");
            str = str.Replace(")", "");

            return str;
        }

        public static string XMLEncode(string str)
        {
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("&", "&amp;");
            str = str.Replace("'", "&apos;");
            str = str.Replace("\"", "&quot;");

            return str;
        }

        public static string XMLDecode(string str)
        {
            str = str.Replace("&lt;", "<");
            str = str.Replace("&gt;", ">");
            str = str.Replace("&amp;", "&");
            str = str.Replace("&apos;", "'");
            str = str.Replace("&quot;", "\"");

            return str;
        }
    }
}
