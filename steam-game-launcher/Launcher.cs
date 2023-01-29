using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.IO.Compression;

namespace steam_game_launcher {
  internal static class Launcher {
    private static void Main(string[] args) {
      try {
        if (args.Length < 3) {
          throw new ArgumentException(
            $"Expected at least 3 arguments, got {args.Length}.\n" +
            "Run like so: steam-game-launcher.exe <steamInstallPath> <steamGameId> <gameName> <asAdmin> <extraProgramPath> <extraProgramName> <userId> <username>\n" +
            "Where: \n" +
            "\t<steamInstallPath> is the Path to Steam installation Folder (this folder needs to contain the `userdata` folder)\n" +
            "\t<steamGameId> is the ID steam gave to the game (also works with non-steam game as steam assign the shortcut an ID)\n" +
            "\t<gameName> is (part of) the game's name of the game\n" +
            "\t<asAdmin> is either \"true\" or \"false\" to determine if everything should be run as admin (this is optional) - default is true\n" +
            "\t<extraProgramPath> is the full path to a program you want to also start at the same time (this is optional) - default is none\n" +
            "\t<extraProgramName> is the name of the extra program you want ot start (this is optional, but required if the extra program path is set)\n" +
            "\t<userId> is the Steam User Id you want to make a backup of MHR save file of\n"+
            "\t<username> is the Steam User Name you want to make a backup of MHR save file of\n"+
            "NOTE: All Paths can be written with forward slash instead of double-backslash for simplicity"
          );
        }

        string steamInstallPath = args[0].Replace('/', '\\');
        string steamGameId = args[1];
        string gameName = args[2];
        string asAdmin = "true";
        string extraProgramPath = null;
        string extraProgramName = null;
        string steamUserId = null;
        string steamUserName = null;

        if (args.Length >= 4) {
          asAdmin = args[3];

          if (args.Length >= 6) {
            extraProgramPath = args[4];
            extraProgramName = args[5];

            if (args.Length >= 7) {
              steamUserId = args[6];
              steamUserName = args[7];
            }
          }
        }

        bool runAsAdmin = asAdmin == "true" ? true : false;

        if (gameName == "Monster Hunter Rise" && steamUserId != null && steamUserName != null) {
          Console.WriteLine("Will backup");
          BackupMhrSaveFiles(steamInstallPath, steamGameId, steamUserId, steamUserName, "save-backups");
        }

        KillProcesses(gameName.ToLower());
        KillProcesses("steam");
        StartGame(steamInstallPath, steamGameId, runAsAdmin);

        KillProcesses(gameName.ToLower());
        KillProcesses("steam");
        StartGame(steamInstallPath, steamGameId, runAsAdmin);

        if (extraProgramName == null) {
          return;
        }
        
        KillProcesses(extraProgramName.ToLower());
        StartProgram(extraProgramPath, runAsAdmin);

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

    private static void StartGame(string steamInstallPath, string steamGameId, bool asAdmin) {
      // Trick to enable starting another process than steam w/ steamGameId being the whole path to EXE
      if (steamInstallPath.ToLower() == "none") {
        StartProgram(steamGameId, asAdmin);
        return;
      }

      var steamStartInfo = new ProcessStartInfo {
        FileName = $"{steamInstallPath}/steam.exe",
        Arguments = "-silent"
      };

      if (asAdmin) {
        steamStartInfo.Verb = "runas";
      }

      Process.Start(steamStartInfo);

      var gameStartInfo = new ProcessStartInfo {
        FileName = $"steam://rungameid/{steamGameId}"
      };
      Process.Start(gameStartInfo);
    }

    private static void StartProgram(string programPath, bool asAdmin) {
      if (programPath == null) {
        return;
      }

      var programStartInfo = new ProcessStartInfo {
          FileName = programPath
      };
      if (asAdmin) {
        programStartInfo.Verb = "runas";
      }

      Process.Start(programStartInfo);
    }

    private static string BackupMhrSaveFiles(string steamInstallPath, string mhrSteamAppId, string steamUserId, string steamUsername, string backupFolderName) {
      string mhrSaveFolder = Path.Combine(
        steamInstallPath,
        "userdata",
        steamUserId,
        mhrSteamAppId,
        "remote"
      );

      string backupFolderPath = Path.Combine(
        steamInstallPath,
        "steamapps",
        "common",
        "MonsterHunterRise",
        backupFolderName
      );

      string backupFolderNowPath = Path.Combine(
        backupFolderPath,
        steamUsername,
        "Monster Hunter Rise"
      );

      return DirectoryCompress(mhrSaveFolder, backupFolderNowPath);
    }

    private static string DirectoryCompress (string sourceDirName, string destDirName) {
      DirectoryInfo dir = new DirectoryInfo(sourceDirName);

      if (!dir.Exists) {
        throw new DirectoryNotFoundException(
          $"Source directory does not exist or could not be found: {sourceDirName}"
        );
      }

      Directory.CreateDirectory(destDirName);

      string fileName = $"{Path.Combine(destDirName, DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"))}.zip";
      ZipFile.CreateFromDirectory(sourceDirName, fileName);
      return fileName;
    }
  }
}
