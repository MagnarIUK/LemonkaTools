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

namespace LemonkaTools
{

    public partial class ClipsCreatop : Window
    {
        public static bool exists { get; set; }
        public ClipsCreatop()
        {
            exists = true;
            InitializeComponent();

            ANIME.Source = new Uri("J:\\Squirrel\\LemonkaTools\\LemonkaTools\\bin\\Debug\\net8.0-windows\\shorts\\in\\Leadale no Daichi nite - S01EP04(ww).mp4");
            ANIME.Play();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }
           
        


    }
}
