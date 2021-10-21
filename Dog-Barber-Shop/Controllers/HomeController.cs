using Dog_Barber_Shop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using static Dog_Barber_Shop.Models.QueueViewModel;

namespace Dog_Barber_Shop.Controllers
{
    [Route("Home")]
    public class HomeController : Controller
    {
        SqlCommand com = new SqlCommand();        
        SqlConnection con = new SqlConnection();
        List<QueueViewModel> queues = new List<QueueViewModel>();
        SomeViewModel model = new SomeViewModel();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            con.ConnectionString = Dog_Barber_Shop.Properties.Resources.ConnectionString;
        }

        [Route("")]
        [Route("index")]
        [Route("~/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route ("CustomerDetails")]
        [HttpPost]
        public JsonResult CustomerDetails(string id)
        {
            int id2 = Convert.ToInt32(id);
            QueueViewModel customer = new QueueViewModel();
            List<QueueViewModel> GetCustomer =  getCustomer(id2);
            customer.customerName = GetCustomer.FirstOrDefault().customerName;
            customer.queueTime = GetCustomer.FirstOrDefault().queueTime;
            customer.creationQueueTime = GetCustomer.FirstOrDefault().creationQueueTime;
            return Json(customer);
        }

        public List<QueueViewModel> getCustomer(int? id)
        {
            List<QueueViewModel> customers = new List<QueueViewModel>();
            DataTable dtG = new DataTable();
            using (SqlConnection con = new SqlConnection(Dog_Barber_Shop.Properties.Resources.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    cmd.CommandText = "SELECT Customers.CustomerID, Customers.CName, CustomersQueue.QueueTime,CustomersQueue.CreationTime FROM Customers Right JOIN CustomersQueue ON Customers.CustomerID = CustomersQueue.CustomerID where Customers.CustomerID = @Id ";
                    cmd.Parameters.AddWithValue("@Id", id);            
                    con.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dtG);
                    foreach (DataRow row in dtG.Rows)
                    {
                        customers.Add(new QueueViewModel()
                        {
                            customerID = Convert.ToInt32(row["CustomerID"]),
                            customerName = row["CName"].ToString(),
                            queueTime = Convert.ToDateTime(row["QueueTime"]),
                            creationQueueTime = Convert.ToDateTime(row["CreationTime"])
                        });
                    }
                    con.Close();
                }
            }

            return customers;
        }
    

    public void GetAllCustomers()
        {
            DataTable dt1 = new DataTable();
            com.Connection = con;
            com.CommandText = "SELECT Customers.CustomerID, Customers.CName, CustomersQueue.QueueTime,CustomersQueue.CreationTime FROM Customers RIGHT JOIN CustomersQueue ON Customers.CustomerID = CustomersQueue.CustomerID";
            SqlDataAdapter adapter = new SqlDataAdapter(com);
            adapter.Fill(dt1);
            foreach (DataRow row in dt1.Rows)
            {
                QueueViewModel q = new QueueViewModel();
                q.customerID = Convert.ToInt32(row["CustomerID"]);
                q.customerName = Convert.ToString(row["CName"]);
                q.queueTime = Convert.ToDateTime(row["QueueTime"]);
                q.creationQueueTime = Convert.ToDateTime(row["QueueTime"]);


                model.samples.Add(q);
            }         
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {

            DataTable dt = new DataTable();

            try
            {
                 using SqlConnection con1 = new SqlConnection();
                con1.ConnectionString = Dog_Barber_Shop.Properties.Resources.ConnectionString;
                SqlCommand command = new SqlCommand("Login");
                command.Connection = con1;
                command.CommandType = CommandType.StoredProcedure;
                //command.Connection = con;
                command.CommandTimeout = 0;
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                con1.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
                con1.Close();
            }
            catch (Exception ex)
            {

            }

            if (dt.Rows[0].ItemArray[0].ToString() != "-1")
            {
                HttpContext.Session.SetString("username", username);
                GetAllCustomers();
                ViewData["QueueViewModel"] = queues;
                return View("Success", model);
            }

            else
            {
                ViewBag.error = "Invalid Account";
                return View("Index");
            }

        }


        [Route("register")]
        [HttpPost]
        public IActionResult Register(string username, string password, string name)
        {

            DataTable dt = new DataTable();

            try
            {
                using SqlConnection con1 = new SqlConnection();
                con1.ConnectionString = Dog_Barber_Shop.Properties.Resources.ConnectionString;
                SqlCommand command = new SqlCommand("Register");
                command.Connection = con1;
                command.CommandType = CommandType.StoredProcedure;
                //command.Connection = con;
                command.CommandTimeout = 0;
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@name", name);
                con1.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
                con1.Close();
            }
            catch (Exception ex)
            {

            }

            if (dt.Rows[0].ItemArray[1].ToString() == username)
            {
                HttpContext.Session.SetString("username", username);
                GetAllCustomers();
                ViewData["QueueViewModel"] = queues;
                return View("Success", model);
            }

            else
            {
                ViewBag.error = "Username and password already exists";
                return View("Index");
            }

            //if (username != null && password != null && username.Equals("acc1") && password.Equals("123"))
            //{
            //    HttpContext.Session.SetString("username", username);
            //    FetchData();
            //    ViewData["QueueViewModel"] = queues;
            //    return View("Success", model);
            //}
            //else
            //{
            //    ViewBag.error = "Invalid Account";
            //    return View("Index");
            //}
        }
        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
