using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace XRouter.Test.Common
{
    /// <summary>
    /// Compares files in two directories recursively.
    /// XML files are first normalized before comparison.
    /// </summary>
    public class XmlDirectoryComparer
    {
        public static bool Equals(string expectedRoot, string actualRoot)
        {
            return Equals(expectedRoot, actualRoot, false);
        }

        public static bool Equals(string expectedRoot, string actualRoot, bool printDetails)
        {
            IEnumerable<MessageFile> correctMessageFiles = GetMessageFilesFromRoot(Path.GetFullPath(expectedRoot));
            IEnumerable<MessageFile> resultMessageFiles = GetMessageFilesFromRoot(Path.GetFullPath(actualRoot));

            PathAndHashCompare pathAndHashCompare = new PathAndHashCompare();
            PathCompare pathCompare = new PathCompare();

            bool areIdentical = correctMessageFiles.SequenceEqual(resultMessageFiles, pathAndHashCompare);

            if (printDetails)
            {
                if (!areIdentical)
                {
                    Console.WriteLine("Found differences between {0} and {1}:\n",
                        expectedRoot, actualRoot);
                    PrintDetails(correctMessageFiles, resultMessageFiles, pathAndHashCompare, pathCompare);
                }
            }
            return areIdentical;
        }

        private static void PrintDetails(
            IEnumerable<MessageFile> correctMessageFiles,
            IEnumerable<MessageFile> resultMessageFiles,
            PathAndHashCompare pathAndHashCompare,
            PathCompare pathCompare)
        {
            //missing message files
            var missingMessageFiles = (from messageFile in correctMessageFiles
                                       select messageFile).Except(resultMessageFiles, pathCompare);
            if (missingMessageFiles.Any())
            {
                Console.WriteLine("Missing files:");
                Console.WriteLine(string.Join("\n", missingMessageFiles.Select((file) => file.path)));
                Console.WriteLine();
            }

            //redundant message files
            var redundantMessageFiles = (from messageFile in resultMessageFiles
                                         select messageFile).Except(correctMessageFiles, pathCompare);
            if (redundantMessageFiles.Any())
            {
                Console.WriteLine("Redundant files:");
                Console.WriteLine(string.Join("\n", redundantMessageFiles.Select((file) => file.path)));
                Console.WriteLine();
            }

            //different message files
            var wrongMessageFiles = (from messageFile in correctMessageFiles
                                     select messageFile)
                                     .Except(resultMessageFiles, pathAndHashCompare)
                                     .Except(missingMessageFiles, pathCompare)
                                     .Except(redundantMessageFiles, pathCompare);
            if (wrongMessageFiles.Any())
            {
                Console.WriteLine("Different files:");
                Console.WriteLine(string.Join("\n", wrongMessageFiles.Select((file) => file.path)));
                Console.WriteLine();
            }
        }

        private static IEnumerable<MessageFile> GetMessageFilesFromRoot(string root)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(root);
            IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);

            List<MessageFile> messageFiles = new List<MessageFile>();
            foreach (FileInfo file in fileList)
            {
                messageFiles.Add(new MessageFile(file, root));
            }

            return messageFiles.ToArray();
        }

        private class MessageFile
        {
            public string path; //relative to the root, also contains the name
            public byte[] hash; //XML hashed is normalized

            public MessageFile(FileInfo file, string root)
            {
                this.path = file.FullName.Replace(root, "");

                XmlDsigC14NTransform xfrm = new XmlDsigC14NTransform(false);
                FileStream fs = file.OpenRead();
                xfrm.LoadInput(fs);
                this.hash = xfrm.GetDigestedOutput(new SHA1Managed());
                fs.Close();
            }
        }

        private class PathAndHashCompare : IEqualityComparer<MessageFile>
        {
            public PathAndHashCompare() { }

            public bool Equals(MessageFile mf1, MessageFile mf2)
            {
                return (mf1.path == mf2.path) && (mf1.hash.SequenceEqual(mf2.hash));
            }

            public int GetHashCode(MessageFile mf)
            {
                string s = String.Format("{0}{1}", mf.path, Encoding.ASCII.GetString(mf.hash));
                return s.GetHashCode();
            }
        }

        private class PathCompare : IEqualityComparer<MessageFile>
        {
            public PathCompare() { }

            public bool Equals(MessageFile mf1, MessageFile mf2)
            {
                return (mf1.path == mf2.path);
            }

            public int GetHashCode(MessageFile mf)
            {
                string s = String.Format("{0}", mf.path);
                return s.GetHashCode();
            }
        }
    }
}
