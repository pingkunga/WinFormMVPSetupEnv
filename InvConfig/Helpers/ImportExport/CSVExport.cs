using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using System.IO;
using System.Reflection;

namespace InvConfig.Helper.ImportExport
{
    public class CSVExport<T> where T : class
    {
        public List<T> Objects;
            
        public CSVExport(List<T> objects)
        {
            Objects = objects;
        }

        public string Export()
        {
            return Export(true, null);
        }

        public string Export(bool includeHeaderLine, List<string> p_OmmitColumn)
        {
            StringBuilder sb = new StringBuilder();
            IList<PropertyInfo> propertyInfos = typeof(T).GetProperties();
            //filter Column ที่ต้องการ Export ออกไป
            if (p_OmmitColumn != null)
            {
                //db.abcs.Where(a => !ID.Contains(a.id)).ToList();
                propertyInfos = propertyInfos.Where(a => !p_OmmitColumn.Contains(a.Name)).ToList<PropertyInfo>();
            }

            //ตรวจสอบว่าต้องส่งการแนบหัว Column ออกไป หรือไม่
            if (includeHeaderLine)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    sb.Append(propertyInfo.Name).Append(",");
                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }
            //อ่านข้อมูลภายใน Object
            foreach (T obj in Objects)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    sb.Append(MakeValueCsvFriendly(propertyInfo.GetValue(obj, null))).Append(",");
                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }
            return sb.ToString();
        }

        private string MakeValueCsvFriendly(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is Nullable && ((INullable)value).IsNull)
            {
                return string.Empty;
            }

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                {
                    return ((DateTime)value).ToString("dd-MMM-yyyy");
                }
                return ((DateTime)value).ToString("dd-MMM-yyyy HH:mm:ss");
            }
            string output = value.ToString();
            if (output.Contains(",") || output.Contains("\""))
            {
                output = '"' + output.Replace("\"", "\"\"") + '"';
            }

            if (output.Contains(Environment.NewLine))
            {
                // newline = \NL\
                output = '"' + output.Replace(Environment.NewLine,"\\NL\\") + '"';
            }
            return output;
        }
        public void ExportToFile(string path)
        {
            File.WriteAllText(path, Export(), Encoding.UTF8);
        }

        public void ExportToFile(string p_Path, Boolean p_IncludeHeader,List<string> p_OmmitColumn)
        {
            File.WriteAllText(p_Path, Export(p_IncludeHeader, p_OmmitColumn), Encoding.UTF8);
        }
        public byte[] ExportToBytes()
        {
            return Encoding.UTF8.GetBytes(Export());
        }
    }
}
