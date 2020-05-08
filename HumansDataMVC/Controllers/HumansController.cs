using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
// add for SQL connection, command etc.
using System.Data.SqlClient;
// add for DataSet
using System.Data;
// add so can reference existing models
using HumansDataMVC.Models;
using System.Security.Permissions;
using Microsoft.SqlServer.Server;

namespace HumansDataMVC.Controllers
{
    public class HumansController : Controller
    {
        // "globals" for this controller .. use the more specific type instead of ICollection
        List<Human> humans;
        DataSet ds;
        cConfig config;
        String gcs;

        // Constructor, currently fetches all current data every time ... later may try to cache some in Session etc.
        public HumansController()
        {
            config = new cConfig();
            gcs = config.GetConnectionString();

            SqlConnection db_conn;
            SqlCommand db_cmd = new SqlCommand();
            SqlDataAdapter db_adapter;
            ds = new DataSet();

            humans = new List<Human>();

            try
            {
                String cs = gcs;
                db_conn = new SqlConnection(cs);
                db_conn.Open();
                db_cmd.Connection = db_conn;
                db_cmd.CommandType = CommandType.StoredProcedure;
                db_cmd.CommandText = "[Humans].[GetAllHumanData]";
                db_cmd.Parameters.Clear();
                db_adapter = new SqlDataAdapter(db_cmd);
                db_adapter.TableMappings.Clear();
                db_adapter.TableMappings.Add("Table", "Humans");
                db_adapter.TableMappings.Add("Table1", "Children");
                db_adapter.Fill(ds);

            }
            catch (Exception theError)
            {
                throw new Exception("DB Lookup failed", theError);
            }

            int theCount = ds.Tables["Humans"].Rows.Count;

            if (theCount != 0)
            {
                int ndx, lastNdx;
                lastNdx = theCount - 1;
                for (ndx = 0; ndx <= lastNdx; ndx++)
                {
                    humans.Add(buildHumanObjectFromData(ndx));
                }
            }
        }
        // Utility routines

        // find the row index in Humans for a SSN
        // should really look at having a key in the dataTable instead
        int findHumanRowForSSN(string ssn)
        {
            int theCount = ds.Tables["Humans"].Rows.Count;
            int ndx;
            for (ndx = 0; ndx < theCount; ndx++)
            {
                if ((string)(ds.Tables["Humans"].Rows[ndx].ItemArray[0]) == ssn)
                {
                    return ndx;
                }
            }
            // throw exception instead
            return -1;
        }
        // given a row index into Humans datatable, return a Human object for that row
        // recurses for any children of this human
        Human buildHumanObjectFromData(int theNdx)
        {
            Human h = new Human();

            h.fakeSSN = (string)(ds.Tables["Humans"].Rows[theNdx].ItemArray[0]);
            h.legalFullName = (string)ds.Tables["Humans"].Rows[theNdx].ItemArray[1];
            h.callByFirstName = (string)(ds.Tables["Humans"].Rows[theNdx].ItemArray[2]);
            h.emailAddress = (string)(ds.Tables["Humans"].Rows[theNdx].ItemArray[3]);
            h.Children = new List<Human>();

            int childrenCount = ds.Tables["Children"].Rows.Count;
            int ndx;
            for (ndx = 0; ndx < childrenCount; ndx++)
            {
                if ((string)(ds.Tables["Children"].Rows[ndx].ItemArray[1]) == h.fakeSSN)
                {
                    int childNdx = findHumanRowForSSN((string)(ds.Tables["Children"].Rows[ndx].ItemArray[0]));
                    h.Children.Add(buildHumanObjectFromData(childNdx));
                }
            }

            return h;
        }

        // end Utility routines

        public ActionResult Index()
        {
            // change from default to pass in humans list
            return View(humans);
        }

        // GET: Details on a Human
       // ** the parameter HAS to be "id" to match routine, e.g. cannot be "ssn"
        public ActionResult Details(string id)
        {
 
            int thisHumanNdx = findHumanRowForSSN(id);
            Human h = buildHumanObjectFromData(thisHumanNdx);
            return View(h);
        }
        
        // Create And Edit.  I originally was looking to combine Edit and Create in one form, and did so using Routing, but
        //   it appeared this may be frowned upon as a practice.  To be sure, in Edit one would not
        //   want to be able to change the human's SSN, and in Create you must provide one;  also
        //   we might want some other properties set only on Create.
       [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Human h)
        {
            SqlConnection db_conn;
            SqlCommand db_cmd = new SqlCommand();

            try
            {
                String cs = gcs;
                db_conn = new SqlConnection(cs);
                db_conn.Open();
                db_cmd.Connection = db_conn;
                db_cmd.CommandType = CommandType.StoredProcedure;
                db_cmd.CommandText = "[Humans].[AddOrUpdateAHuman]";
                db_cmd.Parameters.Clear();

                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "@fakeSSN";
                p1.Value = h.fakeSSN;
                db_cmd.Parameters.Add(p1);

                SqlParameter p2 = new SqlParameter();
                p2.ParameterName = "@legalFullName";
                p2.Value = h.legalFullName;
                db_cmd.Parameters.Add(p2);

                SqlParameter p3 = new SqlParameter();
                p3.ParameterName = "@callByFirstName";
                p3.Value = h.callByFirstName;
                db_cmd.Parameters.Add(p3);

                SqlParameter p4 = new SqlParameter();
                p4.ParameterName = "@emailAddress";
                p4.Value = h.emailAddress;
                db_cmd.Parameters.Add(p4);

                db_cmd.ExecuteNonQuery();
               
                return RedirectToAction("Index");
            }
            catch (Exception theError)
            {
                throw new Exception("DB Update failed", theError);
            }
            
        }
        // this will let you pick a child from a list which are all humans 
        //   that have no parents and are not the parent from which we were invoked
        // id is the parent's SSN
        public ActionResult CreateChild(string id)
        {
            List<Human> possibleChildren = new List<Human>();
            List<string> disallowedChildrenSSNs = new List<string>();
            // remember the parent
            Session["HUMANS_MVC_PARENT_SSN"] = id;

            int ndx;
            int count = humans.Count;
            for (ndx = 0; ndx < count; ndx++)
            {
                Human h = buildHumanObjectFromData(ndx);
                if (h.fakeSSN == id)
                {
                    // this Human is the intended parent, do not add
                    disallowedChildrenSSNs.Add(h.fakeSSN);
                }
                int childCount = h.Children.Count;
                if (childCount > 0)
                {
                    foreach (Human c in h.Children)
                    {
                        disallowedChildrenSSNs.Add(c.fakeSSN);
                    }
                }

            }
        //now build list of allowed children
        for (ndx = 0; ndx < count; ndx++)
            {
                Human h = buildHumanObjectFromData(ndx);
                int s = disallowedChildrenSSNs.FindIndex(item => item == h.fakeSSN);
                if (s == -1)
                {
                    possibleChildren.Add(h);
                }
            }
            return View(possibleChildren);

        }
        //[HttpPost]
       // public ActionResult CreateChild(Human h)
        //{
        //
       // }

