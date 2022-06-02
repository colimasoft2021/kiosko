using kiosko.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;

namespace kiosko.Controllers
{
    public class ReportesController : Controller
    {
        private readonly KioskoCmsContext _context;
        private readonly IWebHostEnvironment _env;

        public ReportesController(KioskoCmsContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;

        }

        // GET: ReportesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReportesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReportesController/Create
        public ActionResult Create()
        {
            return View();
        }

        public IActionResult getFilter()
        {
            var comisionistas = GetComisionistas();
            string stringComisionistas = JsonConvert.SerializeObject(comisionistas.Value);
            var enumComisionistas = JsonConvert.DeserializeObject<IEnumerable<ComisionistasApi>>(stringComisionistas);
            var datosSelect = new SelectComisionistas();
            foreach (var com in enumComisionistas)
            {
                var datosCom = new DataComisionista0();
                datosCom.Id = com.iD_Comisionista;
                datosCom.Nombre = com.nombre_Comisionista;
               
                var existCom = datosSelect.comisionistas.FirstOrDefault(o => o.Id == com.iD_Comisionista);
                if (existCom == null)
                {
                    datosSelect.comisionistas.Add(datosCom);
                }
                var datosTien = new TiendasComisionista();
                datosTien.Id = com.tienda;
                var existTienda = datosCom.tiendas.FirstOrDefault(o => o.Id == com.tienda);
                if (existTienda == null)
                {
                   // datosSelect.comisionistas.Add(datosCom);
                   datosCom.tiendas.Add(datosTien);
                }
                foreach(var emp in com.empleados)
                {
                    datosTien.Nombre = emp.nombreTienda;
                    if(datosTien.Id == com.tienda)
                    {
                        datosCom.tiendas.Add(datosTien);
                    }
                    var datosEmp = new Empleado();
                    datosEmp.Nombre = emp.nombre+" "+emp.apellidos;
                    datosEmp.Id = emp.id_Empleado;
                    var existEmp = datosTien.empleados.FirstOrDefault(o => o.Id == emp.id_Empleado);
                    //if (datosEmp.Id == emp.id_Empleado)
                    if(existEmp == null)
                    {
                        datosTien.empleados.Add(datosEmp);
                    }
                }
            }
            return Ok(datosSelect);
        }

        [HttpPost]
        public IActionResult getFilterData()
        {
            var message = new { status = "", message = "" };
            IActionResult ret = null;
            try
            {
                var idEmpleado = Request.Form["IdEmpleado"];
                var empleados = Request.Form["Empleados"];
                var arrayEmpleados = empleados.ToString();
                Console.WriteLine(arrayEmpleados);
                var empleadosInt = arrayEmpleados.Split(',');
                Console.WriteLine(empleadosInt);
                var results = new Usuarios();

                foreach (var emp in empleadosInt)
                {
                    Console.WriteLine(emp);
                    var dataUser = new Usuario();
                    var progreso = _context.Usuarios.Where(u => u.IdUsuario == Int32.Parse(emp))
                    .Include(p => p.Progresos)
                    .ThenInclude(m => m.IdModuloNavigation).AsNoTracking().FirstOrDefault();
                    if (progreso != null)
                    {
                        dataUser.Progresos = progreso.Progresos;
                        dataUser.Id = progreso.Id;
                        dataUser.IdUsuario = progreso.IdUsuario;
                        dataUser.NombreUsuario = progreso.NombreUsuario;
                        dataUser.Clave = progreso.Clave;
                        dataUser.Rol = progreso.Rol;
                        dataUser.Email = progreso.Email;
                        results.usuarios.Add(progreso);
                        Console.WriteLine(progreso);
                    }
                    
                }
                ret = StatusCode(StatusCodes.Status200OK, results);
            }
            catch (Exception ex)
            {
                message = new { status = "error", message = ex.Message };
                ret = StatusCode(StatusCodes.Status500InternalServerError, message);
            }
            return ret;
        }

        // POST: ReportesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReportesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReportesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GetComisionistas()
        {
            var enpointUrl = "https://accesossicom.mikiosko.mx/api/Usuarios/getcomisionistas";
            var username = "SicomAcess";
            var password = "$1c0om007";
            List<ComisionistasApi> dataComisionistas = new List<ComisionistasApi>();
            try
            {
                string authEncoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                    .GetBytes(username + ":" + password));
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(enpointUrl);
                httpWebRequest.Headers.Add("Authorization", "Basic " + authEncoded);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dataComisionistas = JsonConvert.DeserializeObject<List<ComisionistasApi>>(result);
                }
                return Json(dataComisionistas);
            }
            catch (Exception ex)
            {
                var message = new { status = "error", message = ex.Message };
                return Json(message);
            }
        }
    }
    public class ComisionistasApi
    {
        public ComisionistasApi()
        {
            empleados = new HashSet<ArrayEmpleado>();
        }
        public string correo { get; set; }
        public string tienda { get; set; }
        public string iD_Comisionista { get; set; }
        public string nombre_Comisionista { get; set; }
        public virtual ICollection<ArrayEmpleado> empleados { get; set; }
    }
    public class ArrayEmpleado
    {
        public int id_Empleado { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string nombreTienda { get; set; }
        public int tienda { get; set; }

    }
    //public class ComisionistasRes
    //{
    //    public ComisionistasRes()
    //    {
    //        empleados = new HashSet<dataEmp>();
    //    }
    //    public string correo { get; set; }
    //    public string tienda { get; set; }
    //    public string iD_Comisionista { get; set; }
    //    public string nombre_Comisionista { get; set; }
    //    public virtual ICollection<dataEmp> empleados { get; set; }
    //}
    //public class dataEmp
    //{
    //    public int id_Empleado { get; set; }
    //    public string nombre { get; set; }
    //    public string apellidos { get; set; }
    //    public string nombreTienda { get; set; }
    //    public int tienda { get; set; }
    //}
    public class filtros
    {
        public string idComisionista { get; set; }
        public int tienda { get; set; }
        public int id_Empleado { get; set; }
    }
    public class SelectComisionistas
    {
        public SelectComisionistas()
        {
            comisionistas = new HashSet<DataComisionista0>();
        }
        public virtual ICollection<DataComisionista0> comisionistas { get; set; }
    }
    public class DataComisionista0
    {
        public DataComisionista0()
        {
            tiendas = new HashSet<TiendasComisionista>();
        }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection <TiendasComisionista> tiendas { get; set; }
    }

    public class TiendasComisionista
    {
        public TiendasComisionista()
        {
            empleados = new HashSet<Empleado>();
        }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection<Empleado> empleados { get; set; }
    }

    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class Usuarios
    {
        public Usuarios()
        {
            usuarios = new HashSet<Usuario>();
        }
        public virtual ICollection<Usuario> usuarios { get; set; }
    }
}
