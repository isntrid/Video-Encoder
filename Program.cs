
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

                string inputPath = Path.GetDirectoryName(args[0])!;
                string folderPath = Path.Combine(inputPath, "converted files");
                Directory.CreateDirectory(folderPath);
                string fileName = Guid.NewGuid().ToString().Substring(0, 8);
                string filePath = Path.Combine(folderPath, fileName + ".mp4");


                string[] ffArgs = [
                    "-i", args[0],
                    "-c:v", "h264_nvenc",
                    "-preset", "p1",
                    "-rc", "vbr",
                    "-b:v", "12M",
                    "-maxrate", "16M",
                    "-bufsize", "32M",
                    "-c:a", "aac",
                    "-b:a", "128k",
                    "-y",
                    filePath
                                ];

                foreach (string arg in ffArgs)
                {
                    procInfo.ArgumentList.Add(arg);
                }

                myProcess.Start();

                string output = myProcess.StandardError.ReadToEnd(); 

                myProcess.WaitForExit();

                if (myProcess.ExitCode != 0 || !File.Exists(filePath))
                {
                    File.WriteAllText(
                        Path.Combine(folderPath, "conversion_error.txt"),
                        output
                    );

                    return;
                }

                StringCollection files = [filePath];
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