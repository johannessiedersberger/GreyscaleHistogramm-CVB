using System.Windows;
using System.Windows.Controls;
using Stemmer.Cvb;
using System.Collections.Generic;
using System.Diagnostics;

namespace GreyScaleHistogramm_CVB
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
