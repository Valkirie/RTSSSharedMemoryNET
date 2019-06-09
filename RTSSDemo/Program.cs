using System;
using System.Linq;
using RTSSSharedMemoryNET;

namespace RTSSDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //enforces a nice cleanup
            //just hitting X or Ctrl+C normally won't actually dispose the using() below
            ExitHandler.Init(ctrlType => {
                Console.WriteLine("\nCleaning up and exiting...");
                return true; //cancel event
            });

            ///////////////////////////////////////////////////////////////////

            Console.WriteLine("Current OSD entries:");
            var osdEntries = OSD.GetOSDEntries();
            foreach( var osd in osdEntries )
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(osd.Owner);
                Console.ResetColor();
                Console.WriteLine("{0}\n", osd.Text);
            }

            ///////////////////////////////////////////////////////////////////

            Console.WriteLine("Current app entries with GPU contexts:");
            var appEntries = OSD.GetAppEntries().Where( x => (x.Flags & AppFlags.MASK) != AppFlags.None ).ToArray();
            foreach( var app in appEntries )
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("{0}:{1}", app.ProcessId, app.Name);
                Console.ResetColor();

                Console.WriteLine("{0}, {1}FPS, {2}", app.Flags, app.InstantaneousFrames, app.InstantaneousFrameTime);
            }
            Console.WriteLine();

            ///////////////////////////////////////////////////////////////////

            using( var osd = new OSD("RTSSDemo") )
            while( true )
            {
                    //Console.WriteLine("Enter some text:");
                    //var text = Console.ReadLine();

                    //if we hit Ctrl+C while waiting for ReadLine, it returns null
                    /*if( text == null )
                        break;

                    osd.Update();*/

                    System.Threading.Thread.Sleep(1000);
                    Console.WriteLine("Current app entries with GPU contexts:");
                    appEntries = OSD.GetAppEntries().Where(x => (x.Flags & AppFlags.MASK) != AppFlags.None).ToArray();
                    foreach (var app in appEntries)
                    {
                        if (app.InstantaneousTimeEnd != app.InstantaneousTimeStart)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("{0}:{1}", app.ProcessId, app.Name);
                            Console.ResetColor();
                            uint fps = (app.InstantaneousFrames * 1000) / (app.InstantaneousTimeEnd - app.InstantaneousTimeStart);
                            Console.WriteLine("{0}, {1}FPS", app.Flags, fps);
                        }
                    }
                    Console.WriteLine();

                }
        }
    }
}
