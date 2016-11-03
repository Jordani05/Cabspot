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
    public class basesAPIController : ApiController
    {
        private CabspotDB db = new CabspotDB();

        // GET: api/bases
        public IEnumerable<bases> Getbases()
        {
            if (db.bases.Count() > 0)
            {
                return db.bases.ToList();
            }

            return null;
        }

        // GET: api/bases/5
        [ResponseType(typeof(bases))]
        public async Task<IHttpActionResult> Getbases(int id)
        {
            bases bases = await db.bases.FindAsync(id);
            if (bases == null)
            {
                return NotFound();
            }

            return Ok(bases);
        }

        // PUT: api/bases/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putbases(int id, bases bases)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bases.idBase)
            {
                return BadRequest();
            }

            db.Entry(bases).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!basesExists(id))
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

        // POST: api/bases
        [ResponseType(typeof(bases))]
        public async Task<IHttpActionResult> Postbases(bases bases)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.bases.Add(bases);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = bases.idBase }, bases);
        }

        // DELETE: api/bases/5
        [ResponseType(typeof(bases))]
        public async Task<IHttpActionResult> Deletebases(int id)
        {
            bases bases = await db.bases.FindAsync(id);
            if (bases == null)
            {
                return NotFound();
            }

            db.bases.Remove(bases);
            await db.SaveChangesAsync();

            return Ok(bases);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool basesExists(int id)
        {
            return db.bases.Count(e => e.idBase == id) > 0;
        }
    }
}