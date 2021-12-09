using System;
using System.Diagnostics;
using System.Linq;

namespace steam_game_launcher {
  internal static class Launcher {
    private static void Main(string[] args) {
      try {
        if (args.Length != 2) {
          throw new ArgumentException(
            $"Expected 2 arguments, got {args.Length}.\n" +
            "Run like so: steam-game-launcher.exe <steamInstallPath> <steamGameId>\n" +
            "Where: \n" +
            "\t<steamInstallPath> is the Path to Steam installation Folder (this folder needs to contain the `userdata` folder\n" +
            "\t<steamGameId> is the ID steam gave to the game (also works with non-steam game as steam assign the shortcut an ID)\n" +
            "NOTE: All Paths can be written with forward slash instead of double-backslash for simplicity"
          );
        }

        string steamInstallPath = args[0].Replace('/', '\\');
        string steamGameId = args[1];

        KillProcesses("steam");
        StartGame(steamInstallPath, steamGameId);
        KillProcesses("genshin");
        KillProcesses("steam");
        StartGame(steamInstallPath, steamGameId);

      } catch (Exception err) {
        Console.Write(err);
        Console.Write("\nPress Enter to continue...");
        Console.ReadLine();
      }

    }

    private static void KillProcesses (string name) {
      var processes = Process.GetProcesses();
      var steamProcesses = processes.Where(process => process.ProcessName != "steam-game-launcher" && process.ProcessName.Contains(name));
      steamProcesses.ToList().ForEach(process => {
        Console.WriteLine($"Killing process {process.ProcessName}");
        process.Kill();
      });
    }

    private static void StartGame(string steamInstallPath, string steamGameId) {
      var steamStartInfo = new ProcessStartInfo {
        FileName = $"{steamInstallPath}/steam.exe",
        Verb = "runas",
        Arguments = "-silent"
      };
      Process.Start(steamStartInfo);

      var gameStartInfo = new ProcessStartInfo {
        FileName = $"steam://rungameid/{steamGameId}"
      };
      Process.Start(gameStartInfo);
    }
  }
}
