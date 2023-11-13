using System.Text;
using System.Xml;
using RHTableTool.Cryptor;

namespace RHTableTool
{
    public static class XMLCryptor
    {
        public delegate void WarningLoggedEventHandler(string fileName, string message);
        public static event WarningLoggedEventHandler? WarningLogged;

        public static byte[] RhToXML(byte[] encryptedData, bool repString2)
        {
            try
            {
                // Decrypt RH
                byte[] decryptedData = RHCryptor.Decrypt(encryptedData);

                using MemoryStream stream = new(decryptedData);
                using BinaryReader reader = new(stream);
                StringBuilder xmlBuilder = new();
                xmlBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"unicode\" ?>");

                int numRow = reader.ReadInt32();
                int numCol = reader.ReadInt32();

                if (numCol <= 0)
                {
                    throw new Exception("The rh file data format is incorrect");
                }

                xmlBuilder.Append("<Root>\n");

                List<string> listTitles = new(numCol);
                List<string> listTypes = new(numCol);

                // Get the title
                for (int i = 0; i < numCol; i++)
                {
                    int numStrLen = reader.ReadInt16();
                    string value = DataType.XMLEncode(Encoding.Unicode.GetString(reader.ReadBytes(numStrLen * 2)));
                    listTitles.Add(value);
                }

                // Get the type
                int[] intTypes = new int[numCol];
                for (int i = 0; i < numCol; i++)
                {
                    int t = reader.ReadInt32();
                    string value = DataType.GetColumnType(t, repString2);
                    intTypes[i] = t;
                    listTypes.Add(value);
                }

                xmlBuilder.Append("<Titles ");
                for (int i = 0; i < numCol; i++)
                {
                    string value = listTitles[i];
                    if (repString2 && intTypes[i] == 2)
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
                    xmlBuilder.AppendFormat("c{0}=\"{1}\" ", i + 1, value);
                }
                xmlBuilder.Append("/>\n");

                xmlBuilder.Append("<Types ");
                for (int i = 0; i < numCol; i++)
                {
                    xmlBuilder.AppendFormat("c{0}=\"{1}\" ", i + 1, listTypes[i]);
                }
                xmlBuilder.Append("/>\n");

                // Get all rows
                for (int i = 0; i < numRow; i++)
                {
                    xmlBuilder.Append("<Row ");
                    for (int j = 0; j < numCol; j++)
                    {
                        string value = DataType.GetValueByType(intTypes[j], reader, true);
                        xmlBuilder.AppendFormat("c{0}=\"{1}\" ", j + 1, value);
                    }
                    xmlBuilder.Append("/>\n");
                }

                xmlBuilder.Append("</Root>");
                return Encoding.Unicode.GetBytes(xmlBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static byte[] XMLToRh(byte[] xmlData, string fileName)
        {
            try
            {
                using MemoryStream streamXML = new(xmlData);
                XmlDocument xml = new();
                xml.Load(streamXML);

                XmlNode? xmlTitles = xml.SelectSingleNode("/Root/Titles");
                XmlNode? xmlTypes = xml.SelectSingleNode("/Root/Types");
                XmlNodeList? rows = xml.SelectNodes("/Root/Row");

                if (xmlTitles == null)
                    throw new Exception("XML Data format is incorrect: Missing 'Titles' node");

                if (xmlTypes == null)
                    throw new Exception("XML Data format is incorrect: Missing 'Types' node");

                if (rows == null)
                    throw new Exception("XML Data format is incorrect: Missing or empty 'Row' nodes");

                XmlAttributeCollection? titles = xmlTitles?.Attributes;
                XmlAttributeCollection? types = xmlTypes?.Attributes;

                using MemoryStream stream = new();
                using BinaryWriter writer = new(stream);

                if (titles == null || types == null || titles.Count == 0 || types.Count == 0 || titles.Count != types.Count)
                {
                    throw new Exception("XML Data format is incorrect: Missing or mismatched 'Titles' and 'Types' nodes");
                }

                // Check for null or empty attribute values in Titles and Types
                foreach (XmlAttribute? attribute in titles)
                {
                    if (string.IsNullOrEmpty(attribute?.Value))
                    {
                        throw new Exception($"XML Data format is incorrect: Null or empty attribute value in Titles node: {attribute?.Name}");
                    }
                }

                // Check for valid types in Types attributes
                foreach (XmlAttribute? attribute in types)
                {
                    if (string.IsNullOrEmpty(attribute?.Value) || !DataType.ValidateAttributes(attribute.Value))
                    {
                        throw new Exception($"Invalid type '{attribute?.Value}' in Types node: {attribute?.Name}");
                    }
                }

                int numRow = rows.Count;
                int numCol = titles.Count;

                writer.Write(numRow);
                writer.Write(numCol);

                // Titles
                for (int i = 0; i < numCol; i++)
                {
                    XmlNode? node = titles.Item(i);
                    if (node != null)
                    {
                        try
                        {
                            string value = DataType.XMLDecode(node.InnerText);
                            byte[] strByte = Encoding.Unicode.GetBytes(value);
                            short numStrLen = (short)(strByte.Length / 2);
                            writer.Write(numStrLen);
                            writer.Write(strByte);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error in Title '{node.Name}': {ex.Message}");
                        }
                    }
                }

                // Types
                int[] intTypes = new int[numCol];
                for (int i = 0; i < numCol; i++)
                {
                    XmlNode? node = types.Item(i);
                    if (node != null)
                    {
                        try
                        {
                            string value = node.InnerText;
                            int t = DataType.GetColumnTypeByValue(value);
                            intTypes[i] = t;
                            writer.Write(t);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error in Type '{node.Name}': {ex.Message}");
                        }
                    }
                }

                // Rows
                // Check for nID column duplicates
                HashSet<string> uniqueIDs = new();

                for (int i = 0; i < numRow; i++)
                {
                    XmlNode? node = rows.Item(i);
                    if (node != null)
                    {
                        XmlAttributeCollection? cols = node.Attributes;
                        if (cols != null)
                        {
                            int colCount = cols.Count;
                            if (colCount != numCol)
                            {
                                throw new Exception($"Mismatched number of attributes on Row '{i + 1}' at line '{i + 5}'. Expected '{numCol}', but found '{colCount}'");
                            }

                            string? nID = cols.GetNamedItem("c1")?.Value;
                            if (!string.IsNullOrEmpty(nID))
                            {
                                if (!uniqueIDs.Add(nID))
                                {
                                    string warningMessage = $"Duplicate nID ({nID}) found on Row '{i + 1}' at line '{i + 5}'";
                                    WarningLogged?.Invoke(fileName, warningMessage);
                                }
                            }
                            else
                            {
                                throw new Exception($"nID is null or empty on Row '{i + 1}' at line '{i + 5}'");
                            }

                            for (int j = 0; j < colCount; j++)
                            {
                                XmlNode? nodeCol = cols.Item(j);
                                if (nodeCol != null)
                                {
                                    int type = intTypes[j];
                                    try
                                    {
                                        DataType.WriteValueByType(writer, nodeCol.InnerText, type, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception($"On Row '{i + 1}' on Node '{nodeCol.Name}' at line '{i + 5}': {ex.Message} (Expected: {DataType.GetColumnType(type, false)})");
                                    }
                                }
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

                return RHCryptor.Encrypt(buffer);
            }
            catch (XmlException xmlEx)
            {
                throw new Exception($"Error in XML processing: {xmlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
