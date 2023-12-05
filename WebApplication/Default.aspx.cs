using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetCustomerList_LINQ();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DisplayOrder_LINQ();
        }

        protected void DisplayOrder_LINQ()
        {
            NorthwindDataContext dc = new NorthwindDataContext();
            var ordersQuery = from o in dc.Orders
                               join c in dc.Customers 
                               on o.CustomerID equals c.CustomerID
                               where String.Equals(c.CompanyName, ListBox1.SelectedItem.Text)
                               orderby c.CustomerID ascending
                               select new { o.OrderID, o.OrderDate, o.ShippedDate, o.ShipName, o.ShipAddress, o.ShipCity, o.ShipCountry };


             GridView1.DataSource = ordersQuery;
             GridView1.DataBind();
       }

        protected void DisplayOrders_SQL()
        {
            SqlConnection dataConnection = new SqlConnection();
            try
            {
                dataConnection.ConnectionString = GetConnectionString();
                dataConnection.Open();

                string sqlString = "Select o.OrderID, o.OrderDate, o.ShippedDate, o.ShipName, o.ShipAddress, o.ShipCity, o.ShipCountry ";
                sqlString += "From Customers c ";
                sqlString += "Inner Join Orders o ";
                sqlString += "On c.CustomerID = o.CustomerID ";
                sqlString += "Where c.CompanyName = @CustomerIdParam ";
                sqlString += "Order by o.OrderID";

                SqlCommand dataCommand = new SqlCommand();
                dataCommand.Connection = dataConnection;
                dataCommand.CommandType = CommandType.Text;
                dataCommand.CommandText = sqlString;

                SqlParameter param = new SqlParameter("@CustomerIdParam", SqlDbType.VarChar, 55, "customerName");
                param.Value = ListBox1.SelectedItem.Text;
                dataCommand.Parameters.Add(param);

                SqlDataReader dataReader = dataCommand.ExecuteReader();
                GridView1.DataSource = dataReader;
                GridView1.DataBind();
                dataReader.Close();
            }
            catch (SqlException exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }
            finally
            {
                dataConnection.Close();
            }
        }

         protected void GetCustomerList_LINQ()
        {
            ListBox1.Items.Clear();
            NorthwindDataContext dc = new NorthwindDataContext();
            //var customersQuery = from c in dc.Customers
            //                     group c by c.CompanyName into g
            //                     select new { Key = g.Key, Count = g.Count() };

            //var customersQuery = from c in dc.Customers
            //                     orderby c.CompanyName ascending
            //                     select c;

            //foreach (var customer in customersQuery)
            //{
            //    ListBox1.Items.Add(customer.CompanyName);
                //ListBox1.Items.Add(customer.Key ); //customer.CompanyName);
            //}
        }

        protected void DisplayCustomers_SQL()
        {
            ListBox1.Items.Clear();
            SqlConnection dataConnection = new SqlConnection();
            try
            {
                dataConnection.ConnectionString = GetConnectionString();
                dataConnection.Open();

                string sqlString = "Select distinct CompanyName ";
                sqlString += "From Customers ";
                sqlString += "Order by CompanyName";

                SqlCommand dataCommand = new SqlCommand();
                dataCommand.Connection = dataConnection;
                dataCommand.CommandType = CommandType.Text;
                dataCommand.CommandText = sqlString;

                SqlDataReader dataReader = dataCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0))
                    {
                        ListBox1.Items.Add(dataReader.GetString(0));
                    }
                }
                dataReader.Close();
            }
            catch (SqlException exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }
            finally
            {
                dataConnection.Close();
            }
        }

        protected string GetConnectionString()
        {
            SqlConnectionStringBuilder builder;
            builder = new SqlConnectionStringBuilder();
            builder.DataSource = ".\\SQLExpress";
            builder.InitialCatalog = "Northwind";
            builder.IntegratedSecurity = true;
            return builder.ConnectionString; 
        }
    }
}
