# New in 0.8.0

***Modders read this first one***
<details><summary>New Feature: Improved MelonPreferences_Entry.Value Behavior</summary>

The Value property for entries won't update anymore until the save button is clicked.
If you need to access the new value before it's been saved, you can get the EditedValue property instead. 
</details>

<details><summary>New Feature: Expanded Type Support</summary>

Serialization and parsing is now handled by Tomlet. 
Anything Tomlet supports is now technically supported by UIFramework.

More details in the [Type Support](##type-support-whatever-works-with-tomlet) section
</details> 

<details><summary>New feature: Custom display name attribute</summary>

Just add `[assembly: UIInfo("My Mod's Better\nDisplay Name")]` to your assembly attributes to display your 
mod's name differently on its button in UI Framework. Yes, it supports line breaks

</details>
<details><summary>New Feature: Support IsHidden property for entries.</summary>

Entries with `IsHidden` set to `true` won't be listed in the preferences list anymore.
</details>

<details><summary>New Feature: New Validator Extension System.</summary>

I came up with a system to use MelonLoader's custom validator feature to add extra UI configurations for entries. The new sliders and buttons feature are implemented through this system.
</details>
<details><summary>New Feature: Sliders! (and maybe more eventually 👀)</summary>

Modders can now implement sliders for numeric vlaues.
```cs
MySlider = Category.CreateEntry("MySlider", 0.5f, "My Slider", "Float Slider",false, false, new SliderDescriptor { Min = 0, Max = 1, DecimalPlaces = 3 });
```

</details>

<details><summary> New Feature: Add Buttons to the Entry List</summary>

Modders can now add their own buttons as entries into UI Framework. 

```cs
UI.CreateButtonEntry(MelonPreferences_Category category, string buttonText, string displayName, string description, Action handler)
```

Go to [Buttons](###buttons) for more details.

</details>


<details><summary>Bug Fix: Fixed issue with UI Framework *displaying* ModUI's window instead of hiding</summary>

whoops
</details>

<details><summary>Bug Fix: Finally suppressed saved and loaded message from MelonPreferneces</summary>

I somehow missed an entire boolean. Sorry Ulvak.

\*<sup>This will only affect messages when you save or load through UI Framework. It will not suppress messages from melonloader itself when the game starts or closes.</sup>
</details>




# New in 0.7.1
<details><summary>New setting: VR Toggle</summary>
Toggle UI Framework window using your controllers by pressing both grips on both hands
and pressing both primary buttons on both hands (X and A)

</details>
<details><summary>New setting: Force Hide ModUI</summary>
Never leave ModUI accidentally open again. 
Enabling this setting will make UI Framework hide the ModUI window when UI Framework hides. 
This setting does not support the inactivity timer if ModUI is open by itself but does support hide on scene load

</details>
<details><summary>Bug fix: (hopefully) Fix layout quirks involving scroll views</summary>
Hopefully, this fixes the issue with tabs all being squished to one side or only half showing.

</details>

# New in 0.6.2
<details><summary>New Feature: Plugin support</summary>
I just completely forgot about those.

<sup>I did have to change the .Register() function's signature. Right now, I have an overload for the old function for backwards compatibility. More details at the bottom of the page </sup>
</details>

<details><summary>New Feature: Exposed UI.IsVisible Property</summary>
Your mod can now check if UIFramework is currently open. 
</details>

# Version 0.6.1

<details><summary>New Feature: Exposed OnModSaved event for modders</summary>

You can now subscribe to the `OnModSaved` event that triggers when the saved button is clicked while your mod is selected. 
This is an alternative for `OnPreferencesSaved` from MelonPreferences which gets called per category.
This one is called once and only if your mod is selected.
```cs
UI.Register(this, OBSAutoRecorderSettings, TestCategory1, TestCategory2...).OnModSaved += MyModSaved;
```

</details>
<details><summary>Bug Fix: Increased Supported Mod Name Length</summary>
Longer mod names can now fit into the mod list

<sup>*btw while text wrapping is disabled in mod list buttons, your MelonInfo name property does support spaces and line breaks that you can add manually if your mod name is still too long to fit into one line</sup>
</details>

<details><summary>Bug Fix: Fixed bug that called the selected category save action twice</summary>
</details>

# Version 0.6.0
<details><summary>New Feature: Hide UI on inactivity </summary>
Added settings to hide the UI after a certain amount of inactivity with keyboard and mouse. 

Comes with new settings: 

- `Auto Hide on Inactivity`
- `Inactivity Timeout`

</details>

<details><summary>New Feature: Remembers last category opened for each mod</summary>
Moving between mods will now show the last category you were on for that mod. Will default to the first category. No more having to select the same category again when switching between mods. 
</details>

<details><summary>New Feature: Selection highlighting</summary>
Selected mods and categories will now be colored in the UI.
</details>


# Version 0.5.1

<details><summary> New Feature: Non-contiguous enum support </summary>
You can now use enum types that are non-sequential and non-zero-based for your dropdowns
</details>
<details><summary> New Feature: Discard button </summary>

Made discard button visible to load the saved values from the preferences file

</details>

# Version 0.5.0
- Added support for enum display names
- Save button now saves the entire mod, not just the selected tab

# Version 0.4.0
- Created
