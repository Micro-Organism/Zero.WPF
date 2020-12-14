using System;

namespace Zero.Wpf.CultureInfo
{
    /// <summary>
    /// ACal资源键-值项
    /// </summary>
    [Serializable]
    public class ResourceItem
    {
        #region 字段属性

        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 原值
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// 新值
        /// </summary>
        public string NewValue { get; set; }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ResourceItem()
        {

        }
        #endregion

        #region 基本方法

        #endregion

    }
}
