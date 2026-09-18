const URL_API = 'https://localhost:7081/api/v1/Documento';

async function enviarDocumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const inputArquivo = document.getElementById("arquivo");
    const arquivo = inputArquivo.files[0];

    if(!codigoCliente || !arquivo){
        alert("Informe o código do cliente e selecione um arquivo!");
        return;
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    const response = await fetch(`${URL_API}/upload/${codigoCliente} `,{
        method: "POST",
        body: dadosArquivo
    });

    if(response.ok){
        alert("Documento enviado com sucesso!");
        document.getElementById("codigoCliente").value = "";
        document.getElementById("arquivo").value = "";
    } else{
        const erro = await response.json();
        alert("Erro: "+ (erro.message || "Falha ao enviar o arquivo!"))
    }
}

async function listarArquivos() {

    codigoClienteBusca = document.getElementById('codigoClienteBusca').value
    const response = await fetch(`${URL_API}/listar/${codigoClienteBusca}`)

    const arquivos = await response.json();
    const corpoTable = document.getElementById('corpoTabela');

    arquivos.forEach(c => {
        corpoTable.innerHTML += `
                <td>${c.id}</td>
                <td>${c.name}</td>
                <td>${c.extensao}</td>
                <td>
                    <button class="btn-baixar" onclick="baixarArquivo(${c.id})">Baixar</button>
                    <button class="btn-excluir" onclick="excluirArquivo(${c.id})">Excluir</button>
                </td>`;
    });
}