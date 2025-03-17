Subactions
- Force Movement
	- Might require a new CombatantState
	- Might be able to get away with just using an interface on an obstacle.
- Teleport
	- Hopefully wont require a new CombatantState
		- But can only happen when a combatant is in IdleState
	- Obstacles should just be able to slap on an interface
- Rework Damage
	- Has "overTime" bool
		- Has "turnDuration" int
- Rework Health
	- Has a "by percent" bool
		- Calculates the heal amount based in the user's MAX_HEALTH stat
	- Has "overTime" bool
		- Has "turnDuration" int
- Rework Status Effect
	- Has a "by percent" bool
	- Calculates and constructs the stat set to give to the target's Stats component by percent, instead of by direct value.
- Conditional Property Drawing
	- Look into this
	- If overTime, show turnDuration
	- If byPercent, clamp to 1f

VFX
- Create Prefabs
	- Sprites
	- Anims
	- Prefab for each new one
	- BUT same VFX prefab can be used for multiple CombatActions and/or CombatSubactions.
- Apply them to each CombatAction
	- New `SpawnVfxSO` for each `CombatSubaction` that requires VFX.
		- `SpawnVfxSO` needs a reference to the targeting pattern of the `CombatSubaction` it is meant to apply to.
	- Apply them to the `List<CombatSubaction>` on each CombatAction
	- Sequential operation
		- Remember, the list of CombatSubactions will be applied independently to each target in the order that they are listed in the inspector
		- This means they should be placed in the context of the list right next to the Subaction that they are meant to be an effect for.
- Test them, make sure they look okay on each enemy that they can be used on.

Projectiles
- Create projectile system
	- Instantiate GameObjects
	- Objects move from a specified position to the target position
		- Do they arc?
			- If so, Slerp?
			- Otherwise, Lerp?
		- Over a set amount of time (i.e. variable speed) or at a constant speed (variable duration)
		- Trigger the continuance of further subactions once the projectile hits somehow?
		- Does `ITargetable` have a `ReceiveProjectile(ProjectileController)`? Maybe:
			1. Targets iterate through their `List<ISubactionCommand> TargetedBy` lists
			2. If Target detects a projectile subaction command in its list
			3. Delay execution of further commands.
			4. `ReceiveProjectile(Projectile)` resumes execution of `ISubactionCommand`s
	- If it doesn't seem to be working to implement this with a strategy like the VFX, where each is a CombatSubaction, we'll have to think of another system

Review Building Transparency

Review Audio System

Review Save System

Obstacles
	- Write the remaining Obstacle Classes
		- Dependent on Force Movement Subaction

	- Refactor `Dungeon`
		- Dependent on Writing the remaining Obstacle classes.
		- Needs 4 GameObjects with all the derivatives of abstract class `Obstacle`
			- Static, Undamageable
			- Static, Damageable
			- Dynamic, Undamageable,
			- Dynamic, Damageable
		- Rewire everyone's dungeon prefabs to accept all of these tilemaps in the inspector, maintain the references that they already have (if any)
			- Use IFieldReserializer tool

`IFieldReserializer` for `ScriptableObject`s
	- Either:
		- Modify the "Permanent Renamer" tool that uses `IFieldReserializer to create a tool that allows for the same functionality but with changing `ScriptableObject` classes and every instance of said class in the project.
		- Just write a new derived class or even a similar function that can do a similar thing, also using `IFieldReserializer`

Panel Swapper/Cycler for CharacterMenu
	- See if there is already a cycler or toggler somewhere in Utils we can use, I'm 99% sure theres something in there.
	- If not, write it
	- If so, apply it
		- Right Side - Static panel showing attributes, stats, upgrades.
		- Left Side
			- Equipment Panel
			- Player info panel
			- 

Take off the upper cap on `Attributes`
	- At least for now, we can set it again if we need to when balancing.
	- Find where this is
		- In some Attributes-related class?
		- Not sure which exactly.
	- Just change it to a `const int.MaxValue` or something.

Investigate Stats Panel Truncation
	- Unconfirmed bug
	- Had a thought recently that it looked like the Physical Power stat was really really low
	- Thought this might be because of the TMP text being cut off.
	- Possible issues
		- TMP not set to size.bestFit or whatever
			- If it's this, don't actually leave it set to best fit, just find the value where like 5 digits fit into the box and change the font size on every stat's LabeledField's Value Textbox to
			- *Don't change RectTransform scale!!!*

Popup bug
	- Expected behavior:
		- Popup opens when the mouse hovers over a full slot
		- Popup closes when the mouse exits the slot.
		- When an item is removed from a slot that the mouse is currently hovering over (i.e. in the case of using a double click for movement of items), Popup either:
			- Closes 
			- Moves positions to the slot that the item has been moved to
	- Current behavior:
		- Popup opens when the mouse hovers over a full slot.
		- Popup closes when the mouse exits the slot.
		- When an item is removed from a slot that the mouse is currently hovering over, Popup
			- Remains open
			- Remains open even when the mouse leaves the old item slot
			- Only closes when the mouse hoveing over a full slot forces a close/reopen on the popup.