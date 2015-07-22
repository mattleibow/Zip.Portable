// ZipFile.cs
//
// Copyright (c) 2006-2010 Dino Chiesa
// All rights reserved.
//
// This module is part of DotNetZip, a zipfile class library.
// The class library reads and writes zip files, according to the format
// described by PKware, at:
// http://www.pkware.com/business_and_developers/developer/popups/appnote.txt
//
//
// There are other Zip class libraries available.
//
// - it is possible to read and write zip files within .NET via the J# runtime.
//   But some people don't like to install the extra DLL, which is no longer
//   supported by MS. And also, the J# libraries don't support advanced zip
//   features, like ZIP64, spanned archives, or AES encryption.
//
// - There are third-party GPL and LGPL libraries available. Some people don't
//   like the license, and some of them don't support all the ZIP features, like AES.
//
// - Finally, there are commercial tools (From ComponentOne, XCeed, etc).  But
//   some people don't want to incur the cost.
//
// This alternative implementation is **not** GPL licensed. It is free of cost, and
// does not require J#. It does require .NET 2.0.  It balances a good set of
// features, with ease of use and speed of performance.
//
// This code is released under the Microsoft Public License .
// See the License.txt for details.
//
//
// NB: This implementation originally relied on the
// System.IO.Compression.DeflateStream base class in the .NET Framework
// v2.0 base class library, but now includes a managed-code port of Zlib.
//
// Thu, 08 Oct 2009  17:04
//


using System;
using System.IO;
using System.Collections.Generic;
using Interop = System.Runtime.InteropServices;


