using System.IO;

namespace Ionic.Zip
{
    public static class FileSystemZip
    {
        /// <summary>
        ///   Create a ZipOutputStream that writes to a filesystem file.
        /// </summary>
        ///
        /// <remarks>
        ///   The <see cref="ZipFile"/> class is generally easier to use when creating
        ///   zip files. The ZipOutputStream offers a different metaphor for creating a
        ///   zip file, based on the <see cref="System.IO.Stream"/> class.
        /// </remarks>
        ///
        /// <param name="zipFileName">
        ///   The name of the zip file to create.
        /// </param>
        ///
        /// <example>
        ///
        ///   This example shows how to create a zip file, using the
        ///   ZipOutputStream class.
        ///
        /// <code lang="C#">
        /// private void Zipup()
        /// {
        ///     if (filesToZip.Count == 0)
        ///     {
        ///         System.Console.WriteLine("Nothing to do.");
        ///         return;
        ///     }
        ///
        ///     using (var output= new ZipOutputStream(outputFileName))
        ///     {
        ///         output.Password = "VerySecret!";
        ///         output.Encryption = EncryptionAlgorithm.WinZipAes256;
        ///
        ///         foreach (string inputFileName in filesToZip)
        ///         {
        ///             System.Console.WriteLine("file: {0}", inputFileName);
        ///
        ///             output.PutNextEntry(inputFileName);
        ///             using (var input = File.Open(inputFileName, FileMode.Open, FileAccess.Read,
        ///                                          FileShare.Read | FileShare.Write ))
        ///             {
        ///                 byte[] buffer= new byte[2048];
        ///                 int n;
        ///                 while ((n= input.Read(buffer,0,buffer.Length)) > 0)
        ///                 {
        ///                     output.Write(buffer,0,n);
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Private Sub Zipup()
        ///     Dim outputFileName As String = "XmlData.zip"
        ///     Dim filesToZip As String() = Directory.GetFiles(".", "*.xml")
        ///     If (filesToZip.Length = 0) Then
        ///         Console.WriteLine("Nothing to do.")
        ///     Else
        ///         Using output As ZipOutputStream = New ZipOutputStream(outputFileName)
        ///             output.Password = "VerySecret!"
        ///             output.Encryption = EncryptionAlgorithm.WinZipAes256
        ///             Dim inputFileName As String
        ///             For Each inputFileName In filesToZip
        ///                 Console.WriteLine("file: {0}", inputFileName)
        ///                 output.PutNextEntry(inputFileName)
        ///                 Using input As FileStream = File.Open(inputFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        ///                     Dim n As Integer
        ///                     Dim buffer As Byte() = New Byte(2048) {}
        ///                     Do While (n = input.Read(buffer, 0, buffer.Length) > 0)
        ///                         output.Write(buffer, 0, n)
        ///                     Loop
        ///                 End Using
        ///             Next
        ///         End Using
        ///     End If
        /// End Sub
        /// </code>
        /// </example>
        public static ZipOutputStream CreateOutputStream(string zipFileName)
        {
            return ZipOutputStreamExtensions.Create(zipFileName);
        }

