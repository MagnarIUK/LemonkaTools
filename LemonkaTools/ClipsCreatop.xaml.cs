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
    /// <summary>
    /// Interaction logic for ClipsCreatop.xaml
    /// </summary>
    public partial class ClipsCreatop : Window
    {
        public static bool exists { get; set; }
        public ClipsCreatop()
        {
            exists = true;
            InitializeComponent();

            //ANIME.Source = new Uri()
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
