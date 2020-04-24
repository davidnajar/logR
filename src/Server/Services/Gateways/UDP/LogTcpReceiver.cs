using logR.Server.Services.Interfaces;
using logR.Shared;
using logR.Shared.Gateways.Files;
using Microsoft.Extensions.Options;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace logR.Server.Services.Gateways.UDP
{
    public class LogTcpReceiver
    {
        private readonly LogTcpSettings _settings;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogProcessor _logProcessor;
        XmlNamespaceManager _xmlNamespaceManager;
        NameTable _nameTable;
        XmlParserContext _context;
        Socket _clientSocket;
        XmlReaderSettings _xmlReaderSettings;
        XmlSchema xsd;
        public LogTcpReceiver(IOptions<LogTcpSettings> settings, CancellationTokenSource cancellationToken, ILogProcessor logProcessor)
        {
            _nameTable = new NameTable();
            _xmlNamespaceManager = new XmlNamespaceManager(_nameTable);
            _context = new XmlParserContext(_nameTable, _xmlNamespaceManager, null, XmlSpace.None);
            _xmlReaderSettings = new XmlReaderSettings();
            _xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
            _xmlReaderSettings.IgnoreWhitespace = true;
            _xmlReaderSettings.IgnoreComments = true;
            _xmlReaderSettings.IgnoreProcessingInstructions = true;
            _xmlReaderSettings.CheckCharacters = false;
            _xmlNamespaceManager.AddNamespace("log4net", "ns");
            _settings = settings.Value;
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            xsd = GetSchema();
            _cancellationToken = cancellationToken.Token;
            _logProcessor = logProcessor;
            Task.Factory.StartNew(InitSocket);
        }
        void Setup()
        {

        }

        private async Task InitSocket()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                Socket clientSocket=null;
                try
                {
                  clientSocket=  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    await clientSocket.ConnectAsync(_settings.Server, _settings.Port);
                do
                {
                        byte[] array = new byte[8192];
                        Memory<byte> memory = new Memory<byte>(array);
                        await clientSocket.ReceiveAsync(memory, SocketFlags.None, _cancellationToken);
                        ArraySegment<byte> segment = default;
                        bool leased = false;
                        try
                        {
                            if (!MemoryMarshal.TryGetArray<byte>(memory, out segment))
                            {
                                var arr = ArrayPool<byte>.Shared.Rent(memory.Length);
                                memory.CopyTo(arr);
                                segment = new ArraySegment<byte>(arr, 0, memory.Length);
                                leased = true;
                            }
                            using (var ms = new MemoryStream(segment.Array, segment.Offset, segment.Count))
                            {
                                var settings = new XmlReaderSettings();
                                settings.Schemas.Add(xsd);
                                settings.ValidationType = ValidationType.None;
                                using (var xmlReader = XmlReader.Create(ms, _xmlReaderSettings, _context))
                                {
                              
                                    var le = ParseLog4NetXmlLogEvent(xmlReader, "eds");
                                    if (le != null)
                                    {
                                        foreach (var log in le)
                                        {
                                            await _logProcessor.ProcessLog(log);

                                        }
                                    }

                                }
                            }
                        }
                        finally
                        {
                            if (leased) ArrayPool<byte>.Shared.Return(segment.Array);
                        }
                    } while (!_cancellationToken.IsCancellationRequested);
              


                }
                catch (Exception ex)
                {
                    clientSocket?.Dispose();
                    //reconnect
                }
            }
        }
        public  IList<LogEvent> ParseLog4NetXmlLogEvent(XmlReader reader, string defaultLogger)
        {
            List<LogEvent> logs = new List<LogEvent>();
            try
            {
                
               LogEvent logMsg = new LogEvent();
             
                while(reader.Read())
                {
                    try
                    {

                        if (reader.Name == "log4net:event")
                        {
                            logMsg = new LogEvent();
                         

                            logMsg.Logger = reader.GetAttribute("logger");
                            logMsg.Severity = GetLogLevel(reader.GetAttribute("level"));
                            if (DateTime.TryParse(reader.GetAttribute("timestamp"), out var timeStamp))
                            {
                                logMsg.Time = timeStamp;

                            }
                            else
                            {
                                logMsg.Time = DateTime.Now;
                            }


                        }
                        if (reader.Name == "log4net:message")
                        {
                            logMsg.Message = reader.ReadElementContentAsString();
                            logs.Add(logMsg);

                        }
                        if (reader.Name == "log4net:exception")
                        {
                            logMsg.Exception = reader.ReadElementContentAsString();
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }




               
            }
            catch (Exception ex)
            {
         
                
            }
            return logs;
        }
        Severity GetLogLevel(string level)
        {
            if (level == "DEBUG")
                return Severity.Debug;
            if (level=="INFO")
            {
                return Severity.Info;
            }
            if (level == "WARN") return Severity.Warn;
            if (level == "ERROR") return Severity.Error;

            return Severity.Info;
        }
        XmlSchema GetSchema()
        {
            XmlSchema xsd = new XmlSchema();
            using (var s = new StringReader(log4netXsd.Log4NetSchema))
            {
                XmlSchema.Read(s, new ValidationEventHandler(ValidationCallBack));
            }
            return xsd;
        }
        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            // If Document Validation Fails
         
        }

    }
}
