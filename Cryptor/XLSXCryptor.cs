using OfficeOpenXml;
using System.Text;

namespace RHTableTool.Cryptor
{
    public static class XLSXCryptor
    {
        public delegate void WarningLoggedEventHandler(string fileName, string message);
        public static event WarningLoggedEventHandler? WarningLogged;

        public static byte[] RhToXLSX(byte[] encryptedData, bool repString2)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Decrypt RH
                byte[] decryptedData = RHCryptor.Decrypt(encryptedData);

                using MemoryStream stream = new(decryptedData);
                using BinaryReader reader = new(stream);
                using ExcelPackage package = new();
                using ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet");

                int numRow = reader.ReadInt32();
                int numCol = reader.ReadInt32();

                if (numCol <= 0)
                {
                    throw new Exception("The rh file data format is incorrect");
                }

                List<string> listTitles = new(numCol);
                List<string> listTypes = new(numCol);

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

                if (worksheet.Cells["A1"].LoadFromArrays(new List<string[]> { listTitles.ToArray() }) is ExcelRange rowTitle)
                {
                    rowTitle.Style.Font.Bold = true;
                }

                ExcelRange? rowType = worksheet.Cells["A2"].LoadFromArrays(new List<string[]> { listTypes.ToArray() }) as ExcelRange;

                // Freeze the first two rows
                worksheet.View.FreezePanes(2, 1);

                // Get all rows
                for (int i = 0; i < numRow; i++)
                {
                    List<object> rowValues = new(numCol);
                    for (int j = 0; j < numCol; j++)
                    {
                        int type = intTypes[j];

                        string value = DataType.GetValueByType(type, reader, false);
                        rowValues.Add(value);
                    }
                    ExcelRange? row = worksheet.Cells[i + 3, 1].LoadFromArrays(new List<object[]> { rowValues.ToArray() }) as ExcelRange;
                }

                // Automatically adjust the column widths
                worksheet.Cells.AutoFitColumns();

                using MemoryStream streamBook = new();
                package.SaveAs(streamBook);
                streamBook.Position = 0;
                byte[] buffer = new byte[streamBook.Length];
                streamBook.Read(buffer, 0, buffer.Length);
                return buffer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static byte[] XLSXToRh(byte[] xlsxData, string fileName)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using MemoryStream streamXLSX = new(xlsxData);
                using ExcelPackage package = new(streamXLSX);
                using ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int numLast = worksheet.Dimension.End.Row;
                int numRow = numLast - 2;

                if (numRow < 0)
                {
                    throw new Exception("XLSX data format is incorrect: Number of rows is invalid");
                }

                int numCol = worksheet.Dimension.End.Column;

                if (numCol < 0)
                {
                    throw new Exception("XLSX data format is incorrect: Number of columns is invalid");
                }

                using MemoryStream stream = new();
                using BinaryWriter writer = new(stream);
                writer.Write(numRow);
                writer.Write(numCol);

                // Columns
                List<string> listColumns = new();
                for (int i = 1; i <= numCol; i++)
                {
                    object cellValue = worksheet.Cells[1, i].Value;
                    string value = cellValue?.ToString() ?? "";

                    if (string.IsNullOrEmpty(value))
                    {
                        throw new Exception($"Column name in column '{worksheet.Cells[1, i].Address}' is empty");
                    }

                    byte[] strByte = Encoding.Unicode.GetBytes(value);
                    short numStrLen = (short)(strByte.Length / 2);
                    writer.Write(numStrLen);
                    writer.Write(strByte);
                    listColumns.Add(value);
                }

                // Columns Types
                int[] intTypes = new int[numCol];
                List<string> listTypes = new();
                for (int i = 1; i <= numCol; i++)
                {
                    object cellValue = worksheet.Cells[2, i].Value;
                    string value = cellValue?.ToString() ?? "";

                    if (string.IsNullOrEmpty(value))
                    {
                        throw new Exception($"Column type on Cell '{worksheet.Cells[2, i].Address}' is empty");
                    }
                    if (!DataType.ValidateAttributes(value))
                    {
                        throw new Exception($"Invalid Column type '{cellValue}' on Cell '{worksheet.Cells[2, i].Address}'");
                    }

                    int t = DataType.GetColumnTypeByValue(value);
                    intTypes[i - 1] = t;
                    writer.Write(t);
                    listTypes.Add(value);
                }

                // Rows
                // Check for nID duplicates
                HashSet<string> uniqueIDs = new();

                for (int i = 3; i <= numLast; i++)
                {
                    string? nID = worksheet.Cells[i, 1].Value?.ToString();

                    if (nID != null)
                    {
                        if (!uniqueIDs.Add(nID))
                        {
                            string warningMessage = $"Duplicate nID ({nID}) found on Row '{i}'";
                            WarningLogged?.Invoke(fileName, warningMessage);
                        }
                    }

                    for (int j = 1; j <= numCol; j++)
                    {
                        ExcelRange cell = worksheet.Cells[i, j];
                        int type = intTypes[j - 1];

                        if (cell.Value == null)
                        {
                            if (type != 2 && type != 3) // Skip check for string and string2 columns
                            {
                                throw new Exception($"Cell value is empty at Row '{i}', Column '{listColumns[j - 1]}'. Only string type cells can be empty.");
                            }
                        }

                        string cellValue = cell.Value?.ToString() ?? "";

                        if (cellValue != null)
                        {
                            DataType.WriteValueByType(writer, cellValue, type, false);
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
            catch (Exception)
            {
                throw;
            }
        }

    }
}
