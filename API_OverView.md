# UI Framework API overview
UI Framework builds a UI for modders' configuration for their mods. Particularly MelonPreferences but hopefully expanded to more
## MelonPreferences
MelonPreferences are the built-in configuration and persistent storage system in MelonLoader. 

<details><summary>Expand to see how to initialize and use MelonPreferences</summary>

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
</details>


### Events
- `MelonPreferences_Entry.OnEntryValueChanged`: Event that fires when the value is changed (Value is applied when you hit the save button in the UI Framework window). Provides oldValue and newValue parameters so you can monitor if it's been changed from the previous values. 
Must be subscribed with via the `.Subscribe()` method instead of `+=`

-----
## UI Framework
### Registration

Once you have your MelonPreferences set up, registering to UI Framework is simple.
Just call 
```cs
UI.RegisterMelon(MelonBase modInstance, params MelonPreferences_Category[])
```
So `UI.RegisterMelon(this, TestCategory1, TestCategory2);`
if you're calling it from your main MelonMod class.

Your categories will display as tabs in the order that you put them in the parameter array 
and your entries will show in the order thatw you created them.


**For most usage, this will be enough.**
The UI will automatically present your entries according to their data types. Bools will show as toggles, enums will show as dropdowns, strings and numbers will show as text inputs with the appropriate filters, etc.

### Optional: Custom display names
Add `[assembly: UIInfo("My Mod's Better\nDisplay Name")]` to your assembly attributes to change how the mod's name is displayed
in the UI. Line breaks are supported.
### Enum Display Names
Enum dropdowns support the Display(Name) attribute. If unavailable, it will fall back to the default enum value name. 
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
## UI Presentation control: Validator Extensions

UI Framework piggybacks off of the existing MelonPreferences custom validator system. 
You can influence how the UI is presented by using custom validator classes that implement certain interfaces that act as descriptors.

<details><summary>  <strong>MelonPreferences custom validator system details</strong> </summary>

CreateEntry takes an optional parameter for a custom validator
```cs 
CreateEntry(string identifier, T default_value, string display_name = null, string description = null, bool is_hidden = false, bool dont_save_default = false, Preferences.ValueValidator validator = null)
```

You can make your own custom validator by inheriting the ValueValidator class.

```cs 
public class CustomValidator : ValueValidator
{
    //These two are required members. If you don't care about validation, you can just always say that the passed object is valid
    public override bool IsValid(object value)
    {
        return true;
    }
    public override object EnsureValid(object value)
    {
        return value;
    }
}
```
But UI Framework already has its own default implementation that validates everything in the `UIFramework.ValidatorExtensions` namespace as `DevaultValidator`.
</details>

-----

UI Framework provides default validator classes for the most common interfaces. So for most of them, you just need to pass 
```cs
new DefaultDescriptor {Property = value}
```
into your CreateEntry method along with your other parameters.

Expand the next section if you want to make your own custom validator or combine multiple extensions ↓
<details><summary><strong>UI Framework Validator Extensions details</strong></summary>

### Actual validation
The `DefaultValidator` just approves every value you pass to its functions. 

You can inherit it and override the `IsValid` and `EnsureValid` functions to implement your own.

### Combining Extension types
Some extension types are cross-compatible. If they're not, they shouldn't break, it'll just pick the first one by priority.

<details> <summary>Notes on compatibility</summary>
<sup>Generally extension types that change what prefab represents the entry aren't compatible. You can't be a slider *and* a dropdown at the same time.</sup>
</details>

But you can easily combine `ISliderDescriptor` and `IUserEditedNotifier` to make it a slider *and* notify you of user edits.

You do that by inheriting both into your custom validator

```cs
//Assuming you still don't wanna do validation, you can just inherit from the DefaultValidatorClass
public class NotifyingSlider : DefaultValidator, ISliderDEscriptor, IUserEditedNotifier
{
    public float Min {get; set;}
    public float Max {get; set;}
    public Action<object> OnUserEdit {get; set;}
}
```
```cs
//Pass it into the validator parameter
MySlider = MyCategory.CreateEntry("MySlider", 0, "My Slider", "Example Slider", false, false, new NotifyingSlider {Min = 0, Max = 1, OnUserEdit = MySliderSlid})
```
</details>

-----
### Implemented Extensions
#### Sliders
##### Interface: `ISliderDescriptor`
##### Default extended validator: `SliderDescriptor`
The UI will represent your entry with a slider if you add a validator that implements `ISliderDescriptor`.


```cs
MySlider = MyCategory.CreateEntry("MySlider", 0.5f, "My Slider", "Float Slider",false, false, new SliderDescriptor { Min = 0, Max = 1, DecimalPlaces = 3 });
```

-----
#### Dynamically Editable Dropdown
#### Interface: `IDynamicDropdownDescriptor`
#### Default extended validator: `DynamicDropdownDescriptor`

This lets you have a dropdown where you can edit the contents 

Create an items list

```cs
List<DropdownItem> itemList = new();
```
Create an instance of the `DynamicDropdownDescriptor` class passing the item list as a parameter in the constructor
```cs
public static DynamicDropdownDescriptor DropdownDescriptor = new(itemList);
```
Add items with 
```cs
DropdownDescriptor.AddDropdownItem(new DropdownItem("Display Name", value)); 
```
or declare a list separately and set it with SetDropdownItems



Pass it into the CreateEntry validator parameter
```cs
DropdownTest = Category.CreateEntry("DropdownTest", -1, "Dropdown Test", "Dynamic dropdown test.", false, false, DropdownDescriptor);
```

-----
#### User Edit Notifiers
##### Interface: `IUserEditedNotifier`
##### Default extended validator: `UserEditDefaultNotifier`
This doesn't change the UIs presentation but it does notify you when the user inputs a new value into the UI e.g. when they're done editing a text input, clicked a toggle or finished moving a slider. It also provides you with the new value

```cs
//Define an event handler you can pass into the notifier. It must take an object that represents the new value
void MyToggleToggled(object newValue)
{
    newBool = (bool) newValue;
    LoggerInstance.Msg($"MyToggle has been set to {newBool}");
}
```
```cs
//Instantiate a new UserEditDefaultNotifier and assign your delegate to OnUserEdit.
MyToggle = MyCategory.CreateEntry("MyToggle", true, "My Toggle", "Example Toggle", false, false, new UserEditDefaultNotifier {OnUserEdit = MyToggleToggled})
```
-----
#### Buttons
##### Interface: `IButtonDescriptor`
##### Default extended validator: `ButtonAsEntry` (internal)
This is a special case. You don't need to implement this yourself. Instead, you call 
```cs
UI.CreateButtonEntry(MelonPreferences_Category category, string buttonText, string displayName, string description, Action handler)
```
This method will handle the implementation for you and it will show the button in the entries list.