        /// <summary>
        ///   Create a <c>ZipInputStream</c>, given the name of an existing zip file.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   This constructor opens a <c>FileStream</c> for the given zipfile, and
        ///   wraps a <c>ZipInputStream</c> around that.  See the documentation for the
        ///   <see cref="ZipInputStream(Stream)"/> constructor for full details.
        /// </para>
        ///
        /// <para>
        ///   While the <see cref="ZipFile"/> class is generally easier
        ///   to use, this class provides an alternative to those
        ///   applications that want to read from a zipfile directly,
        ///   using a <see cref="System.IO.Stream"/>.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="zipFileName">
        ///   The name of the filesystem file to read.
        /// </param>
        ///
        /// <example>
        ///
        ///   This example shows how to read a zip file, and extract entries, using the
        ///   <c>ZipInputStream</c> class.
        ///
        /// <code lang="C#">
        /// private void Unzip()
        /// {
        ///     byte[] buffer= new byte[2048];
        ///     int n;
        ///     using (var input= new ZipInputStream(inputFileName))
        ///     {
        ///         ZipEntry e;
        ///         while (( e = input.GetNextEntry()) != null)
        ///         {
        ///             if (e.IsDirectory) continue;
        ///             string outputPath = Path.Combine(extractDir, e.FileName);
        ///             using (var output = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite))
        ///             {
        ///                 while ((n= input.Read(buffer, 0, buffer.Length)) > 0)
        ///                 {
        ///                     output.Write(buffer,0,n);
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Private Sub UnZip()
        ///     Dim inputFileName As String = "MyArchive.zip"
        ///     Dim extractDir As String = "extract"
        ///     Dim buffer As Byte() = New Byte(2048) {}
        ///     Using input As ZipInputStream = New ZipInputStream(inputFileName)
        ///         Dim e As ZipEntry
        ///         Do While (Not e = input.GetNextEntry Is Nothing)
        ///             If Not e.IsDirectory Then
        ///                 Using output As FileStream = File.Open(Path.Combine(extractDir, e.FileName), _
        ///                                                        FileMode.Create, FileAccess.ReadWrite)
        ///                     Dim n As Integer
        ///                     Do While (n = input.Read(buffer, 0, buffer.Length) > 0)
        ///                         output.Write(buffer, 0, n)
        ///                     Loop
        ///                 End Using
        ///             End If
        ///         Loop
        ///     End Using
        /// End Sub
        /// </code>
        /// </example>
        public static ZipInputStream CreateInputStream(string zipFileName)
        {
            return ZipInputStreamExtensions.Create(zipFileName);
        }

        /// <summary>
        /// Reads a zip file archive and returns the instance.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// The stream is read using the default <c>System.Text.Encoding</c>, which is the
        /// <c>IBM437</c> codepage.
        /// </para>
        /// </remarks>
        ///
        /// <exception cref="System.Exception">
        /// Thrown if the <c>ZipFile</c> cannot be read. The implementation of this method
        /// relies on <c>System.IO.File.OpenRead</c>, which can throw a variety of exceptions,
        /// including specific exceptions if a file is not found, an unauthorized access
        /// exception, exceptions for poorly formatted filenames, and so on.
        /// </exception>
        ///
        /// <param name="zipFileName">
        /// The name of the zip archive to open.  This can be a fully-qualified or relative
        /// pathname.
        /// </param>
        ///
        /// <seealso cref="ZipFile.Read(String, ReadOptions)"/>.
        ///
        /// <returns>The instance read from the zip archive.</returns>
        ///
        public static ZipFile Read(string zipFileName)
        {
            return ZipFileExtensions.Read(zipFileName);
        }

        /// <summary>
        /// Reads a zip file archive using the specified text encoding and returns the
        /// instance.
        /// </summary>
        ///
        /// <param name="zipFileName">
        /// The name of the zip archive to open.
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        ///
        /// <param name="encoding">
        /// The <c>System.Text.Encoding</c> to use when reading in the zip archive. Be
        /// careful specifying the encoding.  If the value you use here is not the same
        /// as the Encoding used when the zip archive was created (possibly by a
        /// different archiver) you will get unexpected results and possibly exceptions.
        /// </param>
        ///
        /// <returns>The instance read from the zip archive.</returns>
        ///
        public static ZipFile Read(string zipFileName, System.Text.Encoding encoding)
        {
            return ZipFileExtensions.Read(zipFileName, encoding);
        }

        /// <summary>
        /// Reads a zip file archive using the specified the specified TextWriter for 
        /// status messages and returns the instance.
        /// </summary>
        ///
        /// <param name="zipFileName">
        /// The name of the zip archive to open.
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        ///
        /// <param name="statusMessageWriter">
        /// The <c>System.IO.TextWriter</c> to use for writing verbose status messages
        /// during operations on the zip archive.  A console application may wish to
        /// pass <c>System.Console.Out</c> to get messages on the Console. A graphical
        /// or headless application may wish to capture the messages in a different
        /// <c>TextWriter</c>, such as a <c>System.IO.StringWriter</c>.
        /// </param>
        ///
        /// <returns>The instance read from the zip archive.</returns>
        ///
        public static ZipFile Read(string zipFileName, TextWriter statusMessageWriter)
        {
            return ZipFileExtensions.Read(zipFileName, statusMessageWriter);
        }

