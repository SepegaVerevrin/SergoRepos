/*Admin_Goods_Edit страница редактирования товара*/
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;
namespace CourseWork {
    public partial class Admin_Goods_Edit : Page {
        public int Product_id;
        public class catalog_goods {
            public byte[] image { get; set; }
        }
        byte[] imageData = null; 
        public Admin_Goods_Edit(int product_id) {
            InitializeComponent();
            Product_id = product_id;
            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();
            List<catalog_goods> goods_list = new List<catalog_goods>();
            // выводим старую информацию
            string sqlExpression = "SELECT * FROM Goods WHERE product_id=@product_id_value";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlParameter product_id_param = new SqlParameter("@product_id_value", product_id); command.Parameters.Add(product_id_param);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    catalog_goods st = new catalog_goods();
                    TextBlock_Product_Name.Text = reader.GetString(1);
                    TextBlock_Product_description.Text = reader[2].ToString();
                    TextBlock_Product_Count.Text = reader.GetValue(3).ToString();
                    TextBlock_Product_price.Text = reader.GetValue(4).ToString();
                    st.image = reader[5] as byte[];
                    goods_list.Add(st);
                    Image_Product.DataContext = goods_list;
                }
                reader.Close();
            } else {
                reader.Close();
            }
        }
        // кнопка редактирования товара
        private void Admin_Goods_Continue_Button(object sender, RoutedEventArgs e) {
            int err = 0;
            Regex regex_name = new Regex(@"^[А-Яа-яA-Za-zёЁ0-9_]+$");
            Regex regex_price = new Regex(@"^[0-9,]+$");
            Regex regex_count = new Regex(@"^[0-9]+$");
            int count = 0;
            String product_name="";
            float price=0;
            String description="";
            if ((TextBox_Product_Name.Text.Length == 0) || (!regex_name.IsMatch(TextBox_Product_Name.Text))) {
                Error_messege.Text = " ошибка в имени товара";
                err = -1;
            } else {
                product_name = TextBox_Product_Name.Text;
                Error_messege.Text = "";
            }
            if ((TextBox_Product_Price.Text.Length == 0) || (!regex_price.IsMatch(TextBox_Product_Price.Text))) {
                Error_messege.Text = " ошибка в цене товара";
                err = -1;
            } else {
                price = float.Parse(TextBox_Product_Price.Text);
                Error_messege.Text = "";
            }
            if ((TextBox_Count_Product.Text.Length == 0) || (!regex_count.IsMatch(TextBox_Count_Product.Text))) {
                Error_messege.Text = " ошибка в кол-ве товара";
                err = -1;
            } else {
                count = int.Parse(TextBox_Count_Product.Text);
                Error_messege.Text = "";
            }
            if ((TextBox_Description.Text.Length == 0) || (!regex_name.IsMatch(TextBox_Description.Text))) {
                Error_messege.Text = " ошибка в описании товара ";
                err = -1;
            } else {
                description = TextBox_Description.Text;
                Error_messege.Text = "";
            }
            if (Name_File_Image.Text.Length == 0) {
                Error_messege.Text = "Файл не выбран";
                err = -1;
                MessageBox.Show("Недопустимые значения");
            } else { Error_messege.Text = ""; }
            if (err == 0) {
                try {     
                    SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                    connection.Open();
                    /* обновляем данные о товаре*/
                    string sqlExpression = "UPDATE Goods SET product_name = @new_product_name_value, price = @New_price_value, description = @New_description_value, " +
                        " count = @New_count_value, image = @New_image_value WHERE product_id = @product_id_value";
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlParameter product_id_param = new SqlParameter("@product_id_value", Product_id); command.Parameters.Add(product_id_param);
                    SqlParameter New_product_name_param = new SqlParameter("@New_product_name_value", product_name); command.Parameters.Add(New_product_name_param);
                    SqlParameter New_price_param = new SqlParameter("@New_price_value", price); command.Parameters.Add(New_price_param);
                    SqlParameter New_description_param = new SqlParameter("@New_description_value", ' '); command.Parameters.Add(New_description_param);
                    SqlParameter New_count_param = new SqlParameter("@New_count_value", count); command.Parameters.Add(New_count_param);
                    SqlParameter New_image_param = new SqlParameter("@New_image_value", (object)imageData); command.Parameters.Add(New_image_param);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Данные отредактированы");
                    Manager.MainFrame.GoBack();
                    Manager.MainFrame.GoBack();
                }
                catch (SqlException error) {
                    MessageBox.Show(error.Message);
                }
            }
        }
        // кнопка выбора картинки
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
            }
            catch (SqlException error) {
                MessageBox.Show(error.Message);
            }
        }

    }
}
