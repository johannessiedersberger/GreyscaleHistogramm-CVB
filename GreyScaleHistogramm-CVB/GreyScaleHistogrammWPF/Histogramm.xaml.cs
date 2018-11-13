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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GreyScaleHistogrammWPF
{
    /// <summary>
    /// Interaction logic for Histogramm.xaml
    /// </summary>
    public partial class Histogramm : UserControl
    {
        public Histogramm()
        {
            InitializeComponent();
            DataContext = new HistogrammViewModel();
        }
    }
}