        /// <summary>
        ///   Creates a new <c>ZipFile</c> instance, using the specified name for the
        ///   filename, the specified status message writer, and the specified Encoding.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This constructor works like the <see cref="ZipFile(String)">ZipFile
        ///   constructor that accepts a single string argument.</see> See that
        ///   reference for detail on what this constructor does.
        /// </para>
        ///
        /// <para>
        ///   This version of the constructor allows the caller to pass in a
        ///   <c>TextWriter</c>, and an Encoding.  The <c>TextWriter</c> will collect
        ///   verbose messages that are generated by the library during extraction or
        ///   creation of the zip archive.  A console application may wish to pass
        ///   <c>System.Console.Out</c> to get messages on the Console. A graphical or
        ///   headless application may wish to capture the messages in a different
        ///   <c>TextWriter</c>, for example, a <c>StringWriter</c>, and then display
        ///   the messages in a <c>TextBox</c>, or generate an audit log of
        ///   <c>ZipFile</c> operations.
        /// </para>
        ///
        /// <para>
        ///   The <c>Encoding</c> is used as the default alternate encoding for entries
        ///   with filenames or comments that cannot be encoded with the IBM437 code
        ///   page.  This is a equivalent to setting the <see
        ///   cref="ProvisionalAlternateEncoding"/> property on the <c>ZipFile</c>
        ///   instance after construction.
        /// </para>
        ///
        /// <para>
        ///   To encrypt the data for the files added to the <c>ZipFile</c> instance,
        ///   set the <c>Password</c> property after creating the <c>ZipFile</c>
        ///   instance.
        /// </para>
        ///
        /// <para>
        ///   Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
        ///   not party on a single instance with multiple threads.  You may have
        ///   multiple threads that each use a distinct <c>ZipFile</c> instance, or you
        ///   can synchronize multi-thread access to a single instance.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <exception cref="Ionic.Zip.ZipException">
        /// Thrown if <c>fileName</c> refers to an existing file that is not a valid zip file.
        /// </exception>
        ///
        /// <param name="zipFileName">The filename to use for the new zip archive.</param>
        /// <param name="statusMessageWriter">A TextWriter to use for writing verbose
        /// status messages.</param>
        /// <param name="encoding">
        /// The Encoding is used as the default alternate encoding for entries with
        /// filenames or comments that cannot be encoded with the IBM437 code page.
        /// </param>
        public static ZipFile Read(string zipFileName, TextWriter statusMessageWriter, System.Text.Encoding encoding)
        {
            return ZipFileExtensions.Read(zipFileName, statusMessageWriter, encoding);
        }

