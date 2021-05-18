/*User_Product_Details страница подробностей о товаре для пользователя*/
using System.Collections.Generic;
using System.Windows.Controls;
using System.Data.SqlClient;
namespace CourseWork {
    public partial class User_Product_Details : Page {
        public class catalog_goods {
            public byte[] image { get; set; }
        }
        /*конструктор*/
        public User_Product_Details(int product_id, string ConnectionString) {
            InitializeComponent();
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            List<catalog_goods> goods_list = new List<catalog_goods>();
            /* вывод информации о продукте*/
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
    }
}
