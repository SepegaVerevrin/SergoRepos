/*Страница корзины админа и клиента */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.IO;
namespace CourseWork {
    public partial class Admin_Basket : Page {
        public string ConnectionString;
        public class basket_goods {
            public int product_id { get; set; }
            public string client_id { get; set; }
            public int count_goods { get; set; }
            public string product_name { get; set; }
            public byte[] image { get; set; }
            public string price { get; set; }
        }
        public class operation_goods {
            public string client_id { get; set; }
            public string date_time { get; set; }
            public int product_id { get; set; }
            public int count { get; set; }
        }
        public class price_of_goods {
            public string price { get; set; }
        }
        public double final_price;
        public static string tmp { get; set; }
        /*вывод товаров корзины*/
        public void Write_Admin_Basket(string client_id) {
            Admin_Basket_ListView.ItemsSource = null;
            tmp = client_id;
            string sqlExpression;
            SqlCommand command;
            SqlParameter client_id_param;
            SqlDataReader reader;
            List<basket_goods> goods_list = new List<basket_goods>();
            final_price = 0;
            try {
                SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                /* получаем итогувую цену */
                sqlExpression = "SELECT dbo.Basket.count_goods, dbo.Goods.price, dbo.Goods.price * dbo.Basket.count_goods AS Expr1 FROM dbo.Basket INNER JOIN  dbo.Goods ON dbo.Basket.product_id = dbo.Goods.product_id WHERE(dbo.Basket.client_id = @client_id_value)";
                command = new SqlCommand(sqlExpression, connection);
                client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                reader = command.ExecuteReader();
                if (reader.HasRows) {
                    while (reader.Read()) {
                        price_of_goods og = new price_of_goods();
                        og.price = reader.GetValue(2).ToString();
                        final_price += double.Parse(og.price);
                    }
                }
                reader.Close();
                Final_price_textblock.Text = String.Format("{0:C}", final_price.ToString() + " руб");
                /* получаем данные о товарах в корзине */
                sqlExpression = "SELECT dbo.basket.product_id, dbo.basket.count_goods, dbo.Goods.product_name, dbo.Goods.image, dbo.Goods.price FROM dbo.Basket INNER JOIN dbo.Goods ON dbo.basket.product_id = dbo.Goods.product_id WHERE client_id = @client_id_value";
                command = new SqlCommand(sqlExpression, connection);
                client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                reader = command.ExecuteReader();
                if (reader.HasRows) {
                    while (reader.Read()) {
                        basket_goods st = new basket_goods();
                        st.product_id = reader.GetInt32(0);
                        st.client_id = client_id;
                        st.count_goods = reader.GetInt32(1);
                        st.product_name = reader.GetString(2);
                        st.image = reader[3] as byte[];
                        st.price = reader.GetValue(4).ToString(); 
                        goods_list.Add(st);
                    }
                    Admin_Basket_ListView.ItemsSource = goods_list;
                    reader.Close();
                } else {
                    reader.Close();
                }
            }
            catch (SqlException er) {
                MessageBox.Show(er.Message);
            }
        }
        /*конструктор*/
        public Admin_Basket(string client_id, string connectionString) {
            InitializeComponent();
            ConnectionString = connectionString;
            tmp = client_id;            
            Write_Admin_Basket(client_id);
        }
        /* кнопка удаления товаров из корзину */
        private void Delete_Goods_Button(object sender, RoutedEventArgs e) {
            string client_id = tmp;
            Button button = sender as Button; // !!!
            string sqlExpression;
            SqlCommand command;
            SqlParameter client_id_param;
            try {
                SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                basket_goods good = button.DataContext as basket_goods; // !!!
                if (good != null) {
                    /* удаляем товар из корзины*/
                    sqlExpression = "Delete FROM Basket WHERE client_id=@client_id_value and product_id=@product_id_value";
                    command = new SqlCommand(sqlExpression, connection);
                    client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                    SqlParameter product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                    SqlParameter count_goods_param = new SqlParameter("@count_goods_value", good.count_goods); command.Parameters.Add(count_goods_param);
                    command.ExecuteNonQuery();
                } else { MessageBox.Show("Выберите элемент в таблице"); }
            }
            catch (SqlException er) {
                MessageBox.Show(er.Message);
            }
            Write_Admin_Basket(client_id);
        }
        /* Увеличить кол-во товаров в корзине */
        private void AddProd(object sender, RoutedEventArgs e) {
            string client_id = tmp;
            Button button = sender as Button; 
            string sqlExpression;
            SqlCommand command;
            SqlParameter client_id_param;
            SqlParameter product_id_param;
            int count_of_product_depot=0;
            int count_of_product_basket = 0;
            try {
                SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                basket_goods good = button.DataContext as basket_goods; // !!!
                if (good != null) {
                    /*получаем кол-во товаров на складе*/
                    sqlExpression = "SELECT [count] FROM Goods WHERE product_id = @product_id_value";
                    command = new SqlCommand(sqlExpression, connection);
                    product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows) { 
                        while(reader.Read()) {
                            count_of_product_depot = reader.GetInt32(0);

                        }
                    }
                    reader.Close();
                    /*получаем кол-во товаров в корзине*/
                    sqlExpression = "SELECT count_goods FROM Basket Where client_id = @client_id_value AND product_id = @product_id_value";
                    command = new SqlCommand(sqlExpression, connection);
                    client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                    product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                    reader = command.ExecuteReader();
                    if (reader.HasRows) {
                        while (reader.Read()) {
                            count_of_product_basket = reader.GetInt32(0);
                        }
                    }
                    reader.Close();
                    if (count_of_product_depot > count_of_product_basket) {
                        sqlExpression = "UPDATE Basket SET count_goods = (count_goods + 1)  Where client_id = @client_id_value AND product_id = @product_id_value";
                        command = new SqlCommand(sqlExpression, connection);
                        client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                        product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                        command.ExecuteNonQuery();
                    } else MessageBox.Show("Товар кончился");
                } else { MessageBox.Show("Выберите элемент в таблице"); }
            } catch (SqlException er) {
                MessageBox.Show(er.Message);
            }
            Write_Admin_Basket(client_id);
        }
        /* уменьшаем кол-во товаров в корзине*/
        private void SubProd(object sender, RoutedEventArgs e) {
            string client_id = tmp;
            Button button = sender as Button;
            string sqlExpression;
            SqlCommand command;
            SqlParameter client_id_param;
            try {
                SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                basket_goods good = button.DataContext as basket_goods; 
                if (good != null) {
                    /*уменьшаем кол-во товара в корзине*/
                    sqlExpression = "UPDATE Basket SET count_goods = (count_goods - 1)  Where client_id = @client_id_value AND product_id = @product_id_value";
                    command = new SqlCommand(sqlExpression, connection);
                    client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                    SqlParameter product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                    command.ExecuteNonQuery();
                    /*удаляем товар из корзины если кол-во менее 1*/
                    sqlExpression = "SELECT * FROM Basket WHERE  (client_id = @client_id_value) AND (count_goods <= 0) AND (product_id = @product_id_value)";
                    command = new SqlCommand(sqlExpression, connection);
                    client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                    product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows) {
                        Delete_Goods_Button(sender,e);
                    }
                } else { MessageBox.Show("Выберите элемент в таблице"); }
            }
            catch (SqlException er) {
                MessageBox.Show(er.Message);
            }
            Write_Admin_Basket(client_id);
        }




        /*Совершение покупки. Чтобы совершить операцию нам надо записать данные из таблицы Basket в таблицы Operation(1 раз) и Operations_content(несколько раз), от того и такая каша*/
        private void Operation_Add_Button(object sender, RoutedEventArgs e) {
            InitializeComponent();
            string client_id = tmp; // получаем код клиента
            string current_time = "";            
            string sqlExpression;
            SqlCommand command;
            SqlParameter product_id_param;
            SqlParameter client_id_param;
            SqlParameter date_time_param;
            SqlDataReader reader;
            final_price = 0;
            int res = 0;
            string fname = "cheque" + DateTime.Now.ToString("yyffffff") + ".txt";
            try {
                SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                // проверяем есть ли у клиента в корзине товары
                sqlExpression = "SELECT * FROM dbo.Basket WHERE(dbo.Basket.client_id = @client_id_value)";
                command = new SqlCommand(sqlExpression, connection);
                client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                reader = command.ExecuteReader();
                if (reader.HasRows) {
                    res = 0;
                } else res = -1;
                reader.Close();
                if (res == 0) {
                    // если есть, то получаем их общую стоимость
                    sqlExpression = "SELECT dbo.Basket.count_goods, dbo.Goods.price, dbo.Goods.price * dbo.Basket.count_goods AS Expr1 FROM dbo.Basket INNER JOIN  dbo.Goods ON dbo.Basket.product_id = dbo.Goods.product_id WHERE(dbo.Basket.client_id = @client_id_value)";
                    command = new SqlCommand(sqlExpression, connection);
                    client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                    reader = command.ExecuteReader();
                    if (reader.HasRows) {
                        while (reader.Read()) {
                            price_of_goods og = new price_of_goods();
                            og.price = reader.GetValue(2).ToString();
                            final_price += float.Parse(og.price);
                        }
                    }
                    reader.Close();
                    // и выбираем из корзины данные, чтобы записать в др. таблицы
                    sqlExpression = "SELECT * FROM Basket WHERE client_id = @client_id_value";
                    command = new SqlCommand(sqlExpression, connection);
                    client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                    reader = command.ExecuteReader();
                    if (reader.HasRows) {
                        current_time = DateTime.Now.ToString("yyyy:dd:M:HH:mm:ss:fffffff");
                        reader.Close();
                        // записываем в таблицу операций новую операцию
                        sqlExpression = "INSERT INTO Operation (client_id, date_time, final_price) VALUES (@client_id_value, @date_time_value, @final_price_value)";
                        command = new SqlCommand(sqlExpression, connection);
                        client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                        date_time_param = new SqlParameter("@date_time_value", current_time); command.Parameters.Add(date_time_param);
                        SqlParameter final_price_param = new SqlParameter("@final_price_value", final_price); command.Parameters.Add(final_price_param);
                        command.ExecuteNonQuery();
                        reader.Close();
                        // и далее записываем каждую строку из корзины в детали операций
                        sqlExpression = "SELECT * FROM Basket WHERE client_id = @client_id_value";
                        command = new SqlCommand(sqlExpression, connection);
                        client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                        reader = command.ExecuteReader();
                        while (reader.Read()) {
                            operation_goods og = new operation_goods();
                            og.client_id = reader.GetString(0);
                            og.date_time = current_time;
                            og.product_id = reader.GetInt32(1);
                            og.count = reader.GetInt32(2);
                            reader.Close();
                            // вычитаем из таблицы товаров количество купленных товаров
                            sqlExpression = "UPDATE Goods SET [count]=([count]-(SELECT count_goods FROM Basket WHERE product_id=@product_id_value AND client_id = @client_id_value )) " +
                            "WHERE product_id = @product_id_value";
                            command = new SqlCommand(sqlExpression, connection);
                            client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                            product_id_param = new SqlParameter("@product_id_value", og.product_id); command.Parameters.Add(product_id_param);
                            command.ExecuteNonQuery();
                            // заполнаем детали заказа
                            sqlExpression = "INSERT INTO Operations_content (client_id, date_time, product_id, count) VALUES (@client_id_value, @date_time_value, @product_id_value, @count_value)";
                            command = new SqlCommand(sqlExpression, connection);
                            client_id_param = new SqlParameter("@client_id_value", og.client_id); command.Parameters.Add(client_id_param);
                            date_time_param = new SqlParameter("@date_time_value", og.date_time); command.Parameters.Add(date_time_param);
                            product_id_param = new SqlParameter("@product_id_value", og.product_id); command.Parameters.Add(product_id_param);
                            SqlParameter count_param = new SqlParameter("@count_value", og.product_id); command.Parameters.Add(count_param);
                            command.ExecuteNonQuery();
                            // удаляем из корзины
                            sqlExpression = "DELETE FROM Basket WHERE client_id = @client_id_value AND product_id=@product_id_value";
                            command = new SqlCommand(sqlExpression, connection);
                            client_id_param = new SqlParameter("@client_id_value", og.client_id); command.Parameters.Add(client_id_param);
                            product_id_param = new SqlParameter("@product_id_value", og.product_id); command.Parameters.Add(product_id_param);
                            command.ExecuteNonQuery();
                            // танцы с бубном
                            sqlExpression = "SELECT * FROM Basket WHERE client_id = @client_id_value";
                            command = new SqlCommand(sqlExpression, connection);
                            client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                            reader = command.ExecuteReader();
                        }
                        reader.Close();
                    } else {
                        reader.Close();
                    }
                    Write_Admin_Basket(client_id);
                    try { // записываем чек в файл
                        StreamWriter p = new StreamWriter(fname, append: true);
                        p.WriteLine("  Звездный Подорожничек");
                        p.WriteLine(" Чек Дата " + current_time);
                        /*получаем информацию о купленных товарах*/
                        sqlExpression = "SELECT dbo.Operations_content.client_id, dbo.Operations_content.date_time, dbo.Operations_content.product_id, dbo.Operations_content.count, dbo.Goods.product_name, dbo.Goods.price " +
                            " FROM dbo.Operations_content LEFT OUTER JOIN dbo.Goods ON dbo.Operations_content.product_id = dbo.Goods.product_id " +
                            " WHERE (dbo.Operations_content.client_id = @client_id_value) AND (dbo.Operations_content.date_time = @date_time_value)";
                        command = new SqlCommand(sqlExpression, connection);
                        client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                        date_time_param = new SqlParameter("@date_time_value", current_time); command.Parameters.Add(date_time_param);
                        reader = command.ExecuteReader();
                        if (reader.HasRows) {
                            while (reader.Read()) {
                                p.WriteLine(reader[4].ToString() + " ... " + reader[5].ToString() + " руб X " + reader[3].ToString() + " шт");
                            }  
                        }
                        reader.Close();
                        /*получаем итоговую цену*/
                        sqlExpression = "SELECT * FROM dbo.Operation WHERE (dbo.Operation.client_id = @client_id_value) AND (dbo.Operation.date_time = @date_time_value)";
                        command = new SqlCommand(sqlExpression, connection);
                        client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                        date_time_param = new SqlParameter("@date_time_value", current_time); command.Parameters.Add(date_time_param);
                        reader = command.ExecuteReader();
                        if (reader.HasRows) {
                            while (reader.Read()) {
                                p.WriteLine("   ИТОГ = " + reader[2].ToString() + " руб");
                            }
                        }
                        reader.Close();
                        p.Close();
                    } catch (Exception ex) {
                        MessageBox.Show("Во время выполнения произошла ошибка!", ex.Message);
                    }
                    MessageBox.Show("Поздравляю! Вы совершили оплату. Ваш чек сохранен в файл " + fname);
                } else MessageBox.Show("Тут слишком пусто");
            } catch (SqlException er) {
                MessageBox.Show(er.Message);
            }
        }
        // Кнопка деталей товара
        private void Details_Button(object sender, RoutedEventArgs e) {
            Button button = sender as Button; // !!!          
            basket_goods good = button.DataContext as basket_goods; // !!!
            Manager.MainFrame.Navigate(new User_Product_Details(good.product_id, ConnectionString));
        }
    }
}
