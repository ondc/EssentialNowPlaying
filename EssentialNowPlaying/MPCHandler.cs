﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Essential_Now_Playing
{
    class MPCHandler : SourceHandler
    {
        private Process[] processlist;
        private string path;
        private bool noSong;
        private bool isVLCUp;
        private bool bStop;
        private TextBox preview;
        private string oldName = null;

        public MPCHandler(string p, TextBox preview)
        {
            path = p;
            bStop = false;
            this.preview = preview;
        }

        private Process findMPC()
        {
            Process spotify = null;
            processlist = Process.GetProcessesByName("MPC-HC");

            if (processlist.Length == 0)
            {
                isVLCUp = false;
                Debug.WriteLine("\n\n\n\nDEBUG\n\n\n");
                return null;
            }
            else
            {
                foreach (Process process in processlist)
                {
                    if (process.ProcessName == "MPC-HC")
                    {
                        if (process.MainWindowTitle != "")
                        {
                            spotify = process;
                            //Debug.WriteLine("{0} + {1}", "DEBUG", spotify.MainWindowTitle);
                            noSong = false;
                            isVLCUp = true;
                            if (process.MainWindowTitle == "MPC-HC")
                            {
                                noSong = true;
                            }
                        }
                    }
                    else
                    {
                        isVLCUp = false;
                    }
                }
            }

            return spotify;
        }

        async public override Task pollForSongChanges()
        {
            while (!bStop)
            {
                // get the Spotify process (if it exists)
                

                try
                {
                    Process s = findMPC();

                    string songName = s.MainWindowTitle + " ";
                    //Debug.WriteLine("{0} + {1}", "DEBUG", s.MainWindowTitle);
                    if (!isVLCUp)
                    {
                        writeToPath(path, "MPC-HC not open");
                        preview.Text = "MPC-HC not open";
                    }
                    else if (noSong)
                    {
                        writeToPath(path, "Paused");
                        preview.Text = "Paused";
                        oldName = null;
                    }
                    else
                    {
                        // only update the song if the song changes
                        // strip some extra information from the string, like the theme and the program name
                        if (oldName != null)
                        {
                            if (oldName != songName)
                            {
                                preview.Text = songName;
                                writeToPath(path, songName);
                                oldName = songName;
                            }
                        }
                        else
                        {
                            // first run
                            preview.Text = songName;
                            writeToPath(path, songName);
                            oldName = songName;
                        }
                    }


                    

                }
                catch (NullReferenceException)
                {
                    writeToPath(path, "MPC-HC not open");
                    preview.Text = "MPC-HC not open";
                    
                }

                await Task.Delay(500);
            }
        }

        public override void stop()
        {
            bStop = true;
        }
    }
}
