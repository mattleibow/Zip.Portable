using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PCLStorage;
using Ionic.Zip.PlatformSupport;

namespace Ionic.Zip
{
    public static partial class ZipFileExtensions
    {
        /// <summary>
        ///   Adds an item, either a file or a directory, to a zip file archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This method is handy if you are adding things to zip archive and don't
        ///   want to bother distinguishing between directories or files.  Any files are
        ///   added as single entries.  A directory added through this method is added
        ///   recursively: all files and subdirectories contained within the directory
        ///   are added to the <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        ///   The name of the item may be a relative path or a fully-qualified
        ///   path. Remember, the items contained in <c>ZipFile</c> instance get written
        ///   to the disk only when you call <see cref="ZipFile.Save()"/> or a similar
        ///   save method.
        /// </para>
        ///
        /// <para>
        ///   The directory name used for the file within the archive is the same
        ///   as the directory name (potentially a relative path) specified in the
        ///   <paramref name="fileOrDirectoryName"/>.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to the
        ///   <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddFile(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string)"/>
        ///
        /// <overloads>This method has two overloads.</overloads>
        /// <param name="fileOrDirectoryName">
        /// the name of the file or directory to add.</param>
        ///
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public static ZipEntry AddItem(this ZipFile zipFile, string fileOrDirectoryName)
        {
            return zipFile.AddItem(fileOrDirectoryName, null);
        }

        /// <summary>
        ///   Adds an item, either a file or a directory, to a zip file archive,
        ///   explicitly specifying the directory path to be used in the archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   If adding a directory, the add is recursive on all files and
        ///   subdirectories contained within it.
        /// </para>
        /// <para>
        ///   The name of the item may be a relative path or a fully-qualified path.
        ///   The item added by this call to the <c>ZipFile</c> is not read from the
        ///   disk nor written to the zip file archive until the application calls
        ///   Save() on the <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        ///   This version of the method allows the caller to explicitly specify the
        ///   directory path to be used in the archive, which would override the
        ///   "natural" path of the filesystem file.
        /// </para>
        ///
        /// <para>
        ///   Encryption will be used on the file data if the <c>Password</c> has
        ///   been set on the <c>ZipFile</c> object, prior to calling this method.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to the
        ///   <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <exception cref="System.IO.FileNotFoundException">
        ///   Thrown if the file or directory passed in does not exist.
        /// </exception>
        ///
        /// <param name="fileOrDirectoryName">the name of the file or directory to add.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        ///   The name of the directory path to use within the zip archive.  This path
        ///   need not refer to an extant directory in the current filesystem.  If the
        ///   files within the zip are later extracted, this is the path used for the
        ///   extracted file.  Passing <c>null</c> (<c>Nothing</c> in VB) will use the
        ///   path on the fileOrDirectoryName.  Passing the empty string ("") will
        ///   insert the item at the root path within the archive.
        /// </param>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddFile(string, string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string, string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string, string)"/>
        ///
        /// <example>
        ///   This example shows how to zip up a set of files into a flat hierarchy,
        ///   regardless of where in the filesystem the files originated. The resulting
        ///   zip archive will contain a toplevel directory named "flat", which itself
        ///   will contain files Readme.txt, MyProposal.docx, and Image1.jpg.  A
        ///   subdirectory under "flat" called SupportFiles will contain all the files
        ///   in the "c:\SupportFiles" directory on disk.
        ///
        /// <code>
        /// String[] itemnames= {
        ///   "c:\\fixedContent\\Readme.txt",
        ///   "MyProposal.docx",
        ///   "c:\\SupportFiles",  // a directory
        ///   "images\\Image1.jpg"
        /// };
        ///
        /// try
        /// {
        ///   using (ZipFile zip = new ZipFile())
        ///   {
        ///     for (int i = 1; i &lt; itemnames.Length; i++)
        ///     {
        ///       // will add Files or Dirs, recurses and flattens subdirectories
        ///       zip.AddItem(itemnames[i],"flat");
        ///     }
        ///     zip.Save(ZipToCreate);
        ///   }
        /// }
        /// catch (System.Exception ex1)
        /// {
        ///   System.Console.Error.WriteLine("exception: {0}", ex1);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        ///   Dim itemnames As String() = _
        ///     New String() { "c:\fixedContent\Readme.txt", _
        ///                    "MyProposal.docx", _
        ///                    "SupportFiles", _
        ///                    "images\Image1.jpg" }
        ///   Try
        ///       Using zip As New ZipFile
        ///           Dim i As Integer
        ///           For i = 1 To itemnames.Length - 1
        ///               ' will add Files or Dirs, recursing and flattening subdirectories.
        ///               zip.AddItem(itemnames(i), "flat")
        ///           Next i
        ///           zip.Save(ZipToCreate)
        ///       End Using
        ///   Catch ex1 As Exception
        ///       Console.Error.WriteLine("exception: {0}", ex1.ToString())
        ///   End Try
        /// </code>
        /// </example>
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public static ZipEntry AddItem(this ZipFile zipFile, String fileOrDirectoryName, String directoryPathInArchive)
        {
            string fullPath = GetFullPath(fileOrDirectoryName);

            if (FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync() != null)
                return zipFile.AddFile(fileOrDirectoryName, directoryPathInArchive);

            if (FileSystem.Current.GetFolderFromPathAsync(fullPath).ExecuteSync() != null)
                return zipFile.AddDirectory(fileOrDirectoryName, directoryPathInArchive);

            throw new FileNotFoundException(String.Format("That file or directory ({0}) does not exist!",
                                                          fileOrDirectoryName));
        }


