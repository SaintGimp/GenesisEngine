using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace GenesisEngine
{
    public class StatisticsViewModel
    {
        readonly Statistics _statistics;
        Dispatcher _dispatcher;
        Timer _timer;

        public StatisticsViewModel(Statistics statistics)
        {
            _statistics = statistics;
            StatisticsList = new ObservableCollection<string>();

            _dispatcher = Dispatcher.CurrentDispatcher;
            _timer = new Timer(s => _dispatcher.Invoke((Action)(Update)), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(250));
        }

        public ObservableCollection<string> StatisticsList { get; private set; }

        public void Update()
        {
            // TODO: need to disable changed events while doing this

            StatisticsList.Clear();

            StatisticsList.Add("Frame rate: " + _statistics.FrameRate);
            StatisticsList.Add("Number of quad nodes: " + _statistics.NumberOfQuadNodes);
            StatisticsList.Add("Number of quad nodes per level: " + GetQuadNodesPerLevel());
            StatisticsList.Add("Highest level: " + GetHighestQuadNodeLevel());
            StatisticsList.Add("Number of quad meshes rendered: " + _statistics.PreviousNumberOfQuadMeshesRendered);
            StatisticsList.Add("Number of pending quad node splits: " + _statistics.NumberOfPendingSplits);
            StatisticsList.Add("Number of pending quad node merges: " + _statistics.NumberOfPendingMerges);
            StatisticsList.Add("Camera altitude: " + _statistics.CameraAltitude.ToString("F0") + " m (" + DoubleMathHelper.MetersToFeet(_statistics.CameraAltitude).ToString("F0") + " ft) ASL");
        }

        string GetQuadNodesPerLevel()
        {
            string text = "";
            for (int x = 0; x < _statistics.NumberOfQuadNodesAtLevel.Length; x++)
            {
                if (_statistics.NumberOfQuadNodesAtLevel[x] > 0)
                {
                    text += x + "[" + _statistics.NumberOfQuadNodesAtLevel[x] + "] ";
                }
                else
                {
                    break;
                }
            }

            return text;
        }

        int GetHighestQuadNodeLevel()
        {
            for (int x = 0; x < _statistics.NumberOfQuadNodesAtLevel.Length; x++)
            {
                if (_statistics.NumberOfQuadNodesAtLevel[x] == 0)
                {
                    return x - 1;
                }
            }

            return _statistics.NumberOfQuadNodesAtLevel.Length - 1;
        }
    }
}
