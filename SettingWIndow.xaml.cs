using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Ports;

namespace AccelerationSensorViewer
{
    /// <summary>
    /// SettingWIndow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public bool IsCancel { get; private set; }

        public SettingWindow()
        {
            InitializeComponent();

            cmbPortNo.ItemsSource = SerialPort.GetPortNames();
            cmbRate.ItemsSource = new string[] { "9600", "38400", "115200" };
            cmbData.ItemsSource = new string[] { 
                "7", 
                "8" 
            };
            cmbParity.ItemsSource = new string[] { 
                Parity.None.ToString(), 
                Parity.Even.ToString(), 
                Parity.Mark.ToString(),
                Parity.Odd.ToString(),
                Parity.Space.ToString()
            };
            cmbStopBit.ItemsSource = new string[] { 
                StopBits.None.ToString(),
                StopBits.One.ToString(),
                StopBits.OnePointFive.ToString(),
                StopBits.Two.ToString()
            };
            cmbFlowCtl.ItemsSource = new string[] { 
                Handshake.None.ToString(),
                Handshake.RequestToSend.ToString(),
                Handshake.RequestToSendXOnXOff.ToString(),
                Handshake.XOnXOff.ToString()
            };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsCancel = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var config = SettingData.Load();
            cmbPortNo.Text = config.SerialPortSettingData.PortNum;
            cmbRate.Text = config.SerialPortSettingData.BaudRate.ToString();
            cmbData.Text = config.SerialPortSettingData.Databit.ToString();
            cmbParity.Text = config.SerialPortSettingData.Parity.ToString();
            cmbStopBit.Text = config.SerialPortSettingData.StopBit.ToString();
            cmbFlowCtl.Text = config.SerialPortSettingData.FlowControl.ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var config = SettingData.Load();
            config.SerialPortSettingData.PortNum = cmbPortNo.Text;
            config.SerialPortSettingData.BaudRate = int.Parse(cmbRate.Text);
            config.SerialPortSettingData.Databit = int.Parse(cmbData.Text);
            config.SerialPortSettingData.Parity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text);
            config.SerialPortSettingData.StopBit = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBit.Text);
            config.SerialPortSettingData.FlowControl = (Handshake)Enum.Parse(typeof(Handshake), cmbFlowCtl.Text);
            config.Save();

            IsCancel = false;
            this.Close();
        }
    }
}
