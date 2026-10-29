
using Microsoft.AspNetCore.Mvc;
using DSW_3P_PR01.Models;
using DSW_3P_PR01.Services;

namespace DSW_3P_PR01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MisionesController : ControllerBase
    {
        private readonly IMisionService _service;

        public MisionesController(IMisionService service)
        {
            _service = service;
        }

        // GET: api/misiones
        [HttpGet]
        public ActionResult<List<Mision>> ObtenerTodas()
        {
            return Ok(_service.ObtenerTodas());
        }

        // GET: api/misiones/1
        [HttpGet("{id:int}")]
        public ActionResult<Mision> ObtenerPorId(int id)
        {
            Mision? mision = _service.ObtenerPorId(id);

            if (mision == null)
            {
                return NotFound(new
                {
                    mensaje = "La misión no existe."
                });
            }

            return Ok(mision);
        }

        // POST: api/misiones
        [HttpPost]
        public ActionResult<Mision> Crear([FromBody] Mision mision)
        {
            try
            {
                Mision nuevaMision = _service.Crear(mision);

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = nuevaMision.Id },
                    nuevaMision
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // PUT: api/misiones/1
        [HttpPut("{id:int}")]
        public ActionResult Actualizar(
            int id,
            [FromBody] Mision mision)
        {
            try
            {
                bool actualizado = _service.Actualizar(id, mision);

                if (!actualizado)
                {
                    return NotFound(new
                    {
                        mensaje = "La misión no existe."
                    });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // DELETE: api/misiones/1
        [HttpDelete("{id:int}")]
        public ActionResult Eliminar(int id)
        {
            bool eliminado = _service.Eliminar(id);

            if (!eliminado)
            {
                return NotFound(new
                {
                    mensaje = "La misión no existe."
                });
            }

            return NoContent();
        }
    }
}
