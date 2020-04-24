using logR.Server.Services.Interfaces;
using logR.Shared;
using logR.Shared.Gateways.Files;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace logR.Server.Services
{
    public class LogFileReader
    {
        FileGatewayConfiguration _settings;
        private readonly ILogProcessor _logProcessor;
        FileSystemWatcher _internalWatcher;
        ConcurrentDictionary<string, long> _internalDictionary;
        public LogFileReader(IOptions<FileGatewayConfiguration> settings, ILogProcessor logProcessor)
        {
            _settings = settings.Value;
            _logProcessor = logProcessor;
            _internalDictionary = new ConcurrentDictionary<string, long>();
            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(PollFiles);
            //_internalWatcher.EnableRaisingEvents = true;
            
        }
        bool firstRun = true;
        void PollFiles(long t)
        {
            var directory = new DirectoryInfo(_settings.Directory);
            var files = directory.GetFiles(_settings.FileMask);
            foreach(var file in files)
            {
                
                    var bytes = _internalDictionary.GetOrAdd(file.Name, file.Length);
                if (bytes != file.Length)
                {
                    //process file.Length - bytes
                    Process(file, bytes, file.Length);

                     _internalDictionary.AddOrUpdate(file.Name, file.Length, (k,v)=> file.Length);

                }
              
            }
        }

        private void Process(FileInfo file, long offset, long length)
        {
            using (var f = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))

            {

                var b = new byte[length - offset];
                f.Read(b, (int)offset,(int)( b.Length));
                using (var ms = new MemoryStream(b))
                using (var t = XmlReader.Create(ms, new XmlReaderSettings() { }))
                {
                    var le = ParseLog4NetXmlLogEvent(t, "eds");
                    if (le != null)
                        _logProcessor.ProcessLog(le);
                }



            }
        }

        private void _internalWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType ==WatcherChangeTypes.Changed)
            {
                var bytes = _internalDictionary.GetOrAdd(e.Name, 0);
               

                using (var f = File.OpenRead(e.FullPath))
               
                {

                    var b = new byte[f.Length - bytes];
                    f.Read(b,(int) bytes, b.Length);
                    using (var ms = new MemoryStream(b))
                    using (var t = XmlReader.Create(ms))
                    {
                        var le =ParseLog4NetXmlLogEvent(t, "eds");
                        if (le!=null)
                        _logProcessor.ProcessLog(le);
                    }
                    _internalDictionary.AddOrUpdate(e.Name, bytes + b.Length, (key, offset) => bytes + b.Length);
                  

                }
            }
        }

        public  LogEvent ParseLog4NetXmlLogEvent(XmlReader reader, string defaultLogger)
        {
            try
            {
                var logMsg = new LogEvent();

           //     reader.Read();
                if ((reader.MoveToContent() != XmlNodeType.Element) || (reader.Name != "log4net:event"))
                    throw new Exception("The Log Event is not a valid log4net Xml block.");

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



                return logMsg;
            }
            catch (Exception ex)
            {
                return null;
                
            }
           
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
    }
}