namespace Ionic.Zip
{
    /// <summary>
    ///   The ZipFile type represents a zip archive file.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   This is the main type in the DotNetZip class library. This class reads and
    ///   writes zip files, as defined in the <see
    ///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">specification
    ///   for zip files described by PKWare</see>.  The compression for this
    ///   implementation is provided by a managed-code version of Zlib, included with
    ///   DotNetZip in the classes in the Ionic.Zlib namespace.
    /// </para>
    ///
    /// <para>
    ///   This class provides a general purpose zip file capability.  Use it to read,
    ///   create, or update zip files.  When you want to create zip files using a
    ///   <c>Stream</c> type to write the zip file, you may want to consider the <see
    ///   cref="ZipOutputStream"/> class.
    /// </para>
    ///
    /// <para>
    ///   Both the <c>ZipOutputStream</c> class and the <c>ZipFile</c> class can
    ///   be used to create zip files. Both of them support many of the common zip
    ///   features, including Unicode, different compression methods and levels,
    ///   and ZIP64. They provide very similar performance when creating zip
    ///   files.
    /// </para>
    ///
    /// <para>
    ///   The <c>ZipFile</c> class is generally easier to use than
    ///   <c>ZipOutputStream</c> and should be considered a higher-level interface.  For
    ///   example, when creating a zip file via calls to the <c>PutNextEntry()</c> and
    ///   <c>Write()</c> methods on the <c>ZipOutputStream</c> class, the caller is
    ///   responsible for opening the file, reading the bytes from the file, writing
    ///   those bytes into the <c>ZipOutputStream</c>, setting the attributes on the
    ///   <c>ZipEntry</c>, and setting the created, last modified, and last accessed
    ///   timestamps on the zip entry. All of these things are done automatically by a
    ///   call to <see cref="ZipFile.AddFile(string,string)">ZipFile.AddFile()</see>.
    ///   For this reason, the <c>ZipOutputStream</c> is generally recommended for use
    ///   only when your application emits arbitrary data, not necessarily data from a
    ///   filesystem file, directly into a zip file, and does so using a <c>Stream</c>
    ///   metaphor.
    /// </para>
    ///
    /// <para>
    ///   Aside from the differences in programming model, there are other
    ///   differences in capability between the two classes.
    /// </para>
    ///
    /// <list type="bullet">
    ///   <item>
    ///     <c>ZipFile</c> can be used to read and extract zip files, in addition to
    ///     creating zip files. <c>ZipOutputStream</c> cannot read zip files. If you want
    ///     to use a stream to read zip files, check out the <see cref="ZipInputStream"/> class.
    ///   </item>
    ///
    ///   <item>
    ///     <c>ZipOutputStream</c> does not support the creation of segmented or spanned
    ///     zip files.
    ///   </item>
    ///
    ///   <item>
    ///     <c>ZipOutputStream</c> cannot produce a self-extracting archive.
    ///   </item>
    /// </list>
    ///
    /// <para>
    ///   Be aware that the <c>ZipFile</c> class implements the <see
    ///   cref="System.IDisposable"/> interface.  In order for <c>ZipFile</c> to
    ///   produce a valid zip file, you use use it within a using clause (<c>Using</c>
    ///   in VB), or call the <c>Dispose()</c> method explicitly.  See the examples
    ///   for how to employ a using clause.
    /// </para>
    ///
    /// </remarks>
    public partial class ZipFile :
    System.Collections.IEnumerable,
    System.Collections.Generic.IEnumerable<ZipEntry>,
    IDisposable
    {

        #region public properties

        /// <summary>
        /// Indicates whether to perform a full scan of the zip file when reading it.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   You almost never want to use this property.
        /// </para>
        ///
        /// <para>
        ///   When reading a zip file, if this flag is <c>true</c> (<c>True</c> in
        ///   VB), the entire zip archive will be scanned and searched for entries.
        ///   For large archives, this can take a very, long time. The much more
        ///   efficient default behavior is to read the zip directory, which is
        ///   stored at the end of the zip file. But, in some cases the directory is
        ///   corrupted and you need to perform a full scan of the zip file to
        ///   determine the contents of the zip file. This property lets you do
        ///   that, when necessary.
        /// </para>
        ///
        /// <para>
        ///   This flag is effective only when calling <see
        ///   cref="Initialize(string)"/>. Normally you would read a ZipFile with the
        ///   static <see cref="ZipFile.Read(String)">ZipFile.Read</see>
        ///   method. But you can't set the <c>FullScan</c> property on the
        ///   <c>ZipFile</c> instance when you use a static factory method like
        ///   <c>ZipFile.Read</c>.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        ///
        ///   This example shows how to read a zip file using the full scan approach,
        ///   and then save it, thereby producing a corrected zip file.
        ///
        /// <code lang="C#">
        /// using (var zip = new ZipFile())
        /// {
        ///     zip.FullScan = true;
        ///     zip.Initialize(zipFileName);
        ///     zip.Save(newName);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     zip.FullScan = True
        ///     zip.Initialize(zipFileName)
        ///     zip.Save(newName)
        /// End Using
        /// </code>
        /// </example>
        ///
        public bool FullScan
        {
            get;
            set;
        }


        /// <summary>
        ///   Whether to sort the ZipEntries before saving the file.
        /// </summary>
        ///
        /// <remarks>
        ///   The default is false.  If you have a large number of zip entries, the sort
        ///   alone can consume significant time.
        /// </remarks>
        ///
        /// <example>
        /// <code lang="C#">
        /// using (var zip = new ZipFile())
        /// {
        ///     zip.AddFiles(filesToAdd);
        ///     zip.SortEntriesBeforeSaving = true;
        ///     zip.Save(name);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     zip.AddFiles(filesToAdd)
        ///     zip.SortEntriesBeforeSaving = True
        ///     zip.Save(name)
        /// End Using
        /// </code>
        /// </example>
        ///
        public bool SortEntriesBeforeSaving
        {
            get;
            set;
        }





        /// <summary>
        ///   Size of the IO buffer used while saving.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   First, let me say that you really don't need to bother with this.  It is
        ///   here to allow for optimizations that you probably won't make! It will work
        ///   fine if you don't set or get this property at all. Ok?
        /// </para>
        ///
        /// <para>
        ///   Now that we have <em>that</em> out of the way, the fine print: This
        ///   property affects the size of the buffer that is used for I/O for each
        ///   entry contained in the zip file. When a file is read in to be compressed,
        ///   it uses a buffer given by the size here.  When you update a zip file, the
        ///   data for unmodified entries is copied from the first zip file to the
        ///   other, through a buffer given by the size here.
        /// </para>
        ///
        /// <para>
        ///   Changing the buffer size affects a few things: first, for larger buffer
        ///   sizes, the memory used by the <c>ZipFile</c>, obviously, will be larger
        ///   during I/O operations.  This may make operations faster for very much
        ///   larger files.  Last, for any given entry, when you use a larger buffer
        ///   there will be fewer progress events during I/O operations, because there's
        ///   one progress event generated for each time the buffer is filled and then
        ///   emptied.
        /// </para>
        ///
        /// <para>
        ///   The default buffer size is 8k.  Increasing the buffer size may speed
        ///   things up as you compress larger files.  But there are no hard-and-fast
        ///   rules here, eh?  You won't know til you test it.  And there will be a
        ///   limit where ever larger buffers actually slow things down.  So as I said
        ///   in the beginning, it's probably best if you don't set or get this property
        ///   at all.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// This example shows how you might set a large buffer size for efficiency when
        /// dealing with zip entries that are larger than 1gb.
        /// <code lang="C#">
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///     zip.SaveProgress += this.zip1_SaveProgress;
        ///     zip.AddDirectory(directoryToZip, "");
        ///     zip.UseZip64WhenSaving = Zip64Option.Always;
        ///     zip.BufferSize = 65536*8; // 65536 * 8 = 512k
        ///     zip.Save(ZipFileToCreate);
        /// }
        /// </code>
        /// </example>

        public int BufferSize
        {
            get { return _BufferSize; }
            set { _BufferSize = value; }
        }

        /// <summary>
        ///   Size of the work buffer to use for the ZLIB codec during compression.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     When doing ZLIB or Deflate compression, the library fills a buffer,
        ///     then passes it to the compressor for compression. Then the library
        ///     reads out the compressed bytes. This happens repeatedly until there
        ///     is no more uncompressed data to compress. This property sets the
        ///     size of the buffer that will be used for chunk-wise compression. In
        ///     order for the setting to take effect, your application needs to set
        ///     this property before calling one of the <c>ZipFile.Save()</c>
        ///     overloads.
        ///   </para>
        ///   <para>
        ///     Setting this affects the performance and memory efficiency of
        ///     compression and decompression. For larger files, setting this to a
        ///     larger size may improve compression performance, but the exact
        ///     numbers vary depending on available memory, the size of the streams
        ///     you are compressing, and a bunch of other variables. I don't have
        ///     good firm recommendations on how to set it.  You'll have to test it
        ///     yourself. Or just leave it alone and accept the default.
        ///   </para>
        /// </remarks>
        public int CodecBufferSize
        {
            get;
            set;
        }

        /// <summary>
        ///   Indicates whether extracted files should keep their paths as
        ///   stored in the zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///  <para>
        ///    This property affects Extraction.  It is not used when creating zip
        ///    archives.
        ///  </para>
        ///
        ///  <para>
        ///    With this property set to <c>false</c>, the default, extracting entries
        ///    from a zip file will create files in the filesystem that have the full
        ///    path associated to the entry within the zip file.  With this property set
        ///    to <c>true</c>, extracting entries from the zip file results in files
        ///    with no path: the folders are "flattened."
        ///  </para>
        ///
        ///  <para>
        ///    An example: suppose the zip file contains entries /directory1/file1.txt and
        ///    /directory2/file2.txt.  With <c>FlattenFoldersOnExtract</c> set to false,
        ///    the files created will be \directory1\file1.txt and \directory2\file2.txt.
        ///    With the property set to true, the files created are file1.txt and file2.txt.
        ///  </para>
        ///
        /// </remarks>
        public bool FlattenFoldersOnExtract
        {
            get;
            set;
        }


        /// <summary>
        ///   The compression strategy to use for all entries.
        /// </summary>
        ///
        /// <remarks>
        ///   Set the Strategy used by the ZLIB-compatible compressor, when
        ///   compressing entries using the DEFLATE method. Different compression
        ///   strategies work better on different sorts of data. The strategy
        ///   parameter can affect the compression ratio and the speed of
        ///   compression but not the correctness of the compresssion.  For more
        ///   information see <see
        ///   cref="Ionic.Zlib.CompressionStrategy">Ionic.Zlib.CompressionStrategy</see>.
        /// </remarks>
        public Ionic.Zlib.CompressionStrategy Strategy
        {
            get { return _Strategy; }
            set { _Strategy = value; }
        }




        /// <summary>
        ///   Sets the compression level to be used for entries subsequently added to
        ///   the zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///  <para>
        ///    Varying the compression level used on entries can affect the
        ///    size-vs-speed tradeoff when compression and decompressing data streams
        ///    or files.
        ///  </para>
        ///
        ///  <para>
        ///    As with some other properties on the <c>ZipFile</c> class, like <see
        ///    cref="Password"/>, <see cref="Encryption"/>, and <see
        ///    cref="ZipErrorAction"/>, setting this property on a <c>ZipFile</c>
        ///    instance will cause the specified <c>CompressionLevel</c> to be used on all
        ///    <see cref="ZipEntry"/> items that are subsequently added to the
        ///    <c>ZipFile</c> instance. If you set this property after you have added
        ///    items to the <c>ZipFile</c>, but before you have called <c>Save()</c>,
        ///    those items will not use the specified compression level.
        ///  </para>
        ///
        ///  <para>
        ///    If you do not set this property, the default compression level is used,
        ///    which normally gives a good balance of compression efficiency and
        ///    compression speed.  In some tests, using <c>BestCompression</c> can
        ///    double the time it takes to compress, while delivering just a small
        ///    increase in compression efficiency.  This behavior will vary with the
        ///    type of data you compress.  If you are in doubt, just leave this setting
        ///    alone, and accept the default.
        ///  </para>
        /// </remarks>
        public Ionic.Zlib.CompressionLevel CompressionLevel
        {
            get;
            set;
        }

        /// <summary>
        ///   The compression method for the zipfile.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     By default, the compression method is <c>CompressionMethod.Deflate.</c>
        ///   </para>
        /// </remarks>
        /// <seealso cref="Ionic.Zip.CompressionMethod" />
        public Ionic.Zip.CompressionMethod CompressionMethod
        {
            get
            {
                return _compressionMethod;
            }
            set
            {
                _compressionMethod = value;
            }
        }



        /// <summary>
        ///   A comment attached to the zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   This property is read/write. It allows the application to specify a
        ///   comment for the <c>ZipFile</c>, or read the comment for the
        ///   <c>ZipFile</c>.  After setting this property, changes are only made
        ///   permanent when you call a <c>Save()</c> method.
        /// </para>
        ///
        /// <para>
        ///   According to <see
        ///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
        ///   zip specification</see>, the comment is not encrypted, even if there is a
        ///   password set on the zip file.
        /// </para>
        ///
        /// <para>
        ///   The specification does not describe how to indicate the encoding used
        ///   on a comment string. Many "compliant" zip tools and libraries use
        ///   IBM437 as the code page for comments; DotNetZip, too, follows that
        ///   practice.  On the other hand, there are situations where you want a
        ///   Comment to be encoded with something else, for example using code page
        ///   950 "Big-5 Chinese". To fill that need, DotNetZip will encode the
        ///   comment following the same procedure it follows for encoding
        ///   filenames: (a) if <see cref="AlternateEncodingUsage"/> is
        ///   <c>Never</c>, it uses the default encoding (IBM437). (b) if <see
        ///   cref="AlternateEncodingUsage"/> is <c>Always</c>, it always uses the
        ///   alternate encoding (<see cref="AlternateEncoding"/>). (c) if <see
        ///   cref="AlternateEncodingUsage"/> is <c>AsNecessary</c>, it uses the
        ///   alternate encoding only if the default encoding is not sufficient for
        ///   encoding the comment - in other words if decoding the result does not
        ///   produce the original string.  This decision is taken at the time of
        ///   the call to <c>ZipFile.Save()</c>.
        /// </para>
        ///
        /// <para>
        ///   When creating a zip archive using this library, it is possible to change
        ///   the value of <see cref="AlternateEncoding" /> between each
        ///   entry you add, and between adding entries and the call to
        ///   <c>Save()</c>. Don't do this.  It will likely result in a zip file that is
        ///   not readable by any tool or application.  For best interoperability, leave
        ///   <see cref="AlternateEncoding"/> alone, or specify it only
        ///   once, before adding any entries to the <c>ZipFile</c> instance.
        /// </para>
        ///
        /// </remarks>
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                _contentsChanged = true;
            }
        }




        /// <summary>
        ///   Specifies whether the Creation, Access, and Modified times for entries
        ///   added to the zip file will be emitted in &#147;Windows format&#148;
        ///   when the zip archive is saved.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   An application creating a zip archive can use this flag to explicitly
        ///   specify that the file times for the entries should or should not be stored
        ///   in the zip archive in the format used by Windows. By default this flag is
        ///   <c>true</c>, meaning the Windows-format times are stored in the zip
        ///   archive.
        /// </para>
        ///
        /// <para>
        ///   When adding an entry from a file or directory, the Creation (<see
        ///   cref="ZipEntry.CreationTime"/>), Access (<see
        ///   cref="ZipEntry.AccessedTime"/>), and Modified (<see
        ///   cref="ZipEntry.ModifiedTime"/>) times for the given entry are
        ///   automatically set from the filesystem values. When adding an entry from a
        ///   stream or string, all three values are implicitly set to
        ///   <c>DateTime.Now</c>.  Applications can also explicitly set those times by
        ///   calling <see cref="ZipEntry.SetEntryTimes(DateTime, DateTime,
        ///   DateTime)"/>.
        /// </para>
        ///
        /// <para>
        ///   <see
        ///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
        ///   zip specification</see> describes multiple ways to format these times in a
        ///   zip file. One is the format Windows applications normally use: 100ns ticks
        ///   since January 1, 1601 UTC.  The other is a format Unix applications typically
        ///   use: seconds since January 1, 1970 UTC.  Each format can be stored in an
        ///   "extra field" in the zip entry when saving the zip archive. The former
        ///   uses an extra field with a Header Id of 0x000A, while the latter uses a
        ///   header ID of 0x5455, although you probably don't need to know that.
        /// </para>
        ///
        /// <para>
        ///   Not all tools and libraries can interpret these fields.  Windows
        ///   compressed folders is one that can read the Windows Format timestamps,
        ///   while I believe <see href="http://www.info-zip.org/">the Infozip
        ///   tools</see> can read the Unix format timestamps. Some tools and libraries
        ///   may be able to read only one or the other. DotNetZip can read or write
        ///   times in either or both formats.
        /// </para>
        ///
        /// <para>
        ///   The times stored are taken from <see cref="ZipEntry.ModifiedTime"/>, <see
        ///   cref="ZipEntry.AccessedTime"/>, and <see cref="ZipEntry.CreationTime"/>.
        /// </para>
        ///
        /// <para>
        ///   The value set here applies to all entries subsequently added to the
        ///   <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        ///   This property is not mutually exclusive of the <see
        ///   cref="EmitTimesInUnixFormatWhenSaving" /> property. It is possible and
        ///   legal and valid to produce a zip file that contains timestamps encoded in
        ///   the Unix format as well as in the Windows format, in addition to the <see
        ///   cref="ZipEntry.LastModified">LastModified</see> time attached to each
        ///   entry in the archive, a time that is always stored in "DOS format". And,
        ///   notwithstanding the names PKWare uses for these time formats, any of them
        ///   can be read and written by any computer, on any operating system.  But,
        ///   there are no guarantees that a program running on Mac or Linux will
        ///   gracefully handle a zip file with "Windows" formatted times, or that an
        ///   application that does not use DotNetZip but runs on Windows will be able to
        ///   handle file times in Unix format.
        /// </para>
        ///
        /// <para>
        ///   When in doubt, test.  Sorry, I haven't got a complete list of tools and
        ///   which sort of timestamps they can use and will tolerate.  If you get any
        ///   good information and would like to pass it on, please do so and I will
        ///   include that information in this documentation.
        /// </para>
        /// </remarks>
        ///
        /// <example>
        ///   This example shows how to save a zip file that contains file timestamps
        ///   in a format normally used by Unix.
        /// <code lang="C#">
        /// using (var zip = new ZipFile())
        /// {
        ///     // produce a zip file the Mac will like
        ///     zip.EmitTimesInWindowsFormatWhenSaving = false;
        ///     zip.EmitTimesInUnixFormatWhenSaving = true;
        ///     zip.AddDirectory(directoryToZip, "files");
        ///     zip.Save(outputFile);
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     '' produce a zip file the Mac will like
        ///     zip.EmitTimesInWindowsFormatWhenSaving = False
        ///     zip.EmitTimesInUnixFormatWhenSaving = True
        ///     zip.AddDirectory(directoryToZip, "files")
        ///     zip.Save(outputFile)
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <seealso cref="ZipEntry.EmitTimesInWindowsFormatWhenSaving" />
        /// <seealso cref="EmitTimesInUnixFormatWhenSaving" />
        public bool EmitTimesInWindowsFormatWhenSaving
        {
            get
            {
                return _emitNtfsTimes;
            }
            set
            {
                _emitNtfsTimes = value;
            }
        }


        /// <summary>
        /// Specifies whether the Creation, Access, and Modified times
        /// for entries added to the zip file will be emitted in "Unix(tm)
        /// format" when the zip archive is saved.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   An application creating a zip archive can use this flag to explicitly
        ///   specify that the file times for the entries should or should not be stored
        ///   in the zip archive in the format used by Unix. By default this flag is
        ///   <c>false</c>, meaning the Unix-format times are not stored in the zip
        ///   archive.
        /// </para>
        ///
        /// <para>
        ///   When adding an entry from a file or directory, the Creation (<see
        ///   cref="ZipEntry.CreationTime"/>), Access (<see
        ///   cref="ZipEntry.AccessedTime"/>), and Modified (<see
        ///   cref="ZipEntry.ModifiedTime"/>) times for the given entry are
        ///   automatically set from the filesystem values. When adding an entry from a
        ///   stream or string, all three values are implicitly set to DateTime.Now.
        ///   Applications can also explicitly set those times by calling <see
        ///   cref="ZipEntry.SetEntryTimes(DateTime, DateTime, DateTime)"/>.
        /// </para>
        ///
        /// <para>
        ///   <see
        ///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
        ///   zip specification</see> describes multiple ways to format these times in a
        ///   zip file. One is the format Windows applications normally use: 100ns ticks
        ///   since January 1, 1601 UTC.  The other is a format Unix applications
        ///   typically use: seconds since January 1, 1970 UTC.  Each format can be
        ///   stored in an "extra field" in the zip entry when saving the zip
        ///   archive. The former uses an extra field with a Header Id of 0x000A, while
        ///   the latter uses a header ID of 0x5455, although you probably don't need to
        ///   know that.
        /// </para>
        ///
        /// <para>
        ///   Not all tools and libraries can interpret these fields.  Windows
        ///   compressed folders is one that can read the Windows Format timestamps,
        ///   while I believe the <see href="http://www.info-zip.org/">Infozip</see>
        ///   tools can read the Unix format timestamps. Some tools and libraries may be
        ///   able to read only one or the other.  DotNetZip can read or write times in
        ///   either or both formats.
        /// </para>
        ///
        /// <para>
        ///   The times stored are taken from <see cref="ZipEntry.ModifiedTime"/>, <see
        ///   cref="ZipEntry.AccessedTime"/>, and <see cref="ZipEntry.CreationTime"/>.
        /// </para>
        ///
        /// <para>
        ///   This property is not mutually exclusive of the <see
        ///   cref="EmitTimesInWindowsFormatWhenSaving" /> property. It is possible and
        ///   legal and valid to produce a zip file that contains timestamps encoded in
        ///   the Unix format as well as in the Windows format, in addition to the <see
        ///   cref="ZipEntry.LastModified">LastModified</see> time attached to each
        ///   entry in the zip archive, a time that is always stored in "DOS
        ///   format". And, notwithstanding the names PKWare uses for these time
        ///   formats, any of them can be read and written by any computer, on any
        ///   operating system.  But, there are no guarantees that a program running on
        ///   Mac or Linux will gracefully handle a zip file with "Windows" formatted
        ///   times, or that an application that does not use DotNetZip but runs on
        ///   Windows will be able to handle file times in Unix format.
        /// </para>
        ///
        /// <para>
        ///   When in doubt, test.  Sorry, I haven't got a complete list of tools and
        ///   which sort of timestamps they can use and will tolerate.  If you get any
        ///   good information and would like to pass it on, please do so and I will
        ///   include that information in this documentation.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="ZipEntry.EmitTimesInUnixFormatWhenSaving" />
        /// <seealso cref="EmitTimesInWindowsFormatWhenSaving" />
        public bool EmitTimesInUnixFormatWhenSaving
        {
            get
            {
                return _emitUnixTimes;
            }
            set
            {
                _emitUnixTimes = value;
            }
        }



        /// <summary>
        ///   Indicates whether verbose output is sent to the <see
        ///   cref="StatusMessageTextWriter"/> during <c>AddXxx()</c> and
        ///   <c>ReadXxx()</c> operations.
        /// </summary>
        ///
        /// <remarks>
        ///   This is a <em>synthetic</em> property.  It returns true if the <see
        ///   cref="StatusMessageTextWriter"/> is non-null.
        /// </remarks>
        internal bool Verbose
        {
            get { return (_StatusMessageTextWriter != null); }
        }


        /// <summary>
        ///   Returns true if an entry by the given name exists in the ZipFile.
        /// </summary>
        ///
        /// <param name='name'>the name of the entry to find</param>
        /// <returns>true if an entry with the given name exists; otherwise false.
        /// </returns>
        public bool ContainsEntry(string name)
        {
            // workitem 12534
            return _entries.ContainsKey(SharedUtilities.NormalizePathForUseInZipFile(name));
        }



        /// <summary>
        ///   Indicates whether to perform case-sensitive matching on the filename when
        ///   retrieving entries in the zipfile via the string-based indexer.
        /// </summary>
        ///
        /// <remarks>
        ///   The default value is <c>false</c>, which means don't do case-sensitive
        ///   matching. In other words, retrieving zip["ReadMe.Txt"] is the same as
        ///   zip["readme.txt"].  It really makes sense to set this to <c>true</c> only
        ///   if you are not running on Windows, which has case-insensitive
        ///   filenames. But since this library is not built for non-Windows platforms,
        ///   in most cases you should just leave this property alone.
        /// </remarks>
        public bool CaseSensitiveRetrieval
        {
            get
            {
                return _CaseSensitiveRetrieval;
            }

            set
            {
                // workitem 9868
                if (value != _CaseSensitiveRetrieval)
                {
                    _CaseSensitiveRetrieval = value;
                    _initEntriesDictionary();
                }
            }
        }




        /// <summary>
        ///   Specify whether to use ZIP64 extensions when saving a zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   When creating a zip file, the default value for the property is <see
        ///   cref="Zip64Option.Never"/>. <see cref="Zip64Option.AsNecessary"/> is
        ///   safest, in the sense that you will not get an Exception if a pre-ZIP64
        ///   limit is exceeded.
        /// </para>
        ///
        /// <para>
        ///   You may set the property at any time before calling Save().
        /// </para>
        ///
        /// <para>
        ///   When reading a zip file via the <c>Zipfile.Read()</c> method, DotNetZip
        ///   will properly read ZIP64-endowed zip archives, regardless of the value of
        ///   this property.  DotNetZip will always read ZIP64 archives.  This property
        ///   governs only whether DotNetZip will write them. Therefore, when updating
        ///   archives, be careful about setting this property after reading an archive
        ///   that may use ZIP64 extensions.
        /// </para>
        ///
        /// <para>
        ///   An interesting question is, if you have set this property to
        ///   <c>AsNecessary</c>, and then successfully saved, does the resulting
        ///   archive use ZIP64 extensions or not?  To learn this, check the <see
        ///   cref="OutputUsedZip64"/> property, after calling <c>Save()</c>.
        /// </para>
        ///
        /// <para>
        ///   Have you thought about
        ///   <see href="http://cheeso.members.winisp.net/DotNetZipDonate.aspx">donating</see>?
        /// </para>
        ///
        /// </remarks>
        /// <seealso cref="RequiresZip64"/>
        public Zip64Option UseZip64WhenSaving
        {
            get
            {
                return _zip64;
            }
            set
            {
                _zip64 = value;
            }
        }



        /// <summary>
        ///   Indicates whether the archive requires ZIP64 extensions.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   This property is <c>null</c> (or <c>Nothing</c> in VB) if the archive has
        ///   not been saved, and there are fewer than 65334 <c>ZipEntry</c> items
        ///   contained in the archive.
        /// </para>
        ///
        /// <para>
        ///   The <c>Value</c> is true if any of the following four conditions holds:
        ///   the uncompressed size of any entry is larger than 0xFFFFFFFF; the
        ///   compressed size of any entry is larger than 0xFFFFFFFF; the relative
        ///   offset of any entry within the zip archive is larger than 0xFFFFFFFF; or
        ///   there are more than 65534 entries in the archive.  (0xFFFFFFFF =
        ///   4,294,967,295).  The result may not be known until a <c>Save()</c> is attempted
        ///   on the zip archive.  The Value of this <see cref="System.Nullable"/>
        ///   property may be set only AFTER one of the Save() methods has been called.
        /// </para>
        ///
        /// <para>
        ///   If none of the four conditions holds, and the archive has been saved, then
        ///   the <c>Value</c> is false.
        /// </para>
        ///
        /// <para>
        ///   A <c>Value</c> of false does not indicate that the zip archive, as saved,
        ///   does not use ZIP64.  It merely indicates that ZIP64 is not required.  An
        ///   archive may use ZIP64 even when not required if the <see
        ///   cref="ZipFile.UseZip64WhenSaving"/> property is set to <see
        ///   cref="Zip64Option.Always"/>, or if the <see
        ///   cref="ZipFile.UseZip64WhenSaving"/> property is set to <see
        ///   cref="Zip64Option.AsNecessary"/> and the output stream was not
        ///   seekable. Use the <see cref="OutputUsedZip64"/> property to determine if
        ///   the most recent <c>Save()</c> method resulted in an archive that utilized
        ///   the ZIP64 extensions.
        /// </para>
        ///
        /// </remarks>
        /// <seealso cref="UseZip64WhenSaving"/>
        /// <seealso cref="OutputUsedZip64"/>
        public Nullable<bool> RequiresZip64
        {
            get
            {
                if (_entries.Count > 65534)
                    return new Nullable<bool>(true);

                // If the <c>ZipFile</c> has not been saved or if the contents have changed, then
                // it is not known if ZIP64 is required.
                if (!_hasBeenSaved || _contentsChanged) return null;

                // Whether ZIP64 is required is knowable.
                foreach (ZipEntry e in _entries.Values)
                {
                    if (e.RequiresZip64.Value) return new Nullable<bool>(true);
                }

                return new Nullable<bool>(false);
            }
        }


        /// <summary>
        ///   Indicates whether the most recent <c>Save()</c> operation used ZIP64 extensions.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   The use of ZIP64 extensions within an archive is not always necessary, and
        ///   for interoperability concerns, it may be desired to NOT use ZIP64 if
        ///   possible.  The <see cref="ZipFile.UseZip64WhenSaving"/> property can be
        ///   set to use ZIP64 extensions only when necessary.  In those cases,
        ///   Sometimes applications want to know whether a Save() actually used ZIP64
        ///   extensions.  Applications can query this read-only property to learn
        ///   whether ZIP64 has been used in a just-saved <c>ZipFile</c>.
        /// </para>
        ///
        /// <para>
        ///   The value is <c>null</c> (or <c>Nothing</c> in VB) if the archive has not
        ///   been saved.
        /// </para>
        ///
        /// <para>
        ///   Non-null values (<c>HasValue</c> is true) indicate whether ZIP64
        ///   extensions were used during the most recent <c>Save()</c> operation.  The
        ///   ZIP64 extensions may have been used as required by any particular entry
        ///   because of its uncompressed or compressed size, or because the archive is
        ///   larger than 4294967295 bytes, or because there are more than 65534 entries
        ///   in the archive, or because the <c>UseZip64WhenSaving</c> property was set
        ///   to <see cref="Zip64Option.Always"/>, or because the
        ///   <c>UseZip64WhenSaving</c> property was set to <see
        ///   cref="Zip64Option.AsNecessary"/> and the output stream was not seekable.
        ///   The value of this property does not indicate the reason the ZIP64
        ///   extensions were used.
        /// </para>
        ///
        /// </remarks>
        /// <seealso cref="UseZip64WhenSaving"/>
        /// <seealso cref="RequiresZip64"/>
        public Nullable<bool> OutputUsedZip64
        {
            get
            {
                return _OutputUsesZip64;
            }
        }


        /// <summary>
        ///   Indicates whether the most recent <c>Read()</c> operation read a zip file that uses
        ///   ZIP64 extensions.
        /// </summary>
        ///
        /// <remarks>
        ///   This property will return null (Nothing in VB) if you've added an entry after reading
        ///   the zip file.
        /// </remarks>
        public Nullable<bool> InputUsesZip64
        {
            get
            {
                if (_entries.Count > 65534)
                    return true;

                foreach (ZipEntry e in this)
                {
                    // if any entry was added after reading the zip file, then the result is null
                    if (e.Source != ZipEntrySource.ZipFile) return null;

                    // if any entry read from the zip used zip64, then the result is true
                    if (e._InputUsesZip64) return true;
                }
                return false;
            }
        }




        /// <summary>
        ///   A Text Encoding to use when encoding the filenames and comments for
        ///   all the ZipEntry items, during a ZipFile.Save() operation.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Whether the encoding specified here is used during the save depends
        ///     on <see cref="AlternateEncodingUsage"/>.
        ///   </para>
        /// </remarks>
        public System.Text.Encoding AlternateEncoding
        {
            get
            {
                return _alternateEncoding;
            }
            set
            {
                _alternateEncoding = value;
            }
        }


        /// <summary>
        ///   A flag that tells if and when this instance should apply
        ///   AlternateEncoding to encode the filenames and comments associated to
        ///   of ZipEntry objects contained within this instance.
        /// </summary>
        public ZipOption AlternateEncodingUsage
        {
            get
            {
                return _alternateEncodingUsage;
            }
            set
            {
                _alternateEncodingUsage = value;
            }
        }


        /// <summary>
        /// The default text encoding used in zip archives.  It is numeric 437, also
        /// known as IBM437.
        /// </summary>
        /// <seealso cref="Ionic.Zip.ZipFile.ProvisionalAlternateEncoding"/>
        public static System.Text.Encoding DefaultEncoding
        {
            get
            {
                return _defaultEncoding;
            }
        }


        /// <summary>
        /// Gets or sets the <c>TextWriter</c> to which status messages are delivered
        /// for the instance.
        /// </summary>
        ///
        /// <remarks>
        ///   If the TextWriter is set to a non-null value, then verbose output is sent
        ///   to the <c>TextWriter</c> during <c>Add</c><c>, Read</c><c>, Save</c> and
        ///   <c>Extract</c> operations.  Typically, console applications might use
        ///   <c>Console.Out</c> and graphical or headless applications might use a
        ///   <c>System.IO.StringWriter</c>. The output of this is suitable for viewing
        ///   by humans.
        /// </remarks>
        ///
        /// <example>
        /// <para>
        ///   In this example, a console application instantiates a <c>ZipFile</c>, then
        ///   sets the <c>StatusMessageTextWriter</c> to <c>Console.Out</c>.  At that
        ///   point, all verbose status messages for that <c>ZipFile</c> are sent to the
        ///   console.
        /// </para>
        ///
        /// <code lang="C#">
        /// using (ZipFile zip= ZipFile.Read(FilePath))
        /// {
        ///   zip.StatusMessageTextWriter= System.Console.Out;
        ///   // messages are sent to the console during extraction
        ///   zip.ExtractAll();
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As ZipFile = ZipFile.Read(FilePath)
        ///   zip.StatusMessageTextWriter= System.Console.Out
        ///   'Status Messages will be sent to the console during extraction
        ///   zip.ExtractAll()
        /// End Using
        /// </code>
        ///
        /// <para>
        ///   In this example, a Windows Forms application instantiates a
        ///   <c>ZipFile</c>, then sets the <c>StatusMessageTextWriter</c> to a
        ///   <c>StringWriter</c>.  At that point, all verbose status messages for that
        ///   <c>ZipFile</c> are sent to the <c>StringWriter</c>.
        /// </para>
        ///
        /// <code lang="C#">
        /// var sw = new System.IO.StringWriter();
        /// using (ZipFile zip= ZipFile.Read(FilePath))
        /// {
        ///   zip.StatusMessageTextWriter= sw;
        ///   zip.ExtractAll();
        /// }
        /// Console.WriteLine("{0}", sw.ToString());
        /// </code>
        ///
        /// <code lang="VB">
        /// Dim sw as New System.IO.StringWriter
        /// Using zip As ZipFile = ZipFile.Read(FilePath)
        ///   zip.StatusMessageTextWriter= sw
        ///   zip.ExtractAll()
        /// End Using
        /// 'Status Messages are now available in sw
        ///
        /// </code>
        /// </example>
        public TextWriter StatusMessageTextWriter
        {
            get { return _StatusMessageTextWriter; }
            set { _StatusMessageTextWriter = value; }
        }





        /// <summary>
        /// Sets the password to be used on the <c>ZipFile</c> instance.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   When writing a zip archive, this password is applied to the entries, not
        ///   to the zip archive itself. It applies to any <c>ZipEntry</c> subsequently
        ///   added to the <c>ZipFile</c>, using one of the <c>AddFile</c>,
        ///   <c>AddDirectory</c>, <c>AddEntry</c>, or <c>AddItem</c> methods, etc.
        ///   When reading a zip archive, this property applies to any entry
        ///   subsequently extracted from the <c>ZipFile</c> using one of the Extract
        ///   methods on the <c>ZipFile</c> class.
        /// </para>
        ///
        /// <para>
        ///   When writing a zip archive, keep this in mind: though the password is set
        ///   on the ZipFile object, according to the Zip spec, the "directory" of the
        ///   archive - in other words the list of entries or files contained in the archive - is
        ///   not encrypted with the password, or protected in any way.  If you set the
        ///   Password property, the password actually applies to individual entries
        ///   that are added to the archive, subsequent to the setting of this property.
        ///   The list of filenames in the archive that is eventually created will
        ///   appear in clear text, but the contents of the individual files are
        ///   encrypted.  This is how Zip encryption works.
        /// </para>
        ///
        /// <para>
        ///   One simple way around this limitation is to simply double-wrap sensitive
        ///   filenames: Store the files in a zip file, and then store that zip file
        ///   within a second, "outer" zip file.  If you apply a password to the outer
        ///   zip file, then readers will be able to see that the outer zip file
        ///   contains an inner zip file.  But readers will not be able to read the
        ///   directory or file list of the inner zip file.
        /// </para>
        ///
        /// <para>
        ///   If you set the password on the <c>ZipFile</c>, and then add a set of files
        ///   to the archive, then each entry is encrypted with that password.  You may
        ///   also want to change the password between adding different entries. If you
        ///   set the password, add an entry, then set the password to <c>null</c>
        ///   (<c>Nothing</c> in VB), and add another entry, the first entry is
        ///   encrypted and the second is not.  If you call <c>AddFile()</c>, then set
        ///   the <c>Password</c> property, then call <c>ZipFile.Save</c>, the file
        ///   added will not be password-protected, and no warning will be generated.
        /// </para>
        ///
        /// <para>
        ///   When setting the Password, you may also want to explicitly set the <see
        ///   cref="Encryption"/> property, to specify how to encrypt the entries added
        ///   to the ZipFile.  If you set the Password to a non-null value and do not
        ///   set <see cref="Encryption"/>, then PKZip 2.0 ("Weak") encryption is used.
        ///   This encryption is relatively weak but is very interoperable. If you set
        ///   the password to a <c>null</c> value (<c>Nothing</c> in VB), Encryption is
        ///   reset to None.
        /// </para>
        ///
        /// <para>
        ///   All of the preceding applies to writing zip archives, in other words when
        ///   you use one of the Save methods.  To use this property when reading or an
        ///   existing ZipFile, do the following: set the Password property on the
        ///   <c>ZipFile</c>, then call one of the Extract() overloads on the <see
        ///   cref="ZipEntry" />. In this case, the entry is extracted using the
        ///   <c>Password</c> that is specified on the <c>ZipFile</c> instance. If you
        ///   have not set the <c>Password</c> property, then the password is
        ///   <c>null</c>, and the entry is extracted with no password.
        /// </para>
        ///
        /// <para>
        ///   If you set the Password property on the <c>ZipFile</c>, then call
        ///   <c>Extract()</c> an entry that has not been encrypted with a password, the
        ///   password is not used for that entry, and the <c>ZipEntry</c> is extracted
        ///   as normal. In other words, the password is used only if necessary.
        /// </para>
        ///
        /// <para>
        ///   The <see cref="ZipEntry"/> class also has a <see
        ///   cref="ZipEntry.Password">Password</see> property.  It takes precedence
        ///   over this property on the <c>ZipFile</c>.  Typically, you would use the
        ///   per-entry Password when most entries in the zip archive use one password,
        ///   and a few entries use a different password.  If all entries in the zip
        ///   file use the same password, then it is simpler to just set this property
        ///   on the <c>ZipFile</c> itself, whether creating a zip archive or extracting
        ///   a zip archive.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// <para>
        ///   This example creates a zip file, using password protection for the
        ///   entries, and then extracts the entries from the zip file.  When creating
        ///   the zip file, the Readme.txt file is not protected with a password, but
        ///   the other two are password-protected as they are saved. During extraction,
        ///   each file is extracted with the appropriate password.
        /// </para>
        /// <code>
        /// // create a file with encryption
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///     zip.AddFile("ReadMe.txt");
        ///     zip.Password= "!Secret1";
        ///     zip.AddFile("MapToTheSite-7440-N49th.png");
        ///     zip.AddFile("2008-Regional-Sales-Report.pdf");
        ///     zip.Save("EncryptedArchive.zip");
        /// }
        ///
        /// // extract entries that use encryption
        /// using (ZipFile zip = ZipFile.Read("EncryptedArchive.zip"))
        /// {
        ///     zip.Password= "!Secret1";
        ///     zip.ExtractAll("extractDir");
        /// }
        ///
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     zip.AddFile("ReadMe.txt")
        ///     zip.Password = "123456!"
        ///     zip.AddFile("MapToTheSite-7440-N49th.png")
        ///     zip.Password= "!Secret1";
        ///     zip.AddFile("2008-Regional-Sales-Report.pdf")
        ///     zip.Save("EncryptedArchive.zip")
        /// End Using
        ///
        ///
        /// ' extract entries that use encryption
        /// Using (zip as ZipFile = ZipFile.Read("EncryptedArchive.zip"))
        ///     zip.Password= "!Secret1"
        ///     zip.ExtractAll("extractDir")
        /// End Using
        ///
        /// </code>
        ///
        /// </example>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.Encryption">ZipFile.Encryption</seealso>
        /// <seealso cref="Ionic.Zip.ZipEntry.Password">ZipEntry.Password</seealso>
        public String Password
        {
            set
            {
                _Password = value;
                if (_Password == null)
                {
                    Encryption = EncryptionAlgorithm.None;
                }
                else if (Encryption == EncryptionAlgorithm.None)
                {
                    Encryption = EncryptionAlgorithm.PkzipWeak;
                }
            }
            private get
            {
                return _Password;
            }
        }





        /// <summary>
        ///   The action the library should take when extracting a file that already
        ///   exists.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This property affects the behavior of the Extract methods (one of the
        ///   <c>Extract()</c> or <c>ExtractWithPassword()</c> overloads), when
        ///   extraction would would overwrite an existing filesystem file. If you do
        ///   not set this property, the library throws an exception when extracting an
        ///   entry would overwrite an existing file.
        /// </para>
        ///
        /// <para>
        ///   This property has no effect when extracting to a stream, or when the file
        ///   to be extracted does not already exist.
        /// </para>
        /// </remarks>
        /// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
        public ExtractExistingFileAction ExtractExistingFile
        {
            get;
            set;
        }


        /// <summary>
        ///   The action the library should take when an error is encountered while
        ///   opening or reading files as they are saved into a zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///  <para>
        ///    Errors can occur as a file is being saved to the zip archive.  For
        ///    example, the File.Open may fail, or a File.Read may fail, because of
        ///    lock conflicts or other reasons.
        ///  </para>
        ///
        ///  <para>
        ///    The first problem might occur after having called AddDirectory() on a
        ///    directory that contains a Clipper .dbf file; the file is locked by
        ///    Clipper and cannot be opened for read by another process. An example of
        ///    the second problem might occur when trying to zip a .pst file that is in
        ///    use by Microsoft Outlook. Outlook locks a range on the file, which allows
        ///    other processes to open the file, but not read it in its entirety.
        ///  </para>
        ///
        ///  <para>
        ///    This property tells DotNetZip what you would like to do in the case of
        ///    these errors.  The primary options are: <c>ZipErrorAction.Throw</c> to
        ///    throw an exception (this is the default behavior if you don't set this
        ///    property); <c>ZipErrorAction.Skip</c> to Skip the file for which there
        ///    was an error and continue saving; <c>ZipErrorAction.Retry</c> to Retry
        ///    the entry that caused the problem; or
        ///    <c>ZipErrorAction.InvokeErrorEvent</c> to invoke an event handler.
        ///  </para>
        ///
        ///  <para>
        ///    This property is implicitly set to <c>ZipErrorAction.InvokeErrorEvent</c>
        ///    if you add a handler to the <see cref="ZipError" /> event.  If you set
        ///    this property to something other than
        ///    <c>ZipErrorAction.InvokeErrorEvent</c>, then the <c>ZipError</c>
        ///    event is implicitly cleared.  What it means is you can set one or the
        ///    other (or neither), depending on what you want, but you never need to set
        ///    both.
        ///  </para>
        ///
        ///  <para>
        ///    As with some other properties on the <c>ZipFile</c> class, like <see
        ///    cref="Password"/>, <see cref="Encryption"/>, and <see
        ///    cref="CompressionLevel"/>, setting this property on a <c>ZipFile</c>
        ///    instance will cause the specified <c>ZipErrorAction</c> to be used on all
        ///    <see cref="ZipEntry"/> items that are subsequently added to the
        ///    <c>ZipFile</c> instance. If you set this property after you have added
        ///    items to the <c>ZipFile</c>, but before you have called <c>Save()</c>,
        ///    those items will not use the specified error handling action.
        ///  </para>
        ///
        ///  <para>
        ///    If you want to handle any errors that occur with any entry in the zip
        ///    file in the same way, then set this property once, before adding any
        ///    entries to the zip archive.
        ///  </para>
        ///
        ///  <para>
        ///    If you set this property to <c>ZipErrorAction.Skip</c> and you'd like to
        ///    learn which files may have been skipped after a <c>Save()</c>, you can
        ///    set the <see cref="StatusMessageTextWriter" /> on the ZipFile before
        ///    calling <c>Save()</c>. A message will be emitted into that writer for
        ///    each skipped file, if any.
        ///  </para>
        ///
        /// </remarks>
        ///
        /// <example>
        ///   This example shows how to tell DotNetZip to skip any files for which an
        ///   error is generated during the Save().
        /// <code lang="VB">
        /// Public Sub SaveZipFile()
        ///     Dim SourceFolder As String = "fodder"
        ///     Dim DestFile As String =  "eHandler.zip"
        ///     Dim sw as New StringWriter
        ///     Using zipArchive As ZipFile = New ZipFile
        ///         ' Tell DotNetZip to skip any files for which it encounters an error
        ///         zipArchive.ZipErrorAction = ZipErrorAction.Skip
        ///         zipArchive.StatusMessageTextWriter = sw
        ///         zipArchive.AddDirectory(SourceFolder)
        ///         zipArchive.Save(DestFile)
        ///     End Using
        ///     ' examine sw here to see any messages
        /// End Sub
        ///
        /// </code>
        /// </example>
        ///
        /// <seealso cref="Ionic.Zip.ZipEntry.ZipErrorAction"/>
        /// <seealso cref="Ionic.Zip.ZipFile.ZipError"/>

        public ZipErrorAction ZipErrorAction
        {
            get
            {
                if (ZipError != null)
                    _zipErrorAction = ZipErrorAction.InvokeErrorEvent;
                return _zipErrorAction;
            }
            set
            {
                _zipErrorAction = value;
                if (_zipErrorAction != ZipErrorAction.InvokeErrorEvent && ZipError != null)
                    ZipError = null;
            }
        }


        /// <summary>
        ///   The Encryption to use for entries added to the <c>ZipFile</c>.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   Set this when creating a zip archive, or when updating a zip archive. The
        ///   specified Encryption is applied to the entries subsequently added to the
        ///   <c>ZipFile</c> instance.  Applications do not need to set the
        ///   <c>Encryption</c> property when reading or extracting a zip archive.
        /// </para>
        ///
        /// <para>
        ///   If you set this to something other than EncryptionAlgorithm.None, you
        ///   will also need to set the <see cref="Password"/>.
        /// </para>
        ///
        /// <para>
        ///   As with some other properties on the <c>ZipFile</c> class, like <see
        ///   cref="Password"/> and <see cref="CompressionLevel"/>, setting this
        ///   property on a <c>ZipFile</c> instance will cause the specified
        ///   <c>EncryptionAlgorithm</c> to be used on all <see cref="ZipEntry"/> items
        ///   that are subsequently added to the <c>ZipFile</c> instance. In other
        ///   words, if you set this property after you have added items to the
        ///   <c>ZipFile</c>, but before you have called <c>Save()</c>, those items will
        ///   not be encrypted or protected with a password in the resulting zip
        ///   archive. To get a zip archive with encrypted entries, set this property,
        ///   along with the <see cref="Password"/> property, before calling
        ///   <c>AddFile</c>, <c>AddItem</c>, or <c>AddDirectory</c> (etc.) on the
        ///   <c>ZipFile</c> instance.
        /// </para>
        ///
        /// <para>
        ///   If you read a <c>ZipFile</c>, you can modify the <c>Encryption</c> on an
        ///   encrypted entry, only by setting the <c>Encryption</c> property on the
        ///   <c>ZipEntry</c> itself.  Setting the <c>Encryption</c> property on the
        ///   <c>ZipFile</c>, once it has been created via a call to
        ///   <c>ZipFile.Read()</c>, does not affect entries that were previously read.
        /// </para>
        ///
        /// <para>
        ///   For example, suppose you read a <c>ZipFile</c>, and there is an encrypted
        ///   entry.  Setting the <c>Encryption</c> property on that <c>ZipFile</c> and
        ///   then calling <c>Save()</c> on the <c>ZipFile</c> does not update the
        ///   <c>Encryption</c> used for the entries in the archive.  Neither is an
        ///   exception thrown. Instead, what happens during the <c>Save()</c> is that
        ///   all previously existing entries are copied through to the new zip archive,
        ///   with whatever encryption and password that was used when originally
        ///   creating the zip archive. Upon re-reading that archive, to extract
        ///   entries, applications should use the original password or passwords, if
        ///   any.
        /// </para>
        ///
        /// <para>
        ///   Suppose an application reads a <c>ZipFile</c>, and there is an encrypted
        ///   entry.  Setting the <c>Encryption</c> property on that <c>ZipFile</c> and
        ///   then adding new entries (via <c>AddFile()</c>, <c>AddEntry()</c>, etc)
        ///   and then calling <c>Save()</c> on the <c>ZipFile</c> does not update the
        ///   <c>Encryption</c> on any of the entries that had previously been in the
        ///   <c>ZipFile</c>.  The <c>Encryption</c> property applies only to the
        ///   newly-added entries.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// <para>
        ///   This example creates a zip archive that uses encryption, and then extracts
        ///   entries from the archive.  When creating the zip archive, the ReadMe.txt
        ///   file is zipped without using a password or encryption.  The other files
        ///   use encryption.
        /// </para>
        ///
        /// <code>
        /// // Create a zip archive with AES Encryption.
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///     zip.AddFile("ReadMe.txt");
        ///     zip.Encryption= EncryptionAlgorithm.WinZipAes256;
        ///     zip.Password= "Top.Secret.No.Peeking!";
        ///     zip.AddFile("7440-N49th.png");
        ///     zip.AddFile("2008-Regional-Sales-Report.pdf");
        ///     zip.Save("EncryptedArchive.zip");
        /// }
        ///
        /// // Extract a zip archive that uses AES Encryption.
        /// // You do not need to specify the algorithm during extraction.
        /// using (ZipFile zip = ZipFile.Read("EncryptedArchive.zip"))
        /// {
        ///     zip.Password= "Top.Secret.No.Peeking!";
        ///     zip.ExtractAll("extractDirectory");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// ' Create a zip that uses Encryption.
        /// Using zip As New ZipFile()
        ///     zip.Encryption= EncryptionAlgorithm.WinZipAes256
        ///     zip.Password= "Top.Secret.No.Peeking!"
        ///     zip.AddFile("ReadMe.txt")
        ///     zip.AddFile("7440-N49th.png")
        ///     zip.AddFile("2008-Regional-Sales-Report.pdf")
        ///     zip.Save("EncryptedArchive.zip")
        /// End Using
        ///
        /// ' Extract a zip archive that uses AES Encryption.
        /// ' You do not need to specify the algorithm during extraction.
        /// Using (zip as ZipFile = ZipFile.Read("EncryptedArchive.zip"))
        ///     zip.Password= "Top.Secret.No.Peeking!"
        ///     zip.ExtractAll("extractDirectory")
        /// End Using
        /// </code>
        ///
        /// </example>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.Password">ZipFile.Password</seealso>
        /// <seealso cref="Ionic.Zip.ZipEntry.Encryption">ZipEntry.Encryption</seealso>
        public EncryptionAlgorithm Encryption
        {
            get
            {
                return _Encryption;
            }
            set
            {
                if (value == EncryptionAlgorithm.Unsupported)
                    throw new InvalidOperationException("You may not set Encryption to that value.");
                _Encryption = value;
            }
        }



        /// <summary>
        ///   A callback that allows the application to specify the compression level
        ///   to use for entries subsequently added to the zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   With this callback, the DotNetZip library allows the application to
        ///   determine whether compression will be used, at the time of the
        ///   <c>Save</c>. This may be useful if the application wants to favor
        ///   speed over size, and wants to defer the decision until the time of
        ///   <c>Save</c>.
        /// </para>
        ///
        /// <para>
        ///   Typically applications set the <see cref="CompressionLevel"/> property on
        ///   the <c>ZipFile</c> or on each <c>ZipEntry</c> to determine the level of
        ///   compression used. This is done at the time the entry is added to the
        ///   <c>ZipFile</c>. Setting the property to
        ///   <c>Ionic.Zlib.CompressionLevel.None</c> means no compression will be used.
        /// </para>
        ///
        /// <para>
        ///   This callback allows the application to defer the decision on the
        ///   <c>CompressionLevel</c> to use, until the time of the call to
        ///   <c>ZipFile.Save()</c>. The callback is invoked once per <c>ZipEntry</c>,
        ///   at the time the data for the entry is being written out as part of a
        ///   <c>Save()</c> operation. The application can use whatever criteria it
        ///   likes in determining the level to return.  For example, an application may
        ///   wish that no .mp3 files should be compressed, because they are already
        ///   compressed and the extra compression is not worth the CPU time incurred,
        ///   and so can return <c>None</c> for all .mp3 entries.
        /// </para>
        ///
        /// <para>
        ///   The library determines whether compression will be attempted for an entry
        ///   this way: If the entry is a zero length file, or a directory, no
        ///   compression is used.  Otherwise, if this callback is set, it is invoked
        ///   and the <c>CompressionLevel</c> is set to the return value. If this
        ///   callback has not been set, then the previously set value for
        ///   <c>CompressionLevel</c> is used.
        /// </para>
        ///
        /// </remarks>
        public SetCompressionCallback SetCompression
        {
            get;
            set;
        }