        /// <summary>
        ///   Reads a zip file archive from the named filesystem file using the
        ///   specified options.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This version of the <c>Read()</c> method allows the caller to pass
        ///   in a <c>TextWriter</c> an <c>Encoding</c>, via an instance of the
        ///   <c>ReadOptions</c> class.  The <c>ZipFile</c> is read in using the
        ///   specified encoding for entries where UTF-8 encoding is not
        ///   explicitly specified.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        ///
        /// <para>
        ///   This example shows how to read a zip file using the Big-5 Chinese
        ///   code page (950), and extract each entry in the zip file, while
        ///   sending status messages out to the Console.
        /// </para>
        ///
        /// <para>
        ///   For this code to work as intended, the zipfile must have been
        ///   created using the big5 code page (CP950). This is typical, for
        ///   example, when using WinRar on a machine with CP950 set as the
        ///   default code page.  In that case, the names of entries within the
        ///   Zip archive will be stored in that code page, and reading the zip
        ///   archive must be done using that code page.  If the application did
        ///   not use the correct code page in ZipFile.Read(), then names of
        ///   entries within the zip archive would not be correctly retrieved.
        /// </para>
        ///
        /// <code lang="C#">
        /// string zipToExtract = "MyArchive.zip";
        /// string extractDirectory = "extract";
        /// var options = new ReadOptions
        /// {
        ///   StatusMessageWriter = System.Console.Out,
        ///   Encoding = System.Text.Encoding.GetEncoding(950)
        /// };
        /// using (ZipFile zip = ZipFile.Read(zipToExtract, options))
        /// {
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///      e.Extract(extractDirectory);
        ///   }
        /// }
        /// </code>
        ///
        ///
        /// <code lang="VB">
        /// Dim zipToExtract as String = "MyArchive.zip"
        /// Dim extractDirectory as String = "extract"
        /// Dim options as New ReadOptions
        /// options.Encoding = System.Text.Encoding.GetEncoding(950)
        /// options.StatusMessageWriter = System.Console.Out
        /// Using zip As ZipFile = ZipFile.Read(zipToExtract, options)
        ///     Dim e As ZipEntry
        ///     For Each e In zip
        ///      e.Extract(extractDirectory)
        ///     Next
        /// End Using
        /// </code>
        /// </example>
        ///
        ///
        /// <example>
        ///
        /// <para>
        ///   This example shows how to read a zip file using the default
        ///   code page, to remove entries that have a modified date before a given threshold,
        ///   sending status messages out to a <c>StringWriter</c>.
        /// </para>
        ///
        /// <code lang="C#">
        /// var options = new ReadOptions
        /// {
        ///   StatusMessageWriter = new System.IO.StringWriter()
        /// };
        /// using (ZipFile zip =  ZipFile.Read("PackedDocuments.zip", options))
        /// {
        ///   var Threshold = new DateTime(2007,7,4);
        ///   // We cannot remove the entry from the list, within the context of
        ///   // an enumeration of said list.
        ///   // So we add the doomed entry to a list to be removed later.
        ///   // pass 1: mark the entries for removal
        ///   var MarkedEntries = new System.Collections.Generic.List&lt;ZipEntry&gt;();
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///     if (e.LastModified &lt; Threshold)
        ///       MarkedEntries.Add(e);
        ///   }
        ///   // pass 2: actually remove the entry.
        ///   foreach (ZipEntry zombie in MarkedEntries)
        ///      zip.RemoveEntry(zombie);
        ///   zip.Comment = "This archive has been updated.";
        ///   zip.Save();
        /// }
        /// // can now use contents of sw, eg store in an audit log
        /// </code>
        ///
        /// <code lang="VB">
        /// Dim options as New ReadOptions
        /// options.StatusMessageWriter = New System.IO.StringWriter
        /// Using zip As ZipFile = ZipFile.Read("PackedDocuments.zip", options)
        ///     Dim Threshold As New DateTime(2007, 7, 4)
        ///     ' We cannot remove the entry from the list, within the context of
        ///     ' an enumeration of said list.
        ///     ' So we add the doomed entry to a list to be removed later.
        ///     ' pass 1: mark the entries for removal
        ///     Dim MarkedEntries As New System.Collections.Generic.List(Of ZipEntry)
        ///     Dim e As ZipEntry
        ///     For Each e In zip
        ///         If (e.LastModified &lt; Threshold) Then
        ///             MarkedEntries.Add(e)
        ///         End If
        ///     Next
        ///     ' pass 2: actually remove the entry.
        ///     Dim zombie As ZipEntry
        ///     For Each zombie In MarkedEntries
        ///         zip.RemoveEntry(zombie)
        ///     Next
        ///     zip.Comment = "This archive has been updated."
        ///     zip.Save
        /// End Using
        /// ' can now use contents of sw, eg store in an audit log
        /// </code>
        /// </example>
        ///
        /// <exception cref="System.Exception">
        ///   Thrown if the zipfile cannot be read. The implementation of
        ///   this method relies on <c>System.IO.File.OpenRead</c>, which
        ///   can throw a variety of exceptions, including specific
        ///   exceptions if a file is not found, an unauthorized access
        ///   exception, exceptions for poorly formatted filenames, and so
        ///   on.
        /// </exception>
        ///
        /// <param name="zipFileName">
        /// The name of the zip archive to open.
        /// This can be a fully-qualified or relative pathname.
        /// </param>
        ///
        /// <param name="options">
        /// The set of options to use when reading the zip file.
        /// </param>
        ///
        /// <returns>The ZipFile instance read from the zip archive.</returns>
        ///
        /// <seealso cref="ZipFile.Read(Stream, ReadOptions)"/>
        ///
        public static ZipFile Read(string zipFileName, ReadOptions options)
        {
            return ZipFileExtensions.Read(zipFileName, options);
        }

