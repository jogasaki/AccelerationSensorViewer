using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using OxyPlot;

namespace AccelerationSensorViewer
{
    public class AccelerationEventArgs : EventArgs
    {
        public enum EventType
        {
            Z_UP,   //Z軸アップ
            Z_DOWN  //Z軸ダウン
        }

        public AccelerationEventArgs(EventType type)
        {
            OccurredEvent = type;
        }

        public EventType OccurredEvent { get; set; }
    }


    public class LineChartViewModel
    {
        /// <summary>
        /// イベント
        /// </summary>
        public event EventHandler<AccelerationEventArgs> ActionOccurred; 

        //データ格納配列
        public ObservableCollection<DataPoint> XValues { get; private set; }
        public ObservableCollection<DataPoint> YValues { get; private set; }
        public ObservableCollection<DataPoint> ZValues { get; private set; }

        public int MaxCount;            //最大格納数

        private int _nopCounter = 0;    //解析無効区間判定カウンター

        private const int THRESHOLD_NOP_COUNT = 10;   //無効区間　10回は無視する
        private const int THRESHOLD_SHUFFLE = 150;    //振った際の判定値

        private Object _lockObj = new Object();  

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LineChartViewModel()
        {
#if false
            XValues = new ObservableCollection<DataPoint>
            {
                {new DataPoint(0, -10)},
                {new DataPoint(2, 4)},
                {new DataPoint(5, 8)},
                {new DataPoint(8, 3)},
                {new DataPoint(120, 5)},
            };

            YValues = new ObservableCollection<DataPoint>
            {
                {new DataPoint(5, -10)},
                {new DataPoint(3, 1)},
                {new DataPoint(1, 1)},
                {new DataPoint(5, 3)},
                {new DataPoint(0, 0)},
            };

            ZValues = new ObservableCollection<DataPoint>
            {
                {new DataPoint(0, 1)},
                {new DataPoint(3, 3)},
                {new DataPoint(4, 4)},
                {new DataPoint(5, 5)},
                {new DataPoint(-1, -1)},
            };
#else
            XValues = new ObservableCollection<DataPoint>();
            YValues = new ObservableCollection<DataPoint>();
            ZValues = new ObservableCollection<DataPoint>();

#endif
        }

        public void addData(double x, double y, double z)
        {
            lock (_lockObj)
            {
                XValues.Add(new DataPoint(XValues.Count, x));
                YValues.Add(new DataPoint(YValues.Count, y));
                ZValues.Add(new DataPoint(ZValues.Count, z));

                if (MaxCount == ZValues.Count)
                {
                    XValues.RemoveAt(0);
                    reNumbering(XValues);

                    YValues.RemoveAt(0);
                    reNumbering(YValues);

                    ZValues.RemoveAt(0);
                    reNumbering(ZValues);
                }

                parseData();
            }

        }

        private void parseData()
        {

            if (_nopCounter > 0)
            {
                _nopCounter--;
            }

            //解析
            if ((ZValues.Count > 3) && (_nopCounter == 0))
            {
                double diff = 0;
                diff = ZValues[ZValues.Count - 1].Y - ZValues[ZValues.Count - 2].Y;

                if ((ZValues[ZValues.Count - 1].Y > ZValues[ZValues.Count - 2].Y) && (Math.Abs(diff) > THRESHOLD_SHUFFLE))
                {
                    _nopCounter = THRESHOLD_NOP_COUNT;
                    this.ActionOccurred(this, new AccelerationEventArgs(AccelerationEventArgs.EventType.Z_UP));
                }
                else if ((ZValues[ZValues.Count - 1].Y < ZValues[ZValues.Count - 2].Y) && (Math.Abs(diff) > THRESHOLD_SHUFFLE))
                {
                    _nopCounter = THRESHOLD_NOP_COUNT;
                    this.ActionOccurred(this, new AccelerationEventArgs(AccelerationEventArgs.EventType.Z_DOWN));

                }

                /*
                if (ZValues[ZValues.Count - 1].Y - ZValues[ZValues.Count - 2].Y > THRESHOLD_SHUFFLE)
                {
                    //Z軸アップイベント発生させる
                    _nopCounter = THRESHOLD_NOP_COUNT;
                    this.ActionOccurred(this, new ActionOccurredEventArgs(ActionOccurredEventArgs.EventType.Z_UP));
                }
                else if (ZValues[ZValues.Count - 1].Y - ZValues[ZValues.Count - 2].Y < -THRESHOLD_SHUFFLE)
                {
                    //Z軸ダウンイベント発生させる
                    _nopCounter = THRESHOLD_NOP_COUNT;
                    this.ActionOccurred(this, new ActionOccurredEventArgs(ActionOccurredEventArgs.EventType.Z_DOWN));
                }
                */
            }


        }


        private void reNumbering(ObservableCollection<DataPoint> values)
        {
            int index = 0;
            for (int cnt = 0; cnt < values.Count; cnt++ )
            {
                values[cnt] = new DataPoint(index, values[cnt].Y);
                index++;
            }
        }

        public void ClearAll()
        {
            XValues.Clear();
            YValues.Clear();
            ZValues.Clear();
        }

    }
}
