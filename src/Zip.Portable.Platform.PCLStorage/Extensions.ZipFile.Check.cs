using System;
using System.IO;
using System.Collections.Generic;
using PCLStorage;

namespace Ionic.Zip
{
    public static partial class ZipFileExtensions
    {
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
            return CheckZip(zipFileName, false, null);
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
            bool isOk = true;

            string fullPath = GetFullPath(zipFileName);

            // load the source file
            var file = FileSystem.Current.GetFileFromPathAsync(zipFileName).ExecuteSync();
            if (file == null)
            {
                throw new FileNotFoundException(string.Format("That file ({0}) does not exist!", zipFileName));
            }

            // create the "fixed" file location
            var dir = FileSystem.Current.GetFolderFromPathAsync(Path.GetDirectoryName(fullPath)).ExecuteSync();
            var newFile = dir.CreateFileAsync(
                string.Format("{0}_fixed{1}", Path.GetFileNameWithoutExtension(zipFileName), Path.GetExtension(zipFileName)),
                CreationCollisionOption.FailIfExists).ExecuteSync();

            // do the check
            using (var stream = file.OpenAsync(FileAccess.Read).ExecuteSync())
            using (var newStream = newFile.OpenAsync(FileAccess.Read).ExecuteSync())
            {
                isOk = ZipFile.CheckZip(stream, fixIfNecessary ? newStream : null, writer);
            }

            // delete the temporary file if there was no error
            if (isOk)
            {
                newFile.DeleteAsync().ExecuteSync();
            }

            return isOk;
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
            using (var zip = ZipFileExtensions.Read(zipFileName, new ReadOptions { FullScan = true }))
            {
                zip.Save(zipFileName, true);
            }
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
            string fullPath = GetFullPath(zipFileName);

            var file = FileSystem.Current.GetFileFromPathAsync(fullPath).ExecuteSync();
            if (file == null)
            {
                throw new FileNotFoundException(string.Format("That file ({0}) does not exist!", zipFileName));
            }
            using (var stream = file.OpenAsync(FileAccess.Read).ExecuteSync())
            {
                return ZipFile.CheckZipPassword(stream, password);
            }
        }
    }
}