#if !NETCF
        /// <summary>
        ///   The size threshold for an entry, above which a parallel deflate is used.
        /// </summary>
        ///
        /// <remarks>
        ///
        ///   <para>
        ///     DotNetZip will use multiple threads to compress any ZipEntry,
        ///     if the entry is larger than the given size.  Zero means "always
        ///     use parallel deflate", while -1 means "never use parallel
        ///     deflate". The default value for this property is 512k. Aside
        ///     from the special values of 0 and 1, the minimum value is 65536.
        ///   </para>
        ///
        ///   <para>
        ///     If the entry size cannot be known before compression, as with a
        ///     read-forward stream, then Parallel deflate will never be
        ///     performed, unless the value of this property is zero.
        ///   </para>
        ///
        ///   <para>
        ///     A parallel deflate operations will speed up the compression of
        ///     large files, on computers with multiple CPUs or multiple CPU
        ///     cores.  For files above 1mb, on a dual core or dual-cpu (2p)
        ///     machine, the time required to compress the file can be 70% of the
        ///     single-threaded deflate.  For very large files on 4p machines the
        ///     compression can be done in 30% of the normal time.  The downside
        ///     is that parallel deflate consumes extra memory during the deflate,
        ///     and the deflation is not as effective.
        ///   </para>
        ///
        ///   <para>
        ///     Parallel deflate tends to yield slightly less compression when
        ///     compared to as single-threaded deflate; this is because the original
        ///     data stream is split into multiple independent buffers, each of which
        ///     is compressed in parallel.  But because they are treated
        ///     independently, there is no opportunity to share compression
        ///     dictionaries.  For that reason, a deflated stream may be slightly
        ///     larger when compressed using parallel deflate, as compared to a
        ///     traditional single-threaded deflate. Sometimes the increase over the
        ///     normal deflate is as much as 5% of the total compressed size. For
        ///     larger files it can be as small as 0.1%.
        ///   </para>
        ///
        ///   <para>
        ///     Multi-threaded compression does not give as much an advantage when
        ///     using Encryption. This is primarily because encryption tends to slow
        ///     down the entire pipeline. Also, multi-threaded compression gives less
        ///     of an advantage when using lower compression levels, for example <see
        ///     cref="Ionic.Zlib.CompressionLevel.BestSpeed"/>.  You may have to
        ///     perform some tests to determine the best approach for your situation.
        ///   </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="ParallelDeflateMaxBufferPairs"/>
        ///
        public long ParallelDeflateThreshold
        {
            set
            {
                if ((value != 0) && (value != -1) && (value < 64 * 1024))
                    throw new ArgumentOutOfRangeException("ParallelDeflateThreshold should be -1, 0, or > 65536");
                _ParallelDeflateThreshold = value;
            }
            get
            {
                return _ParallelDeflateThreshold;
            }
        }

        /// <summary>
        ///   The maximum number of buffer pairs to use when performing
        ///   parallel compression.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This property sets an upper limit on the number of memory
        ///   buffer pairs to create when performing parallel
        ///   compression.  The implementation of the parallel
        ///   compression stream allocates multiple buffers to
        ///   facilitate parallel compression.  As each buffer fills up,
        ///   the stream uses <see
        ///   cref="System.Threading.ThreadPool.QueueUserWorkItem(System.Threading.WaitCallback)">
        ///   ThreadPool.QueueUserWorkItem()</see> to compress those
        ///   buffers in a background threadpool thread. After a buffer
        ///   is compressed, it is re-ordered and written to the output
        ///   stream.
        /// </para>
        ///
        /// <para>
        ///   A higher number of buffer pairs enables a higher degree of
        ///   parallelism, which tends to increase the speed of compression on
        ///   multi-cpu computers.  On the other hand, a higher number of buffer
        ///   pairs also implies a larger memory consumption, more active worker
        ///   threads, and a higher cpu utilization for any compression. This
        ///   property enables the application to limit its memory consumption and
        ///   CPU utilization behavior depending on requirements.
        /// </para>
        ///
        /// <para>
        ///   For each compression "task" that occurs in parallel, there are 2
        ///   buffers allocated: one for input and one for output.  This property
        ///   sets a limit for the number of pairs.  The total amount of storage
        ///   space allocated for buffering will then be (N*S*2), where N is the
        ///   number of buffer pairs, S is the size of each buffer (<see
        ///   cref="BufferSize"/>).  By default, DotNetZip allocates 4 buffer
        ///   pairs per CPU core, so if your machine has 4 cores, and you retain
        ///   the default buffer size of 128k, then the
        ///   ParallelDeflateOutputStream will use 4 * 4 * 2 * 128kb of buffer
        ///   memory in total, or 4mb, in blocks of 128kb.  If you then set this
        ///   property to 8, then the number will be 8 * 2 * 128kb of buffer
        ///   memory, or 2mb.
        /// </para>
        ///
        /// <para>
        ///   CPU utilization will also go up with additional buffers, because a
        ///   larger number of buffer pairs allows a larger number of background
        ///   threads to compress in parallel. If you find that parallel
        ///   compression is consuming too much memory or CPU, you can adjust this
        ///   value downward.
        /// </para>
        ///
        /// <para>
        ///   The default value is 16. Different values may deliver better or
        ///   worse results, depending on your priorities and the dynamic
        ///   performance characteristics of your storage and compute resources.
        /// </para>
        ///
        /// <para>
        ///   This property is not the number of buffer pairs to use; it is an
        ///   upper limit. An illustration: Suppose you have an application that
        ///   uses the default value of this property (which is 16), and it runs
        ///   on a machine with 2 CPU cores. In that case, DotNetZip will allocate
        ///   4 buffer pairs per CPU core, for a total of 8 pairs.  The upper
        ///   limit specified by this property has no effect.
        /// </para>
        ///
        /// <para>
        ///   The application can set this value at any time
        ///   before calling <c>ZipFile.Save()</c>.
        /// </para>
        /// </remarks>
        ///
        /// <seealso cref="ParallelDeflateThreshold"/>
        ///
        public int ParallelDeflateMaxBufferPairs
        {
            get
            {
                return _maxBufferPairs;
            }
            set
            {
                if (value < 4)
                    throw new ArgumentOutOfRangeException("ParallelDeflateMaxBufferPairs",
                                                "Value must be 4 or greater.");
                _maxBufferPairs = value;
            }
        }
