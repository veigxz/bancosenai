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
        }
    }
}