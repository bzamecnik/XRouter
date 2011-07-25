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
    /// Compares each file in the correct results with the actual results made by XRouter
    /// </summary>
    public class XmlDirectoryComparer
    {
        private string correctRoot;
        private string resultRoot;

        private IEnumerable<MessageFile> correctMessageFiles;
        private IEnumerable<MessageFile> resultMessageFiles;

        public XmlDirectoryComparer(string correctRoot, string resultRoot)
        {
            this.correctRoot = Path.GetFullPath(correctRoot);
            this.resultRoot = Path.GetFullPath(resultRoot);
        }

        public void Compare()
        {
            correctMessageFiles = GetMessageFilesFromRoot(correctRoot);
            resultMessageFiles = GetMessageFilesFromRoot(resultRoot);

            PathAndHashCompare pathAndHashCompare = new PathAndHashCompare();
            PathCompare pathCompare = new PathCompare();

            bool areIdentical = correctMessageFiles.SequenceEqual(resultMessageFiles, pathAndHashCompare);
            if (areIdentical)
            {
                Console.WriteLine("Results are correct!");
                return;
            }
            else
            {
                Console.WriteLine("Results are incorrect!");
                Console.WriteLine();
            }

            //missing file messages
            var missingMessageFiles = (from messageFile in correctMessageFiles
                                       select messageFile).Except(resultMessageFiles, pathCompare);
            if (missingMessageFiles.Any())
            {
                Console.WriteLine("Missing message files:");
                foreach (var v in missingMessageFiles)
                {
                    Console.WriteLine(v.path);
                }
                Console.WriteLine();
            }

            //redundant file messages
            var redundantMessageFiles = (from messageFile in resultMessageFiles
                                         select messageFile).Except(correctMessageFiles, pathCompare);
            if (redundantMessageFiles.Any())
            {
                Console.WriteLine("Redundant message files:");
                foreach (var v in redundantMessageFiles)
                {
                    Console.WriteLine(v.path);
                }
                Console.WriteLine();
            }

            //wrong file messages
            var wrongMessageFiles = (from messageFile in correctMessageFiles
                                     select messageFile)
                                     .Except(resultMessageFiles, pathAndHashCompare)
                                     .Except(missingMessageFiles, pathCompare)
                                     .Except(redundantMessageFiles, pathCompare);
            if (wrongMessageFiles.Any())
            {
                Console.WriteLine("Wrong message files:");
                foreach (var v in wrongMessageFiles)
                {
                    Console.WriteLine(v.path);
                }
                Console.WriteLine();
            }
        }

        private IEnumerable<MessageFile> GetMessageFilesFromRoot(string root)
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
