
using System.Diagnostics;
using System.Collections.Specialized;

class Program
{   [STAThread]
    static void Main(string[] args)
    {

        try
        {
            using (Process myProcess = new Process())
            {
                ProcessStartInfo procInfo = myProcess.StartInfo;
                procInfo.RedirectStandardError = true;
                procInfo.UseShellExecute = false;
                procInfo.FileName = @"C:\Users\ridn1\Visual Studio Code\file converter\ffmpeg.exe";
                procInfo.CreateNoWindow = true;

                string directory = Path.GetDirectoryName(args[0])!;
                string outputPath = Path.Combine(directory, "converted.mp4");
                
                string[] ffArgs = ["-i", args[0], "-c:v", "libx264", "-crf", "20", "-preset", "veryfast", "-c:a", "aac", outputPath, "-y"];

                foreach (string arg in ffArgs)
                {
                    procInfo.ArgumentList.Add(arg);
                }

                myProcess.Start();

                string output = myProcess.StandardError.ReadToEnd(); 

                myProcess.WaitForExit();

                if (myProcess.ExitCode != 0 || !File.Exists(outputPath))
                {
                    File.WriteAllText(
                        Path.Combine(directory, "conversion_error.txt"),
                        output
                    );

                    return;
                }

                StringCollection files = new StringCollection();
                files.Add(outputPath);

                Clipboard.SetFileDropList(files);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}