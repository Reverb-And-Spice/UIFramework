# UI Framework Architecture Overview
## MVA(-ish) Design Pattern
UI framework keeps the UI and the data separate by applying a design pattern inspired by the model-view-adapter pattern

- ***Views***: In UI Framework's case, these are the game objects that serve as UI elements the user interacts with. 

- ***Adapters***: These are custom game object components added to the views that control how the view presents the data to the user.

- ***Models***:  These are wrapper classes around the data (currently only MelonPreferences) that provide a common interface for the adapters to interact with. 

This separation allows for UI Framework to be customizable and expandable. 

Expandability can be anything from using the [Advanced UI Customization Features](https://github.com/iListen2Sound/UIFramework/blob/main/CustomUI.md#making-custom-entryadapters) to *creating an entirely separate diagetic UI that interfaces with the models*

## Views and adapters are temporary
Whenever a new set of items are loaded, all the previous views (with their adapter components) are destroyed and replaced with new ones. Whenever the UI refreshes, the same thing happens. Instead of updating each view individually with the updated data, they are destroyed and new ones loaded with the correct values. 

This might seem inefficient but it simplifies the logic of making sure which views are properly shown or hidden and then keeping them updated, and instead of the models having to notify their respective views, they just notify UI Framework as a whole, triggering a refresh spawning new views and adapters updated with the current information
