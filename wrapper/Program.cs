using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace wrapper
{
    enum CmdType
    {
        cmd1 = 1,
        cmd2 = 2,
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string type = args[0];
            Output($"type:{type}");
            CmdType cmdType = (CmdType)int.Parse(type);
            string log = ("".DateTimeNow().ToString("yyyyMMdd_HHmmss") + "_" + cmdType.ToString() + ".txt").GetPathInBin();
            fs = new FileStream(log, FileMode.CreateNew);
            try
            {
                string localDir = DownloadFile();
                switch (cmdType)
                {
                    case CmdType.cmd1:
                        {
                            CopyToFile(localDir);
                            string path = localDir + FromB64("XHRlc3RfbW9uaXRvclxiaW5cRGVidWdcbmV0OC4wXHRlc3RfbW9uaXRvci5leGU=");
                            RunSubApp(path);
                            CopyFromFile1(localDir);
                        }
                        break;
                    case CmdType.cmd2:
                        {
                            CopyToFile(localDir);
                            string path = localDir + FromB64("XHRlc3RfY3JhY2tcYmluXERlYnVnXHRlc3RfY3JhY2suZXhl");
                            RunSubApp(path);
                            CopyFromFile2(localDir);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Output(ex.Message);
            }
            fs.Dispose();
            fs = null;
            UploadFileByBaidu(log);
        }

        static void AesEncryptFile(string file, byte[] pwd, int keySize = 128)
        {
            Output($"【aes enc】file:{file}");
            byte[] input = File.ReadAllBytes(file);
            byte[] output = AesUtil.AesEncrypt(input, pwd, keySize);
            File.WriteAllBytes(file, output);
        }

        static void AesDecryptFile(string file, byte[] pwd, int keySize = 128)
        {
            Output($"【aes dec】file:{file}");
            byte[] input = File.ReadAllBytes(file);
            byte[] output = AesUtil.AesDecrypt(input, pwd, keySize);
            File.WriteAllBytes(file, output);
        }

        static void UploadFileByBaidu(string file)
        {
            Output("【tobaidu】file:" + file);
            string remoteFileName = Path.GetFileName(file);
            string remotePath = "/logs/" + remoteFileName;
            string baidu_upload_py = "\\apps\\pyscripts\\baidu_upload.py".GetPathInPrj();
            try
            {
                string err = "";
                string allInfo = "";
                Dictionary<string, string> envVars = new Dictionary<string, string>();
                envVars["HOME"] = "".GetPathInBin();
                int exitCode = RunProcessCore("python.exe", $"\"{baidu_upload_py}\" \"{file}\" \"{remotePath}\"", true, envVars, out err, out allInfo);
                Output("exit code:" + exitCode);
                if (exitCode != 0)
                {
                    throw new Exception("to baidu error," + err);
                }
                Output("to baidu ok");
            }
            catch (Exception ex)
            {
                Output(ex.Message);
            }
        }

        static FileStream fs = null;
        static void Output(string str)
        {
            if (GetShowLog())
            {
                Console.WriteLine($"{"".DateTimeNow().ToString("yyyy-MM-dd HH:mm:ss")} {str}\r\n");
            }
            if (fs == null)
                return;
            byte[] bytes = Encoding.UTF8.GetBytes($"{"".DateTimeNow().ToString("yyyy-MM-dd HH:mm:ss")} {str}\r\n");
            fs.Write(bytes, 0, bytes.Length);
        }

        static void FileCopy(string sourceFileName, string destFileName, bool overwrite)
        {
            Output($"【copy】{sourceFileName}->{destFileName}");
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        static void CopyFromFile1(string localDir)
        {
            string path11 = "1.txt".GetPathInPrj();
            string path21 = localDir + FromB64("XHRlc3RfbW9uaXRvclxjdXJfdmVyLnR4dA==");
            string pwd = GetXPWD();
            AesEncryptFile(path21, pwd.ToHexArray());
            FileCopy(path21, path11, true);
        }

        static void CopyFromFile2(string localDir)
        {
            string path12 = "2.txt".GetPathInPrj();
            string path22 = localDir + FromB64("XHRlc3RfY3JhY2tcY3VyX3Zlci50eHQ=");
            string pwd = GetXPWD();
            AesEncryptFile(path22, pwd.ToHexArray());
            FileCopy(path22, path12, true);
        }

        static void CopyToFile(string localDir)
        {
            string path11 = "1.txt".GetPathInPrj();
            string path12 = "2.txt".GetPathInPrj();
            string path21 = localDir + FromB64("XHRlc3RfbW9uaXRvclxjdXJfdmVyLnR4dA==");
            string path22 = localDir + FromB64("XHRlc3RfY3JhY2tcY3VyX3Zlci50eHQ=");
            string pwd = GetXPWD();
            if (File.Exists(path11))
            {
                FileCopy(path11, path21, true);
                AesDecryptFile(path21, pwd.ToHexArray());
            }
            if (File.Exists(path12))
            {
                FileCopy(path12, path22, true);
                AesDecryptFile(path22, pwd.ToHexArray());
            }
        }

        static bool GetShowLog()
        {
            string value = Environment.GetEnvironmentVariable("X_SHOW_LOG");
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            return value == "1";
        }

        static string GetXPWD()
        {
            string value = Environment.GetEnvironmentVariable("X_PWD");
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("X_PWD is empty");
            }
            return value;
        }

        static string GetXURL()
        {
            string value = Environment.GetEnvironmentVariable("X_URL");
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException("X_URL is empty");
            }
            return value;
        }

        static string FromB64(string str)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        static string DownloadFile()
        {
            SSLOrTLSUtil.FixSSLTLSError();
            string url = GetXURL();
            string pwd = GetXPWD();
            string localFile = $"{GetRandomName()}.7z".GetPathInBin();
            string localDir = GetRandomDir();
            Directory.CreateDirectory(localDir);
            SimpleHttp http = new SimpleHttp();
            http.DownloadFile(url, localFile);
            Unzip(localFile, localDir, pwd);
            return localDir;
        }
        
        static string GetRandomDir()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\").FullName;
            var tempBaseDir = dir.TrimEnd('\\') + Path.DirectorySeparatorChar + GetRandomName().ToString();
            return tempBaseDir;
        }

        static string GetRandomName()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0).ToString().Substring(1);
        }

        static void RunSubApp(string file, params string[] args)
        {
            Output($"【run sub app】{file}");
            string err = "";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["X_BASE_DIR"] = GetRandomDir();
            int exitCode = RunProcess(file, string.Join(" ", args), true, dic, out err);
            Output("exit code:" + exitCode);
            if (exitCode != 0)
            {
                throw new Exception("run sub app," + err);
            }
            Output("run sub app成功");
        }

        static void Unzip(string file, string dir, string pwd)
        {
            Output($"【解压】{file}到{dir}");
            string err = "";
            int exitCode = RunProcess("\\Dlls\\7-Zip\\7z.exe".GetPathInPrj(), $"x -p{pwd} {file} -o{dir}", true, null, out err);
            Output("exit code:" + exitCode);
            if (exitCode != 0)
            {
                throw new Exception("解压失败," + err);
            }
            Output("解压成功");
        }

        static int RunProcess(string file, string args, bool hasOutput, Dictionary<string, string> envVars, out string errInfo)
        {
            var allInfo = "";
            return RunProcess(file, args, hasOutput, envVars, out errInfo, out allInfo);
        }

        static int RunProcess(string file, string args, bool hasOutput, Dictionary<string, string> envVars, out string errInfo, out string allInfo)
        {
            return RunProcessCore(file, args, hasOutput, envVars, out errInfo, out allInfo);
        }

        static int RunProcessCore(string file, string args, bool hasOutput, Dictionary<string, string> envVars, out string errInfo, out string allInfo)
        {
            Output($"file:{file} args:{args}");
            errInfo = "";
            allInfo = "";
            var errInfo_tmp = "";
            var allInfo_tmp = "";
            Process p = new Process();
            p.StartInfo.FileName = file;
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(file);
            p.StartInfo.Arguments = args;
            if (hasOutput)
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Output(e.Data);
                        allInfo_tmp += e.Data + "\r\n";
                    }
                };
                p.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Output($"错误: {e.Data}");
                        errInfo_tmp += e.Data + "\r\n";
                        allInfo_tmp += e.Data + "\r\n";
                    }
                };
            }
            if (envVars != null)
            {
                foreach (var item in envVars)
                {
                    p.StartInfo.EnvironmentVariables.Add(item.Key, item.Value);
                }
            }
            p.Start();
            if (hasOutput)
            {
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
            }
            p.WaitForExit();
            errInfo = errInfo_tmp;
            allInfo = allInfo_tmp;
            return p.ExitCode;
        }
    }
}
