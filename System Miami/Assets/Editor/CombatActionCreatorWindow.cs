using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using SystemMiami;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;

public class CombatActionCreatorWindow : EditorWindow
{
    [SerializeField] private GlobalIDDatabase _idDatabase;

    private static Type[] _allSubactionTypes;
    private static string[] _allSubactionTypeNames;

    private static Type[] _allPatternTypes;
    private static string[] _allPatternTypeNames;

    [System.Serializable]
    private class SubactionCreationInfo
    {
        // For the subaction
        public string subactionAssetName = "NewSubaction";
        public int subactionTypeIndex = 0;

        // For the pattern
        public string patternAssetName = "NewPattern";
        public int patternTypeIndex = 0;
        public PatternOriginType patternOrigin = PatternOriginType.USER;
        public Color targetedTileColor = Color.white;
        public Color targetedCombatantColor = Color.white;

        // Reflection-based custom fields
        // Key   = Field name
        // Value = The string representation of the user's input
        public Dictionary<string, string> subactionFieldValues = new Dictionary<string, string>();
    }

    [Header("Abiliy Creation")]
    private string _abilityName = "New Ability";
    private string _abilityDescription = "";
    private Sprite _abilityIcon;
    private AbilityType _abilityType;
    private float _resourceCost;
    private int _cooldownTurns;
    private AnimatorOverrideController _abilityAnimator;

    private int _abilitySubactionCount = 0;
    private List<SubactionCreationInfo> _abilitySubactions = new List<SubactionCreationInfo>();

    [Header("Consumable Creation")]
    private string _consumableName = "New Consumable";
    private string _consumableDescription = "";
    private Sprite _consumableIcon;
    private int _uses;
    private AnimatorOverrideController _consumableAnimator;
    private bool isEnemyAbility;
    private int price;
    
    private int _consumableSubactionCount = 0;
    private List<SubactionCreationInfo> _consumableSubactions = new List<SubactionCreationInfo>();

    [Header("Equipment Mod Creation")]
    private string _equipmentModName = "New Equipment Mod";
    private string _equipmentModDescription = "";
    private Sprite _equipmentModIcon;
    private StatSetSO _equipmentModStatSet;
    private float _equipmentModPhysicalPower;
    private float _equipmentModMagicalPower;
    private float _equipmentModPhysicalSlots;
    private float _equipmentModMagicalSlots;
    private float _equipmentModStamina;
    private float _equipmentModMana;
    private float _equipmentModMaxHealth;
    private float _equipmentModDamageRDX;
    private float _equipmentModSpeed;

    [MenuItem("Window/Combat Action Creator")]
    public static void ShowWindow()
    {
        var window = GetWindow<CombatActionCreatorWindow>("Combat Action Creator");
        window.InitializeReflectionCaches();
        window.Show();
    }

    private void OnFocus()
    {
        InitializeReflectionCaches();
    }


