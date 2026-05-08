<sup> btw: The changelog doubles as a feature list </sup>
### New in 0.9.0 
<details><summary> New Feature: Reactivity Update! </summary>
UI Framework can now refresh without the user having to press save or manually reloading the tab
</details>
<details><summary> New Feature: Update UI with current values.</summary>
UI now refreshes when an entry value for a preference changes
</details>

<details><summary>New Feature: IsHidden support for categories</summary>
UI Framework will now appropriately not show categories with the IsHidden property set to true.
</details>
<details><summary> New Feature: Modders can now implement an UserEditNotifier validator class.</summary>
This can notify you when the user has edited their entry even when it hasn't been applied to their saved value yet.

Combined with the previous new feature, you can now adjust your UI according to your users' inputs

The following example uses it to change the visibility of certain categories based on user input without them having to hit save

```cs
//Create a method you can pass as a delegate
internal static void UpdateCategoryVis(object newValue)
{
    Experimental.IsHidden = !(bool)newValue;
    TestBooleans.IsHidden = !(bool)newValue;
    TestEmptyDisplayName.IsHidden = !(bool)newValue;
}
```
Create a new instance of `UserEditNotifier` and set this as the action for OnUserEdit
```cs
EnableDebugMode = CatUIFramework.CreateEntry("EnableDebugMode", false, "Enable Debug Logs", "Enables or disables debug logs for UIFramework.", false, false, new UserEditNotifier { OnUserEdit = UpdateCategoryVis });
```
</details>


<details><summary> New Feature: Modders can now request the window to refresh its UI. </summary>

Just call `UI.RequestRefresh(modInstance)`
</details>

<details><summary>New Feature: Dynamic dropdown support</summary>
UI Framework now supports custom dropdown contents, not just from enums. 

Create an items list

```cs
// It takes a list of DropdownItems which take a string as a display name, and a value of type object 
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
</details>

<details> <summary> Bug Fix: Added Flatland support </summary>
UI Framework now works in Flatland
</details>

-----

# For Users
Drop the dll in your mods folder. 
### **Default toggle is the `F9` key**


~~Changing a value of an entry automatically updates the value of the preference and is applied. How that preference's parent mod reacts depends on the modder's implementation.~~

<sup>* the above is no longer true as of 0.8.0. Values will not update until the saved button has been clicked</sup>


The save button writes it to the file for permanent storage. Closing your game might also save preferences to file automatically depending on whether it's closed from the game window or through Steam. **Stopping through Steam doesn't save because it force closes it.**

------

# For Modders 

A (more) detailed API Overview exists over at https://github.com/Reverb-And-Spice/UIFramework/blob/main/API_OverView.md
## Basic Registration
Add `[assembly: MelonAdditionalDependencies("UIFramework")]` to your AssemblyInfo. This prevents your mod from calling on UIFramework before it's been initialized.

[Define](#If-you-havent-used-melonpreferences-before) your MelonPreferences in `OnInitializeMelon` and then register them to the UI.
```cs
UI.Register(this, TestCategory1, TestCategory2...);
```
~~Right now, support is limited to common types like `string`, `int`, `bool`, `double`, `float`, and `enums` without the flags attribute. Working on expanding this.~~

### Type Support: Whatever works with Tomlet
<details> <summary>Expanded Type Support details</summary>

Support is no longer limited to the types mentioned above. Serialization and parsing is now handled by [Tomlet](https://github.com/SamboyCoding/Tomlet). 
This means that it supports types described in [Toml 1.0.0](https://toml.io/en/v1.0.0) and whatever Tomlet supports. [You can even make your own custom mappers](https://github.com/SamboyCoding/Tomlet/blob/master/README.md#creating-your-own-mappers)

Caveat: Types handled by Tomlet will be presented as regular text inputs and they might not always look good. Numerics will have the appropriate filters.
I do plan to continue expanding the number of custom UI presenters like I did with enums and booleans.
 </details>


### Optional: OnSave Event Handler
You can add an event handler that gets called when the save button is clicked while your mod is selected.
```cs
private void MyModSaved()
{
    // Do something when the save button is clicked while your mod is selected
}
```
```cs
UI.Register((MelonBase)this, OBSAutoRecorderSettings, TestCategory1, TestCategory2...).OnModSaved += MyModSaved;
```
<sup>Casting to melonbase isn't necessary but it forces your compiler to use the newer MelonBase registration instead of the obsolete MelonMod registration
In the future, all mods will be registered as MelonBase by default and the cast won't be needed. 
But the cast makes sure that your mod won't break when the old MelonMod registration gets removed</sup>


### Optional: Custom display names


Add `[assembly: UIInfo("My Mod's Better\nDisplay Name")]` to your assembly attributes to change how the mod's name is displayed
in the UI. Line breaks are supported.

-----

# Advanced Usage
I moved this section the [API Overview](https://github.com/Reverb-And-Spice/UIFramework/blob/main/API_OverView.md#ui-presentation-control-validator-extensions)


-----

## If you haven't used melonpreferences before

I detail usage and creation here: 
https://github.com/Reverb-And-Spice/UIFramework/blob/main/API_OverView.md#melonpreferences

And the official docs are here: 
https://melonwiki.xyz/#/modders/preferences?id=melon-preferences





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
UI.Register((MelonBase)this, TestCategory1, TestCategory2);
```

Okay, so for real this time: **<ins>Basic MelonPreferences registration is stable and should always be backwards compatible.</ins>** 

### XML Documentation File
You can place the .xml documentation file for UIFramework in the same folder as the dll to get intellisense documentation for the API. 
It is currently incomplete, however but I do add to it every update.