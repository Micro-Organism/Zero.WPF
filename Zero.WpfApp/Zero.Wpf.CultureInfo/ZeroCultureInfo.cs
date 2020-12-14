using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace Zero.Wpf.CultureInfo
{
    public class ZeroCultureInfo
    {
        #region 字段属性

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ZeroUID
        {
            get
            {
                if (!IsCustomCulture)
                    return Name;
                else
                    return Name + "(Custom)";
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 资源位置
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 用户自定义标识
        /// </summary>
        public bool IsCustomCulture { get; set; }

        /// <summary>
        /// 自定义资源的版本
        /// </summary>
        public int VersionOfCustomCulture { get; set; }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ZeroCultureInfo()
        {

        }
        #endregion

        #region 基本方法

        /// <summary>
        /// 获取国内区域列表
        /// </summary>
        /// <returns></returns>
        public static List<ZeroCultureInfo> GetZeroCultures()
        {
            return new List<ZeroCultureInfo>()
            {
                new ZeroCultureInfo(){ Name="en-US",Description="English",Url="pack://application:,,,/Zero.Wpf.CultureInfo;component/Langs/en-US.xaml",IsCustomCulture=false},
                new ZeroCultureInfo(){ Name="zh-CHS",Description="简体中文(Simplified Chinese)",Url="pack://application:,,,/Zero.Wpf.CultureInfo;component/Langs/zh-CHS.xaml",IsCustomCulture=false},
                new ZeroCultureInfo(){ Name="zh-CHT",Description="繁體中文(Traditional Chinese)",Url="pack://application:,,,/Zero.Wpf.CultureInfo;component/Langs/zh-CHT.xaml", IsCustomCulture=false}
            };
        }

        /// <summary>
        /// 根据区域代码加载系统Xaml文件
        /// </summary>
        /// <param name="cultureName">区域名称</param>
        /// <returns>资源文件对象</returns>
        public static ResourceFileInfo ReadSystemCultureXamlFile(string cultureName)
        {
            string url = string.Format("pack://application:,,,/Zero.Wpf.CultureInfo;component/Langs/{0}.xaml", cultureName);

            ResourceDictionary resxDict = null;
            try
            {
                resxDict = new ResourceDictionary()
                {
                    Source = new Uri(url, UriKind.RelativeOrAbsolute)
                };
            }
            catch
            {
                return null;
            }

            var doc = new XmlDocument();
            XmlElement rootEle = doc.CreateElement("ResourceDictionary");
            doc.AppendChild(rootEle);

            //设置命名空间
            rootEle.SetAttribute("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            rootEle.SetAttribute("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");
            rootEle.SetAttribute("xmlns:s", "clr-namespace:System;assembly=mscorlib");

            SortedDictionary<string, string> sortDict = new SortedDictionary<string, string>();

            ResourceFileInfo resxFileInfo = new ResourceFileInfo();
            resxFileInfo.ResxData = new List<ResourceItem>();

            foreach (DictionaryEntry kv in resxDict)
            {
                object value = resxDict[kv.Key];
                if (value == null)
                {
                    continue;
                }
                resxFileInfo.ResxData.Add(new ResourceItem() { Key = kv.Key.ToString(), OldValue = value.ToString() });
            }

            resxFileInfo.ResxData.Sort(new Comparison<ResourceItem>((ResourceItem item1, ResourceItem item2) => { return item1.Key.CompareTo(item2.Key); }));
            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.GetCultureInfo(cultureName);
            resxFileInfo.CultureName = cultureName;
            resxFileInfo.CultureCaption = ci.DisplayName;

            return resxFileInfo;
        }

        #endregion

    }
}
