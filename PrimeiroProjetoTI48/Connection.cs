using System.Data;
using System.Data.SqlClient;

public class Connection
{
    private SqlConnection conn;

    string connectionString = @"Server=JUN0570952W10-1\BDSENAC; Database=BoaDB; User ID=senaclivre; Password=senaclivre;";

    public SqlConnection Connect()
    {
        conn = new SqlConnection(connectionString);

        if (conn.State == ConnectionState.Closed)
            conn.Open();

        return conn;
    }

    public void Disconnect()
    {
        if (conn != null && conn.State == ConnectionState.Open)
            conn.Close();
    }
}