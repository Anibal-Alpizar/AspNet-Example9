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
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc);

                //Titulo
                Paragraph header = new Paragraph("Catalogo de libros").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(14)
                    .SetFontColor(ColorConstants.BLUE);
                doc.Add(header);


                // Crear tabla con 5 columnas 
                Table table = new Table(5, true);

                // Encabezados de la pagina 
                table.AddHeaderCell("ISBN");
                table.AddHeaderCell("Libro");
                table.AddHeaderCell("Autor");
                table.AddHeaderCell("Precio");
                table.AddHeaderCell("Imagen");

                foreach (var item in lista)
                {
                    // Agregar datos a las celdas
                    table.AddCell(new Paragraph(item.Isbn));
                    table.AddCell(new Paragraph(item.Nombre));
                    table.AddCell(new Paragraph(item.Autor.Nombre));
                    table.AddCell(new Paragraph(item.Precio.ToString()));

                    // Convierte la imagen que viene en Bytes en imagen para PDF
                    Image image = new Image(ImageDataFactory.Create(item.Imagen));
                    image = image.SetHeight(75).SetWidth(75);
                    table.AddCell(image);
                }
                doc.Add(table);


                // Colocar número de páginas
                int numberPages = pdfDoc.GetNumberOfPages();
                for (int i = 1; i <= numberPages; i++)
                {
                    // Texto alineado a un punto especifico
                    doc.ShowTextAligned(new Paragraph(string.Format("pagina {0} de {1}", i, numberPages)
                        ),
                        559, 826, i, TextAlignment.RIGHT, VerticalAlignment.TOP, 0
                        );
                }

                //Terminar document
                doc.Close();

                // Retorna un File
                return File(ms.ToArray(), "application/pdf", "reporte.pdf");

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