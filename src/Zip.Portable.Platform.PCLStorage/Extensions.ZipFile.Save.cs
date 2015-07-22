using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ionic.Zip
{
    public partial class ZipFileExtensions
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
        public void Save(String fileName)
        {
            // Check for the case where we are re-saving a zip archive
            // that was originally instantiated with a stream.  In that case,
            // the _name will be null. If so, we set _writestream to null,
            // which insures that we'll cons up a new WriteStream (with a filesystem
            // file backing it) in the Save() method.
            if (_name == null)
                _writestream = null;

            else _readName = _name; // workitem 13915

            _name = fileName;
            if (Directory.Exists(_name))
                throw new ZipException("Bad Directory", new System.ArgumentException("That name specifies an existing directory. Please specify a filename.", "fileName"));
            _contentsChanged = true;
            _fileAlreadyExists = File.Exists(_name);
            Save();
        }
    }
}
