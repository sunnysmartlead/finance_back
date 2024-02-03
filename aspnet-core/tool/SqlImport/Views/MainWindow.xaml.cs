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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SqlImport.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOpenOrClose_Click(object sender, RoutedEventArgs e)
        {
            if (btnOpenOrClose.IsChecked == true)
            {
                DoubleAnimation StopAnimation = new DoubleAnimation();
                StopAnimation.From = 0;
                StopAnimation.To = -(this.Height - 15);
                StopAnimation.Duration = new Duration(TimeSpan.Parse("0:0:0.5"));
                this.BeginAnimation(MainWindow.TopProperty, StopAnimation);
                btnOpenOrClose.ToolTip = "展开";
            }
            else
            {
                DoubleAnimation OpenAnimation = new DoubleAnimation();
                OpenAnimation.From = -(Height - 15);
                OpenAnimation.To = 0.1;
                OpenAnimation.Duration = new Duration(TimeSpan.Parse("0:0:0.5"));
                this.BeginAnimation(MainWindow.TopProperty, OpenAnimation);
                btnOpenOrClose.ToolTip = "收起";
            }
        }
    }
}
