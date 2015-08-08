using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace Ionic.Zip
{
    public partial class ZipFileExtensions
    {
        internal static string GetFullPath(string fileOrDirectoryName)
        {
            var fullPath = fileOrDirectoryName;
            if (!Path.IsPathRooted(fullPath))
                fullPath = Path.Combine(FileSystem.Current.LocalStorage.Path, fullPath);
            return fullPath;
        }
    }
}
