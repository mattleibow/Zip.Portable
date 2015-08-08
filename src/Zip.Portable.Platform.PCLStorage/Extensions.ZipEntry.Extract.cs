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
    public static partial class ZipEntryExtensions
    {
        /// <summary>
        ///   Extract the entry to the filesystem, starting at the current
        ///   working directory.
        /// </summary>
        ///
        /// <overloads>
        ///   This method has a bunch of overloads! One of them is sure to
        ///   be the right one for you... If you don't like these, check
        ///   out the <c>ExtractWithPassword()</c> methods.
        /// </overloads>
        ///
        /// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
        /// <seealso cref="ZipEntry.Extract(ExtractExistingFileAction)"/>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   This method extracts an entry from a zip file into the current
        ///   working directory.  The path of the entry as extracted is the full
        ///   path as specified in the zip archive, relative to the current
        ///   working directory.  After the file is extracted successfully, the
        ///   file attributes and timestamps are set.
        /// </para>
        ///
        /// <para>
        ///   The action taken when extraction an entry would overwrite an
        ///   existing file is determined by the <see cref="ExtractExistingFile"
        ///   /> property.
        /// </para>
        ///
        /// <para>
        ///   Within the call to <c>Extract()</c>, the content for the entry is
        ///   written into a filesystem file, and then the last modified time of the
        ///   file is set according to the <see cref="LastModified"/> property on
        ///   the entry. See the remarks the <see cref="LastModified"/> property for
        ///   some details about the last modified time.
        /// </para>
        ///
        /// </remarks>
        public static void Extract(this ZipEntry zipEntry)
        {
            zipEntry.InternalExtractToBaseDir(".", null);
        }

        /// <summary>
        ///   Extract the entry to a file in the filesystem, using the specified
        ///   behavior when extraction would overwrite an existing file.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   See the remarks on the <see cref="LastModified"/> property, for some
        ///   details about how the last modified time of the file is set after
        ///   extraction.
        /// </para>
        /// </remarks>
        ///
        /// <param name="extractExistingFile">
        ///   The action to take if extraction would overwrite an existing file.
        /// </param>
        public static void Extract(this ZipEntry zipEntry, ExtractExistingFileAction extractExistingFile)
        {
            zipEntry.ExtractExistingFile = extractExistingFile;
            zipEntry.InternalExtractToBaseDir(".", null);
        }

        /// <summary>
        ///   Extract the entry to the filesystem, starting at the specified base
        ///   directory.
        /// </summary>
        ///
        /// <param name="baseDirectory">the pathname of the base directory</param>
        ///
        /// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
        /// <seealso cref="Ionic.Zip.ZipEntry.Extract(string, ExtractExistingFileAction)"/>
        ///
        /// <example>
        /// This example extracts only the entries in a zip file that are .txt files,
        /// into a directory called "textfiles".
        /// <code lang="C#">
        /// using (ZipFile zip = ZipFile.Read("PackedDocuments.zip"))
        /// {
        ///   foreach (string s1 in zip.EntryFilenames)
        ///   {
        ///     if (s1.EndsWith(".txt"))
        ///     {
        ///       zip[s1].Extract("textfiles");
        ///     }
        ///   }
        /// }
        /// </code>
        /// <code lang="VB">
        ///   Using zip As ZipFile = ZipFile.Read("PackedDocuments.zip")
        ///       Dim s1 As String
        ///       For Each s1 In zip.EntryFilenames
        ///           If s1.EndsWith(".txt") Then
        ///               zip(s1).Extract("textfiles")
        ///           End If
        ///       Next
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   Using this method, existing entries in the filesystem will not be
        ///   overwritten. If you would like to force the overwrite of existing
        ///   files, see the <see cref="ExtractExistingFile"/> property, or call
        ///   <see cref="Extract(string, ExtractExistingFileAction)"/>.
        /// </para>
        ///
        /// <para>
        ///   See the remarks on the <see cref="LastModified"/> property, for some
        ///   details about how the last modified time of the created file is set.
        /// </para>
        /// </remarks>
        public static void Extract(this ZipEntry zipEntry, string baseDirectory)
        {
            zipEntry.InternalExtractToBaseDir(baseDirectory, null);
        }

        /// <summary>
        ///   Extract the entry to the filesystem, starting at the specified base
        ///   directory, and using the specified behavior when extraction would
        ///   overwrite an existing file.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   See the remarks on the <see cref="LastModified"/> property, for some
        ///   details about how the last modified time of the created file is set.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// <code lang="C#">
        /// String sZipPath = "Airborne.zip";
        /// String sFilePath = "Readme.txt";
        /// String sRootFolder = "Digado";
        /// using (ZipFile zip = ZipFile.Read(sZipPath))
        /// {
        ///   if (zip.EntryFileNames.Contains(sFilePath))
        ///   {
        ///     // use the string indexer on the zip file
        ///     zip[sFileName].Extract(sRootFolder,
        ///                            ExtractExistingFileAction.OverwriteSilently);
        ///   }
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Dim sZipPath as String = "Airborne.zip"
        /// Dim sFilePath As String = "Readme.txt"
        /// Dim sRootFolder As String = "Digado"
        /// Using zip As ZipFile = ZipFile.Read(sZipPath)
        ///   If zip.EntryFileNames.Contains(sFilePath)
        ///     ' use the string indexer on the zip file
        ///     zip(sFilePath).Extract(sRootFolder, _
        ///                            ExtractExistingFileAction.OverwriteSilently)
        ///   End If
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="baseDirectory">the pathname of the base directory</param>
        /// <param name="extractExistingFile">
        /// The action to take if extraction would overwrite an existing file.
        /// </param>
        public static void Extract(this ZipEntry zipEntry, string baseDirectory, ExtractExistingFileAction extractExistingFile)
        {
            zipEntry.ExtractExistingFile = extractExistingFile;
            zipEntry.InternalExtractToBaseDir(baseDirectory, null);
        }

        /// <summary>
        ///   Extract the entry to the filesystem, using the current working directory
        ///   and the specified password.
        /// </summary>
        ///
        /// <overloads>
        ///   This method has a bunch of overloads! One of them is sure to be
        ///   the right one for you...
        /// </overloads>
        ///
        /// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
        /// <seealso cref="Ionic.Zip.ZipEntry.ExtractWithPassword(ExtractExistingFileAction, string)"/>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   Existing entries in the filesystem will not be overwritten. If you
        ///   would like to force the overwrite of existing files, see the <see
        ///   cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>property, or call
        ///   <see
        ///   cref="ExtractWithPassword(ExtractExistingFileAction,string)"/>.
        /// </para>
        ///
        /// <para>
        ///   See the remarks on the <see cref="LastModified"/> property for some
        ///   details about how the "last modified" time of the created file is
        ///   set.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        ///   In this example, entries that use encryption are extracted using a
        ///   particular password.
        /// <code>
        /// using (var zip = ZipFile.Read(FilePath))
        /// {
        ///     foreach (ZipEntry e in zip)
        ///     {
        ///         if (e.UsesEncryption)
        ///             e.ExtractWithPassword("Secret!");
        ///         else
        ///             e.Extract();
        ///     }
        /// }
        /// </code>
        /// <code lang="VB">
        /// Using zip As ZipFile = ZipFile.Read(FilePath)
        ///     Dim e As ZipEntry
        ///     For Each e In zip
        ///         If (e.UsesEncryption)
        ///           e.ExtractWithPassword("Secret!")
        ///         Else
        ///           e.Extract
        ///         End If
        ///     Next
        /// End Using
        /// </code>
        /// </example>
        /// <param name="password">The Password to use for decrypting the entry.</param>
        public static void ExtractWithPassword(this ZipEntry zipEntry, string password)
        {
            zipEntry.InternalExtractToBaseDir(".", password);
        }

        /// <summary>
        ///   Extract the entry to the filesystem, starting at the specified base
        ///   directory, and using the specified password.
        /// </summary>
        ///
        /// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
        /// <seealso cref="Ionic.Zip.ZipEntry.ExtractWithPassword(string, ExtractExistingFileAction, string)"/>
        ///
        /// <remarks>
        /// <para>
        ///   Existing entries in the filesystem will not be overwritten. If you
        ///   would like to force the overwrite of existing files, see the <see
        ///   cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>property, or call
        ///   <see
        ///   cref="ExtractWithPassword(ExtractExistingFileAction,string)"/>.
        /// </para>
        ///
        /// <para>
        ///   See the remarks on the <see cref="LastModified"/> property, for some
        ///   details about how the last modified time of the created file is set.
        /// </para>
        /// </remarks>
        ///
        /// <param name="baseDirectory">The pathname of the base directory.</param>
        /// <param name="password">The Password to use for decrypting the entry.</param>
        public static void ExtractWithPassword(this ZipEntry zipEntry, string baseDirectory, string password)
        {
            zipEntry.InternalExtractToBaseDir(baseDirectory, password);
        }

        /// <summary>
        ///   Extract the entry to a file in the filesystem, relative to the
        ///   current directory, using the specified behavior when extraction
        ///   would overwrite an existing file.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   See the remarks on the <see cref="LastModified"/> property, for some
        ///   details about how the last modified time of the created file is set.
        /// </para>
        /// </remarks>
        ///
        /// <param name="password">The Password to use for decrypting the entry.</param>
        ///
        /// <param name="extractExistingFile">
        /// The action to take if extraction would overwrite an existing file.
        /// </param>
        public static void ExtractWithPassword(this ZipEntry zipEntry, ExtractExistingFileAction extractExistingFile, string password)
        {
            zipEntry.ExtractExistingFile = extractExistingFile;
            zipEntry.InternalExtractToBaseDir(".", password);
        }

        /// <summary>
        ///   Extract the entry to the filesystem, starting at the specified base
        ///   directory, and using the specified behavior when extraction would
        ///   overwrite an existing file.
        /// </summary>
        ///
        /// <remarks>
        ///   See the remarks on the <see cref="LastModified"/> property, for some
        ///   details about how the last modified time of the created file is set.
        /// </remarks>
        ///
        /// <param name="baseDirectory">the pathname of the base directory</param>
        ///
        /// <param name="extractExistingFile">The action to take if extraction would
        /// overwrite an existing file.</param>
        ///
        /// <param name="password">The Password to use for decrypting the entry.</param>
        public static void ExtractWithPassword(this ZipEntry zipEntry, string baseDirectory, ExtractExistingFileAction extractExistingFile, string password)
        {
            zipEntry.ExtractExistingFile = extractExistingFile;
            zipEntry.InternalExtractToBaseDir(baseDirectory, password);
        }

        /// <summary>
        /// Pass in either basedir or s, but not both.
        /// In other words, you can extract to a stream or to a directory (filesystem), but not both!
        /// The Password param is required for encrypted entries.
        /// </summary>
        private static void InternalExtractToBaseDir(this ZipEntry zipEntry, string baseDir, string password)
        {
            if (baseDir == null)
                throw new ArgumentNullException("baseDir");

            var zipFile = zipEntry.GetZipFile();

            // workitem 10355
            if (zipFile == null)
                throw new InvalidOperationException("Use Extract() only with ZipFile.");

            // get the full filename
            var f = ZipEntryInternal.NameInArchive(zipEntry.FileName);
            var targetFileName = zipFile.FlattenFoldersOnExtract
                ? Path.Combine(baseDir, Path.GetFileName(f))
                : Path.Combine(baseDir, f);
            // workitem 10639
            targetFileName = targetFileName.Replace('/', PortablePath.DirectorySeparatorChar);
            var fullTargetPath = ZipFileExtensions.GetFullPath(targetFileName);

            var fileExistsBeforeExtraction = false;
            try
            {
                // check if it is a directory
                if (zipEntry.IsDirectory || zipEntry.FileName.EndsWith("/"))
                {
                    CreateDirectory(fullTargetPath);
                    goto ExitTry; // all done, caller will return
                }

                // it is a file, so start the extraction
                if (FileSystem.Current.GetFileFromPathAsync(fullTargetPath).ExecuteSync() != null)
                {
                    fileExistsBeforeExtraction = true;
                    int rc = zipEntry.CheckExtractExistingFile(baseDir, targetFileName);
                    if (rc == 2) goto ExitTry; // cancel
                    if (rc == 1) return; // do not overwrite
                }
                
                // set up the output stream
                var tmpName = Path.GetRandomFileName();
                var tmpPath = Path.Combine(Path.GetDirectoryName(fullTargetPath), tmpName);
                var dirName = Path.GetDirectoryName(tmpPath);

                // ensure the target path exists
                var dir = CreateDirectory(dirName, true);

                // extract
                var file = dir.CreateFileAsync(tmpPath, CreationCollisionOption.ReplaceExisting).ExecuteSync();
                using (var output = file.OpenAsync(FileAccess.ReadAndWrite).ExecuteSync())
                {
                    zipEntry.ExtractWithPassword(output, password);
                }
                MoveFileInPlace(fileExistsBeforeExtraction, fullTargetPath, tmpPath);

                ExitTry:;
            }
            catch (Exception)
            {
                zipEntry.SetIOOperationCanceled(true);
                throw;
            }
            finally
            {
                if (zipEntry.IsIOOperationCanceled() && targetFileName != null)
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
                    var file = FileSystem.Current.GetFileFromPathAsync(fullTargetPath).ExecuteSync();
                    if (file != null && !fileExistsBeforeExtraction)
                        file.DeleteAsync();
                }
            }
        }
    }
}
