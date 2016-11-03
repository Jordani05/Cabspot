using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Cabspot.Models;

namespace Cabspot.Controllers
{
    [Authorize]
    public class clientesController : ApiController
    {
        private CabspotDB db = new CabspotDB();

        // GET: api/clientes
        public IQueryable<clientes> Getclientes()
        {
            return db.clientes;
        }

        // GET: api/clientes/5
        [ResponseType(typeof(clientes))]
        public async Task<IHttpActionResult> Getclientes(int id)
        {
            clientes clientes = await db.clientes.FindAsync(id);
            if (clientes == null)
            {
                return NotFound();
            }

            return Ok(clientes);
        }

        // PUT: api/clientes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putclientes(int id, clientes clientes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != clientes.idCliente)
            {
                return BadRequest();
            }

            db.Entry(clientes).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!clientesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/clientes
        [ResponseType(typeof(clientes))]
        public async Task<IHttpActionResult> Postclientes(clientes clientes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.clientes.Add(clientes);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = clientes.idCliente }, clientes);
        }

        // DELETE: api/clientes/5
        [ResponseType(typeof(clientes))]
        public async Task<IHttpActionResult> Deleteclientes(int id)
        {
            clientes clientes = await db.clientes.FindAsync(id);
            if (clientes == null)
            {
                return NotFound();
            }

            db.clientes.Remove(clientes);
            await db.SaveChangesAsync();

            return Ok(clientes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool clientesExists(int id)
        {
            return db.clientes.Count(e => e.idCliente == id) > 0;
        }
    }
}