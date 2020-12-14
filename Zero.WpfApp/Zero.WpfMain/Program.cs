using System;
using System.Windows;
using Zero.Wpf.CultureInfo;

namespace Zero.WpfMain
{
    /// <summary>
    /// 主程序入口
    /// </summary>
    public class Program
    {
        [STAThread]
        static void Main(params string[] args) 
        {
            ZeroWpfApp app = new ZeroWpfApp();
            app.InitialAppResources();
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            LanguageHelper.Instance.SetDefaultCulture(System.Globalization.CultureInfo.CurrentCulture);
            MainWindow mainWindow = new MainWindow();
            //app.Messager.Register(mainWindow);
            app.Run(mainWindow);
        }
    }
}
