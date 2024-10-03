# 🎵 YouTube MP3 Downloader

Um software de linha de comando em C# que permite baixar músicas do YouTube em formato MP3. O aplicativo suporta o download de vídeos individuais ou de playlists inteiras.

## 📋 Funcionalidades

- **Download de Vídeos**: Baixe vídeos do YouTube no formato MP4.
- **Download de Áudio**: Converta vídeos em áudio MP3.
- **Suporte a Playlists**: Baixe todas as músicas de uma playlist.
- **Limpeza de Arquivos Temporários**: Remova arquivos temporários criados durante o processo de download.

## ⚙️ Requisitos

- .NET 5.0 ou superior
- [YoutubeExplode](https://github.com/TylerLeonhardt/YoutubeExplode) para download de vídeos
- [NReco.VideoConverter](https://github.com/antonik/NReco.VideoConverter) para conversão de vídeos

## 🚀 Instalação

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu_usuario/youtube-mp3-downloader.git
   cd youtube-mp3-downloader
Abra o projeto no seu IDE (como Visual Studio).

Restaure as dependências:

bash
Copiar código
dotnet restore
Compile o projeto:

bash
Copiar código
dotnet build
📖 Como Usar
Execute o programa:

bash
Copiar código
dotnet run
Siga as instruções na tela:

Digite "s" para iniciar o download ou "n" para limpar arquivos temporários.
Insira a letra da unidade e o nome da pasta onde os arquivos serão salvos.
Cole a URL do vídeo ou playlist do YouTube.
Escolha entre baixar o vídeo ou apenas o áudio.
Para encerrar o programa, digite "sair".

💡 Exemplo de Uso
plaintext
Copiar código
Deseja baixar arquivos do YouTube (s/n)?
s
Digite a letra do drive onde deseja salvar os arquivos (ex: E):
E
Digite o nome da pasta onde deseja salvar os arquivos (ou deixe em branco para salvar na raiz do drive):
Musicas
Cole a URL do vídeo do YouTube ou da playlist (ou digite 'sair' para encerrar):
https://www.youtube.com/watch?v=dQw4w9WgXcQ
Deseja baixar o vídeo (v) ou apenas o áudio (a)?
a
🧹 Limpeza de Arquivos Temporários
Para remover arquivos temporários gerados durante o download:

Escolha a opção "n" quando solicitado no início.
Informe a letra da unidade e o nome da pasta onde os arquivos temporários estão localizados.
🤝 Contribuições
Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou enviar pull requests.

📄 Licença
Este projeto está licenciado sob a MIT License. Veja o arquivo LICENSE para mais detalhes.
