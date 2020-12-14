using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zero.Wpf.Core;
using Zero.Wpf.CultureInfo;

namespace Zero.WpfMain
{
    /// <summary>
    /// ZeroWpfApp应用程序类
    /// </summary>
    public class ZeroWpfApp : Application
    {
        #region 字段属性

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ZeroWpfApp()
        {

        }

        #endregion

        #region 基本方法

        /// <summary>
        /// 初始App资源字典
        /// </summary>
        /// <param name="app"></param>
        public void InitialAppResources()
        {
            this.Resources.MergedDictionaries.Clear();
            LanguageHelper.Instance.SetCulture();
        }

        #endregion
    }
}
