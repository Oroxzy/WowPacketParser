using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WowPacketParser.Misc;

namespace WowPacketParser.SQL
{
    public class SQLFile : IDisposable
    {
        private StreamWriter _file;

        private readonly string _fileName;
        private readonly string _header;

        public SQLFile(string file, string header)
        {
            if (string.IsNullOrWhiteSpace(Settings.SQLFileName)) // only delete file if no global
                File.Delete(file);                               // file name was specified
            _fileName = file;
            _header = header;
        }

        ~SQLFile()
        {
            Dispose(false);
        }

        public void WriteData(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return;

            if (_file == null)
            {
                _file = new StreamWriter(_fileName, true);
                _file.WriteLine(_header);
            }

            _file.WriteLine(sql);
        }

        public bool AnythingWritten()
        {
            return _file != null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_file != null)
            {
                _file.Flush();
                _file.Close();
                _file = null;
            }
        }
    }
}