        /// <summary>
        ///   Reads a zip archive from a stream.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   When reading from a file, it's probably easier to just use
        ///   <see cref="ZipFile.Read(String,
        ///   ReadOptions)">ZipFile.Read(String, ReadOptions)</see>.  This
        ///   overload is useful when when the zip archive content is
        ///   available from an already-open stream. The stream must be
        ///   open and readable and seekable when calling this method.  The
        ///   stream is left open when the reading is completed.
        /// </para>
        ///
        /// <para>
        ///   Using this overload, the stream is read using the default
        ///   <c>System.Text.Encoding</c>, which is the <c>IBM437</c>
        ///   codepage. If you want to specify the encoding to use when
        ///   reading the zipfile content, see
        ///   <see cref="ZipFile.Read(Stream,
        ///   ReadOptions)">ZipFile.Read(Stream, ReadOptions)</see>.  This
        /// </para>
        ///
        /// <para>
        ///   Reading of zip content begins at the current position in the
        ///   stream.  This means if you have a stream that concatenates
        ///   regular data and zip data, if you position the open, readable
        ///   stream at the start of the zip data, you will be able to read
        ///   the zip archive using this constructor, or any of the ZipFile
        ///   constructors that accept a <see cref="System.IO.Stream" /> as
        ///   input. Some examples of where this might be useful: the zip
        ///   content is concatenated at the end of a regular EXE file, as
        ///   some self-extracting archives do.  (Note: SFX files produced
        ///   by DotNetZip do not work this way; they can be read as normal
        ///   ZIP files). Another example might be a stream being read from
        ///   a database, where the zip content is embedded within an
        ///   aggregate stream of data.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// <para>
        ///   This example shows how to Read zip content from a stream, and
        ///   extract one entry into a different stream. In this example,
        ///   the filename "NameOfEntryInArchive.doc", refers only to the
        ///   name of the entry within the zip archive.  A file by that
        ///   name is not created in the filesystem.  The I/O is done
        ///   strictly with the given streams.
        /// </para>
        ///
        /// <code>
        /// using (ZipFile zip = ZipFile.Read(InputStream))
        /// {
        ///    zip.Extract("NameOfEntryInArchive.doc", OutputStream);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip as ZipFile = ZipFile.Read(InputStream)
        ///    zip.Extract("NameOfEntryInArchive.doc", OutputStream)
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        ///
        /// <returns>The ZipFile instance read from the stream</returns>
        ///
        public static ZipFile Read(Stream zipStream)
        {
            return ZipFile.Read(zipStream);
        }

        /// <summary>
        ///   Reads a zip file archive from the given stream using the
        ///   specified options.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   When reading from a file, it's probably easier to just use
        ///   <see cref="ZipFile.Read(String,
        ///   ReadOptions)">ZipFile.Read(String, ReadOptions)</see>.  This
        ///   overload is useful when when the zip archive content is
        ///   available from an already-open stream. The stream must be
        ///   open and readable and seekable when calling this method.  The
        ///   stream is left open when the reading is completed.
        /// </para>
        ///
        /// <para>
        ///   Reading of zip content begins at the current position in the
        ///   stream.  This means if you have a stream that concatenates
        ///   regular data and zip data, if you position the open, readable
        ///   stream at the start of the zip data, you will be able to read
        ///   the zip archive using this constructor, or any of the ZipFile
        ///   constructors that accept a <see cref="System.IO.Stream" /> as
        ///   input. Some examples of where this might be useful: the zip
        ///   content is concatenated at the end of a regular EXE file, as
        ///   some self-extracting archives do.  (Note: SFX files produced
        ///   by DotNetZip do not work this way; they can be read as normal
        ///   ZIP files). Another example might be a stream being read from
        ///   a database, where the zip content is embedded within an
        ///   aggregate stream of data.
        /// </para>
        /// </remarks>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        ///
        /// <param name="options">
        ///   The set of options to use when reading the zip file.
        /// </param>
        ///
        /// <exception cref="System.Exception">
        ///   Thrown if the zip archive cannot be read.
        /// </exception>
        ///
        /// <returns>The ZipFile instance read from the stream.</returns>
        ///
        /// <seealso cref="ZipFile.Read(String, ReadOptions)"/>
        ///
        public static ZipFile Read(Stream zipStream, ReadOptions options)
        {
            return ZipFile.Read(zipStream, options);
        }
        
        public static ZipFile Read(Stream zipStream, ZipSegmentedStreamManager segmentsManager)
        {
            return ZipFile.Read(zipStream, segmentsManager);
        }

        public static ZipFile Read(Stream zipStream, ZipSegmentedStreamManager segmentsManager, ReadOptions options)
        {
            return ZipFile.Read(zipStream, segmentsManager, options);
        }
        
