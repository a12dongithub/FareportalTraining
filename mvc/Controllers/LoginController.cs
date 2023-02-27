using Microsoft.AspNetCore.Mvc;
using patientmgmt.Models;
using System.Linq;
namespace patientmgmt.Controllers;
public class LoginController : Controller
{
    private readonly ACE42023Context db = new(); 
    private readonly ISession session;
     public LoginController(ACE42023Context _db, IHttpContextAccessor httpContextAccessor) { 
        db = _db; 
        session = httpContextAccessor.HttpContext.Session; 
        }
    public IActionResult Login() { return View(); }
    [HttpPost] 

    public IActionResult Login(SameerLmsUser obj) { 
        var result = (from i in db.SameerLmsUsers where i.UserEmail == obj.UserEmail && i.Password == obj.Password select i).SingleOrDefault();
        if (result != null) 
        { 
        HttpContext.Session.SetString("UserId", result.UserId); 
        TempData["UserId"] = result.UserId;
        if(result.UserName == "Doctor"){
            return RedirectToAction("Index","Home");
        }
        else{
            return RedirectToAction("Patient","Home");
        }
        } 
        else return View(); 
        }
    public IActionResult Register() { return View(); }
    
    [HttpPost] 
    public IActionResult Register(SameerLmsUser obj) { 
        if( db.SameerLmsUsers.Find(obj.UserId) != null){
            throw new Exception("User ID Already Exists please choose another one!");
        }
        if(obj.UserName == "Patient"){
        string pid = Request.Form["patient-field-2"];
        string pstatus = Request.Form["patient-field-1"];
        string pdisease = Request.Form["patient-field-3"];
        PatientsTable ob = new PatientsTable();
        ob.Pid = Int16.Parse(pid);
        ob.Pstatus = pstatus;
        ob.Pname = obj.UserId;
        ob.Problem = pdisease;
        IDictionary<string, int> d = new Dictionary<string, int>();;
        foreach(var item in db.SameerLmsUsers){
            if(item.UserName == "Doctor"){
                d[item.UserId] = 0;
            }
        }
        foreach(var item in db.PatientsTables){
            if(item.City != null){
                d[item.City] +=1;
            }
        }
        int mn = 100;
        foreach(var item in d){
            if(item.Value < mn){mn = item.Value;}
        }
        foreach(var item in d){
            if(item.Value == mn){
                ob.City = item.Key;
            }
        }
        db.PatientsTables.Add(ob);
        }  
        db.SameerLmsUsers.Add(obj); 
        db.SaveChanges(); 
        return RedirectToAction("Login"); 
        }
    public IActionResult Logout() { 
        HttpContext.Session.Clear(); 
        return RedirectToAction("Login", "Login"); 
        }
}

