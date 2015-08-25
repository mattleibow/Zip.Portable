using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace Ionic.Zip
{
    public static partial class ZipInputStreamExtensions
    {
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
        /// <param name="fileName">
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
        internal static ZipInputStream Create(String fileName)
        {
            string fullPath = ZipFileExtensions.GetFullPath(fileName);

            var file = FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync();
            if (file == null)
            {
                throw new FileNotFoundException(string.Format("That file ({0}) does not exist!", fileName));
            }
            var stream = file.OpenAsync(FileAccess.Read).ExecuteSync();
            return new ZipInputStream(stream);
        }
    }
}
