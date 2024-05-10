using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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

            if (!Directory.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"));
            }

            this.DataContext = new DataContextModel();
        }

        private void relise_creator_button_Click(object sender, RoutedEventArgs e)
        {
            RelizeMaker relizeMaker = new RelizeMaker();
            if (!relizeMaker.IsClosed)
            {
                relizeMaker.Show();
            }
        }

        private void clip_creator_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

    }
}