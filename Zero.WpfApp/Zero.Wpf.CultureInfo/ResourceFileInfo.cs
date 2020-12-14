using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace Zero.Wpf.CultureInfo
{
    /// <summary>
    /// 资源文件信息
    /// </summary>
    [Serializable]
    public class ResourceFileInfo
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResourceFileInfo()
        {

        }
        #endregion

        #region 字段属性
        /// <summary>
        /// ID－数据表主键
        /// </summary>
        public string FileID
        {
            get;
            set;
        }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string CultureName
        {
            get;
            set;
        }

        /// <summary>
        /// 区域标题
        /// </summary>
        public string CultureCaption
        {
            get;
            set;
        }

        /// <summary>
        /// 资源数据
        /// </summary>
        public List<ResourceItem> ResxData { get; set; }

        /// <summary>
        /// 启用标识
        /// </summary>
        public bool IsEnabled { get; set; }

        #endregion

        #region 基本方法
        /// <summary>
        /// 加载用户指定的资源Xaml文件
        /// </summary>
        /// <param name="resxFile">资源文件名称</param>
        /// <returns>返回资源文件信息</returns>
        public static ResourceFileInfo LoadResourXamlFromFile(string resxFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(resxFile);

            List<ResourceItem> itemList = new List<ResourceItem>();
            foreach (XmlNode ele in doc.DocumentElement.ChildNodes)
            {
                if (ele is XmlElement)
                {
                    XmlAttribute attr = ele.Attributes["x:Key"];
                    if (attr == null)
                    {
                        attr = ele.Attributes["x:key"];
                    }
                    if (attr != null)
                    {
                        itemList.Add(new ResourceItem() { Key = attr.Value, OldValue = ele.InnerText });
                    }
                }
            }
            itemList.Sort(new Comparison<ResourceItem>((ResourceItem item1, ResourceItem item2) => { return item1.Key.CompareTo(item2.Key); }));

            ResourceFileInfo resxFileInfo = new ResourceFileInfo();
            resxFileInfo.ResxData = itemList;
            return resxFileInfo;
        }

        /// <summary>
        /// 将资源键-值对集合保存到指定的Xaml文件中
        /// </summary>
        /// <param name="resxList"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool WriteToXamlFile(string fileName)
        {
            var doc = new XmlDocument();
            XmlElement rootEle = doc.CreateElement("ResourceDictionary");
            doc.AppendChild(rootEle);

            rootEle.SetAttribute("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            rootEle.SetAttribute("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");
            rootEle.SetAttribute("xmlns:s", "clr-namespace:System;assembly=mscorlib");

            ResourceFileInfo resxFileInfo = new ResourceFileInfo();
            foreach (ResourceItem kv in resxFileInfo.ResxData)
            {
                XmlElement kvEle = doc.CreateElement("s:String");
                XmlAttribute attr = doc.CreateAttribute("x:key");
                attr.Value = kv.Key;
                kvEle.Attributes.Append(attr);
                XmlText xmltext = doc.CreateTextNode(kv.OldValue);
                kvEle.AppendChild(xmltext);
                rootEle.AppendChild(kvEle);
            }

            try
            {
                doc.Save(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="resxFileInfo">资源文件信息对象</param>
        /// <returns>二进制数据</returns>
        public static byte[] BinarySerialize(ResourceFileInfo resxFileInfo)
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    formatter.Serialize(stream, resxFileInfo);
                    return stream.GetBuffer();
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data">二进制数据</param>
        /// <returns>资源文件信息对象</returns>
        public static ResourceFileInfo BinaryDeserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            using (MemoryStream stream = new MemoryStream(data))
            {
                IFormatter formatter = new BinaryFormatter();
                try
                {
                    return formatter.Deserialize(stream) as ResourceFileInfo;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

    }
}
