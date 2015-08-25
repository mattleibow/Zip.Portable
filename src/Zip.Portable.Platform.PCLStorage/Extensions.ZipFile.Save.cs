using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Ionic.Zip.PlatformSupport;

namespace Ionic.Zip
{
    public static partial class ZipFileExtensions
    {
        /// <summary>
        /// Save the file to a new zipfile, with the given name.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method allows the application to explicitly specify the name of the zip
        /// file when saving. Use this when creating a new zip file, or when
        /// updating a zip archive.
        /// </para>
        ///
        /// <para>
        /// An application can also save a zip archive in several places by calling this
        /// method multiple times in succession, with different filenames.
        /// </para>
        ///
        /// <para>
        /// The <c>ZipFile</c> instance is written to storage, typically a zip file in a
        /// filesystem, only when the caller calls <c>Save</c>.  The Save operation writes
        /// the zip content to a temporary file, and then renames the temporary file
        /// to the desired name. If necessary, this method will delete a pre-existing file
        /// before the rename.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <exception cref="System.ArgumentException">
        /// Thrown if you specify a directory for the filename.
        /// </exception>
        ///
        /// <param name="fileName">
        /// The name of the zip archive to save to. Existing files will
        /// be overwritten with great prejudice.
        /// </param>
        ///
        /// <example>
        /// This example shows how to create and Save a zip file.
        /// <code>
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///   zip.AddDirectory(@"c:\reports\January");
        ///   zip.Save("January.zip");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile()
        ///   zip.AddDirectory("c:\reports\January")
        ///   zip.Save("January.zip")
        /// End Using
        /// </code>
        ///
        /// </example>
        ///
        /// <example>
        /// This example shows how to update a zip file.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read("ExistingArchive.zip"))
        /// {
        ///   zip.AddFile("NewData.csv");
        ///   zip.Save("UpdatedArchive.zip");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As ZipFile = ZipFile.Read("ExistingArchive.zip")
        ///   zip.AddFile("NewData.csv")
        ///   zip.Save("UpdatedArchive.zip")
        /// End Using
        /// </code>
        ///
        /// </example>
        public static void Save(this ZipFile zipFile, String fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            string fullPath = GetFullPath(fileName);

            if (FileSystem.Current.GetFolderFromPathAsync(fullPath).ExecuteSync() != null)
                throw new ZipException("Bad Directory", new ArgumentException("That name specifies an existing directory. Please specify a filename.", "fileName"));

            var fullDirectoryName = Path.GetDirectoryName(fullPath);
            var directoryName = Path.GetDirectoryName(fileName);
            var folder = FileSystem.Current.GetFolderFromPathAsync(fullDirectoryName).ExecuteSync();
            if (folder == null)
                throw new ZipException("Bad Directory", new ArgumentException(string.Format("That folder ({0}) does not exist!", directoryName)));

            // check up here so we may shortcut some IO operations in the future :)
            var fileExists = folder.CheckExistsAsync(fullPath).ExecuteSync() == ExistenceCheckResult.FileExists;

            // write to a temporary file so that we can read from the same zip when saving
            var tmpName = Path.GetRandomFileName();

            try
            {
                ZipSegmentedStreamManager segmentsManager = null;

                if (zipFile.MaxOutputSegmentSize != 0)
                {
                    // save segmented files using the segments manager
                    var manager = new FileSystemZipSegmentedStreamManager(fullPath);
                    segmentsManager = manager;
                    zipFile.Save(segmentsManager);
                    tmpName = manager.TemporaryName;
                }
                else
                {
                // save
                var tmpFile = folder.CreateFileAsync(tmpName, CreationCollisionOption.ReplaceExisting).ExecuteSync();
                tmpName = tmpFile.Path;
                using (var tmpStream = tmpFile.OpenAsync(FileAccess.ReadAndWrite).ExecuteSync())
                {
                    zipFile.Save(tmpStream);
                }
                }

                // if it wasn't canceled
                if (!zipFile.IsSaveOperationCanceled())
                {
                    // disconnect from the stream
                    zipFile.SetUnderlyingZipStream(null);
                    // move the temporary file into position
                    zipFile.OnSaveEvent(ZipProgressEventType.Saving_BeforeRenameTempArchive);
                    ZipEntryExtensions.MoveFileInPlace(fileExists, fullPath, tmpName);
                    zipFile.OnSaveEvent(ZipProgressEventType.Saving_AfterRenameTempArchive);
                    // and now set the read stream to be that of the new file
                    var targetFile = FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync();
                    var fileStream = targetFile.OpenAsync(FileAccess.Read).ExecuteSync();
                    zipFile.SetUnderlyingZipSegmentedStreamManager(segmentsManager);
                    zipFile.SetUnderlyingZipStream(fileStream);
                    zipFile.SetShouldDisposeReadStream(true);
                    zipFile.Name = fullPath;

                    // so we can shortcut the existance checks
                    tmpName = null;
                }
            }
            finally
            {
                // An exception has occurred. If the file exists, check
                // to see if it existed before we tried extracting.  If
                // it did not, attempt to remove the target file. There
                // is a small possibility that the existing file has
                // been extracted successfully, overwriting a previously
                // existing file, and an exception was thrown after that
                // but before final completion (setting times, etc). In
                // that case the file will remain, even though some
                // error occurred.  Nothing to be done about it.
                if (tmpName != null)
                {
                    var tmpFile = folder.GetFileAsync(tmpName).ExecuteSync();
                    tmpFile.DeleteAsync();
                }
            }
        }
    }
}
