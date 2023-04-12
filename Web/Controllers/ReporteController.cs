using ApplicationCore.Services;
using Infraestructure.Models;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;
using Web.Utils;

namespace Web.Controllers
{
    public class ReporteController : Controller
    {
        public ActionResult Index()
        {
            return View("LibroCatalogo");
        }
        public ActionResult LibroCatalogo()
        {
            IEnumerable<Libro> lista = null;
            try
            {
                IServiceLibro _ServiceLibro = new ServiceLibro();
                lista = _ServiceLibro.GetLibro();
                return View(lista);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData.Keep();
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        /// <summary>
        /// https://riptutorial.com/itext
        /// Nugget iText7
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Reportes)]
        public ActionResult CreatePdfLibroCatalogo()
        {
            //Ejemplos IText7 https://kb.itextpdf.com/home/it7kb/examples
            IEnumerable<Libro> lista = null;
            try
            {
                // Extraer informacion
                IServiceLibro _ServiceLibro = new ServiceLibro();
                lista = _ServiceLibro.GetLibro();

                // Crear stream para almacenar en memoria el reporte 
                MemoryStream ms = new MemoryStream();
                //Inicializar writer
                PdfWriter writer = new PdfWriter(ms);

                //Inicializar document
               

                // Crear tabla con 5 columnas 
                

                    // Agregar datos a las celdas
                   

                    // Convierte la imagen que viene en Bytes en imagen para PDF
              

                // Colocar número de páginas
               


                //Terminar document
                
                // Retorna un File
                return View();

            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData.Keep();
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

        }
    }
}