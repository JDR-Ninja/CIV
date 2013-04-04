using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIV.Common
{
    public class BackupProgressEventArgs : EventArgs
    {
        private double _progress;

        public double Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        private double _fileProgress;

        public double FileProgress
        {
            get { return _fileProgress; }
            set { _fileProgress = value; }
        }

        private string _filename;

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        private long _fileSize;

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        private long _totalSize;

        public long TotalSize
        {
            get { return _totalSize; }
            set { _totalSize = value; }
        }

        private int _count;

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private int _totalCount;

        public int TotalCount
        {
            get { return _totalCount; }
            set { _totalCount = value; }
        }


        public BackupProgressEventArgs(double progress, double fileProgress, string filename, long fileSize, long totalSize, int count, int totalCount)
        {
            Progress = progress;
            FileProgress = fileProgress;
            Filename = filename;
            FileSize = fileSize;
            TotalSize = totalSize;
            Count = count;
            TotalCount = totalCount;
        }
    }
}
