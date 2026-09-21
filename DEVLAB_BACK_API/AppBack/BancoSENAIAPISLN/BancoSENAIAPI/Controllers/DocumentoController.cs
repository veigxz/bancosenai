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
            string extensao = Path.GetExtension(arquivo.FileName);
            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            long limiteBytes = 2 * 1024 * 1024;

            string[] extensoesPermitidas = { ".jpg", ".pdf", ".png" };

            if (arquivo.Length > limiteBytes)
            {
                return BadRequest(new { mensagem = "Arquivo não anexado! O arquivo excede o limite máximo de armazenamento de 2 MB." });
            }
            if (!extensoesPermitidas.Contains(extensao.ToLower()))
            {
                return BadRequest(new { mensagem = "Formato de arquivo não suportado. Envie um arquivo PDF, JPG ou PNG." });
            }

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
            _nextId += 1;

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

            if (documento == null)
            {
                return NotFound("Arquivo não encontrado.");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(documento.Caminho);

            return File(fileBytes, "application/octet-stream", documento.Name);
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> ExcluirArquivo(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Arquivo não encontrado.");
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _documentosMetadados.Remove(documento);


            return Ok(new {mensagem = "Documento Excluído com Sucesso" });
        }
    }
}