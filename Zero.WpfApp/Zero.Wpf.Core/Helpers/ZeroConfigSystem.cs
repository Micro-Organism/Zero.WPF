using System;

namespace Zero.Wpf.Core
{
    /// <summary>
    /// Cal2文件系统类
    /// </summary>
    public class ZeroConfigSystem
    {
        #region 单例对象
        static ZeroConfigSystem _Instance = null;
        public static ZeroConfigSystem Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ZeroConfigSystem();
                }
                return _Instance;
            }
        }
        #endregion

        #region 构造函数
        static ZeroConfigSystem()
        {

        }
        #endregion

        #region 通用配置

        /// <summary>
        /// 程序文件夹
        /// </summary>
        public string AppBinPath
        {
            get
            {
                string dir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return System.IO.Path.GetDirectoryName(dir); ;
            }
            set
            {
                AppBinPath = value;
            }
        }

        /// <summary>
        /// 系统配置文件夹
        /// </summary>
        public string SystemConfigPath
        {
            get
            {
                return AppBinPath + "\\Configs\\";
            }
        }

        /// <summary>
        /// 系统当前区域配置文件
        /// </summary>
        public string SysCultureFile
        {
            get
            {
                return SystemConfigPath + "CultureInfo.xml";
            }
        }

        /// <summary>
        /// 自定义语言基路径
        /// </summary>
        public string CustomLangBasePath
        {
            get
            {
                return SystemConfigPath + "\\Langs\\";
            }
        }

        /// <summary>
        /// 用户当前区域配置文件
        /// </summary>
        public string ClientUserCultureFile
        {
            get
            {
                return SystemConfigPath + "CultureInfo.ini";
            }
        }

        #endregion

    }
}
