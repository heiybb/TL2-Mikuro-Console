﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TL2_Mikuro_Console
{
    internal class Program
    {

        static HashSet<string> layoutInvalidMppTrigger = new HashSet<string>
        {
            "PROPERTY NODE",
            "GROUP",
            "ROOM PIECE",
            "LIGHT",
            "LAYOUT LINK",
            "LAYOUT LINK PARTICLE",
            "UNIT SPAWNER",
            "LOGIC GROUP",
            "MONSTER",
            "UNIT TRIGGER",
            "QUEST CONTROLLER",
            "TIMER",
            "QUEST FLAG CONTROLLER",
            "LAYOUT LINK TIMELINE",
            "OUTPUT INCREMENTOR",
            "COUNTER",
            "SOUND",
            "GENERIC MODEL",
            "PLAYER SPHERE TRIGGER",
            "RANDOM CHOICE",
            "TIMELINE",
            "SKILL CONTROLLER",
            "MONSTER SPHERE TRIGGER",
            "LOGIC GATE",
            "ANIMATION CONTROLLER",
            "WARPER",
            "ITEM REFERENCE",
            "UNIT COUNT SPHERE",
            "DIALOG FOR EVENT",
            "PLAYER BOX TRIGGER",
            "TELEPORT",
            "SHOW TEXT",
            "PATHING",
            "MOVIEPLAYER",
            "MENU CONTROLLER",
            "GAME STATE CONTROLLER",
            "MESSAGE BOX",
            "REGIONAL AREA",
            "MUSIC",
            "STAT EVALUATOR",
            "AFFIX APPLICATOR",
            "CAMERA SHAKE",
            "MONEY TAKER",
            "INVENTORY CHECK",
            "LAYOUT LINK CONTROLLER",
            "DAMAGE SHAPE",
            "LIGHT SCHEME",
            "WAYPOINT ACTIVATOR",
            "RANDOM INPUT CHOICE",
            "STATS EVALUATOR"
        };



        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            InitDLL();

            string modsPath = AppDomain.CurrentDomain.BaseDirectory + "mods";
            string userInput = string.Empty;
            Stopwatch sw = new Stopwatch();

            while (userInput != ":q")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                List<string> modList = GetImmediateSubfolderNames(modsPath);
                for (int i = 0; i < modList.Count; i++)
                {
                    Console.WriteLine(i + ": " + modList[i]);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("📝Type ':q' to quit or select MOD number you would like to build->");
                Console.ResetColor();

                userInput = Console.ReadLine();
                if (userInput != ":q")
                {
                    int inputNum;
                    try
                    {
                        inputNum = int.Parse(userInput);
                        if (inputNum >= 0 && inputNum < modList.Count)
                        {
                            string modPath = modsPath + @"\" + modList[inputNum] + @"\MOD.DAT";
                            EditorDLL.EditorSetWorkingMod(modsPath + @"\" + modList[inputNum]);
                            sw.Start();
                            bool buildResult = EditorDLL.CreateMod(modPath, true);
                            sw.Stop();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"🚀Build: {modPath}");
                            Console.ResetColor();
                            if (buildResult)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"🎉Sucess, time cost(ms): {sw.ElapsedMilliseconds}");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("💥Fail, check log for what happen");
                            }
                            Console.ResetColor();

                            //Console.ForegroundColor = ConsoleColor.Green;
                            //Console.WriteLine("🚨Validating MPP files(size 2.5kb)...");
                            //List<string> invalidMppFileList = FindAllInvalidMPP(modsPath + @"\" + modList[inputNum]);
                            //if (invalidMppFileList.Count > 0)
                            //{
                            //    Console.ForegroundColor = ConsoleColor.DarkRed;
                            //    invalidMppFileList.ForEach(invalidMPP => Console.WriteLine($"📌Found invalid MPP file: {invalidMPP.Replace(modsPath, "")}"));
                            //    Console.WriteLine("🔖You may need to re-build again to generate appropriate MPP files");
                            //}
                            //else
                            //{
                            //    Console.WriteLine("✅MPP files check pass");
                            //}
                            //Console.ResetColor();
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("🚨Unsupported input");
                    }
                }
            }
            Console.WriteLine("💡Goodbye!");
        }

        static void InitDLL()
        {
            int initFlag = 0;
            int maxTry = 3;
            for (int i = 0; i < maxTry; i++)
            {
                if (initFlag == 0)
                {
                    try
                    {
                        //int hlnst = Marshal.GetHINSTANCE(typeof(Program).Module).ToInt32();
                        int hlnst = -1;
                        var hWnd = Process.GetCurrentProcess().Handle;

                        Console.WriteLine($"Instance Handle: {hlnst}");
                        Console.WriteLine($"Process Handle: {hWnd}");

                        initFlag = EditorDLL.InitEditor(hlnst, hWnd.ToInt32());
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("GUTS Editor's DLL init failed.");
                        Console.WriteLine("Try again 300ms ater");
                        Console.ResetColor();
                        EditorDLL.ShutdownEditor();
                        Console.Error.WriteLine(ex.Message);
                        Thread.Sleep(300);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("GUTS Editor's DLL init finished.");
                    Console.ResetColor();
                    break;
                }
            }
        }

        static List<string> GetImmediateSubfolderNames(string path)
        {
            string[] subdirectories = Directory.GetDirectories(path);
            List<string> folderNames = new List<string>();

            foreach (string subdirectory in subdirectories)
            {
                string folderName = Path.GetFileName(subdirectory);
                folderNames.Add(folderName);
            }

            return folderNames;
        }



        static string ReadAndConvertBytes(string filePath, int numBytes)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[numBytes];
                int bytesRead = fs.Read(buffer, 0, buffer.Length);

                StringBuilder hexString = new StringBuilder();

                for (int i = 0; i < bytesRead; i++)
                {
                    hexString.Append(buffer[i].ToString("X2"));
                }

                return hexString.ToString();
            }
        }

        static List<string> FindAllInvalidMPP(string modDir)
        {
            string invalidMppFileTrait = "320000003200000000002041000020410000A0410000A041";

            HashSet<string> invalidMppSet = new HashSet<string>();
            try
            {
                string[] files = Directory.GetFiles(modDir, "*.MPP", SearchOption.AllDirectories);

                foreach (string mppFile in files)
                {
                    if (ReadAndConvertBytes(mppFile, 24) == invalidMppFileTrait)
                    {
                        //mpp file always use lowercase ext
                        string layoutFilePath = mppFile.Replace(".mpp", ".LAYOUT");
                        Console.WriteLine($"🚨MPP -> LAYOUT: {layoutFilePath.Replace(modDir, "")}");

                        foreach (string line in File.ReadAllLines(layoutFilePath))
                        {
                            if (line.ToUpper().Contains("<STRING>DESCRIPTOR:".ToUpper()))
                            {
                                string descriptor = line.Trim().Replace("<STRING>DESCRIPTOR:", "");
                                if (layoutInvalidMppTrigger.Contains(descriptor.ToUpper()))
                                {
                                    invalidMppSet.Add(mppFile);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Access denied to some directories.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return invalidMppSet.ToList();
        }
    }
}