        /// <summary>
        /// Checks the given file to see if it appears to be a valid zip file.
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        ///   Calling this method is equivalent to calling <see cref="IsZipFile(string,
        ///   bool)"/> with the testExtract parameter set to false.
        /// </para>
        /// </remarks>
        ///
        /// <param name="zipFileName">The file to check.</param>
        /// <returns>true if the file appears to be a zip file.</returns>
        public static bool IsZipFile(string zipFileName)
        {
            return ZipFileExtensions.IsZipFile(zipFileName);
        }

        /// <summary>
        /// Checks a file to see if it is a valid zip file.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This method opens the specified zip file, reads in the zip archive,
        ///   verifying the ZIP metadata as it reads.
        /// </para>
        ///
        /// <para>
        ///   If everything succeeds, then the method returns true.  If anything fails -
        ///   for example if an incorrect signature or CRC is found, indicating a
        ///   corrupt file, the the method returns false.  This method also returns
        ///   false for a file that does not exist.
        /// </para>
        ///
        /// <para>
        ///   If <paramref name="testExtract"/> is true, as part of its check, this
        ///   method reads in the content for each entry, expands it, and checks CRCs.
        ///   This provides an additional check beyond verifying the zip header and
        ///   directory data.
        /// </para>
        ///
        /// <para>
        ///   If <paramref name="testExtract"/> is true, and if any of the zip entries
        ///   are protected with a password, this method will return false.  If you want
        ///   to verify a <c>ZipFile</c> that has entries which are protected with a
        ///   password, you will need to do that manually.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="zipFileName">The zip file to check.</param>
        /// <param name="testExtract">true if the caller wants to extract each entry.</param>
        /// <returns>true if the file contains a valid zip file.</returns>
        public static bool IsZipFile(string zipFileName, bool testExtract)
        {
            return ZipFileExtensions.IsZipFile(zipFileName, testExtract);
        }

        /// <summary>
        /// Checks a stream to see if it contains a valid zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This method reads the zip archive contained in the specified stream, verifying
        /// the ZIP metadata as it reads.  If testExtract is true, this method also extracts
        /// each entry in the archive, dumping all the bits into <see cref="Stream.Null"/>.
        /// </para>
        ///
        /// <para>
        /// If everything succeeds, then the method returns true.  If anything fails -
        /// for example if an incorrect signature or CRC is found, indicating a corrupt
        /// file, the the method returns false.  This method also returns false for a
        /// file that does not exist.
        /// </para>
        ///
        /// <para>
        /// If <c>testExtract</c> is true, this method reads in the content for each
        /// entry, expands it, and checks CRCs.  This provides an additional check
        /// beyond verifying the zip header data.
        /// </para>
        ///
        /// <para>
        /// If <c>testExtract</c> is true, and if any of the zip entries are protected
        /// with a password, this method will return false.  If you want to verify a
        /// ZipFile that has entries which are protected with a password, you will need
        /// to do that manually.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="IsZipFile(string, bool)"/>
        ///
        /// <param name="stream">The stream to check.</param>
        /// <param name="testExtract">true if the caller wants to extract each entry.</param>
        /// <returns>true if the stream contains a valid zip archive.</returns>
        public static bool IsZipFile(Stream stream, bool testExtract)
        {
            return ZipFile.IsZipFile(stream, testExtract);
        }

        /// <summary>
        /// Checks the given file to see if it appears to be a valid zip file.
        /// </summary>
        /// <remarks>
        ///
        /// <para>
        ///   Calling this method is equivalent to calling <see cref="IsZipFile(string,
        ///   bool)"/> with the testExtract parameter set to false.
        /// </para>
        /// </remarks>
        ///
        /// <param name="stream">The stream to check.</param>
        /// <returns>true if the stream appears to be a zip archive.</returns>
        public static bool IsZipFile(Stream stream)
        {
            return ZipFile.IsZipFile(stream);
        }

