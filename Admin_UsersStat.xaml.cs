/* Admin_UserStat просмотр пользователей для администратора*/
using System.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
namespace CourseWork {    
    public partial class Admin_UsersStat : Page {
        public Admin_UsersStat(string ConnectionString) {
            InitializeComponent();
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            string sqlExpression = "SELECT * FROM clients";
            SqlCommand command = new SqlCommand(sqlExpression, connection);        
            command.ExecuteNonQuery();
            SqlDataAdapter Sql_Data_Adapter = new SqlDataAdapter(command);
            DataTable Data_Table = new DataTable("clients");
            Sql_Data_Adapter.Fill(Data_Table);
            Admin_UsersStat_Grid.ItemsSource = Data_Table.DefaultView;
        }
    }
}
