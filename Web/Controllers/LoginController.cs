using ApplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Utils;

namespace Web.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //public ActionResult Index(Usuario usuario)
        public ActionResult Login(Usuario usuario)
        {
            IServiceUsuario _ServiceUsuario = new ServiceUsuario();
            Usuario oUsuario = null;
            try
            {
                ModelState.Remove("Nombre");
                ModelState.Remove("Apellidos");
                ModelState.Remove("IdRol");
                if (ModelState.IsValid)
                {
                    oUsuario = _ServiceUsuario.GetUsuario(usuario.Email, usuario.Password);
                    if (oUsuario != null)
                    {
                        Session["User"] = oUsuario;
                        Log.Info($"Accede{oUsuario.Nombre} {oUsuario.Apellidos} " +
                            $"con el rol {oUsuario.Rol.IdRol}");
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Login",
                            "Usuario autenticado", Util.SweetAlertMessageType.success);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Log.Warn($"Intento de inicio de secion{usuario.Email}");
                        ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Login",
                            "Usuario no válido", Util.SweetAlertMessageType.warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;

                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
            return View("Index");
        }
    public ActionResult UnAuthorized()
        {
            ViewBag.Message = "No autorizado";
            if (Session["User"] != null)
            {
                Usuario usuario = Session["User"] as Usuario;
                Log.Warn($"No autorizado {usuario.Email}");
            }
            return View();
        }
    public ActionResult Logout()
    {
        try
        {
            Session["User"] = null;
            return RedirectToAction("Index","Login");
        }
        catch (Exception ex)
        {
            Log.Error(ex, MethodBase.GetCurrentMethod());
            TempData["Message"] = "Error al procesar los datos! " + ex.Message;

            // Redireccion a la captura del Error
            return RedirectToAction("Default", "Error");
        }
    }
    }
}