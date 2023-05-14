using System.Text;
using System.Xml;

namespace RHTableTool
{
    public static class XMLCrypt
    {
        public static byte[] RhToXML(byte[] toByte, bool repstring2)
        {
            //First decrypt RH
            toByte = RHDecryptEncrypt.Decrypt(toByte);

            StringBuilder builder = new();
            MemoryStream stream = new(toByte);
            BinaryReader reader = new(stream);
            builder.Append("<?xml version=\"1.0\" encoding=\"unicode\" ?>\n");

            int numRow = reader.ReadInt32();
            int numCol = reader.ReadInt32();

            builder.Append("<Root>\n");

            List<string> listTitles = new(numCol);
            List<string> listTypes = new(numCol);

            //Get the title
            for (int i = 0; i < numCol; i++)
            {
                int numStrLen = reader.ReadInt16();
                string value = XMLEncode(Encoding.Unicode.GetString(reader.ReadBytes(numStrLen * 2)));
                listTitles.Add(value);
            }

            //Get the type
            int[] intTypes = new int[numCol];
            for (int i = 0; i < numCol; i++)
            {
                int t = reader.ReadInt32();
                string value = "";
                switch (t)
                {
                    case 0: value = "int32"; break;
                    case 1: value = "float"; break;
                    case 2: value = repstring2 ? "string" : "string2"; break;
                    case 3: value = "string"; break;
                    case 4: value = "int64"; break;
                }
                intTypes[i] = t;
                listTypes.Add(value);
            }

            builder.Append("<Titles ");
            for (int i = 0; i < numCol; i++)
            {
                string value = listTitles[i];
                if (repstring2 && intTypes[i] == 2)
                {
                    char[] sp = { '_' };
                    string[] values = value.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    for (int x = 0; x < values.Length; x++)
                    {
                        string v = values[x];
                        if (v.StartsWith("sz"))
                            values[x] = "w" + v;
                    }
                    value = string.Join("_", values);
                }

                builder.AppendFormat("c{0}=\"{1}\" ", i + 1, value);
            }
            builder.Append("/>\n");

            builder.Append("<Types ");
            for (int i = 0; i < numCol; i++)
            {
                builder.AppendFormat("c{0}=\"{1}\" ", i + 1, listTypes[i]);
            }
            builder.Append("/>\n");


            //Get all rows
            for (int i = 0; i < numRow; i++)
            {
                builder.Append("<Row ");
                for (int j = 0; j < numCol; j++)
                {
                    string value = string.Empty;
                    switch (intTypes[j])
                    {
                        case 0:
                            value = Convert.ToString(reader.ReadInt32());
                            break;
                        case 1:
                            value = Convert.ToString(reader.ReadSingle());
                            break;
                        case 2:
                        case 3:
                            {
                                int numStrLen = reader.ReadInt16();
                                value = XMLEncode(Encoding.Unicode.GetString(reader.ReadBytes(numStrLen * 2)));
                            }
                            break;
                        case 4:
                            value = Convert.ToString(reader.ReadInt64());
                            break;
                    }
                    builder.AppendFormat("c{0}=\"{1}\" ", j + 1, value);
                }
                builder.Append("/>\n");
            }

            builder.Append("</Root>");

            reader.Close();
            stream.Close();
            stream.Dispose();

            return Encoding.Unicode.GetBytes(builder.ToString());
        }

        public static byte[] XMLToRh(byte[] toByte)
        {
            MemoryStream streamXML = new(toByte);
            XmlDocument xml = new();
            xml.Load(streamXML);
            streamXML.Dispose();

            XmlNode? xmlTitles = xml.SelectSingleNode("/Root/Titles");
            XmlNode? xmlTypes = xml.SelectSingleNode("/Root/Types");
            XmlNodeList? rows = xml.SelectNodes("/Root/Row");

            if (xmlTitles == null || xmlTypes == null)
                throw new Exception("XML Data format is incorrect!");

            XmlAttributeCollection titles = xmlTitles.Attributes;
            XmlAttributeCollection types = xmlTypes.Attributes;

            MemoryStream stream = new();
            BinaryWriter writer = new(stream);
            if (titles.Count > 0 && types.Count > 0 && titles.Count == types.Count)
            {
                int numRow = rows.Count;
                int numCol = titles.Count;

                writer.Write(numRow);
                writer.Write(numCol);

                //Title
                for (int i = 0; i < numCol; i++)
                {
                    XmlNode node = titles.Item(i);
                    string value = XMLDecode(node.InnerText);
                    byte[] strByte = Encoding.Unicode.GetBytes(value);
                    short numStrLen = (short)(strByte.Length / 2);
                    writer.Write(numStrLen);
                    writer.Write(strByte);
                }

                //Types
                int[] intTypes = new int[numCol];
                for (int i = 0; i < numCol; i++)
                {
                    XmlNode node = types.Item(i);
                    string value = node.InnerText;
                    int t = 0;
                    switch (value)
                    {
                        case "int32": t = 0; break;
                        case "float": t = 1; break;
                        case "string2": t = 2; break;
                        case "string": t = 3; break;
                        case "int64": t = 4; break;
                        default: break;
                    }
                    intTypes[i] = t;
                    writer.Write(t);
                }

                //ROWS
                for (int i = 0; i < numRow; i++)
                {
                    XmlNode node = rows.Item(i);
                    XmlAttributeCollection cols = node.Attributes;
                    int colCount = cols.Count;
                    for (int j = 0; j < colCount; j++)
                    {
                        XmlNode nodeCol = cols.Item(j);
                        int type = intTypes[j];
                        switch (type)
                        {
                            case 0:
                                {
                                    int value = Convert.ToInt32(nodeCol.InnerText);
                                    writer.Write(value);
                                }
                                break;
                            case 1:
                                {
                                    float value = Convert.ToSingle(nodeCol.InnerText);
                                    writer.Write(value);
                                }
                                break;
                            case 2:
                            case 3:
                                {
                                    byte[] strByte = Encoding.Unicode.GetBytes(XMLDecode(nodeCol.InnerText));
                                    short numStrLen = (short)(strByte.Length / 2);
                                    writer.Write(numStrLen);
                                    writer.Write(strByte);
                                }
                                break;
                            case 4:
                                {
                                    long value = Convert.ToInt64(nodeCol.InnerText);
                                    writer.Write(value);
                                }
                                break;
                        }
                    }
                }
            }

            writer.Flush();
            stream.Flush();

            int length = (int)stream.Length;
            byte[] buffer = new byte[length];
            stream.Position = 0;
            stream.Read(buffer, 0, length);

            writer.Close();
            stream.Close();
            stream.Dispose();

            //Encrypt into RH
            return RHDecryptEncrypt.Encrypt(buffer);
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
