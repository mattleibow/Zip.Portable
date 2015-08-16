using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ionic.Zip.PlatformSupport
{
    public static class ZipEntryInternal
    {
        public static string NameInArchive(string filename)
        {
            return ZipEntry.NameInArchive(filename, null);
        }

        public static string NameInArchive(string filename, string directoryPathInArchive)
        {
            return ZipEntry.NameInArchive(filename, directoryPathInArchive);
        }

        /// <summary>
        /// Utility routine for transforming path names from filesystem format (on Windows that means backslashes) to
        /// a format suitable for use within zipfiles. This means trimming the volume letter and colon (if any) And
        /// swapping backslashes for forward slashes.
        /// </summary>
        /// <param name="pathName">source path.</param>
        /// <returns>transformed path</returns>
        public static string NormalizePath(string pathName)
        {
            return SharedUtilities.NormalizePathForUseInZipFile(pathName);
        }

        public static void SetLocalFileName(this ZipEntry zipEntry, string localFilename)
        {
            zipEntry._LocalFileName = localFilename;
        }

        public static void SetIOOperationCanceled(this ZipEntry zipEntry, bool canceled)
        {
            zipEntry._ioOperationCanceled = canceled;
        }

        public static bool IsIOOperationCanceled(this ZipEntry zipEntry)
        {
            return zipEntry._ioOperationCanceled;
        }

        public static int CheckExtractExistingFile(this ZipEntry zipEntry, string baseDir, string targetFileName)
        {
            return zipEntry.CheckExtractExistingFile(baseDir, targetFileName);
        }

        public static ZipFile GetZipFile(this ZipEntry zipEntry)
        {
            return zipEntry._container.ZipFile;
        }
    }
}
