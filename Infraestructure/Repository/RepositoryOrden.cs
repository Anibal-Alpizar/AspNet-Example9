using Infraestructure.Models;
using Infraestructure.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public class RepositoryOrden : IRepositoryOrden
    {
        public IEnumerable<Orden> GetOrden()
        {
            List<Orden> ordenes = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    ordenes = ctx.Orden.
                               Include("Cliente").
                               Include("Usuario").
                               ToList<Orden>();
                }
                return ordenes;

            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }

        public Orden GetOrdenByID(int id)
        {
            Orden orden = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    orden = ctx.Orden.
                               Include("Cliente").
                               Include("Usuario").
                               Include("OrdenDetalle").
                               Include("OrdenDetalle.Libro").
                               Where(p => p.IdOrden == id).
                               FirstOrDefault<Orden>();

                }
                return orden;

            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }

        public void GetOrdenCountDate(out string etiquetas, out string valores)
        {
            String varEtiquetas = "";
            String varValores = "";
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;

                    // Obtener la cantidad de ordenas por fechas
                    var resultado = ctx.Orden.GroupBy(x => x.FechaOrden).
                        Select(
                        o => new
                        {
                            count = o.Count(),
                            FechaOrden = o.Key
                        });

                    //Crear etiquetas y valores
                    foreach(var item in resultado)
                    {
                        varEtiquetas = String.Format("{0:dd/MM/yyyy}", item.FechaOrden) + ", ";
                        varValores += item.count + ", ";
                    }
                }
                //Ultima coma
                varEtiquetas = varEtiquetas.Substring(0, varEtiquetas.Length - 1); // ultima coma
                varValores = varValores.Substring(0, varValores.Length - 1);
                //Asignar valores de salida
                etiquetas = varEtiquetas;
                valores = varValores;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }

        public Orden Save(Orden pOrden)
        {
            int resultado = 0;
            Orden orden = null;
            try
            {
                // Salvar pero con transacción porque son 2 tablas
                // 1- Orden
                // 2- OrdenDetalle 
                using (MyContext ctx = new MyContext())
                {
                    using (var transaccion = ctx.Database.BeginTransaction())
                    {
                        ctx.Orden.Add(pOrden);
                        //foreach
                        resultado = ctx.SaveChanges();
                        // Commit 
                        transaccion.Commit();
                    }
                }

                // Buscar la orden que se salvó y reenviarla
                if (resultado >= 0)
                    orden = GetOrdenByID(pOrden.IdOrden);


                return orden;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }
    }
}