using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ionic.Zip
{
    public partial class ZipEntryExtensions
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
        public void Extract()
        {
            InternalExtractToBaseDir(".", null, _container, _Source, FileName);
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
        public void Extract(ExtractExistingFileAction extractExistingFile)
        {
            ExtractExistingFile = extractExistingFile;
            InternalExtractToBaseDir(".", null, _container, _Source, FileName);
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
        public void Extract(string baseDirectory)
        {
            InternalExtractToBaseDir(baseDirectory, null, _container, _Source, FileName);
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
        public void Extract(string baseDirectory, ExtractExistingFileAction extractExistingFile)
        {
            ExtractExistingFile = extractExistingFile;
            InternalExtractToBaseDir(baseDirectory, null, _container, _Source, FileName);
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
        public void ExtractWithPassword(string password)
        {
            InternalExtractToBaseDir(".", password, _container, _Source, FileName);
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
        public void ExtractWithPassword(string baseDirectory, string password)
        {
            InternalExtractToBaseDir(baseDirectory, password, _container, _Source, FileName);
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
        public void ExtractWithPassword(ExtractExistingFileAction extractExistingFile, string password)
        {
            ExtractExistingFile = extractExistingFile;
            InternalExtractToBaseDir(".", password, _container, _Source, FileName);
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
        public void ExtractWithPassword(string baseDirectory, ExtractExistingFileAction extractExistingFile, string password)
        {
            ExtractExistingFile = extractExistingFile;
            InternalExtractToBaseDir(baseDirectory, password, _container, _Source, FileName);
        }

        /// <summary>
        /// Pass in either basedir or s, but not both.
        /// In other words, you can extract to a stream or to a directory (filesystem), but not both!
        /// The Password param is required for encrypted entries.
        /// </summary>
        void InternalExtractToBaseDir(string baseDir, string password, ZipContainer zipContainer, ZipEntrySource zipEntrySource, string fileName)
        {
            if (baseDir == null)
                throw new ArgumentNullException("baseDir");

            // workitem 7958
            if (zipContainer == null)
                throw new BadStateException("This entry is an orphan");

            // workitem 10355
            if (zipContainer.ZipFile == null)
                throw new InvalidOperationException("Use Extract() only with ZipFile.");

            zipContainer.ZipFile.Reset(false);

            if (zipEntrySource != ZipEntrySource.ZipFile)
                throw new BadStateException("You must call ZipFile.Save before calling any Extract method");

            OnBeforeExtract(this, baseDir, zipContainer.ZipFile);

            _ioOperationCanceled = false;

            var fileExistsBeforeExtraction = false;
            var checkLaterForResetDirTimes = false;
            string targetFileName = null;
            try
            {
                ValidateCompression(_CompressionMethod_FromZipFile, fileName, GetUnsupportedCompressionMethod(_CompressionMethod));
                ValidateEncryption(Encryption, fileName, _UnsupportedAlgorithmId);

                if (IsDoneWithOutputToBaseDir(baseDir, out targetFileName))
                {
                    WriteStatus("extract dir {0}...", targetFileName);
                    // if true, then the entry was a directory and has been created.
                    // We need to fire the Extract Event.
                    OnAfterExtract(baseDir);
                    return;
                }

                // workitem 10639
                // do we want to extract to a regular filesystem file?

                // Check for extracting to a previously existing file. The user
                // can specify bejavior for that case: overwrite, don't
                // overwrite, and throw.  Also, if the file exists prior to
                // extraction, it affects exception handling: whether to delete
                // the target of extraction or not. This check needs to be done
                // before the password check is done, because password check may
                // throw a BadPasswordException, which triggers the catch,
                // wherein the existing file may be deleted if not flagged as
                // pre-existing.
                if (File.Exists(targetFileName))
                {
                    fileExistsBeforeExtraction = true;
                    int rc = CheckExtractExistingFile(baseDir, targetFileName);
                    if (rc == 2) goto ExitTry; // cancel
                    if (rc == 1) return; // do not overwrite
                }

                // If no password explicitly specified, use the password on the entry itself,
                // or on the zipfile itself.
                if (_Encryption_FromZipFile != EncryptionAlgorithm.None)
                    EnsurePassword(password);

                // set up the output stream
                var tmpName = Path.GetRandomFileName();
                var tmpPath = Path.Combine(Path.GetDirectoryName(targetFileName), tmpName);
                WriteStatus("extract file {0}...", targetFileName);

                using (var output = OpenFileStream(tmpPath, ref checkLaterForResetDirTimes))
                {
                    if (ExtractToStream(ArchiveStream, output, Encryption, _Crc32))
                        goto ExitTry;

                    output.Close();
                }

                MoveFileInPlace(fileExistsBeforeExtraction, targetFileName, tmpPath, checkLaterForResetDirTimes);

                OnAfterExtract(baseDir);

                ExitTry:;
            }
            catch (Exception)
            {
                _ioOperationCanceled = true;
                throw;
            }
            finally
            {
                if (_ioOperationCanceled && targetFileName != null)
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
                    if (File.Exists(targetFileName) && !fileExistsBeforeExtraction)
                        File.Delete(targetFileName);
                }
            }
        }

        Stream OpenFileStream(string tmpPath, ref bool checkLaterForResetDirTimes)
        {
            var dirName = Path.GetDirectoryName(tmpPath);
            // ensure the target path exists
            if (!Directory.Exists(dirName))
            {
                // we create the directory here, but we do not set the
                // create/modified/accessed times on it because it is being
                // created implicitly, not explcitly. There's no entry in the
                // zip archive for the directory.
                Directory.CreateDirectory(dirName);
            }
            else
            {
                // workitem 8264
                if (_container.ZipFile != null)
                    checkLaterForResetDirTimes = _container.ZipFile._inExtractAll;
            }

            // File.Create(CreateNew) will overwrite any existing file.
            return new FileStream(tmpPath, FileMode.CreateNew);
        }
    }
}
