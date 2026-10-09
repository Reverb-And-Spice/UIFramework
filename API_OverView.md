# UI Framework API overview
UI Framework builds a UI for modders' configuration for their mods. Particularly MelonPreferences, but hopefully it will be expanded to more
## MelonPreferences
MelonPreferences are the built-in configuration and persistent storage system in MelonLoader. 

<details><summary>Expand to see how to initialize and use MelonPreferences</summary>

### Categories
Categories are a base MelonPreferences feature to group entries together. 
#### Variable declaration
```cs 
private MelonPreferences_Category TestCategory1;
private MelonPreferences_Category TestCategory2;
```
#### Instantiation
You can make your categories have a different display name from its actual identifier
Please prefix your category identifiers with your mod's name

```cs
// Please prefix your categories with your mod's name
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

> [!WARNING]
> Please make sure the directory exists before setting the file path. 
If the directory doesn't exist, it might fail silently and your settings won't save with no explanation


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

#### Accessing and modifying values:
Entries are objects. You need to access their Value property to get or set the actual value. 
```cs
string value1 = TestEntry11.Value;
TestEntry11.Value = "New Value";
```
</details>


### Events
- `MelonPreferences_Entry.OnEntryValueChanged`: Event that fires when the value is changed (Value is applied when you hit the save button in the UI Framework window). Provides oldValue and newValue parameters so you can monitor if it's been changed from the previous values. 
Must be subscribed with via the `.Subscribe()` method instead of `+=`
- `MelonPreferences.OnPreferencesSaved`: Event that fires whenever preferences are saved.
This fires regardless of which category triggers it. You have to filter for your file path to only respond to your category saving

-----
## UI Framework
### Registration

Once you have your MelonPreferences set up, registering to UI Framework is simple.
Just call 
```cs
UI.RegisterMelon(modInstance, params MelonPreferences_Category[])
```
So `UI.RegisterMelon(this, TestCategory1, TestCategory2);`
if you're calling it from your main MelonMod class.

Your categories will display as tabs in the order that you put them in the parameter array 
and your entries will show in the order that you created them.


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

## UI Refresh Request
You can request a refresh from the UI through the MelonModel object which is returned by the `UI.RegisterMelon()` function and can be assigned to a variable
```cs
MelonModel MyModel = UI.RegisterMelon(this, myCategory1, myCategory2);
```
If you want to refresh the UI if, for example, you updated an EditedValue through code and you want the UI to reflect that change, you can call 
```cs
MyModel.RequestUpdateUI();
```
And this will cause UI Framework to update all the UI elements on the next `Update()` if your mod is the currently selected one.

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
But UI Framework already has its own default implementation that validates everything in the `UIFramework.UiExtensions` namespace as `DefaultValidator`.
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
//Assuming you still don't want to do validation, you can just inherit from the DefaultValidatorClass
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

### TextInput Customization (New in 0.10.3)
#### Interface: `ITextInputBehaviorDescriptor`
#### Default Implementation: `TextInputBehaviorDescriptor`
This lets you control the behavior of TextInputs. Giving you control of these properties:
- `ContentType` - Lets define the ContentType for the text input. 
    - Standard - Accepts strings and hides the characters behind the char set with the `PasswordChar` 
    - Password - Accepts strings and hides 
    - Email Address - Unity Email Address Text Inputs
     
     <sup>The other unity content types also work. I just haven't tried them. </sup>

- `CharacterLimit`
- `IsReadOnly`

```cs
DemoTextBehavior = Demo.CreateEntry("TextBehaviorDemo", "VerySecurePassword", "Custom Behavior Demo",
				"This tests the BehaviorDescriptor Interface", false, false,
				new TextInputBehaviorDescriptor
					{ ContentType = TMP_InputField.ContentType.Password, 
                    CharacterLimit = 20, 
                    IsReadOnly = true });
