using RHTableTool.Cryptor;
using System.Text;
using System.Xml;

namespace RHTableTool
{
    public static class XMLV2Cryptor
    {
        public delegate void WarningLoggedEventHandler(string fileName, string message);
        public static event WarningLoggedEventHandler? WarningLogged;

        public static byte[] RhToXMLV2(byte[] encryptedData, bool repString2)
        {
            try
            {
                // Decrypt RH
                byte[] decryptedData = RHCryptor.Decrypt(encryptedData);

                using MemoryStream stream = new(decryptedData);
                using BinaryReader reader = new(stream);
                StringBuilder xmlBuilder = new();
                xmlBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");

                int numRow = reader.ReadInt32();
                int numCol = reader.ReadInt32();

                if (numCol <= 0)
                {
                    throw new Exception("The rh file data format is incorrect");
                }

                xmlBuilder.Append("<Root>\n");
                xmlBuilder.Append("<Attributes>\n");

                List<string> listTitles = new(numCol);
                List<string> listTypes = new(numCol);
                Dictionary<string, int> attributeCounter = new();

                // Get the title
                for (int i = 0; i < numCol; i++)
                {
                    int numStrLen = reader.ReadInt16();
                    string value = Encoding.Unicode.GetString(reader.ReadBytes(numStrLen * 2));
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

                // Write the Attributes
                for (int i = 0; i < numCol; i++)
                {
                    string title = listTitles[i];
                    string type = listTypes[i];

                    if (attributeCounter.ContainsKey(title))
                    {
                        attributeCounter[title]++;
                        title = $"{title}-{attributeCounter[title]}";
                    }
                    else
                    {
                        attributeCounter[title] = 1;
                    }

                    xmlBuilder.AppendFormat("<Attribute name=\"{0}\" type=\"{1}\" />\n", DataType.XMLEncodeAttribute(title), type);
                }

                xmlBuilder.Append("</Attributes>\n");
                xmlBuilder.Append("<Data>\n");

                // Get all rows
                for (int i = 0; i < numRow; i++)
                {
                    xmlBuilder.Append("<Row ");

                    // Reset the counter for each row
                    Dictionary<string, int> attributeCounterForRow = new();

                    for (int j = 0; j < numCol; j++)
                    {
                        string title = listTitles[j];
                        string value = DataType.GetValueByType(intTypes[j], reader, true);

                        if (attributeCounterForRow.ContainsKey(title))
                        {
                            attributeCounterForRow[title]++;
                            title = $"{title}-{attributeCounterForRow[title]}";
                        }
                        else
                        {
                            attributeCounterForRow[title] = 1;
                        }

                        // Use XMLEncodeCommon for the values in rows
                        xmlBuilder.AppendFormat("{0}=\"{1}\" ", DataType.XMLEncodeAttribute(title), DataType.XMLEncode(value));
                    }
                    xmlBuilder.Append("/>\n");
                }

                xmlBuilder.Append("</Data>\n");
                xmlBuilder.Append("</Root>");
                return Encoding.UTF8.GetBytes(xmlBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static byte[] XMLV2ToRh(byte[] xmlData, string fileName)
        {
            try
            {
                using MemoryStream streamXML = new(xmlData);
                XmlDocument xml = new();
                xml.Load(streamXML);

                XmlNodeList? attributeNodes = xml.SelectNodes("/Root/Attributes/Attribute");
                XmlNodeList? rows = xml.SelectNodes("/Root/Data/Row");

                if (attributeNodes == null || rows == null)
                    throw new Exception("XML Data format is incorrect");

                // Check for valid types in Attribute 'type'
                foreach (XmlNode attributeNode in attributeNodes)
                {
                    XmlAttribute? typeAttribute = attributeNode.Attributes?["type"];
                    if (typeAttribute != null)
                    {
                        if (string.IsNullOrEmpty(typeAttribute.Value) || !DataType.ValidateAttributes(typeAttribute.Value))
                        {
                            throw new Exception($"Invalid type '{typeAttribute?.Value}' in Attribute node: {attributeNode.OuterXml}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Attribute 'type' is missing in Attribute node: {attributeNode.OuterXml}");
                    }
                }

                int attributesCount = attributeNodes.Count;
                int totalCount = 6 + attributesCount;

                List<string> attributeNames = new();
                foreach (XmlNode attributeNode in attributeNodes)
                {
                    XmlAttribute? nameAttribute = attributeNode.Attributes?["name"];
                    if (nameAttribute != null)
                    {
                        attributeNames.Add(nameAttribute.Value);
                    }
                }

                using MemoryStream stream = new();
                using BinaryWriter writer = new(stream);
                if (attributeNodes.Count > 0)
                {
                    int numRow = rows.Count;
                    int numCol = attributeNodes.Count;

                    writer.Write(numRow);
                    writer.Write(numCol);

                    // Attributes
                    List<string> listTitles = new(numCol);
                    List<string> listTypes = new(numCol);
                    for (int i = 0; i < numCol; i++)
                    {
                        XmlNode? node = attributeNodes.Item(i);
                        if (node != null)
                        {
                            try
                            {
                                XmlAttribute? nameAttribute = node.Attributes?["name"];
                                XmlAttribute? typeAttribute = node.Attributes?["type"];

                                if (nameAttribute != null && typeAttribute != null)
                                {
                                    string name = nameAttribute.Value;
                                    string type = typeAttribute.Value;

                                    if (name.Contains('-'))
                                    {
                                        string[] parts = name.Split('-');
                                        string originalName = parts[0];

                                        // Check if the number is present after the hyphen
                                        if (parts.Length > 1 && int.TryParse(parts[1], out _))
                                        {
                                            name = originalName; // Use the original attribute name
                                        }
                                        else
                                        {
                                            throw new Exception($"Invalid attribute name '{name}' in Attributes");
                                        }
                                    }

                                    if (string.IsNullOrEmpty(name))
                                        throw new Exception($"Attribute '{name}' is missing or empty");
                                    if (string.IsNullOrEmpty(type))
                                        throw new Exception($"Attribute '{type}' is missing or empty");

                                    listTitles.Add(name);
                                    listTypes.Add(type);
                                    byte[] strByte = Encoding.Unicode.GetBytes(name);
                                    short numStrLen = (short)(strByte.Length / 2);
                                    writer.Write(numStrLen);
                                    writer.Write(strByte);
                                }
                                else
                                {
                                    throw new Exception($"Attribute at index '{i}': Missing 'name' or 'type' attribute");
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        else
                        {
                            throw new Exception($"Attribute at index '{i}' is missing");
                        }
                    }

                    // Types
                    int[] intTypes = new int[numCol];
                    for (int i = 0; i < numCol; i++)
                    {
                        string value = listTypes[i];
                        int t = DataType.GetColumnTypeByValue(value);
                        intTypes[i] = t;
                        writer.Write(t);
                    }

                    // Rows
                    // Check for nID column duplicates
                    HashSet<string> uniqueIDs = new();

                    int rowIndex = 0;
                    foreach (XmlNode row in rows)
                    {
                        XmlAttributeCollection? cols = row.Attributes;
                        rowIndex++;
                        if (cols != null)
                        {
                            foreach (XmlAttribute attribute in cols)
                            {
                                // Revert the attribute name if it contains a hyphen and a number
                                if (attribute.Name.Contains('-'))
                                {
                                    string[] parts = attribute.Name.Split('-');
                                    string originalName = parts[0];

                                    // Check if the number is present after the hyphen
                                    if (parts.Length > 1 && int.TryParse(parts[1], out _))
                                    {
                                        if (!attributeNames.Contains(originalName))
                                        {
                                            throw new Exception($"Attribute '{originalName}' is missing in Attributes in row '{rowIndex}'");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"Invalid attribute name '{attribute.Name}' in row '{rowIndex}' at line '{rowIndex + totalCount}'");
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < numRow; i++)
                    {
                        XmlNode? node = rows.Item(i);
                        if (node != null)
                        {
                            XmlAttributeCollection? cols = node.Attributes;
                            if (cols != null)
                            {
                                // Check for 'nID' or 'nid'
                                XmlNode? idNode = cols.GetNamedItem("nID") ?? cols.GetNamedItem("nid");

                                if (idNode != null)
                                {
                                    string? idAttributeName = idNode.Name;
                                    string? id = idNode.Value;

                                    if (!string.IsNullOrEmpty(id))
                                    {
                                        if (!uniqueIDs.Add(id))
                                        {
                                            string warningMessage = $"Duplicate {idAttributeName} ({id}) found on Row '{i + 1}' at line '{i + totalCount}'";
                                            WarningLogged?.Invoke(fileName, warningMessage);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"{idAttributeName} is null or empty on Row '{rowIndex}' at line '{rowIndex + totalCount}'");
                                    }
                                }

                                for (int j = 0; j < numCol; j++)
                                {
                                    if (listTitles[j] != null)
                                    {
                                        XmlAttribute? attributeValue = cols[listTitles[j]];
                                        if (attributeValue != null)
                                        {
                                            if (cols.GetNamedItem(listTitles[j]) == null)
                                            {
                                                throw new Exception($"Attribute '{listTitles[j]}' is missing on Row '{i + 1}' at line '{i + totalCount}'");
                                            }

                                            string? value = attributeValue.Value;
                                            int type = intTypes[j];
                                            try
                                            {
                                                if (value != null)
                                                {
                                                    DataType.WriteValueByType(writer, DataType.XMLDecode(value), type, true);
                                                }
                                                else
                                                {
                                                    throw new Exception($"Value is missing on Row '{i + 1}' on Node '{listTitles[j]}' at line {i + totalCount}");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new Exception($"Value on Row '{i + 1}' on Node '{listTitles[j]}' at line '{i + totalCount}': {ex.Message} (Expected: {DataType.GetColumnType(type, false)})");
                                            }
                                        }
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
