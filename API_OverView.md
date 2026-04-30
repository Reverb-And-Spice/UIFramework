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
//Make sure the directory exists before setting the file path
if (!Directory.Exists(USER_DATA))
    Directory.CreateDirectory(USER_DATA);

TestCategory1.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
TestCategory2.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
```
<details><summary> A note on custom file paths</summary>
Please make sure the directory exists before setting the file path. If the directory doesn't exist, it will fail silently and your settings won't save with no explanation
</details>

### Entries
Entries are the actual settings that you want to save. 
#### Variable declaration
```cs
private MelonPreferences_Entry<string> TestEntry11;
private MelonPreferences_Entry<int> TestEntry12;

private MelonPreferences_Entry<float> TestEntry21;
private MelonPreferences_Entry<bool> TestEntry22;
```
#### Entry Creation 
```cs
TestEntry11 = TestCategory1.CreateEntry("Entry 1-1", "Test Val", "Display Name1", "Test String");
TestEntry12 = TestCategory1.CreateEntry("Entry 1-2", 1, "Display Name2", "Test Int");

TestEntry21 = TestCategory2.CreateEntry("Entry 2-1", "0.5126", "Display Name 3", "Test float");
TestEntry22 = TestCategory2.CreateEntry("Entry 2-2", true, "Display Name 4", "Test bool");
```

#### Acessing and modifying values:
Entries are objects. You need to access their Value property to get or set the actual value. 
```cs
string value1 = TestEntry11.Value;
TestEntry11.Value = "New Value";
```