using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace kiosko.Controllers
{
    public class ReportesController : Controller
    {
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
        public IActionResult getFilterData([FromBody] filtros value)
        {
            bool bitCom = false;
            bool bitTienda = false;
            bool bitEmp = false;

            var comisionistas = GetComisionistas();
            string stringComisionistas = JsonConvert.SerializeObject(comisionistas.Value);
            var enumComisionistas = JsonConvert.DeserializeObject<IEnumerable<ComisionistasApi>>(stringComisionistas);

            if (value.idComisionista != null && value.idComisionista != "")
                bitCom = true;
            if (value.tienda != null && value.tienda != 0)
                bitTienda = true;
            if (value.id_Empleado != null && value.id_Empleado != 0)
                bitEmp = true;

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
                foreach (var emp in com.empleados)
                {
                    datosTien.Nombre = emp.nombreTienda;
                    if (datosTien.Id == com.tienda)
                    {
                        datosCom.tiendas.Add(datosTien);
                    }
                    var datosEmp = new Empleado();
                    datosEmp.Nombre = emp.nombre + " " + emp.apellidos;
                    datosEmp.Id = emp.id_Empleado;
                    var existEmp = datosTien.empleados.FirstOrDefault(o => o.Id == emp.id_Empleado);
                    //if (datosEmp.Id == emp.id_Empleado)
                    if (existEmp == null)
                    {
                        datosTien.empleados.Add(datosEmp);
                    }
                }
            }

            if ((bitCom == true) && (bitTienda == false) && (bitEmp == false))
            {
                var filttroComisionista = datosSelect.comisionistas.Where(c => c.Id == value.idComisionista).ToList();
                return Ok(filttroComisionista);
            }

            if ((bitCom == true && bitTienda == true) && (bitEmp == false))
            {
                var filttroComisionista = datosSelect.comisionistas.Where(c => c.Id == value.idComisionista).ToList();

                var datosComSelect = new SelectComisionistas();
                foreach (var com in filttroComisionista)
                {
                    var datosComFC = new DataComisionista0();
                    datosComFC.Id = com.Id;
                    datosComFC.Nombre = com.Nombre;

                    var existCom = datosSelect.comisionistas.FirstOrDefault(o => o.Id == value.tienda.ToString());
                    if (existCom != null)
                    {
                        datosComSelect.comisionistas.Add(datosComFC);
                    }
                    var datosTienSelect = new TiendasComisionista();
                    foreach(var tien in com.tiendas)
                    {
                        datosTienSelect.Id = tien.Id;
                        //datosTienSelect.Nombre = tien.Nombre;
                        var matchTienda = datosComFC.tiendas.FirstOrDefault(o => o.Id == value.tienda.ToString());
                        if (matchTienda != null)
                        {
                            // datosSelect.comisionistas.Add(datosCom);
                            datosComFC.tiendas.Add(datosTienSelect);
                        }
                        
                    }
                    
                }

                return Ok(datosComSelect);
            }
            if ((bitCom == true && bitTienda == true) && (bitEmp == true))
            {
                var filtroC = enumComisionistas.Where(c => c.iD_Comisionista == value.idComisionista).
                    Where(e => e.tienda == value.tienda.ToString()).ToList();
                
                return Ok();
            }


            return Ok(datosSelect);
        }
        [HttpPost]
        public IActionResult getFilter1([FromBody] filtros value)
        {
            try
            {
                bool com = false;
                bool tienda = false;
                bool emp = false;
                var comisionistas = GetComisionistas();
                string stringComisionistas = JsonConvert.SerializeObject(comisionistas.Value);
                var enumComisionistas = JsonConvert.DeserializeObject<IEnumerable<ComisionistasApi>>(stringComisionistas);



                if (value.idComisionista != null && value.idComisionista != "")
                    com = true;
                if (value.tienda != null && value.tienda != 0)
                    tienda = true;
                if (value.id_Empleado != null && value.id_Empleado != 0)
                    emp = true;

                if ((com == true) && (tienda == false) && (emp == false))
                {
                    var filttroComisionista = enumComisionistas.Where(c => c.iD_Comisionista == value.idComisionista).ToList();
                    return Ok(filttroComisionista);
                }
                if ((com == true && tienda == true) && (emp == true))
                {
                    var filtroEm = enumComisionistas.Where(c => c.iD_Comisionista == value.idComisionista).
                        Where(e => e.tienda == value.tienda.ToString()).ToList().ToArray();

                    //List<ComisionistasRes> comisionists = new List<ComisionistasRes>();
                    //ComisionistasRes comisiont = new ComisionistasRes();

                    
                    //List<dataEmp> trabajadores = new List<dataEmp>();
                    //dataEmp trabajador = new dataEmp();

                    //foreach (var comisionistax in filtroEm)
                    //{
                    //    int match = 0;
                    //    foreach (var emplead in comisionistax.empleados)
                    //    {
                    //        if (emplead.tienda == value.tienda)
                    //        {
                    //            trabajador.id_Empleado = emplead.id_Empleado;
                    //            trabajador.nombre = emplead.nombre;
                    //            trabajador.apellidos = emplead.apellidos;
                    //            trabajador.nombreTienda = emplead.nombreTienda;
                    //            trabajador.tienda = emplead.tienda;
                    //            //trabajadores.Add(trabajador);
                    //            //comisiont.empleados.Add(emplead);
                    //            comisiont.empleados.Add(trabajador);
                    //            comisionists.Add(comisiont);
                    //            match++;
                    //        }
                    //    }
                    //    if (match > 0)
                    //    {
                    //        comisiont.iD_Comisionista = comisionistax.iD_Comisionista;
                    //        comisiont.nombre_Comisionista = comisionistax.nombre_Comisionista;
                    //        comisiont.correo = comisionistax.correo;
                    //        comisiont.tienda = comisionistax.tienda;
                    //        //comisiont.empleados.Add(trabajador);
                    //    }
                    //    comisionists.Add(comisiont);
                    //}
                    //return Ok(comisiont);
                    return Ok();
                }
                if ((com == true && tienda == true) && (emp == false))
                {
                    var filtroC = enumComisionistas.Where(c => c.iD_Comisionista == value.idComisionista).
                        Where(e => e.tienda == value.tienda.ToString()).ToList();

                    ////string stringEmp = JsonConvert.SerializeObject(filtroC.ToList());
                    ////var enumEmp = JsonConvert.DeserializeObject<IEnumerable<ArrayEmpleado>>(stringEmp);
                    ////var filtrot = enumEmp.Where(e => e.tienda == value.tienda).ToList();

                    //List <ComisionistasRes> arrayComisionistas = new List<ComisionistasRes>();
                    //ComisionistasRes arrayComisionista = new ComisionistasRes();
                    //List<dataEmp> empleado = new List<dataEmp>();
                    //dataEmp dataEmpleado = new dataEmp();
                    //foreach (var comicionista in filtroC)
                    //{

                    //    arrayComisionista.tienda = comicionista.tienda;
                    //    arrayComisionista.iD_Comisionista = comicionista.iD_Comisionista;
                    //    arrayComisionista.nombre_Comisionista = comicionista.nombre_Comisionista;
                    //    arrayComisionista.correo = comicionista.correo;
                    //    //arrayComisionistas.Add(arrayComisionista);

                    //    int match = 0;
                    //    foreach (var empl in comicionista.empleados)
                    //    {

                    //        if (empl.tienda == value.tienda)
                    //        {
                    //            dataEmpleado.tienda = empl.tienda;
                    //            dataEmpleado.nombreTienda = empl.nombreTienda;
                    //            dataEmpleado.tienda = empl.tienda;
                    //            dataEmpleado.nombre = empl.nombre;
                    //            dataEmpleado.apellidos = empl.apellidos;
                    //            dataEmpleado.id_Empleado = empl.id_Empleado;
                    //            //empleado.Add(dataEmpleado);
                    //            arrayComisionista.empleados.Add(empl);
                    //            match++;
                    //        }
                    //    }
                    //    //if (match > 0)
                    //    //{
                    //    //    dataEmpleado.tienda = empl.tienda;
                    //    //    dataEmpleado.nombreTienda = empl.nombreTienda;
                    //    //    dataEmpleado.tienda = empl.tienda;
                    //    //    dataEmpleado.nombre = empl.nombre;
                    //    //    dataEmpleado.apellidos = empl.apellidos;
                    //    //    dataEmpleado.id_Empleado = empl.id_Empleado;
                    //    //    //arrayComisionista.empleados.Add(dataEmpleado);
                    //    //    empleado.Add(dataEmpleado);
                    //    //    //arrayComisionista.empleados.Add(empleado);
                    //    //    //arrayComisionistas.Add(arrayComisionista);
                    //    //}
                    //    arrayComisionistas.Add(arrayComisionista);
                    //}

                    //return Ok(arrayComisionistas);
                    return Ok(filtroC);
                }
                
                else
                {
                    return Ok(enumComisionistas);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
}