```

-----
#### Sliders
##### Interface: `ISliderDescriptor`
##### Default Implementation: `SliderDescriptor`
The UI will represent your entry with a slider if you add a validator that implements `ISliderDescriptor`.


```cs
MySlider = MyCategory.CreateEntry("MySlider", 0.5f, "My Slider", "Float Slider",false, false, new SliderDescriptor { Min = 0, Max = 1, DecimalPlaces = 3 });
```

-----
### NumberBox (New in 0.10.0)
##### Interface: `INumberBoxDescriptor`
##### Default Implementation: `NumberBoxDescriptor`

Lets you customize interactions with numeric text inputs. 
```cs
MyNumBox = MyCategory.CreateEntry("MyNumBox", 0.5f, "My Number Box", "Float Number Box",false, false, new NumberBoxDescriptor { Steps = 0.5f, DecimalPlaces = 3 });
```

-----
#### Dynamically Editable Dropdowns (New in 0.9.0)
#### Interface: `IDynamicDropdownDescriptor`
#### Default Implementation: `DynamicDropdownDescriptor`

This lets you describe a dropdown whose options you can change at runtime.
This lets you guide your users to valid options instead of letting them manually type in a text input 
which is prone to errors. 

- DropdownItem: This is a simple class used to describe a dropdown option. It has a DisplayName and a Value property.
The DisplayName is what the user sees in the dropdown and <u>***the Value property is the actual value that gets stored***</u> when the user
selects that option, not the DropdownItem itself.
- Item list: The list of `DropdownItem` objects that get displayed in the dropdown. 

***Creating a DropdownDescriptor***

- Create an items list. You can leave it empty for now or you can pre-populate it with items.

```cs
List<DropdownItem> itemList = new();
```
- Create an instance of the `DynamicDropdownDescriptor` class passing the item list as a parameter in the constructor
```cs
public DynamicDropdownDescriptor DropdownDescriptor = new(itemList);
```
- You can add items with 
```cs
DropdownDescriptor.AddDropdownItem(new DropdownItem("Display Name", value)); 
```
or declare a list separately and set it with SetDropdownItems


***Using your DynamicDropdownDescriptor***

- Declare your entry as usual. 
  - Notes: 
      - An entry is just a normal entry and can be any type like every other preference entry in your mod. 
      - **DO NOT** declare your type as a DropdownItem. 
      - DropdownItems are only used to describe what options appear on the dropdown. They are not the data that's actually stored. 
```cs
MelonPreference_Entry<string> DropdownTest;
```
- Pass it into the CreateEntry validator parameter
```cs
DropdownTest = Category.CreateEntry("DropdownTest", "Default Value", "Dropdown Test", null, false, false, DropdownDescriptor);
```

-----
#### User Edit Notifiers (New in 0.9.0)
##### Interface: `IUserEditedNotifier`
##### Default Implementation: `UserEditDefaultNotifier`
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
##### Default Implementation: `ButtonAsEntry` (internal)
This is a special case. You don't need to implement this yourself. Instead, you call 
```cs
UI.CreateButtonEntry(MelonPreferences_Category category, string buttonText, string displayName, string description, Action handler)
```
This method will handle the implementation for you and it will show the button in the entries list.

-----
#### Custom Entry Presentations (New in 0.10.0)
##### Interface: `ICustomViewProvider`
##### Default Implementation: `CustomViewProvider`
Lets you assign a custom prefab to represent your entry in the UI through its `EntryViewPrefab` property. 
You can make and load new prefabs through assetbundles. 
The prefab must have a component that inherits from `DataEntryAdapter` on its root game object.
More details in the [Custom UI](CustomUI.md) documentation.
Pass it into the CreateEntry method's validator parameter
```cs 
TestEntryCustom = TestCategory2.CreateEntry("TestEntryCustom", "hello; world", "Test Custom Entry", "This is a test custom entry ", false, false, new CustomViewProvider { EntryViewPrefab = customWidget });
```

I've included a unity package with UI Framework's main window structure and some example custom widgets in [TestFit.unitypackage](_Misc/TestFit.unitypackage) so you can build your custom views in there and see how it would fit into UI Framework
