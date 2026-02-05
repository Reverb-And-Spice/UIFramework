# NOT MOD UI

We want people to be able to implement this as easy and painless as possible. 

This will not be diegetic.
## Relevant Docs 
https://melonwiki.xyz/#/modders/preferences?id=melon-preferences

## As melon preferences interface

### Example Usage

**<details><summary> Standard Melon preferences declaration</summary>**
    
```
private const string USER_DATA = "UserData/TestMod/";
private const string CONFIG_FILE = "config.cfg";

private MelonPreferences_Category TestCategory1;
private MelonPreferences_Entry<string> TestEntry11;
private MelonPreferences_Entry<int> TestEntry12;

private MelonPreferences_Category TestCategory2;
private MelonPreferences_Entry<float> TestEntry21;
private MelonPreferences_Entry<bool> TestEntry22;



TestCategory1 = MelonPreferences.CreateCategory("Test Cat 1");
TestCategory1.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
TestEntry11 = TestCategory1.CreateEntry("Entry 1-1", "Test Val", null, "Test String");
TestEntry12 = TestCategory1.CreateEntry("Entry 1-2", 1, null, "Test Int");

TestCategory2 = MelonPreferences.CreateCategory("Test Cat 2");
TestCategory2.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
TestEntry21 = TestCategory1.CreateEntry("Entry 2-1", "0.5126", null, "Test float");
TestEntry22 = TestCategory1.CreateEntry("Entry 2-2", true, null, "Test bool");
```
</details>

#### Registering MelonPreferences_Category to UI Framework
```
UI.Register(TestCategory1);
UI.Register(TestCategory2);
```



-----
### Quick layout mockup
<sup>No it will not look like this. I just needed to show the layout</sup>

![UI Mockup](misc/Mockup.png)

-----

## Proposal for custom interface usage

### Option 1: Copy Melonpreferences process
> [!NOTE]
> Do we want interface creation to resemble MelonPreferences? Gives users a familiar pattern but does add a few steps

```
public class UI_Category
{
    
}

public class UI_Entry
{
    public class Button{}
    public class Text{}
}

private UI_Category TestTab1;
private UI_Entry<UI_Entry.Button> TestButton1;
private UI.TextEntry<UI_Entry.Text> TestText1;

//Make UI entry creation similar to preferences creation
TestTab1 = UIInterface.CreateCategory("Test Tab 1"); 

//What would the default value for a button even be?
TestButton1 = TestTab1.CreateEntry("Test Button 1", /*???*/, null, "This is just a test default ");
TestText1 = TestTab1.CreateEntry("Test Textbox 1", "Default Text", null, "This is just a test textbox default");
```

### Option 2: Create our own

```
private UI.Tab TestTab1 = new UI.Tab("Title text");
private UI.Button Entry1 = new UI.Button();
Entry.OnClick += Entry1Click;

TestTab1.AddControl(Entry1);

Entry1_Click(Button1 sender)
{
    //stuff
}
```

#### Option 2.5: Fluent style

> [!NOTE] 
> should we do WinForms style sender and event args to refer back to the caller?

```
public void Button1_Click(Button1 sender, Params p)
{

}

public void Text1_Input(Text1 sender, Params p)
{

}

public void Apply()
{

}
TestTab1 = new UI.Tab("Title Text)
    .WithDefaultActionButton(true) //this would be the save button on a regular MelonPreferences tab. If on a non pref tab, the user can subscribe to its onclick event
    .DefaultAction(Apply)
    .AddControl(new Button()
        .WithDescription("This is a test button")
        .WithText("Button1")
        .AddOnClick(Button1_Click)
        .Build()
    )
    .AddControl(new Textbox()
        .WithDescription("This is a test textbox")
        .WithText("Default Text 1")
        .AddOnTextChanged(Text1_Input)
        .Build()
    ).Build();
    
```
