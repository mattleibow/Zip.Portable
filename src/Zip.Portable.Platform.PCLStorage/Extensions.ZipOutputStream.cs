using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace Ionic.Zip
{
    public static partial class ZipOutputStreamExtensions
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
        /// <param name="fileName">
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
        public static ZipOutputStream Create(String fileName)
        {
            string fullPath = ZipFileExtensions.GetFullPath(fileName);

            var file = FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync();
            if (file == null)
            {
                var dirName = Path.GetDirectoryName(fullPath);
                var dir = ZipEntryExtensions.CreateDirectory(dirName, true);
                file = dir.CreateFileAsync(Path.GetFileName(fileName), CreationCollisionOption.ReplaceExisting).ExecuteSync();
            }
            var stream = file.OpenAsync(FileAccess.ReadAndWrite).ExecuteSync();
            return new ZipOutputStream(stream);
        }
    }
}
