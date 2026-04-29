## MelonPreferences
MelonPreferences are the built-in configuration and persistent storage system in MelonLoader. 

### Categories
Categories are a base MelonPreferences feature to group entries together. 
#### Variable declarotion
```cs 
private MelonPreferences_Category TestCategory1;
private MelonPreferences_Category TestCategory2;
```
#### Intantiation
You can have make your categories have a different display name to its actual identifier
Please prefix your category identifiers with your mod name

```cs
//* Please prefix your categories with your mod's name
TestCategory1 = MelonPreferences.CreateCategory("MyMod_TestCat1", "DisplayName 1");
TestCategory2 = MelonPreferences.CreateCategory("MyMod_TestCat2", "DisplayName 2");
```
#### File path customization
You can assign each category its own filepath.
```cs
private const string USER_DATA = "UserData/MyMod/";
private const string CONFIG_FILE = "config.cfg";
...

if (!Directory.Exists(USER_DATA))
    Directory.CreateDirectory(USER_DATA);

TestCategory1.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
TestCategory2.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
```
<details><summary> A note on custom file paths</summary>
Please make sure 
</details>
