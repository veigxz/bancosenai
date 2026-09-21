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
        document.getElementById('codigoClienteBusca').value = codigoCliente;
        listarArquivos()
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

    corpoTable.innerHTML = ''; 

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

async function baixarArquivo(id) {
    

   try {
        window.open(`${URL_API}/download/${id}`, "_blank");
    } catch (error) {
        alert("Erro ao baixar o documento.");
    }
}

async function excluirArquivo(id) {
    if (!confirm("Tem certeza que deseja excluir este documento?")) {
        return;
    }

    try {
        const response = await fetch(`${URL_API}/excluir/${id}`, {
            method: "DELETE"
        });

        if (response.ok) {
            alert("Documento excluído com sucesso!");
            listarArquivos();
        } else {
            const erro = await response.text();
            console.error("Erro da API:", response.status, erro);
            alert(`Erro ao excluir: ${response.status}`);
        }
    } catch (error) {
        console.error("Erro ao excluir:", error);
        alert("Erro ao conectar com o servidor.");
    }
}
