using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using WMPLib;
namespace Client
{
    static class Soundmanger
    {

      public static WindowsMediaPlayer BGPlayer;
      public  static WindowsMediaPlayer Player;
      static string workingpath;


        static Soundmanger()
        {
            workingpath = Path.Combine(Directory.GetCurrentDirectory(), "SoundEffects");

            BGPlayer = new WMPLib.WindowsMediaPlayer();
            BGPlayer.URL = Path.Combine(workingpath, "Panda-Background.mp3");
            BGPlayer.settings.setMode("loop", true);
            BGPlayer.settings.volume = 10;
        }


    }
}
