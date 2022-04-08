using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace kiosko.Helpers
{
    public class AuthorizationService
    {
        IConfiguration configuration;

        public AuthorizationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Boolean CheckAuthorization(String paramAuthorization)
        {
            String userAuthorization = this.configuration["userAuthorization"];
            String passwordAuthorization = this.configuration["passwordAuthorization"];
            Console.WriteLine(userAuthorization);
            Console.WriteLine(passwordAuthorization);
            // Get authorization key
            var authHeaderRegex = new Regex(@"Basic (.*)");

            if (!authHeaderRegex.IsMatch(paramAuthorization))
            {
                Console.WriteLine("no coinciden");
                return false;
            }
            var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(paramAuthorization, "$1")));
            var authSplit = authBase64.Split(Convert.ToChar(":"), 2);
            var authUsername = authSplit[0];
            var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Ingrese la contraseña");

            Console.WriteLine(authUsername);
            Console.WriteLine(authPassword);

            if (authUsername != userAuthorization || authPassword != passwordAuthorization)
            {
                Console.WriteLine("no coinciden if 2");
                return false;
            }
            else
            {
                Console.WriteLine("coinciden");
                return true;
            }
        }
    }
    
}
