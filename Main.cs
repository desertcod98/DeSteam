using DeSteam.Unpackers.Variant0x64;
using DeSteam.PeHandlers;
using CommandLine;
using CommandLine.Text;

namespace DeSteam
{
    class Start
    {
        public class Options {
            [Option('v', "verbose", Default = false, HelpText = "Shows additional info when executing.")]
            public bool verbose { get; set; }

            [Option('o', "output", Required = false, HelpText = "Specify unpacked file name.")]
            public string? outputFileName { get; set; }

            [Value(0, Required = true, HelpText = "Path to the file to unpack", MetaName = "Path to file")]
            public string? filePath { get; set; }
        }
        static void Main(string[] args) {
            string? filePath = null;
            string? outputFileName = null;
            var parser = new Parser(x => {
                x.HelpWriter = null;
            });
            var parserResult = parser.ParseArguments<Options>(args);
            string helpText =
@"DeSteam version 1.0.0"+ "\n"+ @"
Usage: ./DeSteam.exe [path_to_file_to_unpack]"+ "\n"+@"
Optional arguments:
-o, --output:           Specify unpacked file name. (Default: [original_name].unpacked.exe)
-v, --verbose:          Shows additional info when executing. (Default: false)
";
            parserResult.WithNotParsed(errs => {
                Console.WriteLine(helpText);
                Logger.Error("Missing required file path!");
            });
            parserResult.WithParsed(x => {
                filePath = x.filePath;
                outputFileName = x.outputFileName;
                Logger.ShowVerbose = x.verbose;
            });
            if(filePath == null || filePath.Trim() == "") {
                return;
            }
            
            Pe64 pe = new Pe64(filePath);

            if(outputFileName == null || outputFileName.Trim() == "") {
                outputFileName = Path.GetFileNameWithoutExtension(filePath) + ".unpacked.exe";
            }
            Variant0x64 v = new Variant0x64(pe, outputFileName);
            if (v.Unpack()) {
                Logger.Info("Unpacked file saved as " + outputFileName);
            }
            else {
                Logger.Error("Failed to unpack file...");
            }
            pe.Dispose();
        }
    }
}