        /// <summary>
        ///   Adds a File to a Zip file archive.
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        ///   This call collects metadata for the named file in the filesystem,
        ///   including the file attributes and the timestamp, and inserts that metadata
        ///   into the resulting ZipEntry.  Only when the application calls Save() on
        ///   the <c>ZipFile</c>, does DotNetZip read the file from the filesystem and
        ///   then write the content to the zip file archive.
        /// </para>
        ///
        /// <para>
        ///   This method will throw an exception if an entry with the same name already
        ///   exists in the <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to the
        ///   <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// <para>
        ///   In this example, three files are added to a Zip archive. The ReadMe.txt
        ///   file will be placed in the root of the archive. The .png file will be
        ///   placed in a folder within the zip called photos\personal.  The pdf file
        ///   will be included into a folder within the zip called Desktop.
        /// </para>
        /// <code>
        ///    try
        ///    {
        ///      using (ZipFile zip = new ZipFile())
        ///      {
        ///        zip.AddFile("c:\\photos\\personal\\7440-N49th.png");
        ///        zip.AddFile("c:\\Desktop\\2008-Regional-Sales-Report.pdf");
        ///        zip.AddFile("ReadMe.txt");
        ///
        ///        zip.Save("Package.zip");
        ///      }
        ///    }
        ///    catch (System.Exception ex1)
        ///    {
        ///      System.Console.Error.WriteLine("exception: " + ex1);
        ///    }
        /// </code>
        ///
        /// <code lang="VB">
        ///  Try
        ///       Using zip As ZipFile = New ZipFile
        ///           zip.AddFile("c:\photos\personal\7440-N49th.png")
        ///           zip.AddFile("c:\Desktop\2008-Regional-Sales-Report.pdf")
        ///           zip.AddFile("ReadMe.txt")
        ///           zip.Save("Package.zip")
        ///       End Using
        ///   Catch ex1 As Exception
        ///       Console.Error.WriteLine("exception: {0}", ex1.ToString)
        ///   End Try
        /// </code>
        /// </example>
        ///
        /// <overloads>This method has two overloads.</overloads>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddItem(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string)"/>
        ///
        /// <param name="fileName">
        ///   The name of the file to add. It should refer to a file in the filesystem.
        ///   The name of the file may be a relative path or a fully-qualified path.
        /// </param>
        /// <returns>The <c>ZipEntry</c> corresponding to the File added.</returns>
        public static ZipEntry AddFile(this ZipFile zipFile, string fileName)
        {
            return zipFile.AddFile(fileName, null);
        }