            public ActionResult AddChild(string id)
        {
            SqlConnection db_conn;
            SqlCommand db_cmd = new SqlCommand();

            string parentSSN = (string)(Session["HUMANS_MVC_PARENT_SSN"]);

            try
            {
                String cs = gcs;
                db_conn = new SqlConnection(cs);
                db_conn.Open();
                db_cmd.Connection = db_conn;
                db_cmd.CommandType = CommandType.StoredProcedure;
                db_cmd.CommandText = "[Humans].[AddChildToParent]";
                db_cmd.Parameters.Clear();

                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "@fakeSSN";
                p1.Value = id;
                db_cmd.Parameters.Add(p1);

                SqlParameter p2 = new SqlParameter();
                p2.ParameterName = "@parentSSN";
                p2.Value = parentSSN;
                db_cmd.Parameters.Add(p2);

                db_cmd.ExecuteNonQuery();

                return Redirect("/humans/CreateChild/" + parentSSN);
            }
            catch (Exception theError)
            {
                throw new Exception("DB Update failed", theError);
            }
        }
       
        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (id == "0")
            {
                id = (string)Session["HUMANS_MVC_PARENT_SSN"];
            }
            int thisHumanNdx = findHumanRowForSSN(id);
            Human h = buildHumanObjectFromData(thisHumanNdx);
            return View(h);
        }

        [HttpPost]
        public ActionResult Edit(Human h)
        {
            SqlConnection db_conn;
            SqlCommand db_cmd = new SqlCommand();

            try
            {
                String cs = gcs;
                db_conn = new SqlConnection(cs);
                db_conn.Open();
                db_cmd.Connection = db_conn;
                db_cmd.CommandType = CommandType.StoredProcedure;
                db_cmd.CommandText = "[Humans].[AddOrUpdateAHuman]";
                db_cmd.Parameters.Clear();

                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "@fakeSSN";
                p1.Value = h.fakeSSN;
                db_cmd.Parameters.Add(p1);

                SqlParameter p2 = new SqlParameter();
                p2.ParameterName = "@legalFullName";
                p2.Value = h.legalFullName;
                db_cmd.Parameters.Add(p2);

                SqlParameter p3 = new SqlParameter();
                p3.ParameterName = "@callByFirstName";
                p3.Value = h.callByFirstName;
                db_cmd.Parameters.Add(p3);

                SqlParameter p4 = new SqlParameter();
                p4.ParameterName = "@emailAddress";
                p4.Value = h.emailAddress;
                db_cmd.Parameters.Add(p4);

                db_cmd.ExecuteNonQuery();

                return RedirectToAction("Index");
            }
            catch (Exception theError)
            {
                throw new Exception("DB Update failed", theError);
            }
        }
        public ActionResult ChildrenList(string id)
        {
            int thisHumanNdx = findHumanRowForSSN(id);
            Human h = buildHumanObjectFromData(thisHumanNdx);
            return View(h.Children);

        }
        //pass in SSN of child, which can have only one parent, delete it from its parent
        public ActionResult DeleteChildFromParent(string id)
        {
            SqlConnection db_conn;
            SqlCommand db_cmd = new SqlCommand();

            try
            {
                String cs = gcs;
                db_conn = new SqlConnection(cs);
                db_conn.Open();
                db_cmd.Connection = db_conn;
                db_cmd.CommandType = CommandType.StoredProcedure;
                db_cmd.CommandText = "[Humans].[DeleteChildFromParent]";
                db_cmd.Parameters.Clear();

                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "@fakeSSN";
                p1.Value = id;
                db_cmd.Parameters.Add(p1);

                SqlParameter p2 = new SqlParameter("@parentSSN", SqlDbType.Char,11);
               p2.Direction = ParameterDirection.Output;
                db_cmd.Parameters.Add(p2);

                db_cmd.ExecuteNonQuery();
                String parentSSN = Convert.ToString(db_cmd.Parameters["@parentSSN"].Value);
                // should have proc send success/fail, check for return status
                // return true;
                // ** need to route back with id of parent
                return Redirect("/humans/Edit/" + parentSSN);
            }
            catch (Exception theError)
            {
                throw new Exception("DB Update failed", theError);
            }
           
        }

        
    }
}