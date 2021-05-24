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

namespace Sandrina.UserElements {
	public partial class Instructions : UserControl {
		public Instructions() {
			InitializeComponent();
		}

		private void GotoGame(object sender, RoutedEventArgs e) {
			MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
			mainWindow.GotoGame(this);
			mainWindow.TopBarElement.ChangeTopBarColor(GetColor.LightYellow);
			mainWindow.TopBarElement.ChangeTopBarButtonsMode(Mode.Dark);
		}
	}
}
