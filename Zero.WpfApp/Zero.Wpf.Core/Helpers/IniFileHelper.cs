using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Zero.Wpf.Core
{
    /// <summary>
    /// INI文件读写类
    /// </summary>
    public class IniFileHelper
    {
        #region ini文件操作
        //声明读写INI文件的API函数 
        [DllImport("kernel32",CharSet = CharSet.Unicode)]
        private static extern int WritePrivateProfileString(string section, string key, string val, string filePath);


        [DllImport("kernel32",CharSet=CharSet.Unicode)]  
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        //写INI文件
        public static void WriteINIValue(string FilePath, string Section, string Key, string Value)
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            }
            int n = WritePrivateProfileString(Section, Key, Value, FilePath);
        }

        //读INI文件
        public static string ReadINIValue(string FilePath, string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(4096);
            GetPrivateProfileString(Section, Key, "", temp, 4096, FilePath);
            return temp.ToString();
        }

        /// <summary>
        /// 读INI文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ReadINIValue(string FilePath, string Section, string key, string defaultValue)
        {
            string ret = ReadINIValue(FilePath, Section, key);
            if (!string.IsNullOrEmpty(ret))
            {
                return ret;
            }
            else
            {
                return defaultValue;
            }
        }
        #endregion
    }
}