    private void InitializeReflectionCaches()
    {
        if (_allSubactionTypes == null || _allSubactionTypes.Length == 0)
        {
            _allSubactionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CombatSubactionSO)))
                .ToArray();

            _allSubactionTypeNames = _allSubactionTypes.Select(t => t.Name).ToArray();
        }

        if (_allPatternTypes == null || _allPatternTypes.Length == 0)
        {
            _allPatternTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TargetingPattern)))
                .ToArray();

            _allPatternTypeNames = _allPatternTypes.Select(t => t.Name).ToArray();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // ▌ OnGUI
    // ─────────────────────────────────────────────────────────────────────────────
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Global ID Database", EditorStyles.boldLabel);
        _idDatabase = (GlobalIDDatabase)EditorGUILayout.ObjectField(
            "ID Database",
            _idDatabase,
            typeof(GlobalIDDatabase),
            false
        );
        if (_idDatabase == null)
        {
            EditorGUILayout.HelpBox("Please assign a GlobalIDDatabase to persist unique IDs.", MessageType.Warning);
        }

        EditorGUILayout.Space(8);

        DrawAbilitySection();
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);
        DrawConsumableSection();
        DrawEquipmentModSection();
    }

    private void DrawAbilitySection()
    {
        EditorGUILayout.LabelField("Create a New Ability", EditorStyles.boldLabel);

        _abilityName = EditorGUILayout.TextField("Name (ItemData.AbilityName)", _abilityName);
        _abilityDescription = EditorGUILayout.TextField("Description (ItemData.Description)", _abilityDescription);
        _abilityIcon = (Sprite)EditorGUILayout.ObjectField("Icon (ItemData.Icon)", _abilityIcon, typeof(Sprite), false);

        _abilityType = (AbilityType)EditorGUILayout.EnumPopup("Ability Type", _abilityType);
        _resourceCost = EditorGUILayout.FloatField("Resource Cost", _resourceCost);
        _cooldownTurns = EditorGUILayout.IntField("Cooldown Turns", _cooldownTurns);
        isEnemyAbility = EditorGUILayout.Toggle("Is Enemy Ability", isEnemyAbility);
        _abilityAnimator = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Animator Override",
            _abilityAnimator,
            typeof(AnimatorOverrideController),
            false
        );

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Subactions for this Ability", EditorStyles.boldLabel);

        _abilitySubactionCount = EditorGUILayout.IntField("Number of Subactions", _abilitySubactionCount);
        if (_abilitySubactionCount < 0) _abilitySubactionCount = 0;


        while (_abilitySubactions.Count < _abilitySubactionCount)
            _abilitySubactions.Add(new SubactionCreationInfo());
        while (_abilitySubactions.Count > _abilitySubactionCount)
            _abilitySubactions.RemoveAt(_abilitySubactions.Count - 1);

        for (int i = 0; i < _abilitySubactions.Count; i++)
        {
            var info = _abilitySubactions[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Subaction {i + 1}", EditorStyles.miniBoldLabel);

            info.subactionAssetName = EditorGUILayout.TextField("Subaction Asset Name", info.subactionAssetName);
            info.subactionTypeIndex =
                EditorGUILayout.Popup("Subaction Type", info.subactionTypeIndex, _allSubactionTypeNames);

            EditorGUILayout.Space(5);

            info.patternAssetName = EditorGUILayout.TextField("Pattern Asset Name", info.patternAssetName);
            info.patternTypeIndex = EditorGUILayout.Popup("Pattern Type", info.patternTypeIndex, _allPatternTypeNames);
            info.patternOrigin = (PatternOriginType)EditorGUILayout.EnumPopup("Pattern Origin", info.patternOrigin);

            info.targetedTileColor = EditorGUILayout.ColorField("Tile Color", info.targetedTileColor);
            info.targetedCombatantColor = EditorGUILayout.ColorField("Combatant Color", info.targetedCombatantColor);

            EditorGUILayout.Space(5);


            DrawSubactionFields(info);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        if (GUILayout.Button("Create Ability"))
        {
            CreateNewAbility();
        }
    }

    private void DrawConsumableSection()
    {
        EditorGUILayout.LabelField("Create a New Consumable", EditorStyles.boldLabel);

        _consumableName = EditorGUILayout.TextField("Name (ItemData.AbilityName)", _consumableName);
        _consumableDescription = EditorGUILayout.TextField("Description (ItemData.Description)", _consumableDescription);
        _consumableIcon =
            (Sprite)EditorGUILayout.ObjectField("Icon (ItemData.Icon)", _consumableIcon, typeof(Sprite), false);

        _uses = EditorGUILayout.IntField("Uses", _uses);
        price = EditorGUILayout.IntField("Price", price);
        _consumableAnimator = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Animator Override",
            _consumableAnimator,
            typeof(AnimatorOverrideController),
            false
        );

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Subactions for this Consumable", EditorStyles.boldLabel);

        _consumableSubactionCount = EditorGUILayout.IntField("Number of Subactions", _consumableSubactionCount);
        if (_consumableSubactionCount < 0) _consumableSubactionCount = 0;

        while (_consumableSubactions.Count < _consumableSubactionCount)
            _consumableSubactions.Add(new SubactionCreationInfo());
        while (_consumableSubactions.Count > _consumableSubactionCount)
            _consumableSubactions.RemoveAt(_consumableSubactions.Count - 1);

        for (int i = 0; i < _consumableSubactions.Count; i++)
        {
            var info = _consumableSubactions[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Subaction {i + 1}", EditorStyles.miniBoldLabel);

            info.subactionAssetName = EditorGUILayout.TextField("Subaction Asset Name", info.subactionAssetName);
            info.subactionTypeIndex =
                EditorGUILayout.Popup("Subaction Type", info.subactionTypeIndex, _allSubactionTypeNames);

            EditorGUILayout.Space(5);

            info.patternAssetName = EditorGUILayout.TextField("Pattern Asset Name", info.patternAssetName);
            info.patternTypeIndex = EditorGUILayout.Popup("Pattern Type", info.patternTypeIndex, _allPatternTypeNames);
            info.patternOrigin = (PatternOriginType)EditorGUILayout.EnumPopup("Pattern Origin", info.patternOrigin);

            info.targetedTileColor = EditorGUILayout.ColorField("Tile Color", info.targetedTileColor);
            info.targetedCombatantColor = EditorGUILayout.ColorField("Combatant Color", info.targetedCombatantColor);

            EditorGUILayout.Space(5);

            // Draw reflection-based fields for this subaction
            DrawSubactionFields(info);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        if (GUILayout.Button("Create Consumable"))
        {
            CreateNewConsumable();
        }
    }

    private void DrawEquipmentModSection()
    {
        EditorGUILayout.LabelField("Create a New Equipment Mod", EditorStyles.boldLabel);

        _equipmentModName = EditorGUILayout.TextField("Name (ItemData.Name)", _equipmentModName);
        _equipmentModDescription = EditorGUILayout.TextField("Description (ItemData.Description)", _equipmentModDescription);
        _equipmentModIcon = (Sprite)EditorGUILayout.ObjectField("Icon (ItemData.Icon)", _equipmentModIcon, typeof(Sprite), false);
        price = EditorGUILayout.IntField("Price", price);
       

       
        _equipmentModPhysicalPower = EditorGUILayout.FloatField("Physical Power", _equipmentModPhysicalPower);
        _equipmentModMagicalPower = EditorGUILayout.FloatField("Magical Power", _equipmentModMagicalPower);
        _equipmentModPhysicalSlots = EditorGUILayout.FloatField("Physical Slots", _equipmentModPhysicalSlots);
        _equipmentModMagicalSlots = EditorGUILayout.FloatField("Magical Slots", _equipmentModMagicalSlots);
        _equipmentModStamina      = EditorGUILayout.FloatField("Stamina", _equipmentModStamina);
        _equipmentModMana         = EditorGUILayout.FloatField("Mana", _equipmentModMana);
        _equipmentModMaxHealth    = EditorGUILayout.FloatField("Max Health", _equipmentModMaxHealth);
        _equipmentModDamageRDX    = EditorGUILayout.FloatField("Damage RDX", _equipmentModDamageRDX);
        _equipmentModSpeed        = EditorGUILayout.FloatField("Speed", _equipmentModSpeed);
        // ───────────────────────────

        if (GUILayout.Button("Create Equipment Mod"))
        {
            CreateEquipmentMod();
        }
    }
    /// <summary>
    /// Draws public fields  for the selected subaction type
    /// and stores/retrieves them in/from subactionFieldValues in SubactionCreationInfo.
    /// </summary>
    private void DrawSubactionFields(SubactionCreationInfo info)
    {
        // Make sure we have valid arrays
        if (_allSubactionTypes == null || info.subactionTypeIndex < 0 ||
            info.subactionTypeIndex >= _allSubactionTypes.Length)
        {
            return;
        }

        Type subactionType = _allSubactionTypes[info.subactionTypeIndex];
        // Get all public instance fields from this subaction type
        FieldInfo[] fields = subactionType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        if (fields.Length > 0)
        {
            EditorGUILayout.LabelField("Subaction Fields", EditorStyles.boldLabel);
        }

        foreach (FieldInfo field in fields)
        {
            // Skip ScriptableObject defaults like 'name', 'hideFlags', etc.
            if (field.DeclaringType == typeof(ScriptableObject) ||
                field.DeclaringType == typeof(UnityEngine.Object))
            {
                continue;
            }

            // Skip if it's obviously the pattern field (we handle that separately)
            if (field.FieldType == typeof(TargetingPattern))
            {
                continue;
            }

            // Get or initialize the stored string value
            if (!info.subactionFieldValues.ContainsKey(field.Name))
            {
                info.subactionFieldValues[field.Name] = "";
            }

            string currentVal = info.subactionFieldValues[field.Name];
            object parsedValue = null;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(field.Name, GUILayout.Width(150));

            // Draw an appropriate GUI control based on field type
            if (field.FieldType == typeof(int))
            {
                int intVal;
                int.TryParse(currentVal, out intVal);
                intVal = EditorGUILayout.IntField(intVal);
                info.subactionFieldValues[field.Name] = intVal.ToString();
            }
            else if (field.FieldType == typeof(float))
            {
                float floatVal;
                float.TryParse(currentVal, out floatVal);
                floatVal = EditorGUILayout.FloatField(floatVal);
                info.subactionFieldValues[field.Name] = floatVal.ToString();
            }
            else if (field.FieldType == typeof(bool))
            {
                bool boolVal;
                bool.TryParse(currentVal, out boolVal);
                boolVal = EditorGUILayout.Toggle(boolVal);
                info.subactionFieldValues[field.Name] = boolVal.ToString();
            }
            else if (field.FieldType == typeof(string))
            {
                string stringVal = EditorGUILayout.TextField(currentVal);
                info.subactionFieldValues[field.Name] = stringVal;
            }
            else if (field.FieldType.IsEnum)
            {
                // Attempt to parse the existing value as this enum
                Enum enumVal = null;
                try
                {
                    enumVal = (Enum)Enum.Parse(field.FieldType, currentVal);
                }
                catch
                {
                    // If parse fails, just pick the first value from the enum
                    enumVal = (Enum)Enum.GetValues(field.FieldType).GetValue(0);
                }

                Enum newEnumVal = EditorGUILayout.EnumPopup(enumVal);
                info.subactionFieldValues[field.Name] = newEnumVal.ToString();
            }
            else
            {
                // For other itemData types, you could extend logic or skip
                EditorGUILayout.LabelField($"(Type {field.FieldType.Name} not supported)");
            }

            EditorGUILayout.EndHorizontal();
        }
    }


    private void CreateNewAbility()
    {
        if (_idDatabase == null)
        {
            Debug.LogError("GlobalIDDatabase is not assigned. Please assign one in the window.");
            return;
        }

        string abilityPath = EditorUtility.SaveFilePanelInProject(
            "Save New Ability",
            _abilityName + ".asset",
            "asset",
            "Choose a location for your new Ability"
        );
        if (string.IsNullOrEmpty(abilityPath)) return;

        NewAbilitySO newAbility = ScriptableObject.CreateInstance<NewAbilitySO>();
        newAbility.name = _abilityName;


        newAbility.itemData.Name = _abilityName;
        newAbility.itemData.Description = _abilityDescription;
        newAbility.itemData.Icon = _abilityIcon;
        newAbility.itemData.itemType = ItemType.PhysicalAbility;
        newAbility.isEnemyAbility = isEnemyAbility;

        newAbility.Icon = _abilityIcon;
        newAbility.AbilityType = _abilityType;
        newAbility.ResourceCost = _resourceCost;
        newAbility.CooldownTurns = _cooldownTurns;
        newAbility.OverrideController = _abilityAnimator;


        if (newAbility.itemData.ID == 0 && newAbility.AbilityType == AbilityType.PHYSICAL && !newAbility.isEnemyAbility)
        {
            newAbility.itemData.ID = _idDatabase.nextPhysicalAbilityID;
            _idDatabase.nextPhysicalAbilityID++;
        }
        else if (newAbility.itemData.ID == 0 && newAbility.AbilityType == AbilityType.MAGICAL  && !newAbility.isEnemyAbility)
        {
            newAbility.itemData.ID = _idDatabase.nextMagicalAbilityID;
            _idDatabase.nextMagicalAbilityID++;
        }
        else if (newAbility.itemData.ID == 0 && newAbility.isEnemyAbility)
        {
            newAbility.itemData.ID = _idDatabase.nextEnemyAbilityID;
            _idDatabase.nextEnemyAbilityID++;
        }


        string folderPath = Path.GetDirectoryName(abilityPath);
        List<CombatSubactionSO> subactions = new List<CombatSubactionSO>();

        foreach (var info in _abilitySubactions)
        {
            var subaction = CreateSubactionWithPattern(info, folderPath);
            if (subaction != null)
            {
                subactions.Add(subaction);
            }
        }

        newAbility.Actions = subactions.ToArray();

        AssetDatabase.CreateAsset(newAbility, abilityPath);
        EditorUtility.SetDirty(_idDatabase);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newAbility;

        Debug.Log($"Created new Ability '{_abilityName}' with ID: {newAbility.itemData.ID}");
    }


    private void CreateNewConsumable()
    {
        if (_idDatabase == null)
        {
            Debug.LogError("GlobalIDDatabase is not assigned. Please assign one in the window.");
            return;
        }

        string consumablePath = EditorUtility.SaveFilePanelInProject(
            "Save New Consumable",
            _consumableName + ".asset",
            "asset",
            "Choose a location for your new Consumable"
        );
        if (string.IsNullOrEmpty(consumablePath)) return;

        ConsumableSO newConsumable = ScriptableObject.CreateInstance<ConsumableSO>();
        newConsumable.name = _consumableName;

        newConsumable.itemData.Name = _consumableName;
        newConsumable.itemData.Description = _consumableDescription;
        newConsumable.itemData.Icon = _consumableIcon;
        newConsumable.itemData.itemType = ItemType.Consumable;
        newConsumable.itemData.Price = price;

        newConsumable.Icon = _consumableIcon;
        newConsumable.Uses = _uses;
        
        newConsumable.OverrideController = _consumableAnimator;

        if (newConsumable.itemData.ID == 0)
        {
            newConsumable.itemData.ID = _idDatabase.nextConsumableID;
            _idDatabase.nextConsumableID++;
        }

        string folderPath = Path.GetDirectoryName(consumablePath);
        List<CombatSubactionSO> subactions = new List<CombatSubactionSO>();

        foreach (var info in _consumableSubactions)
        {
            var subaction = CreateSubactionWithPattern(info, folderPath);
            if (subaction != null)
            {
                subactions.Add(subaction);
            }
        }

        newConsumable.Actions = subactions.ToArray();

        AssetDatabase.CreateAsset(newConsumable, consumablePath);
        EditorUtility.SetDirty(_idDatabase);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newConsumable;

        Debug.Log($"Created new Consumable '{_consumableName}' with ID: {newConsumable.itemData.ID}");
    }

    private void CreateEquipmentMod()
{
    if (_idDatabase == null)
    {
        Debug.LogError("GlobalIDDatabase is not assigned. Please assign one in the window.");
        return;
    }

    string modPath = EditorUtility.SaveFilePanelInProject(
        "Save New Equipment Mod",
        _equipmentModName + ".asset",
        "asset",
        "Choose a location for your new Equipment Mod"
    );
    if (string.IsNullOrEmpty(modPath)) return;

    
    EquipmentModSO newMod = ScriptableObject.CreateInstance<EquipmentModSO>();
    newMod.name = _equipmentModName;

    // Fill out the ItemData
    newMod.itemData.Name = _equipmentModName;
    newMod.itemData.Description = _equipmentModDescription;
    newMod.itemData.Icon = _equipmentModIcon;
    newMod.itemData.itemType = ItemType.EquipmentMod;
    newMod.itemData.Price = price;

    // Assign a unique ID if it's still 0
    if (newMod.itemData.ID == 0)
    {
        newMod.itemData.ID = _idDatabase.nextEquipmentModID;
        _idDatabase.nextEquipmentModID++;
    }

    // 2) Create a new StatSetSO asset to hold the user-input stats
    StatSetSO statSetAsset = ScriptableObject.CreateInstance<StatSetSO>();
    statSetAsset.name = _equipmentModName + "_Stats";

    // Fill the StatSetSO from the UI input
    statSetAsset.PhysicalPower   = _equipmentModPhysicalPower;
    statSetAsset.MagicalPower    = _equipmentModMagicalPower;
    statSetAsset.PhysicalSlots   = _equipmentModPhysicalSlots;
    statSetAsset.MagicalSlots    = _equipmentModMagicalSlots;
    statSetAsset.Stamina         = _equipmentModStamina;
    statSetAsset.Mana            = _equipmentModMana;
    statSetAsset.MaxHealth       = _equipmentModMaxHealth;
    statSetAsset.DamageRDX       = _equipmentModDamageRDX;
    statSetAsset.Speed           = _equipmentModSpeed;

    // Save the new StatSetSO in the same folder as the EquipmentMod
    string folderPath = Path.GetDirectoryName(modPath);
    string statAssetPath = AssetDatabase.GenerateUniqueAssetPath(
        Path.Combine(folderPath, statSetAsset.name + ".asset")
    );
    AssetDatabase.CreateAsset(statSetAsset, statAssetPath);

    
    newMod.StatBonus = statSetAsset;

    
    AssetDatabase.CreateAsset(newMod, modPath);

    // Mark the ID database as dirty ;)
    EditorUtility.SetDirty(_idDatabase);

    
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
    EditorUtility.FocusProjectWindow();
    Selection.activeObject = newMod;

    Debug.Log($"Created new Equipment Mod '{_equipmentModName}' with ID: {newMod.itemData.ID}");
}

    private CombatSubactionSO CreateSubactionWithPattern(SubactionCreationInfo info, string folderPath)
    {

        if (_allPatternTypes == null || info.patternTypeIndex < 0 || info.patternTypeIndex >= _allPatternTypes.Length)
        {
            Debug.LogWarning("Invalid pattern type index.");
            return null;
        }

        Type patternType = _allPatternTypes[info.patternTypeIndex];
        var newPattern = ScriptableObject.CreateInstance(patternType) as TargetingPattern;
        newPattern.name = info.patternAssetName;

        newPattern.PatternOrigin = info.patternOrigin;
        newPattern.TargetedTileColor = info.targetedTileColor;
        newPattern.TargetedCombatantColor = info.targetedCombatantColor;


        if (_allSubactionTypes == null || info.subactionTypeIndex < 0 ||
            info.subactionTypeIndex >= _allSubactionTypes.Length)
        {
            Debug.LogWarning("Invalid subaction type index.");
            return null;
        }

        Type subactionType = _allSubactionTypes[info.subactionTypeIndex];
        var newSubaction = ScriptableObject.CreateInstance(subactionType) as CombatSubactionSO;
        newSubaction.name = info.subactionAssetName;




        PropertyInfo patternProp =
            subactionType.GetProperty("TargetingPattern", BindingFlags.Public | BindingFlags.Instance);
        if (patternProp != null && patternProp.PropertyType == typeof(TargetingPattern) && patternProp.CanWrite)
        {
            patternProp.SetValue(newSubaction, newPattern, null);
        }
        else
        {
            // Alternatively, try a field with the same name
            FieldInfo patternField =
                subactionType.GetField("TargetingPattern", BindingFlags.Public | BindingFlags.Instance);
            if (patternField != null && patternField.FieldType == typeof(TargetingPattern))
            {
                patternField.SetValue(newSubaction, newPattern);
            }
        }


        ApplySubactionFieldValues(newSubaction, info.subactionFieldValues);


        string patternPath =
            AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, info.patternAssetName + ".asset"));
        AssetDatabase.CreateAsset(newPattern, patternPath);

        string subactionPath =
            AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, info.subactionAssetName + ".asset"));
        AssetDatabase.CreateAsset(newSubaction, subactionPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newSubaction;
    }

    /// <summary>
    /// Parses the strings in subactionFieldValues and assigns them to the
    /// coresponding public fields on the subaction 
    /// </summary>
    private void ApplySubactionFieldValues(CombatSubactionSO subaction, Dictionary<string, string> fieldValues)
    {
        if (subaction == null) return;

        Type subactionType = subaction.GetType();
        FieldInfo[] fields = subactionType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            // Skip if it's the pattern field
            if (field.FieldType == typeof(TargetingPattern))
                continue;

            if (!fieldValues.ContainsKey(field.Name))
                continue;

            string stringVal = fieldValues[field.Name];

            object parsedValue = null;
            if (field.FieldType == typeof(int))
            {
                int i;
                int.TryParse(stringVal, out i);
                parsedValue = i;
            }
            else if (field.FieldType == typeof(float))
            {
                float f;
                float.TryParse(stringVal, out f);
                parsedValue = f;
            }
            else if (field.FieldType == typeof(bool))
            {
                bool b;
                bool.TryParse(stringVal, out b);
                parsedValue = b;
            }
            else if (field.FieldType == typeof(string))
            {
                parsedValue = stringVal;
            }
            else if (field.FieldType.IsEnum)
            {
                try
                {
                    parsedValue = Enum.Parse(field.FieldType, stringVal);
                }
                catch
                {
                    // If parsing fails, do nothing
                }
            }

            // If we successfully parsed something, set it
            if (parsedValue != null)
            {
                field.SetValue(subaction, parsedValue);
            }
        }
    }
}