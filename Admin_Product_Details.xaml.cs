/*Admin_Product_Details страница потробностей о товаре для администратора*/
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
namespace CourseWork {
    public partial class Admin_Product_Details : Page {
        public string ConnectionString;
        public class catalog_goods {
            public byte[] image { get; set; }
        }
        public int Product_id;
        public Admin_Product_Details(int product_id, string connectionString) {
            InitializeComponent();
            ConnectionString = connectionString;
            Product_id = product_id;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            List<catalog_goods> goods_list = new List<catalog_goods>();
            /*вывод данных о товаре*/
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
        /* кнопка редактинрования для администратора*/
        private void Admin_Goods_Edit_Button(object sender, RoutedEventArgs e) {
            Manager.MainFrame.Navigate(new Admin_Goods_Edit(Product_id, ConnectionString));
        }
        /* кнопка удаления товара для администратора*/
        private void Admin_Goods_Del_Button(object sender, RoutedEventArgs e) {
            Button button = sender as Button; 
            string sqlExpression;
            SqlCommand command;
            SqlParameter product_id_param;
            if (button != null) {
                catalog_goods good = button.DataContext as catalog_goods; 
                SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                /* удаление товара из каталога*/
                sqlExpression = "DELETE FROM Pharmacy.dbo.Basket WHERE product_id = @product_id_value DELETE FROM Pharmacy.dbo.Goods WHERE product_id = @product_id_value";
                command = new SqlCommand(sqlExpression, connection);
                product_id_param = new SqlParameter("@product_id_value", Product_id); command.Parameters.Add(product_id_param);
                command.ExecuteNonQuery();
                MessageBox.Show("Товар удален");
                Manager.MainFrame.GoBack();
            }
        }
    }
}
