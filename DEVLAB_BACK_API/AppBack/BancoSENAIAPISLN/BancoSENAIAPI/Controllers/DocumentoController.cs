using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
        );

        private static List<Models.DocumentoMetadado> _documentosMetadados = new List<Models.DocumentoMetadado>();

        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if(arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum Arquivo foi enviado.");
            }

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }
            string extensao = Path.GetExtension(pastaCliente);
            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new Models.DocumentoMetadado
            {
                Id = _nextId,
                Name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };

            _documentosMetadados.Add(documentoMetadados);

            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });

        }

        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ListarArquivos(int codigoCliente)
        {

            var documentos = _documentosMetadados.Where(d => d.CodigoCliente == codigoCliente).ToList();    
            
            if(!documentos.Any() || documentos.Count == 0)
            {
                return NotFound("Nenhum arquivo encontrado");
            }

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadArquivos(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound("Arquivo não encontrado.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(documento.Caminho);

            return File(fileBytes, "application/octet-stream", documento.Name);
        }
    }
}