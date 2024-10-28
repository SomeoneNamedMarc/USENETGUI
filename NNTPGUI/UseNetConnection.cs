using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.IO;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Security.Policy;

    namespace NNTPGUI
    {

        public class UseNetConnection : IUseNet
        {

            private TcpClient _tcpClient;
            private NetworkStream? _ns;
            private StreamReader? _reader;
            private StreamWriter? _writer;
            private bool _isConnected;
            public bool IsConnected()
            {
                return _isConnected; 
            }

            public UseNetConnection()
            {
                _tcpClient = new TcpClient();
            }

            public async Task<bool> ConnectAsync(string DNS, int Port, string Username, string Password)
            {
                try
                {
                    string? Response;
                    IPAddress IP = Dns.GetHostEntry(DNS).AddressList[0];
                    IPEndPoint Endpoint = new IPEndPoint(IP, Port);

                    await _tcpClient.ConnectAsync(Endpoint);
                    _ns = _tcpClient.GetStream();
                    _reader = new StreamReader(_ns);//, Encoding.UTF8);
                    _writer = new StreamWriter(_ns);//, Encoding.UTF8);

                    if (!(Response = await _reader.ReadLineAsync()).Contains("200"))
                    {
                        Console.WriteLine("Error in establishing a connection");
                        return false;
                    }

                    await _writer.WriteLineAsync($"AUTHINFO USER {Username}\r\n");
                    _writer.Flush();

                    if (!(Response = await _reader.ReadLineAsync()).Contains("381"))
                    {
                        Console.WriteLine("Error in Username");
                        return false;
                    }

                    await _writer.WriteLineAsync($"AUTHINFO PASS {Password}\r\n");
                    _writer.Flush();

                    if (!(Response = await _reader.ReadLineAsync()).Contains("281"))
                    {
                        Console.WriteLine("Error in Password");
                        return false;
                    }
                
                    _isConnected = true;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                    return false;
                }
            }
            public Task Disconnect()
            {
                if (_isConnected)
                {
                    _writer?.Dispose();
                    _reader?.Dispose();
                    _ns?.Dispose();
                    _tcpClient?.Close();
                    _isConnected = false;
                    Console.WriteLine("Disconnected from the server.");
                }
                return Task.CompletedTask;
            }

            public async Task<ObservableCollection<String>> RetrieveGroupListAsync()
            {
                var siteList = new ObservableCollection<String>();
                string? line;

                await _writer.WriteLineAsync($"LIST\r\n");
                _writer.Flush();

                if (!(line = await _reader.ReadLineAsync()).StartsWith("215"))
                {
                    return siteList;
                }

                while (true)
                {
                    line = await _reader.ReadLineAsync();
                    if (line == "." || line == null)
                    {
                        return siteList;
                    }
                    siteList.Add(line.Substring(0, line.IndexOf(" ")));
                }

            }

            public async Task<ObservableCollection<String>> RetrieveArticleListAsync(string Site)
            {
                var ArticleList = new ObservableCollection<String>();
                string? line;

                await _writer.WriteLineAsync($"GROUP {Site}\r\n");
                _writer.Flush();

                if (!(line = await _reader.ReadLineAsync()).StartsWith("211"))
                {
                    return ArticleList;
                }


                await _writer.WriteLineAsync($"XOVER\r\n");
                _writer.Flush();


                if ((line = await _reader.ReadLineAsync()).StartsWith("224"))
                {
                    while (true)
                    {
                        line = await _reader.ReadLineAsync();
                        if (line == "." || line == null)
                        {
                            return ArticleList;
                        }
                        ArticleList.Add(line.Substring(0, line.IndexOf(" ")));
                    }
                }
                return ArticleList;
            }

            public async Task<ObservableCollection<String>> RetrieveArticleAsync(string ArticleID)
            {
                var Article = new ObservableCollection<String>();
                string? line;

                await _writer.WriteLineAsync($"ARTICLE {ArticleID}\r\n");
                _writer.Flush();

                line = await _reader.ReadLineAsync();
                if (line.StartsWith("220"))
                {
                    while ((line = await _reader.ReadLineAsync()) != null && line != ".")
                    {

                        Article.Add(line);
                    }
                }
                return Article;
            }
        }
    }
