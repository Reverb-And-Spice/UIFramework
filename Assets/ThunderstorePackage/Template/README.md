#### New in this version (expand each for details)
<details>  <summary> New feature: Enum Display Name attribute support </summary>

You can now use the `[Display(Name = "DisplayName")]` attribute on your enum values. [More details further down](#enum-display-names)

</details>
<details><summary> Bug fix: Error in the loader scene </summary>

Trying to toggle the UI in the loader scene won't cause an error anymore. But you need to have loaded into the gym first before the window shows up.
</details>
<details><summary> Bug fix: The save button now saves all categories in a mod</summary>

I think it makes more sense for the save button to save the entire mod's preferences instead of just that specific tab. 

</details>

-----
# For Users
Drop the dll in your mods folder. 
#### **Default toggle is the `F9` key**

Changing a value of an entry automatically updates the value of the preference and is applied. How that preference's parent mod reacts depends on the modder's implementation.

The save button writes it to the file for permanent storage. Closing your game might also save preferences to file automatically depending on whether it's closed from the game window or through Steam. **Stopping through Steam doesn't save because it force closes it.**

------

# For Modders 
Add `[assembly: MelonAdditionalDependencies("UIFramework")]` to your AssemblyInfo. This prevents your mod from calling on UIFramework before it's been initialized.

[Declare](#If-you-havent-used-melonpreferences-before) your MelonPreferences in `OnInitializeMelon` and then register them to the UI.
```cs
UI.Register(this, TestCategory1, TestCategory2...);
```
Right now, support is limited to common types like `string`, `int`, `bool`, `double`, `float`, and `enums` without the flags attribute. Working on expanding this. 

-----



## If you haven't used melonpreferences before
### Here's instructions for basic usage as well as a link to the official documentation for MelonPreferences from the MelonLoader wiki


#### 1. Set a file location. Make sure the directory exists for your mod. Otherwise, it will not cause an error but your preferences don't save at all.
```cs
private const string USER_DATA = "UserData/TestMod/";
private const string CONFIG_FILE = "config.cfg";
if (!Directory.Exists(USER_DATA))
    Directory.CreateDirectory(USER_DATA);
```
#### 2.  Declare, create, and set a file path for your categories
```cs
private MelonPreferences_Category TestCategory1;
TestCategory1 = MelonPreferences.CreateCategory("Test Cat 1");
TestCategory1.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

private MelonPreferences_Category TestCategory2;
TestCategory2 = MelonPreferences.CreateCategory("Test Cat 2");
TestCategory2.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
```
#### 3. Declare your entries.
```cs
private MelonPreferences_Entry<string> TestEntry11;
private MelonPreferences_Entry<int> TestEntry12;

private MelonPreferences_Entry<float> TestEntry21;
private MelonPreferences_Entry<bool> TestEntry22;
```
#### 4. Create Entries by calling the .CreateEntry method on the category they go in. Parameters are `Identifier`, `Default Value`, `Display Name`, and `Description`
```cs
TestEntry11 = TestCategory1.CreateEntry("Entry 1-1", "Test Val", "Display Name1", "Test String");
TestEntry12 = TestCategory1.CreateEntry("Entry 1-2", 1, "Display Name2", "Test Int");

TestEntry21 = TestCategory2.CreateEntry("Entry 2-1", "0.5126", "Display Name 3", "Test float");
TestEntry22 = TestCategory2.CreateEntry("Entry 2-2", true, "Display Name 4", "Test bool");
```
https://melonwiki.xyz/#/modders/preferences?id=melon-preferences

-----

### Enum Display Names
Enum dropdowns will now show the Display(Name) attribute. If unavailable, it will fall back to the default enum value name. 
```cs
using System.ComponentModel.DataAnnotations;
public enum Example
{
    [Display(Name = "DisplayName")]
    value1,
    [Display(Name = "Other Value")]
    value2
}
```
-----



# Ongoing Development Disclosure
This mod is in active development. The plan is to increase extensibility. **<ins>Basic MelonPreferences registration is stable and should always be backwards compatible.</ins>** So while advanced API usage will have a lot of changes for the time that this mod is in [Version 0.x.x](https://semver.org/#spec-item-4), mods that implement the basic use case of this framework don't have to worry about breaking in the future (as long as I don't mess up too bad). 

You can place the .xml documentation file for UIFramework in the same folder as the dll to get intellisense documentation for the API. It is currently incomplete, however but I do add to it every update.