using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class CarteiraController : ControllerBase
    {

        private readonly AppDbContext _context;

        public CarteiraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var carteiras = await _context.Carteira.ToListAsync();   
            return Ok(carteiras);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Carteira novaCarteira)
        {

            if (novaCarteira.ApetiteCarteira < 0)
            {
                return BadRequest(new { message = "O valor do apetite carteira deve ser maior ou igual a zero." });
            }

            if (await _context.Carteira.AnyAsync(a => a.NumeroCarteira == novaCarteira.NumeroCarteira))
                return BadRequest(new { message = "Este número de agência já existe." });

            await _context.Carteira.AddAsync(novaCarteira);
            await _context.SaveChangesAsync();
            return Created("", novaCarteira);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var Carteira = await _context.Carteira.FirstOrDefaultAsync(a => a.NumeroCarteira == codigo);

            if (Carteira == null)
                return NotFound(new { message = "Carteira não encontrada." });

            return Ok(Carteira);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Alterar(int codigo, [FromBody] Carteira CarteiraAtualizada)
        {
           
            if (CarteiraAtualizada.ApetiteCarteira < 0)
            {
                return BadRequest(new { message = "O valor do apetite carteira deve ser maior ou igual a zero." });
            }

            var CarteiraExistente = await _context.Carteira.FirstOrDefaultAsync(a => a.NumeroCarteira == codigo);

            if (CarteiraExistente == null) return NotFound();

            CarteiraExistente.NumeroCarteira = CarteiraAtualizada.NumeroCarteira;
            CarteiraExistente.NomeCarteira = CarteiraAtualizada.NomeCarteira;
            CarteiraExistente.ApetiteCarteira = CarteiraAtualizada.ApetiteCarteira;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var Carteira = await _context.Carteira.FirstOrDefaultAsync(a => a.NumeroCarteira == codigo);

            if (Carteira == null) return NotFound();

            _context.Carteira.Remove(Carteira);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Carteira excluída com sucesso." });
        }
    }
}
