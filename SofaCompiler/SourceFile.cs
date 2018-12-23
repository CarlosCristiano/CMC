using System;
using System.IO;

namespace SofaCompiler
{
    public class SourceFile
    {
        public static char EOL = '\n';
        public const char EOT = (char)0;
        private BinaryReader source;

        public SourceFile(string sourceFileName)
        {
            try
            {
                source = new BinaryReader(new FileStream(sourceFileName, FileMode.Open));
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("*** FILE NOT FOUND *** (" + sourceFileName + ")");
                Environment.Exit(0);
            }
        }

        public char GetSource()
        {
            try
            {
                int c = source.Read();
                if (c < 0)
                    return EOT;
                else
                    return (char)c;
            }
            catch (IOException ex)
            {
                return EOT;
            }
        }
    }
}
