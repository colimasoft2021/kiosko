using kiosko.Models;
using Microsoft.AspNetCore.Mvc;

namespace kiosko.Helpers
{
    public class ErrorService
    {
        IConfiguration configuration;
        private readonly KioskoCmsContext _context;
        public ErrorService(IConfiguration configuration, KioskoCmsContext context)
        {
            this.configuration = configuration;
            _context = context;
        }

        public void SaveErrorMessage(String Bloque, String Controlador, String Methodo, String Message)
        {
            var mensaje = new Error();
            mensaje.Bloque = Bloque;
            mensaje.Controlador = Controlador;
            mensaje.Metodo = Methodo;
            mensaje.Mensaje = Message;
            if(mensaje != null)
            {
                _context.Errors.Add(mensaje);
                _context.SaveChanges();
            }
            
        }
    }
}