        /// <summary>
        ///   Adds a File to a Zip file archive, potentially overriding the path to be
        ///   used within the zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   The file added by this call to the <c>ZipFile</c> is not written to the
        ///   zip file archive until the application calls Save() on the <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        ///   This method will throw an exception if an entry with the same name already
        ///   exists in the <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        ///   This version of the method allows the caller to explicitly specify the
        ///   directory path to be used in the archive.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to the
        ///   <c>ZipEntry</c> added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// <para>
        ///   In this example, three files are added to a Zip archive. The ReadMe.txt
        ///   file will be placed in the root of the archive. The .png file will be
        ///   placed in a folder within the zip called images.  The pdf file will be
        ///   included into a folder within the zip called files\docs, and will be
        ///   encrypted with the given password.
        /// </para>
        /// <code>
        /// try
        /// {
        ///   using (ZipFile zip = new ZipFile())
        ///   {
        ///     // the following entry will be inserted at the root in the archive.
        ///     zip.AddFile("c:\\datafiles\\ReadMe.txt", "");
        ///     // this image file will be inserted into the "images" directory in the archive.
        ///     zip.AddFile("c:\\photos\\personal\\7440-N49th.png", "images");
        ///     // the following will result in a password-protected file called
        ///     // files\\docs\\2008-Regional-Sales-Report.pdf  in the archive.
        ///     zip.Password = "EncryptMe!";
        ///     zip.AddFile("c:\\Desktop\\2008-Regional-Sales-Report.pdf", "files\\docs");
        ///     zip.Save("Archive.zip");
        ///   }
        /// }
        /// catch (System.Exception ex1)
        /// {
        ///   System.Console.Error.WriteLine("exception: {0}", ex1);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        ///   Try
        ///       Using zip As ZipFile = New ZipFile
        ///           ' the following entry will be inserted at the root in the archive.
        ///           zip.AddFile("c:\datafiles\ReadMe.txt", "")
        ///           ' this image file will be inserted into the "images" directory in the archive.
        ///           zip.AddFile("c:\photos\personal\7440-N49th.png", "images")
        ///           ' the following will result in a password-protected file called
        ///           ' files\\docs\\2008-Regional-Sales-Report.pdf  in the archive.
        ///           zip.Password = "EncryptMe!"
        ///           zip.AddFile("c:\Desktop\2008-Regional-Sales-Report.pdf", "files\documents")
        ///           zip.Save("Archive.zip")
        ///       End Using
        ///   Catch ex1 As Exception
        ///       Console.Error.WriteLine("exception: {0}", ex1)
        ///   End Try
        /// </code>
        /// </example>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddItem(string,string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string, string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string,string)"/>
        ///
        /// <param name="fileName">
        ///   The name of the file to add.  The name of the file may be a relative path
        ///   or a fully-qualified path.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        ///   Specifies a directory path to use to override any path in the fileName.
        ///   This path may, or may not, correspond to a real directory in the current
        ///   filesystem.  If the files within the zip are later extracted, this is the
        ///   path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
        ///   VB) will use the path on the fileName, if any.  Passing the empty string
        ///   ("") will insert the item at the root path within the archive.
        /// </param>
        ///
        /// <returns>The <c>ZipEntry</c> corresponding to the file added.</returns>
        public static ZipEntry AddFile(this ZipFile zipFile, string fileName, String directoryPathInArchive)
        {
            string fullPath = GetFullPath(fileName);

            string nameInArchive = ZipEntryInternal.NameInArchive(fileName, directoryPathInArchive);
            ZipEntry entry = zipFile.AddEntry(nameInArchive, (name) => {
                var file = FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync();
                if (file == null)
                {
                    throw new FileNotFoundException(string.Format("That file ({0}) does not exist!", fileName));
                }
                return file.OpenAsync(FileAccess.Read).ExecuteSync();
            }, (name, stream) => {
                if (stream != null)
                {
                    stream.Dispose();
                }
            });
            entry.SetLocalFileName(fullPath);
            return entry;
        }

        /// <summary>
        ///   This method adds a set of files to the <c>ZipFile</c>.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   Use this method to add a set of files to the zip archive, in one call.
        ///   For example, a list of files received from
        ///   <c>System.IO.Directory.GetFiles()</c> can be added to a zip archive in one
        ///   call.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to each
        ///   ZipEntry added.
        /// </para>
        /// </remarks>
        ///
        /// <param name="fileNames">
        ///   The collection of names of the files to add. Each string should refer to a
        ///   file in the filesystem. The name of the file may be a relative path or a
        ///   fully-qualified path.
        /// </param>
        ///
        /// <example>
        ///   This example shows how to create a zip file, and add a few files into it.
        /// <code>
        /// String ZipFileToCreate = "archive1.zip";
        /// String DirectoryToZip = "c:\\reports";
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   zip.AddFiles(filenames);
        ///   zip.Save(ZipFileToCreate);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Dim ZipFileToCreate As String = "archive1.zip"
        /// Dim DirectoryToZip As String = "c:\reports"
        /// Using zip As ZipFile = New ZipFile
        ///     ' Store all files found in the top level directory, into the zip archive.
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     zip.AddFiles(filenames)
        ///     zip.Save(ZipFileToCreate)
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddSelectedFiles(String, String)" />
        public static void AddFiles(this ZipFile zipFile, System.Collections.Generic.IEnumerable<String> fileNames)
        {
            zipFile.AddFiles(fileNames, null);
        }

        /// <summary>
        ///   Adds or updates a set of files in the <c>ZipFile</c>.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   Any files that already exist in the archive are updated. Any files that
        ///   don't yet exist in the archive are added.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to each
        ///   ZipEntry added.
        /// </para>
        /// </remarks>
        ///
        /// <param name="fileNames">
        ///   The collection of names of the files to update. Each string should refer to a file in
        ///   the filesystem. The name of the file may be a relative path or a fully-qualified path.
        /// </param>
        ///
        public static void UpdateFiles(this ZipFile zipFile, System.Collections.Generic.IEnumerable<String> fileNames)
        {
            zipFile.UpdateFiles(fileNames, null);
        }

