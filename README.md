# Steam Game Launcher

Small tool to launch Steam as Admin & launch a game afterward. Useful for games that requires Steam to run as admin (e.g.: controller support)

## Usage

1. Compile the project, or use the compiled version found from [the latest
   release](https://github.com/yonguelink/steam-game-launcher/releases/latest) or directly
   from
   [here](https://github.com/yonguelink/steam-game-launcher/releases/latest/download/steam-game-launcher.exe)
1. The executable takes two arguments:

    1. Steam folder complete path (e.g.: `C:/Program Files/Steam`)
    1. The Steam ID of the game (or even non-steam game added to steam) - To get the game ID you can do the following:
        1. Open Steam
        1. Find the game in your library
        1. Right click on it, navigate to `Manage` and click on `Add desktop shortcut`
        1. Go on your desktop, find the new shortcut, right click on it and hit `Properties`
        1. In the `Target` you should see the following: `steam:://rungameid/{bunch-of-numbers}`
        1. The bunch of numbers is the game ID, copy that
        1. Delete the shortcut

    NOTE: For simplicity, all paths can be defined with either forward slashes (`/`) OR
    double-backslashes (`\\`). The program deals with both format.

1. You can run the executable using CMD or create a shortcut

### How To Create the shortcut

1. Create a new shortcut pointing to the compiled EXE of this tool
    * Right click on the EXE and click `Create Shortcut`
1. Right click on the shortcut and click on `Properties`
1. Inside the `Properties`, in the `Target` box append the arguments explained above

    * If any of your path contains spaces you will need to quote them like so
    * An exemple

        `C:\steam-game-launcher.exe "C:/Program Files/Steam" 1234567890`

## Releasing

The release process happens with GitHub's `hub` CLI.

1. Build the project in `Release` mode

    ```sh
    dotnet build --configuration Release
    ```

1. Write up your release title in `RELEASE.md`
1. Add an empty line after your title
1. Add whatever message you want
1. Create the release in GitHub

    ```sh
    hub release create -oa steam-game-launcher/bin/Release/steam-game-launcher.exe -F RELEASE.md v1.0.0
    ```

1. A browser page will open, confirm the release happened correctly and you're done!
