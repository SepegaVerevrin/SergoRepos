/*MainWindow главное окно*/
using System;
using System.Windows;

namespace CourseWork {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();           
            MainFrame.Navigate(new Page1());
            Manager.MainFrame = MainFrame;
        }

        private void Back(object sender, RoutedEventArgs e) {
            Manager.MainFrame.GoBack();
        }

        private void Main_ContentRendered(object sender, EventArgs e) {
            if (MainFrame.CanGoBack)
                Back_button.Visibility = Visibility.Visible;
            else
                Back_button.Visibility = Visibility.Hidden;
        }

        private void MainFrame_Navigated() {
        }

    }
}