        /// <summary>
        ///   Adds a set of files to the <c>ZipFile</c>, using the
        ///   specified directory path in the archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   Any directory structure that may be present in the
        ///   filenames contained in the list is "flattened" in the
        ///   archive.  Each file in the list is added to the archive in
        ///   the specified top-level directory.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see
        ///   cref="Encryption"/>, <see cref="Password"/>, <see
        ///   cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see
        ///   cref="ExtractExistingFile"/>, <see
        ///   cref="ZipErrorAction"/>, and <see
        ///   cref="CompressionLevel"/>, their respective values at the
        ///   time of this call will be applied to each ZipEntry added.
        /// </para>
        /// </remarks>
        ///
        /// <param name="fileNames">
        ///   The names of the files to add. Each string should refer to
        ///   a file in the filesystem.  The name of the file may be a
        ///   relative path or a fully-qualified path.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        ///   Specifies a directory path to use to override any path in the file name.
        ///   Th is path may, or may not, correspond to a real directory in the current
        ///   filesystem.  If the files within the zip are later extracted, this is the
        ///   path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
        ///   VB) will use the path on each of the <c>fileNames</c>, if any.  Passing
        ///   the empty string ("") will insert the item at the root path within the
        ///   archive.
        /// </param>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddSelectedFiles(String, String)" />
        public static void AddFiles(this ZipFile zipFile, System.Collections.Generic.IEnumerable<String> fileNames, String directoryPathInArchive)
        {
            zipFile.AddFiles(fileNames, directoryPathInArchive, false);
        }

