using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ionic.Zip
{
    public partial class ZipFileExtensions
    {
        /// <summary>
        ///   Creates a new <c>ZipFile</c> instance, using the specified filename.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   Applications can use this constructor to create a new ZipFile for writing,
        ///   or to slurp in an existing zip archive for read and update purposes.
        /// </para>
        ///
        /// <para>
        ///   To create a new zip archive, an application can call this constructor,
        ///   passing the name of a file that does not exist.  The name may be a fully
        ///   qualified path. Then the application can add directories or files to the
        ///   <c>ZipFile</c> via <c>AddDirectory()</c>, <c>AddFile()</c>, <c>AddItem()</c>
        ///   and then write the zip archive to the disk by calling <c>Save()</c>. The
        ///   zip file is not actually opened and written to the disk until the
        ///   application calls <c>ZipFile.Save()</c>.  At that point the new zip file
        ///   with the given name is created.
        /// </para>
        ///
        /// <para>
        ///   If you won't know the name of the <c>Zipfile</c> until the time you call
        ///   <c>ZipFile.Save()</c>, or if you plan to save to a stream (which has no
        ///   name), then you should use the no-argument constructor.
        /// </para>
        ///
        /// <para>
        ///   The application can also call this constructor to read an existing zip
        ///   archive.  passing the name of a valid zip file that does exist. But, it's
        ///   better form to use the static <see cref="ZipFile.Read(String)"/> method,
        ///   passing the name of the zip file, because using <c>ZipFile.Read()</c> in
        ///   your code communicates very clearly what you are doing.  In either case,
        ///   the file is then read into the <c>ZipFile</c> instance.  The app can then
        ///   enumerate the entries or can modify the zip file, for example adding
        ///   entries, removing entries, changing comments, and so on.
        /// </para>
        ///
        /// <para>
        ///   One advantage to this parameterized constructor: it allows applications to
        ///   use the same code to add items to a zip archive, regardless of whether the
        ///   zip file exists.
        /// </para>
        ///
        /// <para>
        ///   Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
        ///   not party on a single instance with multiple threads.  You may have
        ///   multiple threads that each use a distinct <c>ZipFile</c> instance, or you
        ///   can synchronize multi-thread access to a single instance.
        /// </para>
        ///
        /// <para>
        ///   By the way, since DotNetZip is so easy to use, don't you think <see
        ///   href="http://cheeso.members.winisp.net/DotNetZipDonate.aspx">you should
        ///   donate $5 or $10</see>?
        /// </para>
        ///
        /// </remarks>
        ///
        /// <exception cref="Ionic.Zip.ZipException">
        /// Thrown if name refers to an existing file that is not a valid zip file.
        /// </exception>
        ///
        /// <example>
        /// This example shows how to create a zipfile, and add a few files into it.
        /// <code>
        /// String ZipFileToCreate = "archive1.zip";
        /// String DirectoryToZip  = "c:\\reports";
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   zip.AddFiles(filenames, "files");
        ///   zip.Save(ZipFileToCreate);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Dim ZipFileToCreate As String = "archive1.zip"
        /// Dim DirectoryToZip As String = "c:\reports"
        /// Using zip As ZipFile = New ZipFile()
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     zip.AddFiles(filenames, "files")
        ///     zip.Save(ZipFileToCreate)
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="fileName">The filename to use for the new zip archive.</param>
        ///
        public ZipFile(string fileName)
        {
            try
            {
                _InitInstance(fileName, null);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("Could not read {0} as a zip file", fileName), e1);
            }
        }

        /// <summary>
        ///   Creates a new <c>ZipFile</c> instance, using the specified name for the
        ///   filename, and the specified Encoding.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   See the documentation on the <see cref="ZipFile(String)">ZipFile
        ///   constructor that accepts a single string argument</see> for basic
        ///   information on all the <c>ZipFile</c> constructors.
        /// </para>
        ///
        /// <para>
        ///   The Encoding is used as the default alternate encoding for entries with
        ///   filenames or comments that cannot be encoded with the IBM437 code page.
        ///   This is equivalent to setting the <see
        ///   cref="ProvisionalAlternateEncoding"/> property on the <c>ZipFile</c>
        ///   instance after construction.
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
        /// Thrown if name refers to an existing file that is not a valid zip file.
        /// </exception>
        ///
        /// <param name="fileName">The filename to use for the new zip archive.</param>
        /// <param name="encoding">The Encoding is used as the default alternate
        /// encoding for entries with filenames or comments that cannot be encoded
        /// with the IBM437 code page. </param>
        public ZipFile(string fileName, System.Text.Encoding encoding)
        {
            try
            {
                AlternateEncoding = encoding;
                AlternateEncodingUsage = ZipOption.Always;
                _InitInstance(fileName, null);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }

        /// <summary>
        ///   Creates a new <c>ZipFile</c> instance, using the specified name for the
        ///   filename, and the specified status message writer.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   See the documentation on the <see cref="ZipFile(String)">ZipFile
        ///   constructor that accepts a single string argument</see> for basic
        ///   information on all the <c>ZipFile</c> constructors.
        /// </para>
        ///
        /// <para>
        ///   This version of the constructor allows the caller to pass in a TextWriter,
        ///   to which verbose messages will be written during extraction or creation of
        ///   the zip archive.  A console application may wish to pass
        ///   System.Console.Out to get messages on the Console. A graphical or headless
        ///   application may wish to capture the messages in a different
        ///   <c>TextWriter</c>, for example, a <c>StringWriter</c>, and then display
        ///   the messages in a TextBox, or generate an audit log of ZipFile operations.
        /// </para>
        ///
        /// <para>
        ///   To encrypt the data for the files added to the <c>ZipFile</c> instance,
        ///   set the Password property after creating the <c>ZipFile</c> instance.
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
        /// Thrown if name refers to an existing file that is not a valid zip file.
        /// </exception>
        ///
        /// <example>
        /// <code>
        /// using (ZipFile zip = new ZipFile("Backup.zip", Console.Out))
        /// {
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   // note: this code does not recurse subdirectories!
        ///   // Status messages will be written to Console.Out
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   zip.AddFiles(filenames);
        ///   zip.Save();
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile("Backup.zip", Console.Out)
        ///     ' Store all files found in the top level directory, into the zip archive.
        ///     ' note: this code does not recurse subdirectories!
        ///     ' Status messages will be written to Console.Out
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     zip.AddFiles(filenames)
        ///     zip.Save()
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="fileName">The filename to use for the new zip archive.</param>
        /// <param name="statusMessageWriter">A TextWriter to use for writing
        /// verbose status messages.</param>
        public ZipFile(string fileName, TextWriter statusMessageWriter)
        {
            try
            {
                _InitInstance(fileName, statusMessageWriter);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
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
        public ZipFile(string fileName, TextWriter statusMessageWriter,
                       System.Text.Encoding encoding)
        {
            try
            {
                AlternateEncoding = encoding;
                AlternateEncodingUsage = ZipOption.Always;
                _InitInstance(fileName, statusMessageWriter);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }

        /// <summary>
        ///   Initialize a <c>ZipFile</c> instance by reading in a zip file.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   This method is primarily useful from COM Automation environments, when
        ///   reading or extracting zip files. In COM, it is not possible to invoke
        ///   parameterized constructors for a class. A COM Automation application can
        ///   update a zip file by using the <see cref="ZipFile()">default (no argument)
        ///   constructor</see>, then calling <c>Initialize()</c> to read the contents
        ///   of an on-disk zip archive into the <c>ZipFile</c> instance.
        /// </para>
        ///
        /// <para>
        ///   .NET applications are encouraged to use the <c>ZipFile.Read()</c> methods
        ///   for better clarity.
        /// </para>
        ///
        /// </remarks>
        /// <param name="fileName">the name of the existing zip file to read in.</param>
        public void Initialize(string fileName)
        {
            try
            {
                _InitInstance(fileName, null);
            }
            catch (Exception e1)
            {
                throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
            }
        }
    }
}
