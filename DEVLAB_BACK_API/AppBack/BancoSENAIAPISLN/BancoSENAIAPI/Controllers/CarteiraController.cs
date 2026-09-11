using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private static List<Carteira> _carteiras = new List<Carteira>
        {
            new Carteira { NumeroCarteira = 0001, NomeCarteira = "Carteira01", ApetiteCarteira = 1000 },
            new Carteira { NumeroCarteira = 0002, NomeCarteira = "Carteira02", ApetiteCarteira = 1000 },
            new Carteira { NumeroCarteira = 0003, NomeCarteira = "Carteira03", ApetiteCarteira = 1000 }
        };

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_carteiras);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Carteira novaCarteira)
        {

            if (novaCarteira.ApetiteCarteira < 0)
            {
                return BadRequest(new { message = "O valor do apetite carteira deve ser maior ou igual a zero." });
            }

            if (_carteiras.Any(a => a.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new { message = "Este número de agência já existe." });

            _carteiras.Add(novaCarteira);
            return Created("", novaCarteira);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var Carteira = _carteiras.FirstOrDefault(a => a.NumeroCarteira == codigo);

            if (Carteira == null)
                return NotFound(new { message = "Carteira não encontrada." });

            return Ok(Carteira);
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(int codigo, [FromBody] Carteira CarteiraAtualizada)
        {
           
            if (CarteiraAtualizada.ApetiteCarteira < 0)
            {
                return BadRequest(new { message = "O valor do apetite carteira deve ser maior ou igual a zero." });
            }

            var CarteiraExistente = _carteiras.FirstOrDefault(a => a.NumeroCarteira == codigo);

            if (CarteiraExistente == null) return NotFound();

            CarteiraExistente.NumeroCarteira = CarteiraAtualizada.NumeroCarteira;
            CarteiraExistente.NomeCarteira = CarteiraAtualizada.NomeCarteira;
            CarteiraExistente.ApetiteCarteira = CarteiraAtualizada.ApetiteCarteira;

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var Carteira = _carteiras.FirstOrDefault(a => a.NumeroCarteira == codigo);

            if (Carteira == null) return NotFound();

            _carteiras.Remove(Carteira);
            return Ok(new { message = "Carteira excluída com sucesso." });
        }
    }
}
