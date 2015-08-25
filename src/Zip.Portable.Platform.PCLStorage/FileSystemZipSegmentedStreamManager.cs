using PCLStorage;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Ionic.Zip
{
    internal class FileSystemZipSegmentedStreamManager : ZipSegmentedStreamManager
    {
        private readonly string _baseName;
        private readonly string _fullBaseName;
        private readonly IFolder _baseFolder;
        private string _temporaryName;

        public FileSystemZipSegmentedStreamManager(string baseName) 
        {
            _baseName = baseName;
            _fullBaseName = ZipFileExtensions.GetFullPath(_baseName);

            var baseDir = Path.GetDirectoryName(_fullBaseName);
            _baseFolder = FileSystem.Current.GetFolderFromPathAsync(baseDir).ExecuteSync();
            if (_baseFolder == null)
            {
                var dir = Path.GetDirectoryName(_baseName);
                throw new ZipException("Bad Directory", new ArgumentException(string.Format("That folder ({0}) does not exist!", dir)));
            }
        }

        public string TemporaryName
        {
            get { return _temporaryName; }
        }

        public override Stream CreateTemporarySegment()
        {
            // get a new temp file
            var tmpFile = _baseFolder.CreateFileAsync(Path.GetRandomFileName(), CreationCollisionOption.ReplaceExisting).ExecuteSync();
            _temporaryName = tmpFile.Path;

            // open it
            var tmpStream = tmpFile.OpenAsync(FileAccess.ReadAndWrite).ExecuteSync();
            return tmpStream;
        }

        public override Stream OpenSegment(uint segment, uint totalSegments, bool write)
        {
            var segmentName = GetSegmentName(_fullBaseName, segment, totalSegments);
            var file = FileSystem.Current.GetFileFromPathAsync(segmentName).ExecuteSync();
            if (file == null)
            {
                var name = GetSegmentName(_baseName, segment, totalSegments);
                throw new FileNotFoundException(string.Format("That file ({0}) does not exist!", name));
            }
            return file.OpenAsync(write ? FileAccess.ReadAndWrite : FileAccess.Read).ExecuteSync();
        }

        public override void PersistTemporarySegment(uint segment)
        {
            if (_temporaryName != null)
            {
                // remove old segment
                var segmentName = GetSegmentName(_fullBaseName, segment, 0);
                var file = FileSystem.Current.GetFileFromPathAsync(segmentName).ExecuteSync();
                if (file != null)
                {
                    file.DeleteAsync().ExecuteSync();
                }

                // rename the temp
                file = FileSystem.Current.GetFileFromPathAsync(_temporaryName).ExecuteSync();
                file.MoveAsync(segmentName);

                // no more temp
                _temporaryName = null;
            }
        }

        public override void TruncateSegments(uint startSegment, uint endSegment)
        {
            // remove the temporary segment
            if (_temporaryName != null)
            {
                var file = FileSystem.Current.GetFileFromPathAsync(_temporaryName).ExecuteSync();
                if (file != null)
                {
                    file.DeleteAsync().ExecuteSync();
                }
                
                // no more temp
                _temporaryName = null;
            }

            // remove current and intervening segments
            for (var i = startSegment; i <= endSegment; i--)
            {
                var segmentName = GetSegmentName(_fullBaseName, i, 0);
                var file = FileSystem.Current.GetFileFromPathAsync(segmentName).ExecuteSync();
                if (file != null)
                {
                    file.DeleteAsync().ExecuteSync();
                }
            }
        }
    }
}
