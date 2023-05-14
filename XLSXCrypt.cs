using OfficeOpenXml;
using System.Text;

namespace RHTableTool
{
    public static class XLSXCrypt
    {
        public static byte[] RhToXLSX(byte[] toByte, bool repString2)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //First decrypt RH
            toByte = RHDecryptEncrypt.Decrypt(toByte);

            MemoryStream stream = new(toByte);
            BinaryReader reader = new(stream);

            ExcelPackage package = new();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet");

            int numRow = reader.ReadInt32();
            int numCol = reader.ReadInt32();

            if (numCol <= 0)
            {
                reader.Close();
                stream.Close();
                stream.Dispose();
                throw new Exception("The data format is incorrect");
            }

            List<string> listTitles = new(numCol);
            List<string> listTypes = new(numCol);

            //Get the title
            for (int i = 0; i < numCol; i++)
            {
                int numStrLen = reader.ReadInt16();
                String value = Encoding.Unicode.GetString(reader.ReadBytes(numStrLen * 2));
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
                    case 2: value = repString2 ? "string" : "string2"; break;
                    case 3: value = "string"; break;
                    case 4: value = "int64"; break;
                }
                intTypes[i] = t;
                listTypes.Add(value);
            }

            ExcelRange? rowTitle = worksheet.Cells["A1"].LoadFromArrays(new List<string[]> { listTitles.ToArray() }) as ExcelRange;
            rowTitle.Style.Font.Bold = true;

            ExcelRange? rowType = worksheet.Cells["A2"].LoadFromArrays(new List<string[]> { listTypes.ToArray() }) as ExcelRange;


            //Freeze the first two lines
            worksheet.View.FreezePanes(2, 1);

            //Automatically adjust the following widths
            worksheet.Cells.AutoFitColumns();


            //Get all rows
            for (int i = 0; i < numRow; i++)
            {
                List<object> rowValues = new(numCol);
                for (int j = 0; j < numCol; j++)
                {
                    switch (intTypes[j])
                    {
                        case 0:
                            {
                                rowValues.Add(reader.ReadInt32());
                            }
                            break;
                        case 1:
                            {
                                double f = double.Parse(reader.ReadSingle().ToString());
                                rowValues.Add(f);
                            }
                            break;
                        case 2:
                        case 3:
                            {
                                int numStrLen = reader.ReadInt16();
                                string value = Encoding.Unicode.GetString(reader.ReadBytes(numStrLen * 2));
                                rowValues.Add(value);
                            }
                            break;
                        case 4:
                            {
                                rowValues.Add(reader.ReadInt64());
                            }
                            break;
                    }
                }
                ExcelRange? row = worksheet.Cells[i + 3, 1].LoadFromArrays(new List<object[]> { rowValues.ToArray() }) as ExcelRange;

            }

            worksheet.Cells.AutoFitColumns();

            reader.Close();
            stream.Close();
            stream.Dispose();

            MemoryStream streamBook = new();
            package.SaveAs(streamBook);
            streamBook.Position = 0;
            byte[] buffer = new byte[streamBook.Length];
            streamBook.Read(buffer, 0, buffer.Length);
            streamBook.Close();
            streamBook.Dispose();
            return buffer;
        }

        public static byte[] XLSXToRh(byte[] toByte)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            MemoryStream streamXLSX = new(toByte);
            ExcelPackage package = new(streamXLSX);
            streamXLSX.Close();
            streamXLSX.Dispose();

            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

            int numLast = worksheet.Dimension.End.Row;
            int numRow = numLast - 2;
            if (numRow < 0) throw new Exception("XLSX data format is incorrect");
            int numCol = worksheet.Dimension.End.Column;
            if (numCol < 0) throw new Exception("XLSX data format is incorrect");

            MemoryStream stream = new();
            BinaryWriter writer = new(stream);

            writer.Write(numRow);
            writer.Write(numCol);

            //Title
            List<string> listTitles = new();
            for (int i = 1; i <= numCol; i++)
            {
                string? value = worksheet.Cells[1, i].Value.ToString();
                byte[] strByte = Encoding.Unicode.GetBytes(value);
                short numStrLen = (short)(strByte.Length / 2);
                writer.Write(numStrLen);
                writer.Write(strByte);
                listTitles.Add(value);
            }

            //Types
            int[] intTypes = new int[numCol];
            List<string> listTypes = new();
            for (int i = 1; i <= numCol; i++)
            {
                string? value = worksheet.Cells[2, i].Value.ToString();
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
                intTypes[i - 1] = t;
                writer.Write(t);
                listTypes.Add(value);
            }

            //ROWS
            for (int i = 3; i <= numLast; i++)
            {
                _ = new List<object>(numCol);
                for (int j = 1; j <= numCol; j++)
                {
                    ExcelRange cell = worksheet.Cells[i, j];
                    int type = intTypes[j - 1];
                    if (cell.Value == null)
                    {
                        switch (type)
                        {
                            case 0:
                            case 1:
                            case 4:
                                {
                                    writer.Write(0);
                                }
                                break;
                            case 2:
                            case 3:
                                {
                                    writer.Write((short)0);
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case 0:
                                {
                                    int value = Convert.ToInt32(cell.Value);
                                    writer.Write(value);
                                }
                                break;
                            case 1:
                                {
                                    float value = Convert.ToSingle(cell.Value);
                                    writer.Write(value);
                                }
                                break;
                            case 2:
                            case 3:
                                {
                                    string? value = cell.Value.ToString();
                                    byte[] strByte = Encoding.Unicode.GetBytes(value);
                                    short numStrLen = (short)(strByte.Length / 2);
                                    writer.Write(numStrLen);
                                    writer.Write(strByte);
                                }
                                break;
                            case 4:
                                {
                                    long value = Convert.ToInt64(cell.Value);
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
    }
}