using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSolicitudes.Data;
using SistemaSolicitudes.Models;
using System.Security.Cryptography;
using System.Text;

namespace SistemaSolicitudes.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contraseña)
        {
            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contraseña))
            {
                ModelState.AddModelError("", "Nombre de usuario y contraseña son requeridos.");
                return View();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

            if (usuario == null || !VerificarContraseña(contraseña, usuario.Contraseña))
            {
                ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos.");
                return View();
            }

            // Iniciar sesión (guardar en sesión)
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
            HttpContext.Session.SetString("Rol", usuario.Rol);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);

            // Redirigir según rol
            if (usuario.Rol == "Admin")
            {
                return RedirectToAction("Index", "Solicitudes");
            }
            else
            {
                return RedirectToAction("Index", "Solicitudes");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private bool VerificarContraseña(string contraseña, string contraseñaHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash == contraseñaHash;
            }
        }
    }
}