using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase
    {
        private static int _proximoCodigo = 3;

        public static List<Cliente> Clientes { get; set; } = new List<Cliente>
        {
            new Cliente
            {
                CodigoCliente = 1,
                NomeCliente = "João Silva",
                Cpf = "123.456.789-00",
                NumeroAgencia = 10,
                SaldoTotal = 1500,
                Sexo = "M",
                Endereço = "Rua A, 123",
                cidade = "São Paulo",
                estado = "SP"
            },
            new Cliente
            {
                CodigoCliente = 2,
                NomeCliente = "Maria Souza",
                Cpf = "987.654.321-11",
                NumeroAgencia = 10,
                SaldoTotal = 0,
                Sexo = "F",
                Endereço = "Av. B, 456",
                cidade = "Campinas",
                estado = "SP"
            }
        };

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(Clientes);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Cliente novoCliente)
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
            if (Clientes.Any(c => c.Cpf == novoCliente.Cpf))
            {
                return BadRequest(new { message = "Este CPF já está cadastrado." });
            }

            // Aplica o padrão caso o número da agência não seja informado ou seja inválido
            if (novoCliente.NumeroAgencia <= 0)
            {
                novoCliente.NumeroAgencia = 10;
            }

            // Gerando o código do cliente automaticamente pelo sistema
            novoCliente.CodigoCliente = _proximoCodigo++;

            Clientes.Add(novoCliente);
            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var cliente = Clientes.FirstOrDefault(c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            return Ok(cliente);
        }

        [HttpPut("{codigo}")]
        public IActionResult Alterar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            if (string.IsNullOrWhiteSpace(clienteAtualizado.NomeCliente))
            {
                return BadRequest(new { message = "O nome do cliente é obrigatório." });
            }

            if (string.IsNullOrWhiteSpace(clienteAtualizado.Cpf))
            {
                return BadRequest(new { message = "O CPF é obrigatório." });
            }

            var clienteExistente = Clientes.FirstOrDefault(c => c.CodigoCliente == codigo);

            if (clienteExistente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            // Atualização dos campos
            clienteExistente.NomeCliente = clienteAtualizado.NomeCliente;
            clienteExistente.Cpf = clienteAtualizado.Cpf;
            clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia <= 0 ? 10 : clienteAtualizado.NumeroAgencia;
            clienteExistente.SaldoTotal = clienteAtualizado.SaldoTotal;
            clienteExistente.Sexo = clienteAtualizado.Sexo;
            clienteExistente.Endereço = clienteAtualizado.Endereço;
            clienteExistente.cidade = clienteAtualizado.cidade;
            clienteExistente.estado = clienteAtualizado.estado;

            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var cliente = Clientes.FirstOrDefault(c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            Clientes.Remove(cliente);
            return Ok(new { message = "Cliente excluído com sucesso." });
        }
    }
}