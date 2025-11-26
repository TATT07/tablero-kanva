namespace Kanban.Core.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; }     // 👈 ESTO ES LO QUE ASP.NET ESPERA
        public string Password { get; set; }  // 👈 ESTO TAMBIÉN
    }
}