        /// <summary>
        ///   Checks a zip file to see if its directory is consistent.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   In cases of data error, the directory within a zip file can get out
        ///   of synch with the entries in the zip file.  This method checks the
        ///   given zip file and returns true if this has occurred.
        /// </para>
        ///
        /// <para> This method may take a long time to run for large zip files.  </para>
        ///
        /// <para>
        ///   This method is not supported in the Reduced or Compact Framework
        ///   versions of DotNetZip.
        /// </para>
        ///
        /// <para>
        ///   Developers using COM can use the <see
        ///   cref="ComHelper.CheckZip(String)">ComHelper.CheckZip(String)</see>
        ///   method.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to check.</param>
        ///
        /// <returns>true if the named zip file checks OK. Otherwise, false. </returns>
        ///
        /// <seealso cref="FixZipDirectory(string)"/>
        /// <seealso cref="CheckZip(string,bool,System.IO.TextWriter)"/>
        public static bool CheckZip(string zipFileName)
        {
            return ZipFileExtensions.CheckZip(zipFileName);
        }

        /// <summary>
        ///   Checks a zip file to see if its directory is consistent,
        ///   and optionally fixes the directory if necessary.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   In cases of data error, the directory within a zip file can get out of
        ///   synch with the entries in the zip file.  This method checks the given
        ///   zip file, and returns true if this has occurred. It also optionally
        ///   fixes the zipfile, saving the fixed copy in <em>Name</em>_Fixed.zip.
        /// </para>
        ///
        /// <para>
        ///   This method may take a long time to run for large zip files.  It
        ///   will take even longer if the file actually needs to be fixed, and if
        ///   <c>fixIfNecessary</c> is true.
        /// </para>
        ///
        /// <para>
        ///   This method is not supported in the Reduced or Compact
        ///   Framework versions of DotNetZip.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to check.</param>
        ///
        /// <param name="fixIfNecessary">If true, the method will fix the zip file if
        ///     necessary.</param>
        ///
        /// <param name="writer">
        /// a TextWriter in which messages generated while checking will be written.
        /// </param>
        ///
        /// <returns>true if the named zip is OK; false if the file needs to be fixed.</returns>
        ///
        /// <seealso cref="CheckZip(string)"/>
        /// <seealso cref="FixZipDirectory(string)"/>
        public static bool CheckZip(string zipFileName, bool fixIfNecessary, TextWriter writer)
        {
            return ZipFileExtensions.CheckZip(zipFileName, fixIfNecessary, writer);
        }

        /// <summary>
        ///   Rewrite the directory within a zipfile.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   In cases of data error, the directory in a zip file can get out of
        ///   synch with the entries in the zip file.  This method attempts to fix
        ///   the zip file if this has occurred.
        /// </para>
        ///
        /// <para> This can take a long time for large zip files. </para>
        ///
        /// <para> This won't work if the zip file uses a non-standard
        /// code page - neither IBM437 nor UTF-8. </para>
        ///
        /// <para>
        ///   This method is not supported in the Reduced or Compact Framework
        ///   versions of DotNetZip.
        /// </para>
        ///
        /// <para>
        ///   Developers using COM can use the <see
        ///   cref="ComHelper.FixZipDirectory(String)">ComHelper.FixZipDirectory(String)</see>
        ///   method.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to fix.</param>
        ///
        /// <seealso cref="CheckZip(string)"/>
        /// <seealso cref="CheckZip(string,bool,System.IO.TextWriter)"/>
        public static void FixZipDirectory(string zipFileName)
        {
            ZipFileExtensions.FixZipDirectory(zipFileName);
        }

        /// <summary>
        ///   Verify the password on a zip file.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     Keep in mind that passwords in zipfiles are applied to
        ///     zip entries, not to the entire zip file. So testing a
        ///     zipfile for a particular password doesn't work in the
        ///     general case. On the other hand, it's often the case
        ///     that a single password will be used on all entries in a
        ///     zip file. This method works for that case.
        ///   </para>
        ///   <para>
        ///     There is no way to check a password without doing the
        ///     decryption. So this code decrypts and extracts the given
        ///     zipfile into <see cref="System.IO.Stream.Null"/>
        ///   </para>
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to fix.</param>
        ///
        /// <param name="password">The password to check.</param>
        ///
        /// <returns>a bool indicating whether the password matches.</returns>
        public static bool CheckZipPassword(string zipFileName, string password)
        {
            return ZipFileExtensions.CheckZipPassword(zipFileName, password);
        }

