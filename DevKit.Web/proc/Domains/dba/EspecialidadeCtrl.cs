﻿using DataModel;
using LinqToDB;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;

namespace DevKit.Web.Controllers
{
	public class EspecialidadeController : ApiControllerBase
	{
        public IHttpActionResult Get()
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var filter = new EspecialidadeFilter
            {
                skip = Request.GetQueryStringValue("skip", 0),
                take = Request.GetQueryStringValue("take", 15),
            };

            return Ok(new Especialidade().ComposedFilters(db, filter));                        
        }
            
        public IHttpActionResult Get(long id)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = db.Empresa.Where(y => y.id == id).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);
            
            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

        public IHttpActionResult Post(Especialidade mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Create(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            return Ok(mdl);
        }

        public IHttpActionResult Put(long id, Especialidade mdl)
		{
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            if (!mdl.Update(db, ref apiError))
                return BadRequest(apiError);

            mdl.LoadAssociations(db);

            return Ok();			
		}
        
	}
}
