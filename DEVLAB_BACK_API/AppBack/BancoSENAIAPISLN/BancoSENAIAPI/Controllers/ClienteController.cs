using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var clientes = await _context.Cliente.ToListAsync();
            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Cliente novoCliente)
        {
            // Validações dos campos obrigatórios
            if (string.IsNullOrWhiteSpace(novoCliente.NomeCliente))
            {
                return BadRequest(new { message = "O nome do cliente é obrigatório." });
            }

            if (string.IsNullOrWhiteSpace(novoCliente.Cpf))
            {
                return BadRequest(new { message = "O CPF é obrigatório." });
            }

            // Verifica se o CPF já está cadastrado
            if (await _context.Cliente.AnyAsync(c => c.Cpf == novoCliente.Cpf))
            {
                return BadRequest(new { message = "Este CPF já está cadastrado." });
            }

            // Aplica o padrão caso o número da agência não seja informado ou seja inválido
            if (novoCliente.NumeroAgencia <= 0)
            {
                novoCliente.NumeroAgencia = 10;
            }

            _context.Cliente.Add(novoCliente);
            await _context.SaveChangesAsync();
            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            return Ok(cliente);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Alterar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            if (string.IsNullOrWhiteSpace(clienteAtualizado.NomeCliente))
            {
                return BadRequest(new { message = "O nome do cliente é obrigatório." });
            }

            if (string.IsNullOrWhiteSpace(clienteAtualizado.Cpf))
            {
                return BadRequest(new { message = "O CPF é obrigatório." });
            }

            var clienteExistente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (clienteExistente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            // Atualização dos campos
            clienteExistente.NomeCliente = clienteAtualizado.NomeCliente;
            clienteExistente.Cpf = clienteAtualizado.Cpf;
            clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia <= 0 ? 10 : clienteAtualizado.NumeroAgencia;
            clienteExistente.SaldoTotal = clienteAtualizado.SaldoTotal;
            /*clienteExistente.Sexo = clienteAtualizado.Sexo;
            clienteExistente.Endereco = clienteAtualizado.Endereco;
            clienteExistente.cidade = clienteAtualizado.cidade;
            clienteExistente.estado = clienteAtualizado.estado;*/

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cliente excluído com sucesso." });
        }
    }
}