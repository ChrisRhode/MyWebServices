using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// add for SQL connection, command etc.
using System.Data.SqlClient;
using System.Data;

namespace HumansDataMVC
{
    public partial class Start : System.Web.UI.Page
    {
        cConfig config = new cConfig();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void linkStartWithSampleData_Click(object sender, EventArgs e)
        {
            SqlConnection db_conn;
            SqlCommand db_cmd = new SqlCommand();

            try
            {
                String cs = config.GetConnectionString();
                db_conn = new SqlConnection(cs);
                db_conn.Open();
                db_cmd.Connection = db_conn;
                db_cmd.CommandType = CommandType.StoredProcedure;
                db_cmd.CommandText = "[Humans].[Initialize]";
                db_cmd.Parameters.Clear();

                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "@initMode";
                p1.Value = 1;
                db_cmd.Parameters.Add(p1);

                db_cmd.ExecuteNonQuery();
            }
            catch (Exception theError)
            {
                throw new Exception("DB Init failed", theError);
            }
            Response.Redirect(config.GetMVCRoot(), true);
        }

        protected void linkStartWithEmptyDBClick(object sender, EventArgs e)
        {
            SqlConnection db_conn;
            SqlCommand db_cmd = new SqlCommand();

            try
            {
                String cs = config.GetConnectionString();
                db_conn = new SqlConnection(cs);
                db_conn.Open();
                db_cmd.Connection = db_conn;
                db_cmd.CommandType = CommandType.StoredProcedure;
                db_cmd.CommandText = "[Humans].[Initialize]";
                db_cmd.Parameters.Clear();

                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "@initMode";
                p1.Value = 2;
                db_cmd.Parameters.Add(p1);

                db_cmd.ExecuteNonQuery();

                
            }
            catch (Exception theError)
            {
                throw new Exception("DB Init failed", theError);
            }
            Response.Redirect(config.GetMVCRoot(), true);
        }
    }
}