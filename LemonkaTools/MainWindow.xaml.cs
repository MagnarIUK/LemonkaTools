using System.IO;
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

namespace LemonkaTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class DataContextModel
        {
            public string ProgramVersion { get; } = "A-1.0.0";
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new DataContextModel();
            if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")))
            {
                var config = Tools.Read<Dictionary<string, Dictionary<string, List<string>>>>("config.json");





            }

        }
    }
}