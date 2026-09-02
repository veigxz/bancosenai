using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AgenciaController : ControllerBase
    {
        private static List<Agencia> _agencias = new List<Agencia>
        {
            new Agencia { NumeroAgencia = 1001, Cidade = "Aracaju", SiglaEstado = "SE" },
            new Agencia { NumeroAgencia = 2002, Cidade = "São Paulo", SiglaEstado = "SP" },
            new Agencia { NumeroAgencia = 3003, Cidade = "Salvador", SiglaEstado = "BA" }
        };

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_agencias);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Agencia novaAgencia)
        {
            
            if (_agencias.Any(a => a.NumeroAgencia == novaAgencia.NumeroAgencia))
                return BadRequest(new { message = "Este número de agência já existe." });

            _agencias.Add(novaAgencia);
            // Retorna Status 201 Created conforme boas práticas REST [6, 8]
            return Created("", novaAgencia);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var agencia = _agencias.FirstOrDefault(a => a.NumeroAgencia == codigo);

            if (agencia == null)
                return NotFound(new { message = "Agência não encontrada." }); // Status 404 [6, 7]

            return Ok(agencia); // Status 200 OK [6, 7]
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(int codigo, [FromBody] Agencia agenciaAtualizada)
        {
            var agenciaExistente = _agencias.FirstOrDefault(a => a.NumeroAgencia == codigo);

            if (agenciaExistente == null) return NotFound();

            agenciaExistente.NumeroAgencia = agenciaAtualizada.NumeroAgencia; //Possibilita a alteração do número da agência.
            agenciaExistente.Cidade = agenciaAtualizada.Cidade;
            agenciaExistente.SiglaEstado = agenciaAtualizada.SiglaEstado;

            // Retorna Status 204 No Content para atualizações bem-sucedidas [6, 9]
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var agencia = _agencias.FirstOrDefault(a => a.NumeroAgencia == codigo);

            if (agencia == null) return NotFound();

            _agencias.Remove(agencia);
            return Ok(new { message = "Agência excluída com sucesso." }); // Status 200 [6]
        }
    }
}
