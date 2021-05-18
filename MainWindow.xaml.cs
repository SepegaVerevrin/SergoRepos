/*MainWindow главное окно*/
using System;
using System.Windows;
namespace CourseWork {
    public partial class MainWindow : Window {
        public const string ConnectionString = @"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public MainWindow() {
            InitializeComponent();           
            MainFrame.Navigate(new Page1(ConnectionString));
            Manager.MainFrame = MainFrame;
        }
        /*кнопка перейти назад*/
        private void Back(object sender, RoutedEventArgs e) {
            Manager.MainFrame.GoBack();
        }
        /*показывать или нет кнопку назад*/
        private void Main_ContentRendered(object sender, EventArgs e) {
            if (MainFrame.CanGoBack)
                Back_button.Visibility = Visibility.Visible;
            else
                Back_button.Visibility = Visibility.Hidden;
        }
    }
}
