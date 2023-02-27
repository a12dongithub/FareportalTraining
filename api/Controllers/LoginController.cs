using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using patientmgmtapi.Models;
namespace patientmgmt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ACE42023Context _db;
        private readonly ISession _session;

        public LoginController(ACE42023Context db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _session = httpContextAccessor.HttpContext.Session;
        }

        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(SameerLmsUser obj)
        {
            var result = (from i in _db.SameerLmsUsers
                          where i.UserEmail == obj.UserEmail && i.Password == obj.Password
                          select i).SingleOrDefault();
            if (result != null)
            {
                _session.SetString("UserId", result.UserId);
                // TempData["UserId"] = result.UserId;
                if (result.UserName == "Doctor")
                {
                    return Ok(new { redirectUrl = "/Home/Index" });
                }
                else
                {
                    return Ok(new { redirectUrl = "/Home/Patient" });
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid email or password" });
            }
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(SameerLmsUser obj)
        {
            if (_db.SameerLmsUsers.Find(obj.UserId) != null)
            {
                throw new Exception("User ID Already Exists please choose another one!");
            }

            if (obj.UserName == "Patient")
            {
                string pid = Request.Form["patient-field-2"];
                string pstatus = Request.Form["patient-field-1"];
                string pdisease = Request.Form["patient-field-3"];
                PatientsTable ob = new PatientsTable();
                ob.Pid = Int16.Parse(pid);
                ob.Pstatus = pstatus;
                ob.Pname = obj.UserId;
                ob.Problem = pdisease;
                IDictionary<string, int> d = new Dictionary<string, int>(); ;
                foreach (var item in _db.SameerLmsUsers)
                {
                    if (item.UserName == "Doctor")
                    {
                        d[item.UserId] = 0;
                    }
                }
                foreach (var item in _db.PatientsTables)
                {
                    if (item.City != null)
                    {
                        d[item.City] += 1;
                    }
                }
                int mn = 100;
                foreach (var item in d)
                {
                    if (item.Value < mn) { mn = item.Value; }
                }
                foreach (var item in d)
                {
                    if (item.Value == mn)
                    {
                        ob.City = item.Key;
                    }
                }
                _db.PatientsTables.Add(ob);
            }
            _db.SameerLmsUsers.Add(obj);
            _db.SaveChanges();
            return Ok(new { message = "Registration successful" });
        }
    }
}
