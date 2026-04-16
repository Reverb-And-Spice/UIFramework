### New in 0.6.2
<details><summary>New Feature: Plugin support</summary>
I just completely forgot about those.

<sup>I did have to change the .Register() function's signature. Right now, I have an overload for the old function for backwards compatibility. More details at the bottom of the page </sup>
</details>

<details><summary>New Feature: Exposed UI.IsVisible Property</summary>
Your mod can now check if UIFramework is currently open. 
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

[Define](#If-you-havent-used-melonpreferences-before) your MelonPreferences in `OnInitializeMelon` and then register them to the UI.
```cs
UI.Register(this, TestCategory1, TestCategory2...);
```
Right now, support is limited to common types like `string`, `int`, `bool`, `double`, `float`, and `enums` without the flags attribute. Working on expanding this. 


### Optional: OnSave Event Handler
You can add an event handler that gets called when the save button is clicked while your mod is selected.
```cs
private void MyModSaved()
{
    // Do something when the save button is clicked while your mod is selected
}
```
```cs
UI.Register(this, OBSAutoRecorderSettings, TestCategory1, TestCategory2...).OnModSaved += MyModSaved;
```


-----



## If you haven't used melonpreferences before
### Here's instructions for basic usage as well as a link to the official documentation for MelonPreferences from the MelonLoader wiki


#### 1. Define a file location. 
Make sure the directory exists for your mod. Otherwise, it will not fail silently and your preferences don't save at all.
```cs
private const string USER_DATA = "UserData/MyMod/";
private const string CONFIG_FILE = "config.cfg";
if (!Directory.Exists(USER_DATA))
    Directory.CreateDirectory(USER_DATA);
```
#### 2. Declare MelonPreferences_Category and MelonPreferences_Entry variables
```cs
private MelonPreferences_Category TestCategory1;
private MelonPreferences_Category TestCategory2;
```
```cs
private MelonPreferences_Entry<string> TestEntry11;
private MelonPreferences_Entry<int> TestEntry12;

private MelonPreferences_Entry<float> TestEntry21;
private MelonPreferences_Entry<bool> TestEntry22;
```

#### 3.  Call the CreateCategory method and set file paths
```cs
TestCategory1 = MelonPreferences.CreateCategory("TestCat1", "Category DisplayName 1");
TestCategory1.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

TestCategory2 = MelonPreferences.CreateCategory("TestCat2", "Category DisplayName 2");
TestCategory2.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
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
This mod is in active development. The plan is to increase extensibility. 
**<ins>~~Basic MelonPreferences registration is stable and should always be backwards compatible.~~</ins>** 
So while advanced API usage will have a lot of changes for the time that this mod is in [Version 0.x.x](https://semver.org/#spec-item-4),
mods that implement the basic use case of this framework don't have to worry about breaking in the future (as long as I don't mess up too bad). 

### Oops (0.6.2)
Well, so much for always be backwards compatible. In order to support plugins, I'm having to change .Register to use MelonBase as the instance instead of MelonMod
Currently, I have an obsolete bridging function for backwards compatibility. 
But that will be removed in a future version if I'm confident that enough mods have migrated that it won't be too big of a problem. 

In order to make your mod future proof, explicitly cast your MelonMod instance to MelonBase in your next update. 
You don't need to publish that update now for this small change but it makes it so that when the old function does become actually deprecated, you won't need to push an update specific to it.

```cs
UIFModel.ModelMod me = UI.Register((MelonBase)this, TestCategory1, TestCategory2);
```

Okay, so for real this time: **<ins>Basic MelonPreferences registration is stable and should always be backwards compatible.</ins>** 

### XML Documentation File
You can place the .xml documentation file for UIFramework in the same folder as the dll to get intellisense documentation for the API. It is currently incomplete, however but I do add to it every update.