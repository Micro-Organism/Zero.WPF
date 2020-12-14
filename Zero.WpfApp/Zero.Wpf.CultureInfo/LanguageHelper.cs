using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using Zero.Wpf.Core;

namespace Zero.Wpf.CultureInfo
{
    /// <summary>
    /// 语言帮助类
    /// </summary>
    public class LanguageHelper
    {
        #region 字段属性

        static Dictionary<string, ZeroCultureInfo> langFileNames = new Dictionary<string, ZeroCultureInfo>();

        private static LanguageHelper _Instance;
        public static LanguageHelper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LanguageHelper();
                }
                return _Instance;
            }
        }

        /// <summary>
        /// 数据格式区域代码
        /// </summary>
        public string FormatCulture { get; set; }
        public bool EnableSetLanguage { get; private set; }
        public string SystemDefaultCulture { get; private set; }
        public string CurrentCulture { get; private set; }
        public bool IsCustomCulture { get; private set; }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static LanguageHelper()
        {
            List<ZeroCultureInfo> aciList = null;
            aciList = ZeroCultureInfo.GetZeroCultures();
            foreach (ZeroCultureInfo ci in aciList)
            {
                langFileNames.Add(ci.ZeroUID, ci);
            }
        }
        #endregion

        #region 基本方法

        /// <summary>
        /// 设置区域信息
        /// </summary>
        public void SetCulture()
        {
            // 获取操作系统当前设定的区域信息
            System.Globalization.CultureInfo currentCulture = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name);
            // 重写 xaml 界面控件的默认区域信息为指定的区域信息
            FormatCulture = currentCulture.Name;

            // 设定所有线程的默认区域信息
            SetDefaultCulture(currentCulture);
            // 读取 ACal 配置文件的多国语言设置
            SetCulture(ZeroConfigSystem.Instance.ClientUserCultureFile);
        }

        /// <summary>
        /// 加载语言文件
        /// </summary>
        /// <param name="languagefileName">文件名</param>
        public ResourceDictionary LoadLanguageFile(string languagefileName)
        {
            ResourceDictionary resxDict = new ResourceDictionary()
            {
                Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
            };

            if (Application.Current.Resources.MergedDictionaries.Count == 0)
            {
                Application.Current.Resources.MergedDictionaries.Add(resxDict);
            }
            else
            {
                Application.Current.Resources.MergedDictionaries[0] = resxDict;
            }
            return resxDict;
        }

        /// <summary>
        /// 设置区域信息
        /// </summary>
        /// <param name="cultureConfigFile">语言配置文件路径</param>
        public void SetCulture(string cultureConfigFile)
        {
            System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument(ZeroConfigSystem.Instance.SysCultureFile);
            System.Xml.XPath.XPathNavigator navigator = doc.CreateNavigator();
            bool enableSetCulture = false;
            if (navigator.SelectSingleNode("/CultureInfo/EnableSetCulture") != null)
            {
                string value = navigator.SelectSingleNode("/CultureInfo/EnableSetCulture").Value;
                if (!string.IsNullOrEmpty(value))
                {
                    enableSetCulture = value.Equals("1");
                }
            }

            this.EnableSetLanguage = enableSetCulture;
            this.SystemDefaultCulture = "en-US";
            if (navigator.SelectSingleNode("/CultureInfo/Culture") != null)
            {
                this.SystemDefaultCulture = navigator.SelectSingleNode("/CultureInfo/Culture").Value;
                if (string.IsNullOrEmpty(this.SystemDefaultCulture))
                {
                    this.SystemDefaultCulture = "en-US";
                }
            }

            this.CurrentCulture = this.SystemDefaultCulture;
            this.IsCustomCulture = false;
            if (File.Exists(cultureConfigFile))
            {
                string customCulture = IniFileHelper.ReadINIValue(cultureConfigFile, "Langauge", "Default");
                if (!string.IsNullOrEmpty(customCulture))
                {
                    CurrentCulture = customCulture;
                }

                string enableCustomCulture = IniFileHelper.ReadINIValue(cultureConfigFile, "Langauge", "EnableCustomCulture");
                if (!string.IsNullOrEmpty(enableCustomCulture))
                {
                    this.IsCustomCulture = enableCustomCulture.Equals("1");
                }
            }

            SetCulture(CurrentCulture, this.IsCustomCulture);
        }

        /// <summary>
        /// 设置区域信息
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="isCustom"></param>
        public void SetCulture(string culture, bool isCustom)
        {
            this.CurrentCulture = culture;
            this.IsCustomCulture = isCustom;


            ResourceDictionary resxCurrent = null;
            if (!isCustom)
            {
                resxCurrent = LoadLanguageFile(langFileNames[culture].Url);
            }
            else
            {
                string langFile = string.Format(@"{0}\{1}.xaml", ZeroConfigSystem.Instance.CustomLangBasePath, culture);
                if (!File.Exists(langFile))
                {
                    resxCurrent = LoadLanguageFile(langFileNames[this.SystemDefaultCulture].Url);
                }
                else
                {
                    try
                    {
                        resxCurrent = LoadLanguageFile(langFile);
                        ResourceDictionary resxSystem = new ResourceDictionary()
                        {
                            Source = new Uri(langFileNames[this.SystemDefaultCulture].Url, UriKind.RelativeOrAbsolute)
                        };

                        foreach (DictionaryEntry kv in resxSystem)
                        {
                            object value = resxCurrent[kv.Key];
                            if (value == null)
                            {
                                resxCurrent.Add(kv.Key, kv.Value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ACal can't correct loading custom language resource file.", "Resource File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        resxCurrent = LoadLanguageFile(langFileNames[this.SystemDefaultCulture].Url);
                    }
                }
            }

            if (!string.IsNullOrEmpty(FormatCulture))
            {
                // 重写 xaml 界面控件的默认区域信息为指定的区域信息
                FrameworkElement.LanguageProperty.OverrideMetadata(
                   typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(
                      System.Windows.Markup.XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.IetfLanguageTag)));
            }
        }

        #endregion

        #region NET Framework

        /// <summary>
        /// 设定所有线程的默认 CurrentCulture 和 CurrentUICulture
        /// </summary>
        /// <param name="culture">指定的区域信息</param>
//#if NET45
        public void SetDefaultCulture(System.Globalization.CultureInfo culture)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(culture.Name);
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(culture.Name);
        }
//#else
//        public void SetDefaultCulture(System.Globalization.CultureInfo culture)
//        {
//            // 标记当前使用的 .Net 是否是 4.0 以前的版本
//            bool isPriorToDotNet40 = true;
//            Type type = typeof(System.Globalization.CultureInfo);

//            // .Net 4.0 使用 s_userDefaultCulture 和 s_userDefaultUICulture 字段保存线程默认语言参数
//            try
//            {
//                type.InvokeMember("s_userDefaultCulture",
//                                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
//                                    null,
//                                    culture,
//                                    new object[] { culture });

//                type.InvokeMember("s_userDefaultUICulture",
//                                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
//                                    null,
//                                    culture,
//                                    new object[] { culture });

//                // 上面两句 InvokeMember() 未抛异常，表示 CultureInfo 中含有私有字段 s_userDefaultCulture 和 s_userDefaultUICulture，及 .Net 版本为 4.0
//                isPriorToDotNet40 = false;
//            }
//            catch { }

//            if (isPriorToDotNet40)
//            {
//                // .Net 4.0 以前的版本才有 m_userDefaultCulture 和 m_userDefaultUICulture 字段
//                try
//                {
//                    type.InvokeMember("m_userDefaultCulture",
//                                        BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
//                                        null,
//                                        culture,
//                                        new object[] { culture });

//                    type.InvokeMember("m_userDefaultUICulture",
//                                        BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
//                                        null,
//                                        culture,
//                                        new object[] { culture });
//                }
//                catch { }
//            }
//        }
//#endif

        #endregion
    }
}
