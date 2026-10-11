# GitHub Copilot Instructions for "Don't Tempt Me! (Continued)" Mod

## Mod Overview and Purpose

"Don't Tempt Me! (Continued)" is a mod for RimWorld that limits the player's ability to use developer tools within the game, promoting a more challenging and immersive experience. This mod is a continuation and update of KeenKrozzy's original modification.

## Key Features and Systems

- **Developer Mode Removal**: Eliminates the option to enable developer mode from the options menu, preventing players from using debug tools.
- **AI Change Option Removal**: Removes the option to change AI settings from within the options menu.
- **Selective Game Loading**: Ensures that games started with "Don't Tempt Me!" enabled can only be loaded when the mod is active, and vice versa.
- **Debug Log Access**: While developer mode is disabled, the debug log remains accessible through the mod options for troubleshooting purposes.

## Coding Patterns and Conventions

The mod adheres to the following coding patterns and conventions:

- **Namespace Consistency**: All classes belong to the `DontTemptMe` namespace, maintaining organizational consistency.
- **Prefix Harmony Patching**: Utilizes Harmony Prefix methods to patch RimWorld methods without altering the original game assemblies directly.
- **Single Responsibility Principle**: Each class and method is designed to fulfill a single role, simplifying maintenance and future updates.

## XML Integration

- The mod uses a single XML file, `About.xml`, which provides metadata such as the mod's name, author information, and a brief description. Ensure this file is kept updated to reflect any changes to the mod or its dependencies.

## Harmony Patching

Harmony is an integral part of this mod, used for altering game behavior. The following classes are involved in Harmony patching:

- **DTMPatching**: Manages the overall patching process and initializes individual patches.
- **GameDataSaveLoader_SaveGame, Listing_Standard_ButtonText, Listing_Standard_CheckboxLabeled, SavedGameLoaderNow_LoadGameFromSaveFileNow**: Each class contains a `Prefix` method that intercepts and modifies game functions to enforce the mod’s features.

### Example Harmony Patch

Here's a template for adding a new Harmony Prefix patch:

csharp
[HarmonyPatch(typeof(TargetClassName), "TargetMethodName")]
public static class PatchClassName
{
    [HarmonyPrefix]
    public static bool PrefixMethodName(ref Parameters)
    {
        // Modifying behavior
        return true; // Return false to skip the original method execution
    }
}


## Suggestions for Copilot

- **Code Completion**: When working on new patches, use Copilot to suggest boilerplate code for Harmony patches based on the provided patterns.
- **Error Handling**: Leverage Copilot to generate exception handling blocks surrounding potentially fail-prone logic within the patches.
- **Documentation**: Use Copilot to auto-generate XML documentation comments for methods, ensuring the code remains well-documented as it grows.
- **Pattern Recognition**: Copilot can help identify and suggest consistent coding patterns, especially useful when dealing with multiple similar patches.

For additional support and discussions regarding the mod, please refer to the author's [Reddit page](https://www.reddit.com) or the designated Discord channel for error-reporting and feature suggestions.

### Important

Before making any changes:
- Backup the existing source files.
- Test your changes thoroughly to avoid introducing game-breaking bugs, as developer mode tools will not be available to aid in debugging.

By following these instructions and using Copilot effectively, you can maintain and expand the "Don't Tempt Me! (Continued)" mod with ease. Happy modding!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

