using ApplicationCore.Services;
using Infraestructure.Models;
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
    public class LibroController : Controller
    {
        // GET: Libro
        public ActionResult Index()
        {
            IEnumerable<Libro> lista = null;
            try
            {
                IServiceLibro _ServiceLibro = new ServiceLibro();
                lista = _ServiceLibro.GetLibro();
                ViewBag.title = "Lista Libros";
                //Lista autores
                IServiceAutor _ServiceAutor = new ServiceAutor();
                ViewBag.listaAutores = _ServiceAutor.GetAutor();
                return View(lista);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;

                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }
        public ActionResult buscarLibroxNombre(string filtro)
        {
            IEnumerable<Libro> lista = null;
            IServiceLibro _ServiceLibro = new ServiceLibro();

            //Texto vacío
            if (string.IsNullOrEmpty(filtro))
            {
                lista = _ServiceLibro.GetLibro();
            }
            else
            {
                lista = _ServiceLibro.GetLibroByNombre(filtro);
            }
            return PartialView("_PartialViewLibroAdmin", lista);
        }
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult IndexAdmin()
        {
            IEnumerable<Libro> lista = null;
            try
            {
                IServiceLibro _ServiceLibro = new ServiceLibro();
                lista = _ServiceLibro.GetLibro();
                //Lista autocompletado de autores
                ViewBag.listaNombres = _ServiceLibro.GetLibroNombres();
                return View(lista);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;

                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }
        public PartialViewResult librosxAutor(int? id)
        {
            //Contenido a actualizar
            IEnumerable<Libro> lista = null;
            IServiceLibro _ServicioLibro = new ServiceLibro();
            if (id != null) {
                lista = _ServicioLibro.GetLibroByAutor((int)id);
                //Convert.ToInt32(id)
            }
            //Nombre de vista parcial, datos para la vista
            return PartialView("_PartialViewLibro", lista);
        }

        // GET: Libro/Details/5
        public ActionResult Details(int? id)
        {
            ServiceLibro _ServiceLibro = new ServiceLibro();
            Libro libro = null;

            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                libro = _ServiceLibro.GetLibroByID(Convert.ToInt32(id));
                if (libro == null)
                {
                    TempData["Message"] = "No existe el libro solicitado";
                    TempData["Redirect"] = "Libro";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "IndexAdmin";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        // GET: Libro/Create
        public ActionResult Create()
        {
            //Que recursos necesito para crear un libro

            //Autor
            ViewBag.IdAutor = listaAutores();
            ViewBag.IdCategoria = listaCategorias();
            //Categorías

            return View();
        }
        // GET: Libro/Edit/5
        public ActionResult Edit(int? id)
        {

            // Que recursos necesito para actualizar un libro
            ServiceLibro _ServiceLibro = new ServiceLibro();
            Libro libro = null;
            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("IndexAdmin");
                }
                //Libro 
                libro = _ServiceLibro.GetLibroByID(Convert.ToInt32(id));
                if (libro == null)
                {
                    TempData["Message"] = "No existe el libro solicitado";
                    TempData["Redirect"] = "Libro";
                    TempData["Redirect-Action"] = "IndexAdmin";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }

                //Autor
                ViewBag.IdAutor = listaAutores(libro.IdAutor);

                //Categorías
                ViewBag.IdCategoria = listaCategorias(libro.Categoria);
                return View(libro);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "IndexAdmin";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

        }

        // POST: Libro/Create
        [HttpPost]
        public ActionResult Save(Libro libro, HttpPostedFileBase ImageFile, string[] selectedCategorias)
        {
            MemoryStream target = new MemoryStream();
            IServiceLibro _ServiceLibro = new ServiceLibro();
            try
            {
                // Cuando es Insert Image viene en null porque se pasa diferente
                if (libro.Imagen == null)
                {
                    if (ImageFile != null)
                    {
                        ImageFile.InputStream.CopyTo(target);
                        libro.Imagen = target.ToArray();
                        ModelState.Remove("Imagen");
                    }

                }
                if (ModelState.IsValid)
                {
                    Libro oLibroI = _ServiceLibro.Save(libro, selectedCategorias);
                }
                else
                {
                    // Valida Errores si Javascript está deshabilitado
                    Utils.Util.ValidateErrors(this);
                    ViewBag.IdAutor = listaAutores(libro.IdAutor);
                    ViewBag.IdCategoria = listaCategorias(libro.Categoria);
                    return View("Create", libro);
                }

                return RedirectToAction("IndexAdmin");
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "IndexAdmin";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }
        private SelectList listaAutores(int idAutor = 0)
        {
            IServiceAutor _ServiceAutor = new ServiceAutor();
            IEnumerable<Autor> listaAutores = _ServiceAutor.GetAutor();
            return new SelectList(listaAutores, "IdAutor", "Nombre", idAutor);
        }
        private MultiSelectList listaCategorias(ICollection<Categoria> categorias = null)
        {
            IServiceCategoria _ServiceCategoria = new ServiceCategoria();
            IEnumerable<Categoria> listaCategorias = _ServiceCategoria.GetCategoria();
            //Selecionar las categorias / Modificar
            int[] listaCategoriasSelect = null;
            if (categorias != null)
            {
                listaCategoriasSelect = categorias.Select(c => c.IdCategoria).ToArray();
            }

            return new MultiSelectList(listaCategorias, "IdCategoria", "Nombre", listaCategoriasSelect);
        }


        // GET: Libro/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Libro/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
