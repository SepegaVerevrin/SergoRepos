/*Admin_Goods_Add Страница администратора для добавления товаров*/
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
namespace CourseWork {
    public partial class Admin_Goods_Add : Page {
        public string ConnectionString;
        byte[] imageData = null; // Where i am ?
        public Admin_Goods_Add(string connectionString) {
            InitializeComponent();
            ConnectionString = connectionString;
        }
        /*кнопка добавления товара*/
        private void Admin_Goods_Continue_Button(object sender, RoutedEventArgs e) {
            int err = 0;
            Regex regex_name = new Regex(@"^[А-Яа-яA-Za-zёЁ0-9_]+$");
            Regex regex_price = new Regex(@"[0-9]*[,?][0-9]{2}$"); 
            Regex regex_count = new Regex(@"^[0-9]+$");
            if ((TextBox_Product_Name.Text.Length == 0) || (!regex_name.IsMatch(TextBox_Product_Name.Text))) {
                Error_messege.Text = "неверное имя, нельзя использовать спец. символы: ./{(*#@#) и т.п.";
                err = -1;
                MessageBox.Show("Недопустимые значения");
            } else { Error_messege.Text = "";  }
            if ((TextBox_Product_Price.Text.Length == 0) || (!regex_price.IsMatch(TextBox_Product_Price.Text))) {
                Error_messege.Text = "неверная цена товара, можно использовать только цифры и (напр: 0,01)";
                err = -1;
                MessageBox.Show("Недопустимые значения");
            } else { Error_messege.Text = ""; }
            if ((TextBox_Count_Product.Text.Length == 0) || (!regex_count.IsMatch(TextBox_Count_Product.Text))) {
                Error_messege.Text = "неверное кол-во товара, можно использовать только цифры";
                err = -1;
                MessageBox.Show("Недопустимые значения");
            } else { Error_messege.Text = "";  }
            if ((TextBox_Description.Text.Length == 0) || (!regex_name.IsMatch(TextBox_Description.Text))) {
                Error_messege.Text = "неверное описание, нельзя использовать спец. символы: ./{(*#@#) и т.п.";
                err = -1;
                MessageBox.Show("Недопустимые значения");
            } else { Error_messege.Text = "";  }
            if (Name_File_Image.Text.Length == 0) {
                Error_messege.Text = "Файл не выбран";
                err = -1;
                MessageBox.Show("Недопустимые значения");
            } else { Error_messege.Text = ""; }
            if (err == 0) {
                try {
                    int count = int.Parse(TextBox_Count_Product.Text);
                    String product_name = TextBox_Product_Name.Text;
                    float price = float.Parse(TextBox_Product_Price.Text);
                    String description = TextBox_Description.Text;
                    SqlConnection connection = new SqlConnection(ConnectionString);
                    connection.Open();
                    /*добавление товара*/
                    string sqlExpression = "INSERT INTO Goods(product_id ,product_name ,description ,price ,count ,image) " +
                    "VALUES ((SELECT MAX(product_id+1) FROM Goods), @product_name_value, @description_value, @price_value, @count_value, @image_value)";
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlParameter product_name_param = new SqlParameter("@product_name_value", product_name); command.Parameters.Add(product_name_param);
                    SqlParameter price_param = new SqlParameter("@price_value", price); command.Parameters.Add(price_param);
                    SqlParameter description_param = new SqlParameter("@description_value", ' '); command.Parameters.Add(description_param);
                    SqlParameter count_param = new SqlParameter("@count_value", count); command.Parameters.Add(count_param);
                    SqlParameter image_param = new SqlParameter("@image_value", (object)imageData); command.Parameters.Add(image_param);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Данные добавлены");
                    Manager.MainFrame.GoBack();
                } catch (SqlException error) {
                    MessageBox.Show(error.Message);
                }
            }
        }
        /*кнопка выбора фотографии*/
        private void Admin_Selection_Goods_Image_Button(object sender, RoutedEventArgs e) {
            try {
                  OpenFileDialog myDialog = new OpenFileDialog();
                  myDialog.Filter = "Картинки(*.JPG;*.GIF;*.png;)|*.JPG;*.GIF;*.png;" + "|Все файлы (*.*)|*.* ";
                  myDialog.CheckFileExists = true;
                  myDialog.Multiselect = true;
                  if (myDialog.ShowDialog() == true) {
                        /*---------------Проверка-На-Объем-Файла-------------*/
                        FileInfo fInfo = new FileInfo(myDialog.FileName);
                        long numBytes = fInfo.Length;
                        if (numBytes > 10485760) {
                            MessageBox.Show("Error download file!");
                        } else {
                            Name_File_Image.Text = myDialog.FileName;
                            FileStream FileStream = new FileStream(myDialog.FileName, FileMode.Open, FileAccess.Read);
                            BinaryReader BinaryReader = new BinaryReader(FileStream);
                            imageData = BinaryReader.ReadBytes((int)numBytes);
                        }
                  }
            } catch (SqlException error) {
                  MessageBox.Show(error.Message);
            }
        }
    }
}
