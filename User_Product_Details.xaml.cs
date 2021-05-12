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

using System.Data.SqlClient;
using System.IO;

namespace CourseWork {
    public partial class User_Product_Details : Page {

        public class catalog_goods {
            public byte[] image { get; set; }
        }

        public User_Product_Details(int product_id) {
            InitializeComponent();

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();

            List<catalog_goods> goods_list = new List<catalog_goods>();

            string sqlExpression = "SELECT * FROM Goods WHERE product_id=@product_id_value";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlParameter product_id_param = new SqlParameter("@product_id_value", product_id); command.Parameters.Add(product_id_param);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {

                    catalog_goods st = new catalog_goods();
                    TextBlock_Product_Name.Text = reader.GetString(1);
                    //st.description = reader.GetValue(2).ToString();
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
