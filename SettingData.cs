using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace AccelerationSensorViewer
{

    [System.Xml.Serialization.XmlRoot("SettingData")]
    public class SettingData
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        private static SettingData _current = null;

        /// <summary>
        /// シリアルポート設定
        /// </summary>
        public SerialPortSettings SerialPortSettingData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private const string SAVE_FILE_NAME = "mysettings.xml";

        /// <summary>
        /// インスタンスの取得
        /// </summary>
        /// <returns></returns>
        public static SettingData GetInstance()
        {
            if (_current == null)
            {
                _current = SettingData.Load();
                if (_current == null)
                {
                    _current = new SettingData();
                }
            }

            return _current;
        }

        public static SettingData Load()
        {
            FileStream fs = null;
            SettingData model = null;
            App ap = App.Current as App;
            string file = Path.Combine(ap.StartUpFolder, SAVE_FILE_NAME);

            try
            {
                fs = new FileStream(file, System.IO.FileMode.Open);
                var serializer = new XmlSerializer(typeof(SettingData));
                model = (SettingData)serializer.Deserialize(fs);
                fs.Close();                
            }
            catch(Exception)
            {
                model = new SettingData();
                if (fs != null) fs.Close();
            }

            return model;
        }


        /// <summary>
        /// XML保存
        /// </summary>
        /// <param name="data"></param>
        public void Save()
        {
            //出力先XMLのストリーム
            App ap = App.Current as App;
            string saveFile = Path.Combine(ap.StartUpFolder, SAVE_FILE_NAME);

            FileStream stream = new FileStream(saveFile, System.IO.FileMode.Create);
            StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8);

            XmlSerializer serializer = new XmlSerializer(typeof(SettingData));
            serializer.Serialize(writer, this);

            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SettingData()
        {
            //シングルトン
            SerialPortSettingData = new SerialPortSettings();
        }        


    }
    
}
