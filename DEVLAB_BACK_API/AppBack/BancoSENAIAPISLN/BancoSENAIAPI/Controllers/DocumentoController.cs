using BancoSENAIAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
        );

        private readonly AppDbContext _context;

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum Arquivo foi enviado.");
            }

            long limiteBytes = 2 * 1024 * 1024;

            string[] extensoesPermitidas = { ".jpg", ".pdf", ".png" };

            string extensao = Path.GetExtension(arquivo.FileName).ToLower();

            if (arquivo.Length > limiteBytes)
            {
                return BadRequest(new
                {
                    mensagem = "Arquivo não anexado! O arquivo excede o limite máximo de armazenamento de 2 MB."
                });
            }

            if (!extensoesPermitidas.Contains(extensao))
            {
                return BadRequest(new
                {
                    mensagem = "Formato de arquivo não suportado. Envie um arquivo PDF, JPG ou PNG."
                });
            }

            string pastaCliente = Path.Combine(
                _caminhoRaiz,
                codigoCliente.ToString()
            );

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);

            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";

            string caminhoFinal = Path.Combine(
                pastaCliente,
                novoNome
            );

            using (var stream = new FileStream(
                caminhoFinal,
                FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new Models.DocumentoMetadado
            {
                Nome = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            _context.Documento.Add(documentoMetadados);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Documento anexado com sucesso",
                arquivoSalvo = novoNome,
                id = documentoMetadados.Id
            });
        }

        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ListarArquivos(int codigoCliente)
        {
            var documentos = await _context.Documento
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToListAsync();

            if (!documentos.Any())
            {
                return NotFound("Nenhum arquivo encontrado");
            }

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadArquivos(int id)
        {
            var documento = await _context.Documento.FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Arquivo não encontrado.");
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound("Arquivo físico não encontrado.");
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(documento.Caminho);

            return File(
                fileBytes,
                "application/octet-stream",
                documento.Nome + documento.Extensao
            );
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> ExcluirArquivo(int id)
        {
            var documento = await _context.Documento
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Arquivo não encontrado.");
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _context.Documento.Remove(documento);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Documento Excluído com Sucesso"
            });
        }
    }
}