﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Ionic.Zip.PlatformSupport;

namespace Ionic.Zip
{
    public partial class ZipFileExtensions
    {
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
        /// <param name="fileName">
        /// The name of the zip archive to open.  This can be a fully-qualified or relative
        /// pathname.
        /// </param>
        ///
        /// <seealso cref="ZipFile.Read(String, ReadOptions)"/>.
        ///
        /// <returns>The instance read from the zip archive.</returns>
        ///
        public static ZipFile Read(string fileName)
        {
            return Read(fileName, new ReadOptions());
        }

        /// <summary>
        /// Reads a zip file archive using the specified text encoding and returns the
        /// instance.
        /// </summary>
        ///
        /// <param name="fileName">
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
        public static ZipFile Read(string fileName, System.Text.Encoding encoding)
        {
            return Read(fileName, new ReadOptions { Encoding = encoding });
        }

        /// <summary>
        /// Reads a zip file archive using the specified the specified TextWriter for 
        /// status messages and returns the instance.
        /// </summary>
        ///
        /// <param name="fileName">
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
        public static ZipFile Read(string fileName, TextWriter statusMessageWriter)
        {
            return Read(fileName, new ReadOptions { StatusMessageWriter = statusMessageWriter });
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
        /// <param name="fileName">The filename to use for the new zip archive.</param>
        /// <param name="statusMessageWriter">A TextWriter to use for writing verbose
        /// status messages.</param>
        /// <param name="encoding">
        /// The Encoding is used as the default alternate encoding for entries with
        /// filenames or comments that cannot be encoded with the IBM437 code page.
        /// </param>
        public static ZipFile Read(string fileName, TextWriter statusMessageWriter, System.Text.Encoding encoding)
        {
            return Read(fileName, new ReadOptions { Encoding = encoding, StatusMessageWriter = statusMessageWriter });
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
        /// <param name="fileName">
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
        public static ZipFile Read(string fileName, ReadOptions options)
        {
            Stream stream = null;
            try
            {
                string fullPath = GetFullPath(fileName);

                var file = FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync();
                if (file == null)
                {
                    throw new FileNotFoundException(string.Format("That file ({0}) does not exist!", fileName));
                }
                // don't close the stream, leave it up to the zip file
                stream = file.OpenAsync(FileAccess.Read).ExecuteSync();
                // handle segmented archives
                var segmentsManager = new FileSystemZipSegmentedStreamManager(fullPath);
                var zipFile = ZipFile.Read(stream, segmentsManager, options);
                zipFile.SetShouldDisposeReadStream(true);
                zipFile.Name = fullPath;
                return zipFile;
            }
            catch (Exception e1)
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
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
        /// <param name="fileName">The file to check.</param>
        /// <returns>true if the file appears to be a zip file.</returns>
        public static bool IsZipFile(string fileName)
        {
            return IsZipFile(fileName, false);
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
        /// <param name="fileName">The zip file to check.</param>
        /// <param name="testExtract">true if the caller wants to extract each entry.</param>
        /// <returns>true if the file contains a valid zip file.</returns>
        public static bool IsZipFile(string fileName, bool testExtract)
        {
            bool result = false;
            try
            {
                string fullPath = GetFullPath(fileName);

                var file = FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync();
                if (file == null) return false;

                using (var s = file.OpenAsync(FileAccess.Read).ExecuteSync())
                {
                    result = ZipFile.IsZipFile(s, testExtract);
                }
            }
            catch (IOException) { }
            catch (ZipException) { }
            return result;
        }
    }
}
