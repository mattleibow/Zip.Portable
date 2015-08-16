using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ionic.Zip.PlatformSupport
{
    public static class ZipFileInternal
    {
        public static void OnAddStarted(this ZipFile zipFile)
        {
            zipFile.OnAddStarted();
        }

        public static void OnAddCompleted(this ZipFile zipFile)
        {
            zipFile.OnAddCompleted();
        }

        public static void OnExtractAllStarted(this ZipFile zipFile, string path)
        {
            zipFile.OnExtractAllStarted(path);
        }

        public static void OnExtractEntry(this ZipFile zipFile, int current, bool before, ZipEntry currentEntry, string path)
        {
            zipFile.OnExtractEntry(current, before, currentEntry, path);
        }

        public static void OnExtractAllCompleted(this ZipFile zipFile, string path)
        {
            zipFile.OnExtractAllCompleted(path);
        }

        public static void OnSaveEvent(this ZipFile zipFile, ZipProgressEventType eventFlavor)
        {
            zipFile.OnSaveEvent(eventFlavor);
        }

        public static void SetAddOperationCanceled(this ZipFile zipFile, bool canceled)
        {
            zipFile._addOperationCanceled = canceled;
        }

        public static bool IsAddOperationCanceled(this ZipFile zipFile)
        {
            return zipFile._addOperationCanceled;
        }

        public static void SetExtractOperationCanceled(this ZipFile zipFile, bool canceled)
        {
            zipFile._extractOperationCanceled = canceled;
        }

        public static bool IsExtractOperationCanceled(this ZipFile zipFile)
        {
            return zipFile._extractOperationCanceled;
        }

        public static bool IsSaveOperationCanceled(this ZipFile zipFile)
        {
            return zipFile._saveOperationCanceled;
        }

        public static void SetInAddAll(this ZipFile zipFile, bool inAddAll)
        {
            zipFile._inAddAll = inAddAll;
        }

        public static void SetInExtractAll(this ZipFile zipFile, bool inExtractAll)
        {
            zipFile._inExtractAll = inExtractAll;
        }

        public static void SetShouldDisposeReadStream(this ZipFile zipFile, bool shouldDisposeReadStream)
        {
            zipFile._ReadStreamIsOurs = shouldDisposeReadStream;
        }

        public static void SetUnderlyingZipStream(this ZipFile zipFile, Stream newZipStream)
        {
            // we can close the streams
            if (zipFile._ReadStreamIsOurs)
            {
                if (zipFile.WriteStream != null)
                {
                    zipFile.WriteStream.Dispose();
                }
                if (zipFile.ReadStream != null)
                {
                    zipFile.ReadStream.Dispose();
                }
            }
            // reset all the entry streams
            foreach (var entry in zipFile.Entries)
            {
                var zss1 = entry._archiveStream as ZipSegmentedStream;
                if (zss1 != null)
                {
                    zss1.Dispose();
                }
                entry._archiveStream = null;
            }
            // set the new stream
            zipFile.ReadStream = newZipStream;
        }
    }
}
