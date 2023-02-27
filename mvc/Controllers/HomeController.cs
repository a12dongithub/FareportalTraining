using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using patientmgmt.Models;

namespace patientmgmt.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    public static ACE42023Context db = new();
    [Route("/Index.html")]
    public IActionResult Index()
    {
        if(HttpContext.Session.GetString("UserId")!=null){
            ViewBag.DocData = HttpContext.Session.GetString("UserId");
            int v = 0;
            foreach(var item in db.PatientsTables){
                if(item.City == ViewBag.DocData){
                    v=1;
                }
            }
            ViewBag.TableData = v;
        return View(db.PatientsTables.ToList());
        }
        else{
        return RedirectToAction("Login","Login");
        }
    }
    [HttpGet]
    [Route("myapi/patients", Name = "GetAllPatients")]
    public async Task<ActionResult<IEnumerable<PatientsTable>>> Patients(){
        return Ok(db.PatientsTables.ToList());
    }
    public IActionResult Remove(int id){

        foreach(var i in db.PatientsTables){
            if(i.Pid == id){
                db.PatientsTables.Remove(i);
            }
        }
        db.SaveChanges();   

        return RedirectToAction("Index");
    }
    public IActionResult New()
    {
        if(HttpContext.Session.GetString("UserId")!=null)
        return View();
        else
        return RedirectToAction("Login","Login");
    }
    [HttpPost]
    public IActionResult New(PatientsTable model)
    {
        // p.Doj = DateTime.Now;
        model.City = HttpContext.Session.GetString("UserId");
        db.PatientsTables.Add(model);
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet]
   public IActionResult Patient(){
    ViewBag.MyData = TempData["UserId"];
    return View(db.PatientsTables.ToList());
   }
//    public IActionResult Edit(int id)
// {
//     // Retrieve patient data based on the id parameter
//     var patient = db.PatientsTables.Find(id);
    
//     // Check if the patient exists
//     if (patient == null)
//     {
//         return NotFound();
//     }
    
//     // Return a view with a form for editing the patient data
//     return View(patient);
// }
public IActionResult Getpatient(){
    return View();
}
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
