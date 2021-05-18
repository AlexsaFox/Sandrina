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

namespace Sandrina {
    public enum Mode {
        Dark,
        Light 
	}

    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

		#region Навигация по страницам
		public void GotoInstructions(UIElement hide) {
            hide.Visibility = Visibility.Hidden;
            InstructionsPage.Visibility = Visibility.Visible;
        }
        public void GotoGame(UIElement hide) {
            hide.Visibility = Visibility.Hidden;
            GamePage.Visibility = Visibility.Visible;
        }
        public void ChangeTopBarColor(Color color) {
            TopBar.Background = new SolidColorBrush(color);
		}
        public void ChangeTopBarButtonsMode(Mode mode) {
            switch(mode) {
                case Mode.Dark:
                    ExitButton.Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80));
                    break;

                case Mode.Light:
                    ExitButton.Foreground = new SolidColorBrush(Color.FromRgb(221, 221, 221));
                    break;
			}
		}
		#endregion
		#region Настройки окна
		private void DragWindow(object sender, MouseButtonEventArgs e) {
            if(e.ChangedButton == MouseButton.Left) {
                this.DragMove();
			}
		}
		private void CloseWindow(object sender, RoutedEventArgs e) {
            Close();
		}
		#endregion
	}
}