        /// <summary>
        ///   Checks a zip file to see if its directory is consistent.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   In cases of data error, the directory within a zip file can get out
        ///   of synch with the entries in the zip file.  This method checks the
        ///   given zip file and returns true if this has occurred.
        /// </para>
        ///
        /// <para> This method may take a long time to run for large zip files.  </para>
        ///
        /// <para>
        ///   This method is not supported in the Reduced or Compact Framework
        ///   versions of DotNetZip.
        /// </para>
        ///
        /// <para>
        ///   Developers using COM can use the <see
        ///   cref="ComHelper.CheckZip(String)">ComHelper.CheckZip(String)</see>
        ///   method.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to check.</param>
        ///
        /// <returns>true if the named zip file checks OK. Otherwise, false. </returns>
        ///
        /// <seealso cref="FixZipDirectory(string)"/>
        /// <seealso cref="CheckZip(string,bool,System.IO.TextWriter)"/>
        public static bool CheckZip(Stream zipFileStream)
        {
            return ZipFile.CheckZip(zipFileStream);
        }

        /// <summary>
        ///   Checks a zip file to see if its directory is consistent.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   In cases of data error, the directory within a zip file can get out
        ///   of synch with the entries in the zip file.  This method checks the
        ///   given zip file and returns true if this has occurred.
        /// </para>
        ///
        /// <para> This method may take a long time to run for large zip files.  </para>
        ///
        /// <para>
        ///   This method is not supported in the Reduced or Compact Framework
        ///   versions of DotNetZip.
        /// </para>
        ///
        /// <para>
        ///   Developers using COM can use the <see
        ///   cref="ComHelper.CheckZip(String)">ComHelper.CheckZip(String)</see>
        ///   method.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to check.</param>
        ///
        /// <returns>true if the named zip file checks OK. Otherwise, false. </returns>
        ///
        /// <seealso cref="FixZipDirectory(string)"/>
        /// <seealso cref="CheckZip(string,bool,System.IO.TextWriter)"/>
        public static bool CheckZip(Stream zipFileStream, Stream fixedZipFileStream)
        {
            return ZipFile.CheckZip(zipFileStream, fixedZipFileStream);
        }

        /// <summary>
        ///   Checks a zip file to see if its directory is consistent,
        ///   and optionally fixes the directory if necessary.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   In cases of data error, the directory within a zip file can get out of
        ///   synch with the entries in the zip file.  This method checks the given
        ///   zip file, and returns true if this has occurred. It also optionally
        ///   fixes the zipfile, saving the fixed copy in <em>Name</em>_Fixed.zip.
        /// </para>
        ///
        /// <para>
        ///   This method may take a long time to run for large zip files.  It
        ///   will take even longer if the file actually needs to be fixed, and if
        ///   <c>fixIfNecessary</c> is true.
        /// </para>
        ///
        /// <para>
        ///   This method is not supported in the Reduced or Compact
        ///   Framework versions of DotNetZip.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to check.</param>
        ///
        /// <param name="fixIfNecessary">If true, the method will fix the zip file if
        ///     necessary.</param>
        ///
        /// <param name="writer">
        /// a TextWriter in which messages generated while checking will be written.
        /// </param>
        ///
        /// <returns>true if the named zip is OK; false if the file needs to be fixed.</returns>
        ///
        /// <seealso cref="CheckZip(string)"/>
        /// <seealso cref="FixZipDirectory(string)"/>
        public static bool CheckZip(Stream zipFileStream, Stream fixedZipFileStream, TextWriter writer)
        {
            return ZipFile.CheckZip(zipFileStream, fixedZipFileStream, writer);
        }

        /// <summary>
        ///   Verify the password on a zip file.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     Keep in mind that passwords in zipfiles are applied to
        ///     zip entries, not to the entire zip file. So testing a
        ///     zipfile for a particular password doesn't work in the
        ///     general case. On the other hand, it's often the case
        ///     that a single password will be used on all entries in a
        ///     zip file. This method works for that case.
        ///   </para>
        ///   <para>
        ///     There is no way to check a password without doing the
        ///     decryption. So this code decrypts and extracts the given
        ///     zipfile into <see cref="System.IO.Stream.Null"/>
        ///   </para>
        /// </remarks>
        ///
        /// <param name="zipFileName">The filename to of the zip file to fix.</param>
        ///
        /// <param name="password">The password to check.</param>
        ///
        /// <returns>a bool indicating whether the password matches.</returns>
        public static bool CheckZipPassword(Stream zipFileStream, string password)
        {
            return ZipFile.CheckZipPassword(zipFileStream, password);
        }
    }
}
