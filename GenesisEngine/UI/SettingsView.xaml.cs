using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenesisEngine
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window, IView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public object Model
        {
            get { return DataContext; }
            set { DataContext = value; }
        }

        void IView.Show()
        {
            this.ShowActivated = false;
            Show();
        }
    }
}
