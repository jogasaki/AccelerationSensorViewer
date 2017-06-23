using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO.Ports;
using System.Windows.Media.Media3D;


namespace AccelerationSensorViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

    #region 依存プロパティー
        /// <summary>
        /// 縦軸の最大値 依存プロパティー
        /// </summary>
        public static readonly DependencyProperty VerticalScaleMaxProperty =
                DependencyProperty.Register(
                    "VerticalScaleMax",                // プロパティ名を指定
                    typeof(int),                // プロパティの型を指定
                    typeof(MainWindow),         // プロパティを所有する型を指定
                    new PropertyMetadata(400));  // メタデータを指定。ここではデフォルト値を設定してる

        /// <summary>
        /// 縦軸の最小値 依存プロパティー
        /// </summary>
        public static readonly DependencyProperty VerticalScaleMinProperty =
                DependencyProperty.Register(
                    "VerticalScaleMin",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(-400)); 

        /// <summary>
        /// 横軸の最大値 依存プロパティー
        /// </summary>
        public static readonly DependencyProperty HorizontalScaleMaxProperty =
                DependencyProperty.Register(
                    "HorizontalScaleMax",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(100));
        /// <summary>
        /// 横軸の最小値 依存プロパティー
        /// </summary>
        public static readonly DependencyProperty HorizontalScaleMinProperty =
                DependencyProperty.Register(
                    "HorizontalScaleMin",
                    typeof(int),
                    typeof(MainWindow),
                    new PropertyMetadata(0));

    #endregion 依存プロパティー

        /// <summary>
        /// 縦軸スケールの最大値
        /// </summary>
        public int VerticalScaleMax
        {
            get { return (int)GetValue(VerticalScaleMaxProperty); }
            set { SetValue(VerticalScaleMaxProperty, value); }
        }

        /// <summary>
        /// 縦軸スケールの最小値
        /// </summary>
        public int VerticalScaleMin
        {
            get { return (int)GetValue(VerticalScaleMinProperty); }
            set { SetValue(VerticalScaleMinProperty, value); }
        }

        /// <summary>
        /// 横軸スケールの最大値
        /// </summary>
        public int HorizontalScaleMax
        {
            get { return (int)GetValue(HorizontalScaleMaxProperty); }
            set { SetValue(HorizontalScaleMaxProperty, value); }
        }

        /// <summary>
        /// 横軸スケールの最小値
        /// </summary>
        public int HorizontalScaleMin
        {
            get { return (int)GetValue(HorizontalScaleMinProperty); }
            set { SetValue(HorizontalScaleMinProperty, value); }
        }

        /// <summary>
        /// シリアルポートの監視
        /// </summary>
        SerialPort _ports = null;


        LineChartViewModel _lineViewModel = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _lineViewModel = DataContext as LineChartViewModel;
            _lineViewModel.MaxCount = (HorizontalScaleMax - HorizontalScaleMin);
            _lineViewModel.ActionOccurred += _lineViewModel_ActionOccurred;

            comportOpen();

        }

        private void comportOpen()
        {
            var config = SettingData.Load();
            _ports = new SerialPort(config.SerialPortSettingData.PortNum);
            _ports.BaudRate = (int)config.SerialPortSettingData.BaudRate;
            _ports.Parity = config.SerialPortSettingData.Parity;
            _ports.StopBits = config.SerialPortSettingData.StopBit;
            _ports.DataBits = config.SerialPortSettingData.Databit;
            _ports.Handshake = config.SerialPortSettingData.FlowControl;
            //_ports.RtsEnable = true;

            _ports.DataReceived -= _ports_DataReceived;
            _ports.DataReceived += _ports_DataReceived;

            try
            {
                _ports.Open();
            }
            catch (Exception e)
            {
                var asmttl =(System.Reflection.AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                                                        System.Reflection.Assembly.GetExecutingAssembly(),
                                                        typeof(System.Reflection.AssemblyTitleAttribute));
                MessageBox.Show(e.Message, asmttl.Title, MessageBoxButton.OK, MessageBoxImage.Error);

                _ports.Dispose();
                _ports = null;
            }
            
        }

        private void comportClose()
        {
            if (_ports != null)
            {
                _ports.DataReceived -= _ports_DataReceived;
                _ports.Close();
                _ports.Dispose();
                _ports = null;
            }
        }

        void _lineViewModel_ActionOccurred(object sender, AccelerationEventArgs e)
        {

            var task = Task.Run(() =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    switch (e.OccurredEvent)
                    {
                        case AccelerationEventArgs.EventType.Z_UP:
                            elAction.Fill = new SolidColorBrush(Colors.Red);
                            keySend(e.OccurredEvent);
                            break;
                        case AccelerationEventArgs.EventType.Z_DOWN:
                            elAction.Fill = new SolidColorBrush(Colors.Blue);
                            keySend(e.OccurredEvent);
                            break;
                    }       
                    
                }));
                
                System.Threading.Thread.Sleep(100);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    elAction.Fill = new SolidColorBrush(Colors.Black);
                }));                
            });            
        }

        void _ports_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            var values = parseString(indata);
            if (values.HasValue)
            {
                this.Dispatcher.BeginInvoke((Action)(() => {
                    _lineViewModel.addData(values.Value.X, values.Value.Y, values.Value.Z);
                }));
            }
        }

        private Point3D? parseString(string data)
        {
            Point3D ret = new Point3D();
            var xPos = data.IndexOf(":x=");
            if (xPos < 0)
            {
                return null;
            }

            var yPos = data.IndexOf(":y=");
            var zPos = data.IndexOf(":z=");

            ret.X = Double.Parse(data.Substring(xPos + 3, 4));
            ret.Y = Double.Parse(data.Substring(yPos + 3, 4));
            ret.Z = Double.Parse(data.Substring(zPos + 3, 4));

            return ret;
        }

        private void chkVisible_Checked(object sender, RoutedEventArgs e)
        {
            if (lineSeriesX == null)
            {
                return;
            }

            CheckBox target = sender as CheckBox;
            if (target == this.chkXVisible)
            {
                lineSeriesX.Visibility = System.Windows.Visibility.Visible;
            }
            else if (target == this.chkYVisible)
            {
                lineSeriesY.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                lineSeriesZ.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void chkVisible_Unchecked(object sender, RoutedEventArgs e)
        {
            if (lineSeriesX == null)
            {
                return;
            }

            CheckBox target = sender as CheckBox;
            if (target == this.chkXVisible )
            {
                lineSeriesX.Visibility = System.Windows.Visibility.Collapsed; 
            }
            else if(target == this.chkYVisible )
            {
                lineSeriesY.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                lineSeriesZ.Visibility = System.Windows.Visibility.Collapsed; 
            }
        }

        /// <summary>
        /// 終了メニュークリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemEnd_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// シリアルポート設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSettingPort_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow w = new SettingWindow();
            w.ShowDialog();

            if (!w.IsCancel)
            {
                comportClose();
                comportOpen();
            }
        }

        private void MenuItemClearGraph_Click(object sender, RoutedEventArgs e)
        {
            _lineViewModel.ClearAll();
        }

        private void MenuItemHiddenGraph_Click(object sender, RoutedEventArgs e)
        {
            if (menuHidenGraph.Header.ToString().Equals("グラフ非表示"))
            {
                grdGraph.Visibility = System.Windows.Visibility.Collapsed;
                menuHidenGraph.Header = "グラフ表示";
            }
            else
            {
                grdGraph.Visibility = System.Windows.Visibility.Visible;
                menuHidenGraph.Header = "グラフ非表示";
            }
            
        }

        private void ChartWindow_Closed(object sender, EventArgs e)
        {
            comportClose();
            _lineViewModel.ActionOccurred -= _lineViewModel_ActionOccurred;
        }

    
        private void keySend(AccelerationEventArgs.EventType type)
        {
            if (chkWindowLink.IsChecked == false)
            {
                return;
            }

            IntPtr hWnd;
            string sClassName = "screenClass";
            string sWindowText = null; 
            //IntPtr wParam, lParam;
            //bool bresult;

            // NotepadのWindowハンドル取得
            hWnd = NativeMethod.FindWindow(sClassName, sWindowText);
            if (IntPtr.Zero == hWnd)
            {
                return;
            }

            //対象のWindowをアクティブにする
            NativeMethod.SetForegroundWindow(hWnd);
            if (type == AccelerationEventArgs.EventType.Z_UP)
            {
                System.Windows.Forms.SendKeys.SendWait("{UP}");
            }
            else
            {
                System.Windows.Forms.SendKeys.SendWait("{DOWN}");
            }            
        }
    }
}
