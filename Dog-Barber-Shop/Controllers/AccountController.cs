using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Dog_Barber_Shop.Controllers
{
    public class AccountController : Controller
    {
       public IActionResult Register()
        {
            return View();
        }
    }
}