#endif


        /// <summary>Provides a string representation of the instance.</summary>
        /// <returns>a string representation of the instance.</returns>
        public override String ToString()
        {
            return String.Format("ZipFile::{0}", "(stream)");
        }


        internal void NotifyEntryChanged()
        {
            _contentsChanged = true;
        }





        // called by ZipEntry in ZipEntry.Extract(), when there is no stream set for the
        // ZipEntry.
        internal void Reset(bool whileSaving)
        {
            if (_JustSaved)
            {
                // read in the just-saved zip archive
                using (ZipFile x = new ZipFile())
                {
                    // workitem 10735
                    x.AlternateEncoding = this.AlternateEncoding;
                    x.AlternateEncodingUsage = this.AlternateEncodingUsage;
                    ReadIntoInstance(x);
                    // copy the contents of the entries.
                    // cannot just replace the entries - the app may be holding them
                    foreach (ZipEntry e1 in x)
                    {
                        foreach (ZipEntry e2 in this)
                        {
                            if (e1.FileName == e2.FileName)
                            {
                                e2.CopyMetaData(e1);
                                break;
                            }
                        }
                    }
                }
                _JustSaved = false;
            }
        }


        #endregion

        #region Constructors




        /// <summary>
        ///   Create a zip file, without specifying a target filename or stream to save to.
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
        ///   After instantiating with this constructor and adding entries to the
        ///   archive, the application should call <see cref="ZipFile.Save(String)"/> or
        ///   <see cref="ZipFile.Save(System.IO.Stream)"/> to save to a file or a
        ///   stream, respectively.  The application can also set the <see cref="Name"/>
        ///   property and then call the no-argument <see cref="Save()"/> method.  (This
        ///   is the preferred approach for applications that use the library through
        ///   COM interop.)  If you call the no-argument <see cref="Save()"/> method
        ///   without having set the <c>Name</c> of the <c>ZipFile</c>, either through
        ///   the parameterized constructor or through the explicit property , the
        ///   Save() will throw, because there is no place to save the file.  </para>
        ///
        /// <para>
        ///   Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
        ///   have multiple threads that each use a distinct <c>ZipFile</c> instance, or
        ///   you can synchronize multi-thread access to a single instance.  </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// This example creates a Zip archive called Backup.zip, containing all the files
        /// in the directory DirectoryToZip. Files within subdirectories are not zipped up.
        /// <code>
        /// using (ZipFile zip = new ZipFile())
        /// {
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   // note: this code does not recurse subdirectories!
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   zip.AddFiles(filenames, "files");
        ///   zip.Save("Backup.zip");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     ' Store all files found in the top level directory, into the zip archive.
        ///     ' note: this code does not recurse subdirectories!
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     zip.AddFiles(filenames, "files")
        ///     zip.Save("Backup.zip")
        /// End Using
        /// </code>
        /// </example>
        public ZipFile()
        {
            _InitInstance(null, null);
        }


        /// <summary>
        ///   Create a zip file, specifying a text Encoding, but without specifying a
        ///   target filename or stream to save to.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   See the documentation on the <see cref="ZipFile(String)">ZipFile
        ///   constructor that accepts a single string argument</see> for basic
        ///   information on all the <c>ZipFile</c> constructors.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <param name="encoding">
        /// The Encoding is used as the default alternate encoding for entries with
        /// filenames or comments that cannot be encoded with the IBM437 code page.
        /// </param>
        public ZipFile(System.Text.Encoding encoding)
        {
            AlternateEncoding = encoding;
            AlternateEncodingUsage = ZipOption.Always;
            _InitInstance(null, null);
        }





        private void _initEntriesDictionary()
        {
            // workitem 9868
            StringComparer sc = (CaseSensitiveRetrieval) ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            _entries = (_entries == null)
                ? new Dictionary<String, ZipEntry>(sc)
                : new Dictionary<String, ZipEntry>(_entries, sc);
        }


        private void _InitInstance(string zipFileName, TextWriter statusMessageWriter)
        {
            // create a new zipfile
            _StatusMessageTextWriter = statusMessageWriter;
            _contentsChanged = true;
            CompressionLevel = Ionic.Zlib.CompressionLevel.Default;
#if !NETCF
            ParallelDeflateThreshold = 512 * 1024;
#endif
            // workitem 7685, 9868
            _initEntriesDictionary();


            return;
        }
        #endregion



        #region Indexers and Collections

        private List<ZipEntry> ZipEntriesAsList
        {
            get
            {
                if (_zipEntriesAsList == null)
                    _zipEntriesAsList = new List<ZipEntry>(_entries.Values);
                return _zipEntriesAsList;
            }
        }

        /// <summary>
        ///   This is an integer indexer into the Zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This property is read-only.
        /// </para>
        ///
        /// <para>
        ///   Internally, the <c>ZipEntry</c> instances that belong to the
        ///   <c>ZipFile</c> are stored in a Dictionary.  When you use this
        ///   indexer the first time, it creates a read-only
        ///   <c>List&lt;ZipEntry&gt;</c> from the Dictionary.Values Collection.
        ///   If at any time you modify the set of entries in the <c>ZipFile</c>,
        ///   either by adding an entry, removing an entry, or renaming an
        ///   entry, a new List will be created, and the numeric indexes for the
        ///   remaining entries may be different.
        /// </para>
        ///
        /// <para>
        ///   This means you cannot rename any ZipEntry from
        ///   inside an enumeration of the zip file.
        /// </para>
        ///
        /// <param name="ix">
        ///   The index value.
        /// </param>
        ///
        /// </remarks>
        ///
        /// <returns>
        ///   The <c>ZipEntry</c> within the Zip archive at the specified index. If the
        ///   entry does not exist in the archive, this indexer throws.
        /// </returns>
        ///
        public ZipEntry this[int ix]
        {
            // workitem 6402
            get
            {
                return ZipEntriesAsList[ix];
            }
        }


        /// <summary>
        ///   This is a name-based indexer into the Zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This property is read-only.
        /// </para>
        ///
        /// <para>
        ///   The <see cref="CaseSensitiveRetrieval"/> property on the <c>ZipFile</c>
        ///   determines whether retrieval via this indexer is done via case-sensitive
        ///   comparisons. By default, retrieval is not case sensitive.  This makes
        ///   sense on Windows, in which filesystems are not case sensitive.
        /// </para>
        ///
        /// <para>
        ///   Regardless of case-sensitivity, it is not always the case that
        ///   <c>this[value].FileName == value</c>. In other words, the <c>FileName</c>
        ///   property of the <c>ZipEntry</c> retrieved with this indexer, may or may
        ///   not be equal to the index value.
        /// </para>
        ///
        /// <para>
        ///   This is because DotNetZip performs a normalization of filenames passed to
        ///   this indexer, before attempting to retrieve the item.  That normalization
        ///   includes: removal of a volume letter and colon, swapping backward slashes
        ///   for forward slashes.  So, <c>zip["dir1\\entry1.txt"].FileName ==
        ///   "dir1/entry.txt"</c>.
        /// </para>
        ///
        /// <para>
        ///   Directory entries in the zip file may be retrieved via this indexer only
        ///   with names that have a trailing slash. DotNetZip automatically appends a
        ///   trailing slash to the names of any directory entries added to a zip.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <example>
        /// This example extracts only the entries in a zip file that are .txt files.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read("PackedDocuments.zip"))
        /// {
        ///   foreach (string s1 in zip.EntryFilenames)
        ///   {
        ///     if (s1.EndsWith(".txt"))
        ///       zip[s1].Extract("textfiles");
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
        /// <seealso cref="Ionic.Zip.ZipFile.RemoveEntry(string)"/>
        ///
        /// <exception cref="System.ArgumentException">
        ///   Thrown if the caller attempts to assign a non-null value to the indexer.
        /// </exception>
        ///
        /// <param name="fileName">
        ///   The name of the file, including any directory path, to retrieve from the
        ///   zip.  The filename match is not case-sensitive by default; you can use the
        ///   <see cref="CaseSensitiveRetrieval"/> property to change this behavior. The
        ///   pathname can use forward-slashes or backward slashes.
        /// </param>
        ///
        /// <returns>
        ///   The <c>ZipEntry</c> within the Zip archive, given by the specified
        ///   filename. If the named entry does not exist in the archive, this indexer
        ///   returns <c>null</c> (<c>Nothing</c> in VB).
        /// </returns>
        ///
        public ZipEntry this[String fileName]
        {
            get
            {
                var key = SharedUtilities.NormalizePathForUseInZipFile(fileName);
                if (_entries.ContainsKey(key))
                    return _entries[key];
                // workitem 11056
                key = key.Replace("/", "\\");
                if (_entries.ContainsKey(key))
                    return _entries[key];
                return null;

#if MESSY
                foreach (ZipEntry e in _entries.Values)
                {
                    if (this.CaseSensitiveRetrieval)
                    {
                        // check for the file match with a case-sensitive comparison.
                        if (e.FileName == fileName) return e;
                        // also check for equivalence
                        if (fileName.Replace("\\", "/") == e.FileName) return e;
                        if (e.FileName.Replace("\\", "/") == fileName) return e;

                        // check for a difference only in trailing slash
                        if (e.FileName.EndsWith("/"))
                        {
                            var fileNameNoSlash = e.FileName.Trim("/".ToCharArray());
                            if (fileNameNoSlash == fileName) return e;
                            // also check for equivalence
                            if (fileName.Replace("\\", "/") == fileNameNoSlash) return e;
                            if (fileNameNoSlash.Replace("\\", "/") == fileName) return e;
                        }

                    }
                    else
                    {
                        // check for the file match in a case-insensitive manner.
                        if (String.Compare(e.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                        // also check for equivalence
                        if (String.Compare(fileName.Replace("\\", "/"), e.FileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                        if (String.Compare(e.FileName.Replace("\\", "/"), fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;

                        // check for a difference only in trailing slash
                        if (e.FileName.EndsWith("/"))
                        {
                            var fileNameNoSlash = e.FileName.Trim("/".ToCharArray());

                            if (String.Compare(fileNameNoSlash, fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                            // also check for equivalence
                            if (String.Compare(fileName.Replace("\\", "/"), fileNameNoSlash, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                            if (String.Compare(fileNameNoSlash.Replace("\\", "/"), fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;

                        }

                    }

                }
                return null;

#endif
            }
        }


        /// <summary>
        ///   The list of filenames for the entries contained within the zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///   According to the ZIP specification, the names of the entries use forward
        ///   slashes in pathnames.  If you are scanning through the list, you may have
        ///   to swap forward slashes for backslashes.
        /// </remarks>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.this[string]"/>
        ///
        /// <example>
        ///   This example shows one way to test if a filename is already contained
        ///   within a zip archive.
        /// <code>
        /// String zipFileToRead= "PackedDocuments.zip";
        /// string candidate = "DatedMaterial.xps";
        /// using (ZipFile zip = new ZipFile(zipFileToRead))
        /// {
        ///   if (zip.EntryFilenames.Contains(candidate))
        ///     Console.WriteLine("The file '{0}' exists in the zip archive '{1}'",
        ///                       candidate,
        ///                       zipFileName);
        ///   else
        ///     Console.WriteLine("The file, '{0}', does not exist in the zip archive '{1}'",
        ///                       candidate,
        ///                       zipFileName);
        ///   Console.WriteLine();
        /// }
        /// </code>
        /// <code lang="VB">
        ///   Dim zipFileToRead As String = "PackedDocuments.zip"
        ///   Dim candidate As String = "DatedMaterial.xps"
        ///   Using zip As ZipFile.Read(ZipFileToRead)
        ///       If zip.EntryFilenames.Contains(candidate) Then
        ///           Console.WriteLine("The file '{0}' exists in the zip archive '{1}'", _
        ///                       candidate, _
        ///                       zipFileName)
        ///       Else
        ///         Console.WriteLine("The file, '{0}', does not exist in the zip archive '{1}'", _
        ///                       candidate, _
        ///                       zipFileName)
        ///       End If
        ///       Console.WriteLine
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <returns>
        ///   The list of strings for the filenames contained within the Zip archive.
        /// </returns>
        ///
        public System.Collections.Generic.ICollection<String> EntryFileNames
        {
            get
            {
                return _entries.Keys;
            }
        }


        /// <summary>
        ///   Returns the readonly collection of entries in the Zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        ///   If there are no entries in the current <c>ZipFile</c>, the value returned is a
        ///   non-null zero-element collection.  If there are entries in the zip file,
        ///   the elements are returned in no particular order.
        /// </para>
        /// <para>
        ///   This is the implied enumerator on the <c>ZipFile</c> class.  If you use a
        ///   <c>ZipFile</c> instance in a context that expects an enumerator, you will
        ///   get this collection.
        /// </para>
        /// </remarks>
        /// <seealso cref="EntriesSorted"/>
        public System.Collections.Generic.ICollection<ZipEntry> Entries
        {
            get
            {
                return _entries.Values;
            }
        }


        /// <summary>
        ///   Returns a readonly collection of entries in the Zip archive, sorted by FileName.
        /// </summary>
        ///
        /// <remarks>
        ///   If there are no entries in the current <c>ZipFile</c>, the value returned
        ///   is a non-null zero-element collection.  If there are entries in the zip
        ///   file, the elements are returned sorted by the name of the entry.
        /// </remarks>
        ///
        /// <example>
        ///
        ///   This example fills a Windows Forms ListView with the entries in a zip file.
        ///
        /// <code lang="C#">
        /// using (ZipFile zip = ZipFile.Read(zipFile))
        /// {
        ///     foreach (ZipEntry entry in zip.EntriesSorted)
        ///     {
        ///         ListViewItem item = new ListViewItem(n.ToString());
        ///         n++;
        ///         string[] subitems = new string[] {
        ///             entry.FileName.Replace("/","\\"),
        ///             entry.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
        ///             entry.UncompressedSize.ToString(),
        ///             String.Format("{0,5:F0}%", entry.CompressionRatio),
        ///             entry.CompressedSize.ToString(),
        ///             (entry.UsesEncryption) ? "Y" : "N",
        ///             String.Format("{0:X8}", entry.Crc)};
        ///
        ///         foreach (String s in subitems)
        ///         {
        ///             ListViewItem.ListViewSubItem subitem = new ListViewItem.ListViewSubItem();
        ///             subitem.Text = s;
        ///             item.SubItems.Add(subitem);
        ///         }
        ///
        ///         this.listView1.Items.Add(item);
        ///     }
        /// }
        /// </code>
        /// </example>
        ///
        /// <seealso cref="Entries"/>
        public System.Collections.Generic.ICollection<ZipEntry> EntriesSorted
        {
            get
            {
                var coll = new System.Collections.Generic.List<ZipEntry>();
                foreach (var e in this.Entries)
                {
                    coll.Add(e);
                }
                StringComparison sc = (CaseSensitiveRetrieval) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                coll.Sort((x, y) => { return String.Compare(x.FileName, y.FileName, sc); });
                return new System.Collections.ObjectModel.ReadOnlyCollection<ZipEntry>(coll);
            }
        }


        /// <summary>
        /// Returns the number of entries in the Zip archive.
        /// </summary>
        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }



        /// <summary>
        ///   Removes the given <c>ZipEntry</c> from the zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   After calling <c>RemoveEntry</c>, the application must call <c>Save</c> to
        ///   make the changes permanent.
        /// </para>
        /// </remarks>
        ///
        /// <exception cref="System.ArgumentException">
        ///   Thrown if the specified <c>ZipEntry</c> does not exist in the <c>ZipFile</c>.
        /// </exception>
        ///
        /// <example>
        ///   In this example, all entries in the zip archive dating from before
        ///   December 31st, 2007, are removed from the archive.  This is actually much
        ///   easier if you use the RemoveSelectedEntries method.  But I needed an
        ///   example for RemoveEntry, so here it is.
        /// <code>
        /// String ZipFileToRead = "ArchiveToModify.zip";
        /// System.DateTime Threshold = new System.DateTime(2007,12,31);
        /// using (ZipFile zip = ZipFile.Read(ZipFileToRead))
        /// {
        ///   var EntriesToRemove = new System.Collections.Generic.List&lt;ZipEntry&gt;();
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///     if (e.LastModified &lt; Threshold)
        ///     {
        ///       // We cannot remove the entry from the list, within the context of
        ///       // an enumeration of said list.
        ///       // So we add the doomed entry to a list to be removed later.
        ///       EntriesToRemove.Add(e);
        ///     }
        ///   }
        ///
        ///   // actually remove the doomed entries.
        ///   foreach (ZipEntry zombie in EntriesToRemove)
        ///     zip.RemoveEntry(zombie);
        ///
        ///   zip.Comment= String.Format("This zip archive was updated at {0}.",
        ///                              System.DateTime.Now.ToString("G"));
        ///
        ///   // save with a different name
        ///   zip.Save("Archive-Updated.zip");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        ///   Dim ZipFileToRead As String = "ArchiveToModify.zip"
        ///   Dim Threshold As New DateTime(2007, 12, 31)
        ///   Using zip As ZipFile = ZipFile.Read(ZipFileToRead)
        ///       Dim EntriesToRemove As New System.Collections.Generic.List(Of ZipEntry)
        ///       Dim e As ZipEntry
        ///       For Each e In zip
        ///           If (e.LastModified &lt; Threshold) Then
        ///               ' We cannot remove the entry from the list, within the context of
        ///               ' an enumeration of said list.
        ///               ' So we add the doomed entry to a list to be removed later.
        ///               EntriesToRemove.Add(e)
        ///           End If
        ///       Next
        ///
        ///       ' actually remove the doomed entries.
        ///       Dim zombie As ZipEntry
        ///       For Each zombie In EntriesToRemove
        ///           zip.RemoveEntry(zombie)
        ///       Next
        ///       zip.Comment = String.Format("This zip archive was updated at {0}.", DateTime.Now.ToString("G"))
        ///       'save as a different name
        ///       zip.Save("Archive-Updated.zip")
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <param name="entry">
        /// The <c>ZipEntry</c> to remove from the zip.
        /// </param>
        ///
        /// <seealso cref="Ionic.Zip.ZipFile.RemoveSelectedEntries(string)"/>
        ///
        public void RemoveEntry(ZipEntry entry)
        {
            //if (!_entries.Values.Contains(entry))
            //    throw new ArgumentException("The entry you specified does not exist in the zip archive.");
            if (entry == null)
                throw new ArgumentNullException("entry");

            _entries.Remove(SharedUtilities.NormalizePathForUseInZipFile(entry.FileName));
            _zipEntriesAsList = null;

#if NOTNEEDED
            if (_direntries != null)
            {
                bool FoundAndRemovedDirEntry = false;
                foreach (ZipDirEntry de1 in _direntries)
                {
                    if (entry.FileName == de1.FileName)
                    {
                        _direntries.Remove(de1);
                        FoundAndRemovedDirEntry = true;
                        break;
                    }
                }

                if (!FoundAndRemovedDirEntry)
                    throw new BadStateException("The entry to be removed was not found in the directory.");
            }
#endif
            _contentsChanged = true;
        }




        /// <summary>
        /// Removes the <c>ZipEntry</c> with the given filename from the zip archive.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   After calling <c>RemoveEntry</c>, the application must call <c>Save</c> to
        ///   make the changes permanent.
        /// </para>
        ///
        /// </remarks>
        ///
        /// <exception cref="System.InvalidOperationException">
        ///   Thrown if the <c>ZipFile</c> is not updatable.
        /// </exception>
        ///
        /// <exception cref="System.ArgumentException">
        ///   Thrown if a <c>ZipEntry</c> with the specified filename does not exist in
        ///   the <c>ZipFile</c>.
        /// </exception>
        ///
        /// <example>
        ///
        ///   This example shows one way to remove an entry with a given filename from
        ///   an existing zip archive.
        ///
        /// <code>
        /// String zipFileToRead= "PackedDocuments.zip";
        /// string candidate = "DatedMaterial.xps";
        /// using (ZipFile zip = ZipFile.Read(zipFileToRead))
        /// {
        ///   if (zip.EntryFilenames.Contains(candidate))
        ///   {
        ///     zip.RemoveEntry(candidate);
        ///     zip.Comment= String.Format("The file '{0}' has been removed from this archive.",
        ///                                Candidate);
        ///     zip.Save();
        ///   }
        /// }
        /// </code>
        /// <code lang="VB">
        ///   Dim zipFileToRead As String = "PackedDocuments.zip"
        ///   Dim candidate As String = "DatedMaterial.xps"
        ///   Using zip As ZipFile = ZipFile.Read(zipFileToRead)
        ///       If zip.EntryFilenames.Contains(candidate) Then
        ///           zip.RemoveEntry(candidate)
        ///           zip.Comment = String.Format("The file '{0}' has been removed from this archive.", Candidate)
        ///           zip.Save
        ///       End If
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <param name="fileName">
        /// The name of the file, including any directory path, to remove from the zip.
        /// The filename match is not case-sensitive by default; you can use the
        /// <c>CaseSensitiveRetrieval</c> property to change this behavior. The
        /// pathname can use forward-slashes or backward slashes.
        /// </param>
        ///
        public void RemoveEntry(String fileName)
        {
            string modifiedName = ZipEntry.NameInArchive(fileName, null);
            ZipEntry e = this[modifiedName];
            if (e == null)
                throw new ArgumentException("The entry you specified was not found in the zip archive.");

            RemoveEntry(e);
        }


        #endregion

        #region Destructors and Disposers

        //         /// <summary>
        //         /// This is the class Destructor, which gets called implicitly when the instance
        //         /// is destroyed.  Because the <c>ZipFile</c> type implements IDisposable, this
        //         /// method calls Dispose(false).
        //         /// </summary>
        //         ~ZipFile()
        //         {
        //             // call Dispose with false.  Since we're in the
        //             // destructor call, the managed resources will be
        //             // disposed of anyways.
        //             Dispose(false);
        //         }

        /// <summary>
        ///   Closes the read and write streams associated
        ///   to the <c>ZipFile</c>, if necessary.
        /// </summary>
        ///
        /// <remarks>
        ///   The Dispose() method is generally employed implicitly, via a <c>using(..) {..}</c>
        ///   statement. (<c>Using...End Using</c> in VB) If you do not employ a using
        ///   statement, insure that your application calls Dispose() explicitly.  For
        ///   example, in a Powershell application, or an application that uses the COM
        ///   interop interface, you must call Dispose() explicitly.
        /// </remarks>
        ///
        /// <example>
        /// This example extracts an entry selected by name, from the Zip file to the
        /// Console.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read(zipfile))
        /// {
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///     if (WantThisEntry(e.FileName))
        ///       zip.Extract(e.FileName, Console.OpenStandardOutput());
        ///   }
        /// } // Dispose() is called implicitly here.
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As ZipFile = ZipFile.Read(zipfile)
        ///     Dim e As ZipEntry
        ///     For Each e In zip
        ///       If WantThisEntry(e.FileName) Then
        ///           zip.Extract(e.FileName, Console.OpenStandardOutput())
        ///       End If
        ///     Next
        /// End Using ' Dispose is implicity called here
        /// </code>
        /// </example>
        public void Dispose()
        {
            // dispose of the managed and unmanaged resources
            Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Disposes any managed resources, if the flag is set, then marks the
        ///   instance disposed.  This method is typically not called explicitly from
        ///   application code.
        /// </summary>
        ///
        /// <remarks>
        ///   Applications should call <see cref="Dispose()">the no-arg Dispose method</see>.
        /// </remarks>
        ///
        /// <param name="disposeManagedResources">
        ///   indicates whether the method should dispose streams or not.
        /// </param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!this._disposed)
            {
                if (disposeManagedResources)
                {
                    // dispose managed resources

#if !NETCF
                    // workitem 10030
                    if (this.ParallelDeflater != null)
                    {
                        this.ParallelDeflater.Dispose();
                        this.ParallelDeflater = null;
                    }
#endif
                }
                this._disposed = true;
            }
        }
        #endregion


        #region private properties

        internal Stream ReadStream
        {
            get
            {
                return _readstream;
            }
        }



        private Stream WriteStream
        {
            // workitem 9763
            get
            {
                return _writestream;
            }
            set
            {
                if (value != null)
                    throw new ZipException("Cannot set the stream to a non-null value.");
                _writestream = null;
            }
        }
        #endregion

        #region private fields
        private TextWriter _StatusMessageTextWriter;
        private bool _CaseSensitiveRetrieval;
        private Stream _readstream;
        private Stream _writestream;
        private UInt16 _versionMadeBy;
        private UInt16 _versionNeededToExtract;
        private UInt32 _diskNumberWithCd;
        private ZipErrorAction _zipErrorAction;
        private bool _disposed;
        //private System.Collections.Generic.List<ZipEntry> _entries;
        private System.Collections.Generic.Dictionary<String, ZipEntry> _entries;
        private List<ZipEntry> _zipEntriesAsList;
        private string _Comment;
        internal string _Password;
        private bool _emitNtfsTimes = true;
        private bool _emitUnixTimes;
        private Ionic.Zlib.CompressionStrategy _Strategy = Ionic.Zlib.CompressionStrategy.Default;
        private Ionic.Zip.CompressionMethod _compressionMethod = Ionic.Zip.CompressionMethod.Deflate;
        private bool _fileAlreadyExists;
        private bool _contentsChanged;
        private bool _hasBeenSaved;
        private object LOCK = new object();
        private bool _saveOperationCanceled;
        private bool _extractOperationCanceled;
        private bool _addOperationCanceled;
        private EncryptionAlgorithm _Encryption;
        private bool _JustSaved;
        private long _locEndOfCDS = -1;
        private uint _OffsetOfCentralDirectory;
        private Int64 _OffsetOfCentralDirectory64;
        private Nullable<bool> _OutputUsesZip64;
        internal bool _inExtractAll;
        private System.Text.Encoding _alternateEncoding = System.Text.Encoding.GetEncoding("IBM437"); // UTF-8
        private ZipOption _alternateEncodingUsage = ZipOption.Never;
        private static System.Text.Encoding _defaultEncoding = System.Text.Encoding.GetEncoding("IBM437");

        private int _BufferSize = BufferSizeDefault;

#if !NETCF
        internal Ionic.Zlib.ParallelDeflateOutputStream ParallelDeflater;
        private long _ParallelDeflateThreshold;
        private int _maxBufferPairs = 16;
#endif

        internal Zip64Option _zip64 = Zip64Option.Default;

        /// <summary>
        ///   Default size of the buffer used for IO.
        /// </summary>
        public static readonly int BufferSizeDefault = 32768;

        #endregion
    }

    /// <summary>
    ///   Options for using ZIP64 extensions when saving zip archives.
    /// </summary>
    ///
    /// <remarks>
    ///
    /// <para>
    ///   Designed many years ago, the <see
    ///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">original zip
    ///   specification from PKWARE</see> allowed for 32-bit quantities for the
    ///   compressed and uncompressed sizes of zip entries, as well as a 32-bit quantity
    ///   for specifying the length of the zip archive itself, and a maximum of 65535
    ///   entries.  These limits are now regularly exceeded in many backup and archival
    ///   scenarios.  Recently, PKWare added extensions to the original zip spec, called
    ///   "ZIP64 extensions", to raise those limitations.  This property governs whether
    ///   DotNetZip will use those extensions when writing zip archives. The use of
    ///   these extensions is optional and explicit in DotNetZip because, despite the
    ///   status of ZIP64 as a bona fide standard, many other zip tools and libraries do
    ///   not support ZIP64, and therefore a zip file with ZIP64 extensions may be
    ///   unreadable by some of those other tools.
    /// </para>
    ///
    /// <para>
    ///   Set this property to <see cref="Zip64Option.Always"/> to always use ZIP64
    ///   extensions when saving, regardless of whether your zip archive needs it.
    ///   Suppose you add 5 files, each under 100k, to a ZipFile. If you specify Always
    ///   for this flag, you will get a ZIP64 archive, though the archive does not need
    ///   to use ZIP64 because none of the original zip limits had been exceeded.
    /// </para>
    ///
    /// <para>
    ///   Set this property to <see cref="Zip64Option.Never"/> to tell the DotNetZip
    ///   library to never use ZIP64 extensions.  This is useful for maximum
    ///   compatibility and interoperability, at the expense of the capability of
    ///   handling large files or large archives.  NB: Windows Explorer in Windows XP
    ///   and Windows Vista cannot currently extract files from a zip64 archive, so if
    ///   you want to guarantee that a zip archive produced by this library will work in
    ///   Windows Explorer, use <c>Never</c>. If you set this property to <see
    ///   cref="Zip64Option.Never"/>, and your application creates a zip that would
    ///   exceed one of the Zip limits, the library will throw an exception while saving
    ///   the zip file.
    /// </para>
    ///
    /// <para>
    ///   Set this property to <see cref="Zip64Option.AsNecessary"/> to tell the
    ///   DotNetZip library to use the ZIP64 extensions when required by the
    ///   entry. After the file is compressed, the original and compressed sizes are
    ///   checked, and if they exceed the limits described above, then zip64 can be
    ///   used. That is the general idea, but there is an additional wrinkle when saving
    ///   to a non-seekable device, like the ASP.NET <c>Response.OutputStream</c>, or
    ///   <c>Console.Out</c>.  When using non-seekable streams for output, the entry
    ///   header - which indicates whether zip64 is in use - is emitted before it is
    ///   known if zip64 is necessary.  It is only after all entries have been saved
    ///   that it can be known if ZIP64 will be required.  On seekable output streams,
    ///   after saving all entries, the library can seek backward and re-emit the zip
    ///   file header to be consistent with the actual ZIP64 requirement.  But using a
    ///   non-seekable output stream, the library cannot seek backward, so the header
    ///   can never be changed. In other words, the archive's use of ZIP64 extensions is
    ///   not alterable after the header is emitted.  Therefore, when saving to
    ///   non-seekable streams, using <see cref="Zip64Option.AsNecessary"/> is the same
    ///   as using <see cref="Zip64Option.Always"/>: it will always produce a zip
    ///   archive that uses ZIP64 extensions.
    /// </para>
    ///
    /// </remarks>
    public enum Zip64Option
    {
        /// <summary>
        /// The default behavior, which is "Never".
        /// (For COM clients, this is a 0 (zero).)
        /// </summary>
        Default = 0,
        /// <summary>
        /// Do not use ZIP64 extensions when writing zip archives.
        /// (For COM clients, this is a 0 (zero).)
        /// </summary>
        Never = 0,
        /// <summary>
        /// Use ZIP64 extensions when writing zip archives, as necessary.
        /// For example, when a single entry exceeds 0xFFFFFFFF in size, or when the archive as a whole
        /// exceeds 0xFFFFFFFF in size, or when there are more than 65535 entries in an archive.
        /// (For COM clients, this is a 1.)
        /// </summary>
        AsNecessary = 1,
        /// <summary>
        /// Always use ZIP64 extensions when writing zip archives, even when unnecessary.
        /// (For COM clients, this is a 2.)
        /// </summary>
        Always
    }


    /// <summary>
    ///  An enum representing the values on a three-way toggle switch
    ///  for various options in the library. This might be used to
    ///  specify whether to employ a particular text encoding, or to use
    ///  ZIP64 extensions, or some other option.
    /// </summary>
    public enum ZipOption
    {
        /// <summary>
        /// The default behavior. This is the same as "Never".
        /// (For COM clients, this is a 0 (zero).)
        /// </summary>
        Default = 0,
        /// <summary>
        /// Never use the associated option.
        /// (For COM clients, this is a 0 (zero).)
        /// </summary>
        Never = 0,
        /// <summary>
        /// Use the associated behavior "as necessary."
        /// (For COM clients, this is a 1.)
        /// </summary>
        AsNecessary = 1,
        /// <summary>
        /// Use the associated behavior Always, whether necessary or not.
        /// (For COM clients, this is a 2.)
        /// </summary>
        Always
    }


    enum AddOrUpdateAction
    {
        AddOnly = 0,
        AddOrUpdate
    }

}



// ==================================================================
//
// Information on the ZIP format:
//
// From
// http://www.pkware.com/documents/casestudies/APPNOTE.TXT
//
//  Overall .ZIP file format:
//
//     [local file header 1]
//     [file data 1]
//     [data descriptor 1]  ** sometimes
//     .
//     .
//     .
//     [local file header n]
//     [file data n]
//     [data descriptor n]   ** sometimes
//     [archive decryption header]
//     [archive extra data record]
//     [central directory]
//     [zip64 end of central directory record]
//     [zip64 end of central directory locator]
//     [end of central directory record]
//
// Local File Header format:
//         local file header signature ... 4 bytes  (0x04034b50)
//         version needed to extract ..... 2 bytes
//         general purpose bit field ..... 2 bytes
//         compression method ............ 2 bytes
//         last mod file time ............ 2 bytes
//         last mod file date............. 2 bytes
//         crc-32 ........................ 4 bytes
//         compressed size................ 4 bytes
//         uncompressed size.............. 4 bytes
//         file name length............... 2 bytes
//         extra field length ............ 2 bytes
//         file name                       varies
//         extra field                     varies
//
//
// Data descriptor:  (used only when bit 3 of the general purpose bitfield is set)
//         (although, I have found zip files where bit 3 is not set, yet this descriptor is present!)
//         local file header signature     4 bytes  (0x08074b50)  ** sometimes!!! Not always
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//
//
//   Central directory structure:
//
//       [file header 1]
//       .
//       .
//       .
//       [file header n]
//       [digital signature]
//
//
//       File header:  (This is a ZipDirEntry)
//         central file header signature   4 bytes  (0x02014b50)
//         version made by                 2 bytes
//         version needed to extract       2 bytes
//         general purpose bit flag        2 bytes
//         compression method              2 bytes
//         last mod file time              2 bytes
//         last mod file date              2 bytes
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//         file name length                2 bytes
//         extra field length              2 bytes
//         file comment length             2 bytes
//         disk number start               2 bytes
//         internal file attributes **     2 bytes
//         external file attributes ***    4 bytes
//         relative offset of local header 4 bytes
//         file name (variable size)
//         extra field (variable size)
//         file comment (variable size)
//
// ** The internal file attributes, near as I can tell,
// uses 0x01 for a file and a 0x00 for a directory.
//
// ***The external file attributes follows the MS-DOS file attribute byte, described here:
// at http://support.microsoft.com/kb/q125019/
// 0x0010 => directory
// 0x0020 => file
//
//
// End of central directory record:
//
//         end of central dir signature    4 bytes  (0x06054b50)
//         number of this disk             2 bytes
//         number of the disk with the
//         start of the central directory  2 bytes
//         total number of entries in the
//         central directory on this disk  2 bytes
//         total number of entries in
//         the central directory           2 bytes
//         size of the central directory   4 bytes
//         offset of start of central
//         directory with respect to
//         the starting disk number        4 bytes
//         .ZIP file comment length        2 bytes
//         .ZIP file comment       (variable size)
//
// date and time are packed values, as MSDOS did them
// time: bits 0-4 : seconds (divided by 2)
//            5-10: minute
//            11-15: hour
// date  bits 0-4 : day
//            5-8: month
//            9-15 year (since 1980)
//
// see http://msdn.microsoft.com/en-us/library/ms724274(VS.85).aspx

