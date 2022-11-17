using System;
using System.Diagnostics;
using System.Linq;

namespace steam_game_launcher {
  internal static class Launcher {
    private static void Main(string[] args) {
      try {
        if (args.Length != 3) {
          throw new ArgumentException(
            $"Expected 3 arguments, got {args.Length}.\n" +
            "Run like so: steam-game-launcher.exe <steamInstallPath> <steamGameId>\n" +
            "Where: \n" +
            "\t<steamInstallPath> is the Path to Steam installation Folder (this folder needs to contain the `userdata` folder)\n" +
            "\t<steamGameId> is the ID steam gave to the game (also works with non-steam game as steam assign the shortcut an ID)\n" +
            "\t<gameName> is (part of) the game's name of the game\n" +
            "NOTE: All Paths can be written with forward slash instead of double-backslash for simplicity"
          );
        }

        string steamInstallPath = args[0].Replace('/', '\\');
        string steamGameId = args[1];
        string gameName = args[2];

        KillProcesses(gameName.ToLower());
        KillProcesses("steam");
        StartGame(steamInstallPath, steamGameId);
        KillProcesses(gameName.ToLower());
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
      var processesToKill = processes.Where(process => process.ProcessName != "steam-game-launcher" && process.ProcessName.ToLower().Contains(name)).ToList();
      processesToKill.ToList().ForEach(process => {
        Console.WriteLine($"Killing process {process.ProcessName}");
        process.Kill();
      });
    }

    private static void StartGame(string steamInstallPath, string steamGameId) {
      // Trick to enable starting another process than steam w/ steamGameId being the whole path to EXE
      if (steamInstallPath.ToLower() == "none") {
        var nonSteamGameStartInfo = new ProcessStartInfo {
          FileName = steamGameId
        };
        Process.Start(nonSteamGameStartInfo);
        return;
      }

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
