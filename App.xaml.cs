using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AccelerationSensorViewer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Exeのあるフォルダ
        /// </summary>
        public string StartUpFolder { get; private set; }


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //EXEの場所を覚えておく
            string exePath = Environment.GetCommandLineArgs()[0];
            string exeFullPath = System.IO.Path.GetFullPath(exePath);
            string startupPath = System.IO.Path.GetDirectoryName(exeFullPath);
            StartUpFolder = startupPath;
        }
    }
}
