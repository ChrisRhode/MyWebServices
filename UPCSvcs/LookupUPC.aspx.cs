using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// needed for Stream/StringReaders
using System.IO;
// added NuGet/Project Reference Newtonsoft.JSON 12.0.3 for JSON Deserialization
using Newtonsoft.Json;
// auto added?
using System.Data.SqlClient;
using Newtonsoft.Json.Serialization;
using System.Data;
using System.Runtime.InteropServices;

namespace UPCSvcs
{
    public partial class LookupUPC : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           //NameValueCollection QueryParams = Request.QueryString;
            //NameValueCollection QueryParams = new NameValueCollection();

            // QueryParams.Add("cmd", "LookupUPC");
            // QueryParams.Add("UPCValue", "897351000427");
            // var rdr = new StreamReader(Request.InputStream);
            //var rdr = new StreamReader(HttpContext.Current.Request.InputStream);
            //String content = rdr.ReadToEnd();
            //Response.Write("RcvdTest:[" + content + "]");
           // String cmd = "none";
            String cmd = Request.Form["cmd"];
            //String cmd = QueryParams["cmd"];
            //Response.Write("Cmd:" + cmd);
            //String cmd = QueryParams["cmd"];
            if (cmd == "LookupUPC")
            {
                String UPCValue = Request.Form["UPCValue"];
                //String UPCValue = Request.Form["UPCValue"];
                //Response.Write(",UPCValue:" + UPCValue);
                // see if we know about it already
                String knownDescription = lookupUPCInOurDB(UPCValue);
                //String knownDescription = "HELLOWORLD";
                if (knownDescription != "")
                {
                    sendBackKnownValue(knownDescription);
                }
                else
                {
                    sendBackPossibleValues(UPCValue);
                }

            }

        }

        private String lookupUPCInOurDB(String upcvalue)
        {

            //     Dim DBcon As New SqlClient.SqlConnection
            //Dim DBcmd As New SqlClient.SqlCommand
            //Dim DBAdapter As SqlClient.SqlDataAdapter

            //Dim CS As New cConnections

            // gds = New DataSet

            // Try
            //     DBcon.ConnectionString = CS.MainConnection
            //     DBcon.Open()
            //     DBcmd.Connection = DBcon
            //     DBcmd.CommandType = CommandType.StoredProcedure
            //     DBcmd.CommandText = "[Dice].[GetDisplay]"
            //     DBcmd.Parameters.Clear()
            //     DBcmd.Parameters.Add(New SqlClient.SqlParameter("@GameNum", gintGameNum))
            //     DBAdapter = New SqlClient.SqlDataAdapter(DBcmd)
            //     DBAdapter.TableMappings.Clear()
            //     DBAdapter.TableMappings.Add("Table", "Banknotes")
            //     DBAdapter.TableMappings.Add("Table1", "Dice")
            //     DBAdapter.TableMappings.Add("Table2", "GameStatus")
            //     DBAdapter.TableMappings.Add("Table3", "DiceInHands")
            //     DBAdapter.TableMappings.Add("Table4", "PlayerDetails")
            //     DBAdapter.Fill(gds)
            // Catch ex As Exception
            //     Throw New Exception("Data Load Failed", ex)
            // Finally
            //     If(DBcon.State <> ConnectionState.Closed) Then DBcon.Close()
            // End Try

            SqlConnection db_conn;
            SqlCommand db_cmd = new SqlCommand();
            SqlDataAdapter db_adapter;
            DataSet ds = new DataSet();

            try {
                String cs = "Data Source=tcp:sql2k802.discountasp.net;Initial Catalog=SQL2008_786647_crdata;Persist Security Info=True;User ID=xxxx;Password=yyyy";
                db_conn = new SqlConnection(cs);
                db_conn.Open();
                db_cmd.Connection = db_conn;
                db_cmd.CommandType = CommandType.StoredProcedure;
                db_cmd.CommandText = "[UPC].[LookupKnownDescription]";
                db_cmd.Parameters.Clear();
                SqlParameter p = new SqlParameter();
                p.ParameterName = "@UPCValue";
                p.Value = upcvalue;
                db_cmd.Parameters.Add(p);
                db_adapter = new SqlDataAdapter(db_cmd);
                db_adapter.TableMappings.Clear();
                db_adapter.TableMappings.Add("Table", "QueryResults");
                db_adapter.Fill(ds);

            }
            catch (Exception theError)
            {
                throw new Exception("DB Lookup failed",theError);
            }

            int theCount = ds.Tables["QueryResults"].Rows.Count;
            if (theCount != 0)
            {
                return ds.Tables["QueryResults"].Rows[0].ItemArray[0] as String;
            }
            else
            {
                return "";
            }    
               
        }

        private void sendBackKnownValue(String descriptionValue)
        {
            UPCSvcs_SendBackJSON.sendbackRoot return_json = new UPCSvcs_SendBackJSON.sendbackRoot();
            return_json.response_type = "KnownValue";
            return_json.known_description = descriptionValue;
            return_json.possible_descriptions = new List<UPCSvcs_SendBackJSON.aDescr>();

            String jsonResponse = JsonConvert.SerializeObject(return_json);
            Response.Write(jsonResponse);
        }

        private void sendBackPossibleValues(String upcvalue)
        {
           
            String urlToSend = "https://api.upcitemdb.com/prod/trial/lookup?upc=" + upcvalue;

            var possibleDescriptions = new List<string>();
            int ndx, lastNdx;
            int ndx2, lastNdx2;
          
            // ** need to check for failure, or item not found
            WebClient webClient = new WebClient();
            Stream response = webClient.OpenRead(urlToSend);
            StreamReader responseData = new StreamReader(response);
            String responseAsString = responseData.ReadToEnd();
            response.Close();
            // optional cleanup of returned JSON before deserialization
            //responseAsString = responseAsString.Replace("@", "");
            //responseAsString = responseAsString.Replace("?xml", "xmlinfo");
            //responseAsString = responseAsString.Replace("#cdata-section", "cdatasection");
            
            // hardcode the deserialization desired here
            PollWebServiceTest_UPC.UPCAPI s = JsonConvert.DeserializeObject<PollWebServiceTest_UPC.UPCAPI>(responseAsString);

            lastNdx = s.items.Count - 1;
            for (ndx = 0; ndx <= lastNdx; ndx++)
            {
                possibleDescriptions.Add(s.items[ndx].brand + " " + s.items[ndx].title + " " + s.items[ndx].size);

                lastNdx2 = s.items[ndx].offers.Count - 1;
                for (ndx2 = 0; ndx2 <= lastNdx2; ndx2++)
                {
                    possibleDescriptions.Add(s.items[ndx].offers[ndx2].title);
                }
            }
            //
            UPCSvcs_SendBackJSON.sendbackRoot return_json = new UPCSvcs_SendBackJSON.sendbackRoot();
            return_json.response_type = "PossibleValues";
            return_json.possible_descriptions = new List<UPCSvcs_SendBackJSON.aDescr>();
            lastNdx = possibleDescriptions.Count - 1;
            for (ndx = 0; ndx <= lastNdx; ndx++)
            {
                UPCSvcs_SendBackJSON.aDescr a = new UPCSvcs_SendBackJSON.aDescr();
                a.description_value = possibleDescriptions[ndx];
                return_json.possible_descriptions.Add(a);
            }
            
            String jsonResponse = JsonConvert.SerializeObject(return_json);
            Response.Write(jsonResponse);
        }
    }
}