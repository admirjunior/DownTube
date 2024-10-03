using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using NReco.VideoConverter;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace YoutubeDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var youtube = new YoutubeClient();
            var converter = new FFMpegConverter();

            Console.WriteLine("Deseja baixar arquivos do YouTube (s/n)?");
            var resposta = Console.ReadLine().ToLower();

            if (resposta == "s")
            {
                // Baixar arquivos do YouTube
                await BaixarArquivosYoutube(youtube, converter);
            }
            else if (resposta == "n")
            {
                // Limpar arquivos temporários
                LimparArquivosTemporarios();
            }
            else
            {
                Console.WriteLine("Resposta inválida. O programa será encerrado.");
            }

            Console.WriteLine("Pressione qualquer tecla para sair.");
            Console.ReadKey();
        }

        static async Task BaixarArquivosYoutube(YoutubeClient youtube, FFMpegConverter converter)
        {
            // Solicita a letra do drive
            Console.WriteLine("Digite a letra do drive onde deseja salvar os arquivos (ex: E):");
            string driveLetter = Console.ReadLine().Trim().ToUpper() + ":\\";

            // Solicita o nome da pasta
            Console.WriteLine("Digite o nome da pasta onde deseja salvar os arquivos (ou deixe em branco para salvar na raiz do drive):");
            string folderName = Console.ReadLine().Trim();

            // Define o diretório de saída
            string outputDirectory = string.IsNullOrEmpty(folderName) ? driveLetter : Path.Combine(driveLetter, folderName);

            // Verifica se o diretório existe, se não, cria-o
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                Console.WriteLine($"O diretório '{outputDirectory}' foi criado com sucesso.");
            }

            List<string> failedDownloads = new List<string>();

            bool continuar = true;
            while (continuar)
            {
                Console.WriteLine("Cole a URL do vídeo do YouTube ou da playlist (ou digite 'sair' para encerrar):");
                string url = Console.ReadLine();

                if (url.ToLower() == "sair")
                {
                    continuar = false;
                    continue;
                }

                Console.WriteLine("Deseja baixar o vídeo (v) ou apenas o áudio (a)?");
                var opcao = Console.ReadLine().ToLower();

                try
                {
                    if (IsPlaylistUrl(url))
                    {
                        var playlistId = PlaylistId.Parse(url);
                        var playlist = await youtube.Playlists.GetAsync(playlistId);
                        var videos = await youtube.Playlists.GetVideosAsync(playlistId);

                        Console.WriteLine($"Playlist '{playlist.Title}' encontrada. Iniciando o download dos vídeos...");

                        foreach (var playlistVideo in videos)
                        {
                            var video = await youtube.Videos.GetAsync(playlistVideo.Id);
                            if (opcao == "v")
                            {
                                if (!await DownloadVideo(youtube, outputDirectory, video))
                                {
                                    failedDownloads.Add($"{video.Title} ({video.Url})");
                                }
                            }
                            else if (opcao == "a")
                            {
                                if (!await DownloadAndConvertVideo(youtube, converter, outputDirectory, video))
                                {
                                    failedDownloads.Add($"{video.Title} ({video.Url})");
                                }
                            }
                        }

                        if (failedDownloads.Any())
                        {
                            Console.WriteLine("Alguns vídeos não puderam ser baixados ou convertidos. Consulte o arquivo log.txt para mais informações.");
                            File.WriteAllLines(Path.Combine(outputDirectory, "log.txt"), failedDownloads);
                        }
                        else
                        {
                            Console.WriteLine("Todos os vídeos da playlist foram baixados com sucesso.");
                            File.WriteAllText(Path.Combine(outputDirectory, "log.txt"), "Todos os arquivos foram baixados com sucesso!");
                        }
                    }
                    else
                    {
                        var video = await youtube.Videos.GetAsync(url);
                        if (opcao == "v")
                        {
                            if (!await DownloadVideo(youtube, outputDirectory, video))
                            {
                                failedDownloads.Add($"{video.Title} ({video.Url})");
                            }
                        }
                        else if (opcao == "a")
                        {
                            if (!await DownloadAndConvertVideo(youtube, converter, outputDirectory, video))
                            {
                                failedDownloads.Add($"{video.Title} ({video.Url})");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro ao baixar/converter o vídeo: {ex.Message}");
                }
            }
        }

        static async Task<bool> DownloadAndConvertVideo(YoutubeClient youtube, FFMpegConverter converter, string outputDirectory, Video video)
        {
            string outputFile = Path.Combine(outputDirectory, SanitizeFileName($"{video.Title}.mp3"));
            string tempOutputFile = Path.Combine(outputDirectory, SanitizeFileName($"{video.Title}_temp.mp3"));

            var streamInfoSet = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var audioStreams = streamInfoSet.GetAudioOnlyStreams().OrderByDescending(s => s.Bitrate);
            var streamInfo = audioStreams.FirstOrDefault();

            if (streamInfo != null)
            {
                await youtube.Videos.Streams.DownloadAsync(streamInfo, tempOutputFile);
                converter.ConvertMedia(tempOutputFile, outputFile, "mp3");
                Console.WriteLine($"'{video.Title}' foi baixado e convertido para MP3 com sucesso.");

                // Remove o arquivo temporário
                File.Delete(tempOutputFile);

                return true;
            }
            else
            {
                Console.WriteLine($"Não foi possível encontrar um fluxo de áudio para o vídeo '{video.Title}'.");
                return false;
            }
        }

        static async Task<bool> DownloadVideo(YoutubeClient youtube, string outputDirectory, Video video)
        {
            string outputFile = Path.Combine(outputDirectory, SanitizeFileName($"{video.Title}.mp4"));

            var streamInfoSet = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var streamInfo = streamInfoSet.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).FirstOrDefault();

            if (streamInfo != null)
            {
                await youtube.Videos.Streams.DownloadAsync(streamInfo, outputFile);
                Console.WriteLine($"'{video.Title}' foi baixado com sucesso.");
                return true;
            }
            else
            {
                Console.WriteLine($"Não foi possível encontrar um fluxo de vídeo para o vídeo '{video.Title}'.");
                return false;
            }
        }

        static void LimparArquivosTemporarios()
        {
            Console.WriteLine("Digite a letra do drive onde estão os arquivos temporários:");
            string driveLetter = Console.ReadLine().Trim().ToUpper() + ":\\";

            // Solicita o nome da pasta
            Console.WriteLine("Digite o nome da pasta onde estão os arquivos temporários:");
            string folderName = Console.ReadLine().Trim();

            // Define o diretório de saída
            string tempDirectory = string.IsNullOrEmpty(folderName) ? driveLetter : Path.Combine(driveLetter, folderName);

            // Verifica se o diretório existe
            if (Directory.Exists(tempDirectory))
            {
                string[] arquivosTemporarios = Directory.GetFiles(tempDirectory, "*_temp.mp3");
                foreach (var arquivo in arquivosTemporarios)
                {
                    File.Delete(arquivo);
                }
                Console.WriteLine("Arquivos temporários removidos com sucesso.");
            }
            else
            {
                Console.WriteLine($"O diretório '{tempDirectory}' não existe.");
            }
        }

        static bool IsPlaylistUrl(string url)
        {
            return url.ToLower().Contains("playlist");
        }

        static string SanitizeFileName(string fileName)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c.ToString(), "");
            }
            return fileName;
        }
    }
}
