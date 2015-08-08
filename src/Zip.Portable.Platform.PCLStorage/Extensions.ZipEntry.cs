using PCLStorage;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Ionic.Zip
{
    public static partial class ZipEntryExtensions
    {
        public static IFolder CreateDirectory(string dirName, bool throwIfError)
        {
            var dir = CreateDirectory(dirName);
            if (dir == null && throwIfError)
            {
                throw new ArgumentOutOfRangeException("baseDir", string.Format("Specified directory '{0}' could not be created or found.", dirName));
            }
            return dir;
        }

        public static IFolder CreateDirectory(string dirName)
        {
            // remove any trailing slashes for the file system
            dirName = dirName.TrimEnd('/', '\\');

            var dir = FileSystem.Current.GetFolderFromPathAsync(dirName).ExecuteSync();
            if (dir == null)
            {
                // dir does not exist
                var parentName = Path.GetDirectoryName(dirName);
                if (!string.IsNullOrEmpty(parentName))
                {
                    var parent = FileSystem.Current.GetFolderFromPathAsync(parentName).ExecuteSync();
                    if (parent == null)
                    {
                        // parent doesn't exist, so create it first
                        parent = CreateDirectory(parentName);
                    }
                    if (parent != null)
                    {
                        // parent exists, so create the sub folder and return that
                        dir = parent.CreateFolderAsync(Path.GetFileName(dirName), CreationCollisionOption.OpenIfExists).ExecuteSync();
                    }
                }
            }
            return dir;
        }

        internal static void MoveFileInPlace(bool fileExists, string targetFileName, string tmpPath)
        {
            // workitem 10639
            // move file to permanent home
            string zombie = null;

            var targetFile = FileSystem.Current.GetFileFromPathAsync(targetFileName).ExecuteSync();
            var tempFile = FileSystem.Current.GetFileFromPathAsync(tmpPath).ExecuteSync();

            if (fileExists)
            {
                // An AV program may hold the target file open, which means
                // File.Delete() will succeed, though the actual deletion
                // remains pending. This will prevent a subsequent
                // File.Move() from succeeding. To avoid this, when the file
                // already exists, we need to replace it in 3 steps:
                //
                //     1. rename the existing file to a zombie name;
                //     2. rename the extracted file from the temp name to
                //        the target file name;
                //     3. delete the zombie.
                //
                zombie = targetFileName + Path.GetRandomFileName() + ".PendingOverwrite";
                targetFile.MoveAsync(zombie).ExecuteSync();
            }

            tempFile.MoveAsync(targetFileName).ExecuteSync();

            if (fileExists)
            {
                targetFile.DeleteAsync().ExecuteSync();
            }
        }
    }
}