        /// <summary>
        ///   Adds a set of files to the <c>ZipFile</c>, using the specified directory
        ///   path in the archive, and preserving the full directory structure in the
        ///   filenames.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   Think of the <paramref name="directoryPathInArchive"/> as a "root" or
        ///   base directory used in the archive for the files that get added.  when
        ///   <paramref name="preserveDirHierarchy"/> is true, the hierarchy of files
        ///   found in the filesystem will be placed, with the hierarchy intact,
        ///   starting at that root in the archive. When <c>preserveDirHierarchy</c>
        ///   is false, the path hierarchy of files is flattned, and the flattened
        ///   set of files gets placed in the root within the archive as specified in
        ///   <c>directoryPathInArchive</c>.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to each
        ///   ZipEntry added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="fileNames">
        ///   The names of the files to add. Each string should refer to a file in the
        ///   filesystem.  The name of the file may be a relative path or a
        ///   fully-qualified path.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        ///   Specifies a directory path to use as a prefix for each entry name.
        ///   This path may, or may not, correspond to a real directory in the current
        ///   filesystem.  If the files within the zip are later extracted, this is the
        ///   path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
        ///   VB) will use the path on each of the <c>fileNames</c>, if any.  Passing
        ///   the empty string ("") will insert the item at the root path within the
        ///   archive.
        /// </param>
        ///
        /// <param name="preserveDirHierarchy">
        ///   whether the entries in the zip archive will reflect the directory
        ///   hierarchy that is present in the various filenames.  For example, if
        ///   <paramref name="fileNames"/> includes two paths,
        ///   \Animalia\Chordata\Mammalia\Info.txt and
        ///   \Plantae\Magnoliophyta\Dicotyledon\Info.txt, then calling this method
        ///   with <paramref name="preserveDirHierarchy"/> = <c>false</c> will
        ///   result in an exception because of a duplicate entry name, while
        ///   calling this method with <paramref name="preserveDirHierarchy"/> =
        ///   <c>true</c> will result in the full direcory paths being included in
        ///   the entries added to the ZipFile.
        /// </param>
        /// <seealso cref="Ionic.Zip.ZipFile.AddSelectedFiles(String, String)" />
        public static void AddFiles(this ZipFile zipFile, System.Collections.Generic.IEnumerable<String> fileNames, String directoryPathInArchive, bool preserveDirHierarchy)
        {
            if (fileNames == null)
                throw new ArgumentNullException("fileNames");

            zipFile.SetAddOperationCanceled(false);
            zipFile.SetInAddAll(true);
            try
            {
            zipFile.OnAddStarted();
            if (preserveDirHierarchy)
            {
                foreach (var f in fileNames)
                {
                    if (zipFile.IsAddOperationCanceled()) break;
                    if (directoryPathInArchive != null)
                    {
                        //string s = SharedUtilities.NormalizePath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(f)));
                        string s = ZipEntryInternal.NormalizePath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(f)));
                        zipFile.AddFile(f, s);
                    }
                    else
                        zipFile.AddFile(f, null);
                }
            }
            else
            {
                foreach (var f in fileNames)
                {
                    if (zipFile.IsAddOperationCanceled()) break;
                    zipFile.AddFile(f, directoryPathInArchive);
                }
            }
            if (!zipFile.IsAddOperationCanceled())
                zipFile.OnAddCompleted();
            }
            finally
            {
                zipFile.SetInAddAll(false);
            }
        }

        /// <summary>
        ///   Adds or updates a set of files to the <c>ZipFile</c>, using the specified
        ///   directory path in the archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   Any files that already exist in the archive are updated. Any files that
        ///   don't yet exist in the archive are added.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to each
        ///   ZipEntry added.
        /// </para>
        /// </remarks>
        ///
        /// <param name="fileNames">
        ///   The names of the files to add or update. Each string should refer to a
        ///   file in the filesystem.  The name of the file may be a relative path or a
        ///   fully-qualified path.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        ///   Specifies a directory path to use to override any path in the file name.
        ///   This path may, or may not, correspond to a real directory in the current
        ///   filesystem.  If the files within the zip are later extracted, this is the
        ///   path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
        ///   VB) will use the path on each of the <c>fileNames</c>, if any.  Passing
        ///   the empty string ("") will insert the item at the root path within the
        ///   archive.
        /// </param>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddSelectedFiles(String, String)" />
        public static void UpdateFiles(this ZipFile zipFile, System.Collections.Generic.IEnumerable<String> fileNames, String directoryPathInArchive)
        {
            if (fileNames == null)
                throw new ArgumentNullException("fileNames");

            zipFile.OnAddStarted();
            foreach (var f in fileNames)
                zipFile.UpdateFile(f, directoryPathInArchive);
            zipFile.OnAddCompleted();
        }

        /// <summary>
        ///   Adds or Updates a File in a Zip file archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This method adds a file to a zip archive, or, if the file already exists
        ///   in the zip archive, this method Updates the content of that given filename
        ///   in the zip archive.  The <c>UpdateFile</c> method might more accurately be
        ///   called "AddOrUpdateFile".
        /// </para>
        ///
        /// <para>
        ///   Upon success, there is no way for the application to learn whether the file
        ///   was added versus updated.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to the
        ///   <c>ZipEntry</c> added.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        ///
        ///   This example shows how to Update an existing entry in a zipfile. The first
        ///   call to UpdateFile adds the file to the newly-created zip archive.  The
        ///   second call to UpdateFile updates the content for that file in the zip
        ///   archive.
        ///
        /// <code>
        /// using (ZipFile zip1 = new ZipFile())
        /// {
        ///   // UpdateFile might more accurately be called "AddOrUpdateFile"
        ///   zip1.UpdateFile("MyDocuments\\Readme.txt");
        ///   zip1.UpdateFile("CustomerList.csv");
        ///   zip1.Comment = "This zip archive has been created.";
        ///   zip1.Save("Content.zip");
        /// }
        ///
        /// using (ZipFile zip2 = ZipFile.Read("Content.zip"))
        /// {
        ///   zip2.UpdateFile("Updates\\Readme.txt");
        ///   zip2.Comment = "This zip archive has been updated: The Readme.txt file has been changed.";
        ///   zip2.Save();
        /// }
        ///
        /// </code>
        /// <code lang="VB">
        ///   Using zip1 As New ZipFile
        ///       ' UpdateFile might more accurately be called "AddOrUpdateFile"
        ///       zip1.UpdateFile("MyDocuments\Readme.txt")
        ///       zip1.UpdateFile("CustomerList.csv")
        ///       zip1.Comment = "This zip archive has been created."
        ///       zip1.Save("Content.zip")
        ///   End Using
        ///
        ///   Using zip2 As ZipFile = ZipFile.Read("Content.zip")
        ///       zip2.UpdateFile("Updates\Readme.txt")
        ///       zip2.Comment = "This zip archive has been updated: The Readme.txt file has been changed."
        ///       zip2.Save
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddFile(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string)"/>
        ///
        /// <param name="fileName">
        ///   The name of the file to add or update. It should refer to a file in the
        ///   filesystem.  The name of the file may be a relative path or a
        ///   fully-qualified path.
        /// </param>
        ///
        /// <returns>
        ///   The <c>ZipEntry</c> corresponding to the File that was added or updated.
        /// </returns>
        public static ZipEntry UpdateFile(this ZipFile zipFile, string fileName)
        {
            return zipFile.UpdateFile(fileName, null);
        }

        /// <summary>
        ///   Adds or Updates a File in a Zip file archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This method adds a file to a zip archive, or, if the file already exists
        ///   in the zip archive, this method Updates the content of that given filename
        ///   in the zip archive.
        /// </para>
        ///
        /// <para>
        ///   This version of the method allows the caller to explicitly specify the
        ///   directory path to be used in the archive.  The entry to be added or
        ///   updated is found by using the specified directory path, combined with the
        ///   basename of the specified filename.
        /// </para>
        ///
        /// <para>
        ///   Upon success, there is no way for the application to learn if the file was
        ///   added versus updated.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to the
        ///   <c>ZipEntry</c> added.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddFile(string,string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string,string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string,string)"/>
        ///
        /// <param name="fileName">
        ///   The name of the file to add or update. It should refer to a file in the
        ///   filesystem.  The name of the file may be a relative path or a
        ///   fully-qualified path.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        ///   Specifies a directory path to use to override any path in the
        ///   <c>fileName</c>.  This path may, or may not, correspond to a real
        ///   directory in the current filesystem.  If the files within the zip are
        ///   later extracted, this is the path used for the extracted file.  Passing
        ///   <c>null</c> (<c>Nothing</c> in VB) will use the path on the
        ///   <c>fileName</c>, if any.  Passing the empty string ("") will insert the
        ///   item at the root path within the archive.
        /// </param>
        ///
        /// <returns>
        ///   The <c>ZipEntry</c> corresponding to the File that was added or updated.
        /// </returns>
        public static ZipEntry UpdateFile(this ZipFile zipFile, string fileName, String directoryPathInArchive)
        {
            // ideally this would all be transactional!
            var key = ZipEntryInternal.NameInArchive(fileName, directoryPathInArchive);
            if (zipFile[key] != null)
                zipFile.RemoveEntry(key);
            return zipFile.AddFile(fileName, directoryPathInArchive);
        }

        /// <summary>
        ///   Add or update a directory in a zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///   If the specified directory does not exist in the archive, then this method
        ///   is equivalent to calling <c>AddDirectory()</c>.  If the specified
        ///   directory already exists in the archive, then this method updates any
        ///   existing entries, and adds any new entries. Any entries that are in the
        ///   zip archive but not in the specified directory, are left alone.  In other
        ///   words, the contents of the zip file will be a union of the previous
        ///   contents and the new files.
        /// </remarks>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string)"/>
        ///
        /// <param name="directoryName">
        ///   The path to the directory to be added to the zip archive, or updated in
        ///   the zip archive.
        /// </param>
        ///
        /// <returns>
        /// The <c>ZipEntry</c> corresponding to the Directory that was added or updated.
        /// </returns>
        public static ZipEntry UpdateDirectory(this ZipFile zipFile, string directoryName)
        {
            return zipFile.UpdateDirectory(directoryName, null);
        }

        /// <summary>
        ///   Add or update a directory in the zip archive at the specified root
        ///   directory in the archive.
        /// </summary>
        ///
        /// <remarks>
        ///   If the specified directory does not exist in the archive, then this method
        ///   is equivalent to calling <c>AddDirectory()</c>.  If the specified
        ///   directory already exists in the archive, then this method updates any
        ///   existing entries, and adds any new entries. Any entries that are in the
        ///   zip archive but not in the specified directory, are left alone.  In other
        ///   words, the contents of the zip file will be a union of the previous
        ///   contents and the new files.
        /// </remarks>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string,string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string,string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string,string)"/>
        ///
        /// <param name="directoryName">
        ///   The path to the directory to be added to the zip archive, or updated
        ///   in the zip archive.
        /// </param>
        ///
        /// <param name="directoryPathInArchive">
        ///   Specifies a directory path to use to override any path in the
        ///   <c>directoryName</c>.  This path may, or may not, correspond to a real
        ///   directory in the current filesystem.  If the files within the zip are
        ///   later extracted, this is the path used for the extracted file.  Passing
        ///   <c>null</c> (<c>Nothing</c> in VB) will use the path on the
        ///   <c>directoryName</c>, if any.  Passing the empty string ("") will insert
        ///   the item at the root path within the archive.
        /// </param>
        ///
        /// <returns>
        ///   The <c>ZipEntry</c> corresponding to the Directory that was added or updated.
        /// </returns>
        public static ZipEntry UpdateDirectory(this ZipFile zipFile, string directoryName, String directoryPathInArchive)
        {
            return zipFile.AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOrUpdate);
        }

        /// <summary>
        ///   Add or update a file or directory in the zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This is useful when the application is not sure or does not care if the
        ///   item to be added is a file or directory, and does not know or does not
        ///   care if the item already exists in the <c>ZipFile</c>. Calling this method
        ///   is equivalent to calling <c>RemoveEntry()</c> if an entry by the same name
        ///   already exists, followed calling by <c>AddItem()</c>.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to the
        ///   <c>ZipEntry</c> added.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddItem(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string)"/>
        ///
        /// <param name="itemName">
        ///  the path to the file or directory to be added or updated.
        /// </param>
        public static void UpdateItem(this ZipFile zipFile, string itemName)
        {
            zipFile.UpdateItem(itemName, null);
        }

        /// <summary>
        ///   Add or update a file or directory.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This method is useful when the application is not sure or does not care if
        ///   the item to be added is a file or directory, and does not know or does not
        ///   care if the item already exists in the <c>ZipFile</c>. Calling this method
        ///   is equivalent to calling <c>RemoveEntry()</c>, if an entry by that name
        ///   exists, and then calling <c>AddItem()</c>.
        /// </para>
        ///
        /// <para>
        ///   This version of the method allows the caller to explicitly specify the
        ///   directory path to be used for the item being added to the archive.  The
        ///   entry or entries that are added or updated will use the specified
        ///   <c>DirectoryPathInArchive</c>. Extracting the entry from the archive will
        ///   result in a file stored in that directory path.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to the
        ///   <c>ZipEntry</c> added.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddItem(string, string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string, string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string, string)"/>
        ///
        /// <param name="itemName">
        ///   The path for the File or Directory to be added or updated.
        /// </param>
        /// <param name="directoryPathInArchive">
        ///   Specifies a directory path to use to override any path in the
        ///   <c>itemName</c>.  This path may, or may not, correspond to a real
        ///   directory in the current filesystem.  If the files within the zip are
        ///   later extracted, this is the path used for the extracted file.  Passing
        ///   <c>null</c> (<c>Nothing</c> in VB) will use the path on the
        ///   <c>itemName</c>, if any.  Passing the empty string ("") will insert the
        ///   item at the root path within the archive.
        /// </param>
        public static void UpdateItem(this ZipFile zipFile, string itemName, string directoryPathInArchive)
        {
            string fullPath = GetFullPath(itemName);

            if (FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync() != null)
                zipFile.UpdateFile(itemName, directoryPathInArchive);

            else if (FileSystem.Current.GetFolderFromPathAsync(fullPath).ExecuteSync() != null)
                zipFile.UpdateDirectory(itemName, directoryPathInArchive);

            else
                throw new FileNotFoundException(String.Format("That file or directory ({0}) does not exist!", itemName));
        }

        /// <summary>
        ///   Adds the contents of a filesystem directory to a Zip file archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   The name of the directory may be a relative path or a fully-qualified
        ///   path. Any files within the named directory are added to the archive.  Any
        ///   subdirectories within the named directory are also added to the archive,
        ///   recursively.
        /// </para>
        ///
        /// <para>
        ///   Top-level entries in the named directory will appear as top-level entries
        ///   in the zip archive.  Entries in subdirectories in the named directory will
        ///   result in entries in subdirectories in the zip archive.
        /// </para>
        ///
        /// <para>
        ///   If you want the entries to appear in a containing directory in the zip
        ///   archive itself, then you should call the AddDirectory() overload that
        ///   allows you to explicitly specify a directory path for use in the archive.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to each
        ///   ZipEntry added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddItem(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddFile(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string, string)"/>
        ///
        /// <overloads>This method has 2 overloads.</overloads>
        ///
        /// <param name="directoryName">The name of the directory to add.</param>
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public static ZipEntry AddDirectory(this ZipFile zipFile, string directoryName)
        {
            return zipFile.AddDirectory(directoryName, null);
        }

        /// <summary>
        ///   Adds the contents of a filesystem directory to a Zip file archive,
        ///   overriding the path to be used for entries in the archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   The name of the directory may be a relative path or a fully-qualified
        ///   path. The add operation is recursive, so that any files or subdirectories
        ///   within the name directory are also added to the archive.
        /// </para>
        ///
        /// <para>
        ///   Top-level entries in the named directory will appear as top-level entries
        ///   in the zip archive.  Entries in subdirectories in the named directory will
        ///   result in entries in subdirectories in the zip archive.
        /// </para>
        ///
        /// <para>
        ///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
        ///   cref="Password"/>, <see cref="SetCompression"/>, <see
        ///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
        ///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
        ///   respective values at the time of this call will be applied to each
        ///   ZipEntry added.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// <para>
        ///   In this code, calling the ZipUp() method with a value of "c:\reports" for
        ///   the directory parameter will result in a zip file structure in which all
        ///   entries are contained in a toplevel "reports" directory.
        /// </para>
        ///
        /// <code lang="C#">
        /// public void ZipUp(string targetZip, string directory)
        /// {
        ///   using (var zip = new ZipFile())
        ///   {
        ///     zip.AddDirectory(directory, System.IO.Path.GetFileName(directory));
        ///     zip.Save(targetZip);
        ///   }
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.AddItem(string, string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.AddFile(string, string)"/>
        /// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string, string)"/>
        ///
        /// <param name="directoryName">The name of the directory to add.</param>
        ///
        /// <param name="directoryPathInArchive">
        ///   Specifies a directory path to use to override any path in the
        ///   DirectoryName.  This path may, or may not, correspond to a real directory
        ///   in the current filesystem.  If the zip is later extracted, this is the
        ///   path used for the extracted file or directory.  Passing <c>null</c>
        ///   (<c>Nothing</c> in VB) or the empty string ("") will insert the items at
        ///   the root path within the archive.
        /// </param>
        ///
        /// <returns>The <c>ZipEntry</c> added.</returns>
        public static ZipEntry AddDirectory(this ZipFile zipFile, string directoryName, string directoryPathInArchive)
        {
            return zipFile.AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOnly);
        }

        internal static ZipEntry AddOrUpdateDirectoryImpl(this ZipFile zipFile, string directoryName, string rootDirectoryPathInArchive, AddOrUpdateAction action)
        {
            if (rootDirectoryPathInArchive == null)
            {
                rootDirectoryPathInArchive = "";
            }

            return zipFile.AddOrUpdateDirectoryImpl(directoryName, rootDirectoryPathInArchive, action, true, 0);
        }

        internal static ZipEntry AddOrUpdateDirectoryImpl(this ZipFile zipFile, string directoryName, string rootDirectoryPathInArchive, AddOrUpdateAction action, bool recurse, int level)
        {
            string fullPath = GetFullPath(directoryName);

            if (zipFile.StatusMessageTextWriter != null)
                zipFile.StatusMessageTextWriter.WriteLine("{0} {1}...",
                                                  (action == AddOrUpdateAction.AddOnly) ? "adding" : "Adding or updating",
                                                  directoryName);

            if (level == 0)
            {
                zipFile.SetAddOperationCanceled(false);
                zipFile.OnAddStarted();
            }

            // workitem 13371
            if (zipFile.IsAddOperationCanceled())
                return null;

            string dirForEntries = rootDirectoryPathInArchive;
            ZipEntry baseDir = null;

            if (level > 0)
            {
                int f = directoryName.Length;
                for (int i = level; i > 0; i--)
                    f = directoryName.LastIndexOfAny("/\\".ToCharArray(), f - 1, f - 1);

                dirForEntries = directoryName.Substring(f + 1);
                dirForEntries = Path.Combine(rootDirectoryPathInArchive, dirForEntries);
            }

            // if not top level, or if the root is non-empty, then explicitly add the directory
            if (level > 0 || rootDirectoryPathInArchive != "")
            {
                // add the directory only if it does not exist.
                // It's not an error if it already exists.
                dirForEntries = ZipEntryInternal.NameInArchive(dirForEntries) + '/';
                if (!zipFile.EntryFileNames.Contains(dirForEntries))
                {
                    baseDir = zipFile.AddDirectoryByName(dirForEntries);
                }
            }

            if (!zipFile.IsAddOperationCanceled())
            {
                IFile fileCheck = FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync();
                if (fileCheck != null)
                {
                    throw new IOException(string.Format("That path ({0}) is a file, not a directory!", directoryName));
                }
                IFolder folder = FileSystem.Current.GetFolderFromPathAsync(fullPath).ExecuteSync();
                if (folder == null)
                {
                    throw new FileNotFoundException(string.Format("That folder ({0}) does not exist!", directoryName));
                }
                IList<IFile> files = folder.GetFilesAsync().ExecuteSync();

                if (recurse)
                {
                    // add the files:
                    foreach (IFile file in files)
                    {
                        if (zipFile.IsAddOperationCanceled()) break;
                        if (action == AddOrUpdateAction.AddOnly)
                            zipFile.AddFile(file.Path, dirForEntries);
                        else
                            zipFile.UpdateFile(file.Path, dirForEntries);
                    }

                    if (!zipFile.IsAddOperationCanceled())
                    {
                        // add the subdirectories:
                        IList<IFolder> dirs = folder.GetFoldersAsync().ExecuteSync();
                        foreach (IFolder dir in dirs)
                        {
                                zipFile.AddOrUpdateDirectoryImpl(dir.Path, rootDirectoryPathInArchive, action, recurse, level + 1);
                        }

                    }
                }
            }

            if (level == 0)
                zipFile.OnAddCompleted();

            return baseDir;
        }
    }

    enum AddOrUpdateAction
    {
        AddOnly = 0,
        AddOrUpdate
    }
}
