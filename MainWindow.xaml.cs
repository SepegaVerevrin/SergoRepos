﻿using System;
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
