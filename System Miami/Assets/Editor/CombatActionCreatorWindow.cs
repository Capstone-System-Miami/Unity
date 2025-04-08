using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using SystemMiami;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using SystemMiami.CombatRefactor;


public class CombatActionCreatorWindow : EditorWindow
{
    private enum WizardStep
    {
        ChooseActionType,
        FillActionInfo,
        ManageSubactions,
        Summary
    }

    private WizardStep _currentStep = WizardStep.ChooseActionType;

    [Header("Global ID Database")]
    [SerializeField] private GlobalIDDatabase _idDatabase;

    // Which type of final asset are we making?
    private enum ActionAssetType { Ability, Consumable, EquipmentMod }
    private ActionAssetType _assetType;

    // =========== Common Fields ==============
    private string _assetName = "New Action";
    private string _assetDescription = "";
    private Sprite _assetIcon;
    private AnimatorOverrideController _animOverride;
    private AnimatorOverrideController _fighterAnimOverride;
    private AnimatorOverrideController _tankAnimOverride;
    private AnimatorOverrideController _mageAnimOverride;
    private AnimatorOverrideController _rogueAnimOverride;
    private int _price;
    private int _minLevel;
    private int _maxLevel;
    private bool _isEnemyAbility;
    

    // =========== Ability-specific fields ==============
    private AbilityType _abilityType;
    private float _resourceCost;
    private int _cooldownTurns;
    private bool _isGeneralAbility;

    // =========== Consumable-specific fields ==============
    private int _consumableUses;

    // =========== Equipment Mod fields ==============
    private float _equipmentModPhysicalPower;
    private float _equipmentModMagicalPower;
    private float _equipmentModPhysicalSlots;
    private float _equipmentModMagicalSlots;
    private float _equipmentModStamina;
    private float _equipmentModMana;
    private float _equipmentModMaxHealth;
    private float _equipmentModDamageRDX;
    private float _equipmentModSpeed;

    // =========== Subaction Info ==============
    private List<SubactionCreationData> _subactions = new List<SubactionCreationData>();
    private enum TargetingPatternType {SingleTile, AreaOfEffect}

    // For scrolling in the subaction list
    private Vector2 _scrollPos;

    #region Structs and Helpers

    [Serializable]
    private class SubactionCreationData
    {
        public SubactionSource source = SubactionSource.Preset;
        public SubactionPresetType presetType; // If using a preset
        public CombatSubactionSO cloneSource;   // If cloning an existing asset

        // Reflection-based (or direct) fields
        // NOTE: We no longer rely on subactionName to set the final .name
        public string subactionName;
        public string patternName;
        public TargetingPatternType targetingPattern;
        public PatternOriginType patternOrigin = PatternOriginType.USER;
        public Color tileColor = Color.white;
        public Color combatantColor = Color.white;

        // The subaction class type we’re eventually creating
        public Type subactionClassType;
        public Type TargetingType;
        public int subactionTypeIndex;  // for reflection-based approach

        // Reflection-based fields
        public Dictionary<string, string> fieldValues = new Dictionary<string, string>();
    }

    private enum SubactionSource
    {
        Preset,
        Clone,
        Custom
    }

    private enum SubactionPresetType
    {
        Damage,
        RestoreResource,
        InflictStatusEffect
    }

    private static Type[] _allSubactionTypes;
    private static string[] _allSubactionTypeNames;
    private static Type AOE = typeof(AreaOfEffectPattern);
    private static Type SingleTile = typeof(SingleTilePattern);
      
    private static string[] _allPatternTypeNames;

    private static void InitializeReflectionCaches()
    {
        if (_allSubactionTypes == null || _allSubactionTypes.Length == 0)
        {
            _allSubactionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CombatSubactionSO)))
                .ToArray();

            _allSubactionTypeNames = _allSubactionTypes.Select(t => t.Name).ToArray();
        }
        
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            break;
        }
        return path;
    }

    #endregion

    [MenuItem("Window/Combat Action Wizard")]
    public static void ShowWindow()
    {
        var window = GetWindow<CombatActionCreatorWindow>("Combat Action Wizard");
        InitializeReflectionCaches();
        window.Show();
    }

    private void OnEnable()
    {
        InitializeReflectionCaches();
    }

    private void OnGUI()
    {
        if (_idDatabase == null)
        {
            EditorGUILayout.HelpBox("Please assign a GlobalIDDatabase to persist unique IDs.", MessageType.Warning);
        }

        _idDatabase = (GlobalIDDatabase)EditorGUILayout.ObjectField("Global ID Database", _idDatabase, typeof(GlobalIDDatabase), false);

        EditorGUILayout.Space(10);
        switch (_currentStep)
        {
            case WizardStep.ChooseActionType:
                DrawStepChooseActionType();
                break;
            case WizardStep.FillActionInfo:
                DrawStepFillActionInfo();
                break;
            case WizardStep.ManageSubactions:
                DrawStepManageSubactions();
                break;
            case WizardStep.Summary:
                DrawStepSummary();
                break;
        }
    }

    #region Step 1: Choose Action Type

    private void DrawStepChooseActionType()
    {
        EditorGUILayout.LabelField("Step 1: Choose what kind of asset you want to create", EditorStyles.boldLabel);
        _assetType = (ActionAssetType)EditorGUILayout.EnumPopup("Asset Type", _assetType);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Next →", GUILayout.Height(30)))
        {
            _currentStep = WizardStep.FillActionInfo;
        }
    }

    #endregion

    #region Step 2: Fill Basic Action Info

    private void DrawStepFillActionInfo()
    {
        EditorGUILayout.LabelField("Step 2: Basic Info", EditorStyles.boldLabel);

        _assetName = EditorGUILayout.TextField("Name", _assetName);
        _assetDescription = EditorGUILayout.TextField("Description", _assetDescription);
        _assetIcon = (Sprite)EditorGUILayout.ObjectField("Icon", _assetIcon, typeof(Sprite), false);
        _minLevel = EditorGUILayout.IntField("Min Level", _minLevel);
        _maxLevel = EditorGUILayout.IntField("Max Level", _maxLevel);
      

        switch (_assetType)
        {
            case ActionAssetType.Ability:
                DrawAbilityFields();
                break;
            case ActionAssetType.Consumable:
                DrawConsumableFields();
                break;
            case ActionAssetType.EquipmentMod:
                DrawEquipmentModFields();
                break;
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("← Back", GUILayout.Height(30)))
        {
            _currentStep = WizardStep.ChooseActionType;
        }

        if (GUILayout.Button("Next →", GUILayout.Height(30)))
        {
            // If no subactions exist yet (and we're not doing EquipmentMod),
            // we add one subaction by default (Damage).
            if (_subactions.Count == 0 && _assetType != ActionAssetType.EquipmentMod)
            {
                AddSubactionFromPreset(SubactionPresetType.Damage);
            }
            _currentStep = WizardStep.ManageSubactions;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawAbilityFields()
    {
        EditorGUILayout.LabelField("Ability-Specific Fields", EditorStyles.boldLabel);
        _abilityType = (AbilityType)EditorGUILayout.EnumPopup("Ability Type", _abilityType);
        _resourceCost = EditorGUILayout.FloatField("Resource Cost", _resourceCost);
        _cooldownTurns = EditorGUILayout.IntField("Cooldown Turns", _cooldownTurns);
        _isEnemyAbility = EditorGUILayout.Toggle("Is Enemy Ability", _isEnemyAbility);
        _isGeneralAbility = EditorGUILayout.Toggle("Is General Ability", _isGeneralAbility);

        _animOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Animator Override",
            _animOverride,
            typeof(AnimatorOverrideController),
            false
        );
        
        _fighterAnimOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Fighter Animator Override",
            _fighterAnimOverride,
            typeof(AnimatorOverrideController),
            false
        );
        
        _mageAnimOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Mage Animator Override",
            _mageAnimOverride,
            typeof(AnimatorOverrideController),
            false
        );
        _tankAnimOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Tank Animator Override",
            _tankAnimOverride,
            typeof(AnimatorOverrideController),
            false
        );
        
        _rogueAnimOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Rogue Animator Override",
            _rogueAnimOverride,
            typeof(AnimatorOverrideController),
            false
        );
    }

    private void DrawConsumableFields()
    {
        EditorGUILayout.LabelField("Consumable-Specific Fields", EditorStyles.boldLabel);
        _consumableUses = EditorGUILayout.IntField("Uses", _consumableUses);
        _price = EditorGUILayout.IntField("Price", _price);

        _animOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Animator Override",
            _animOverride,
            typeof(AnimatorOverrideController),
            false
        );
        
        _fighterAnimOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Fighter Animator Override",
            _fighterAnimOverride,
            typeof(AnimatorOverrideController),
            false
        );
        
        _mageAnimOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Mage Animator Override",
            _mageAnimOverride,
            typeof(AnimatorOverrideController),
            false
        );
        _tankAnimOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Tank Animator Override",
            _tankAnimOverride,
            typeof(AnimatorOverrideController),
            false
        );
        
        _rogueAnimOverride = (AnimatorOverrideController)EditorGUILayout.ObjectField(
            "Rogue Animator Override",
            _rogueAnimOverride,
            typeof(AnimatorOverrideController),
            false
        );
    }

    private void DrawEquipmentModFields()
    {
        EditorGUILayout.LabelField("Equipment Mod Fields", EditorStyles.boldLabel);
        _price = EditorGUILayout.IntField("Price", _price);

        _equipmentModPhysicalPower = EditorGUILayout.FloatField("Physical Power", _equipmentModPhysicalPower);
        _equipmentModMagicalPower = EditorGUILayout.FloatField("Magical Power", _equipmentModMagicalPower);
        _equipmentModPhysicalSlots = EditorGUILayout.FloatField("Physical Slots", _equipmentModPhysicalSlots);
        _equipmentModMagicalSlots = EditorGUILayout.FloatField("Magical Slots", _equipmentModMagicalSlots);
        _equipmentModStamina = EditorGUILayout.FloatField("Stamina", _equipmentModStamina);
        _equipmentModMana = EditorGUILayout.FloatField("Mana", _equipmentModMana);
        _equipmentModMaxHealth = EditorGUILayout.FloatField("Max Health", _equipmentModMaxHealth);
        _equipmentModDamageRDX = EditorGUILayout.FloatField("Damage RDX", _equipmentModDamageRDX);
        _equipmentModSpeed = EditorGUILayout.FloatField("Speed", _equipmentModSpeed);
    }

    #endregion

    #region Step 3: Manage Subactions

    private void DrawStepManageSubactions()
    {
        EditorGUILayout.LabelField("Step 3: Subactions for This Asset", EditorStyles.boldLabel);

        if (_assetType == ActionAssetType.EquipmentMod)
        {
            EditorGUILayout.HelpBox("Equipment Mods typically do not have subactions. Click Next.", MessageType.Info);
        }
        else
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MinHeight(200));
            for (int i = 0; i < _subactions.Count; i++)
            {
                SubactionCreationData subData = _subactions[i];
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField($"Subaction {i + 1}", EditorStyles.boldLabel);

                // Source
                subData.source = (SubactionSource)EditorGUILayout.EnumPopup("Source", subData.source);

                if (subData.source == SubactionSource.Preset)
                {
                    subData.presetType = (SubactionPresetType)EditorGUILayout.EnumPopup("Preset Type", subData.presetType);
                }
                else if (subData.source == SubactionSource.Clone)
                {
                    subData.cloneSource = (CombatSubactionSO)EditorGUILayout.ObjectField(
                        "Clone From",
                        subData.cloneSource,
                        typeof(CombatSubactionSO),
                        false
                    );
                }
                else if (subData.source == SubactionSource.Custom)
                {
                    // Show a popup for subaction type
                    subData.subactionTypeIndex = EditorGUILayout.Popup("Custom Subaction Type",
                        subData.subactionTypeIndex, _allSubactionTypeNames);

                    if (subData.subactionTypeIndex >= 0 && subData.subactionTypeIndex < _allSubactionTypes.Length)
                    {
                        subData.subactionClassType = _allSubactionTypes[subData.subactionTypeIndex];
                    }
                }

                EditorGUILayout.Space(5);

                

                // Pattern info
                subData.patternName = EditorGUILayout.TextField("Pattern Name", subData.patternName);
                subData.patternOrigin = (PatternOriginType)EditorGUILayout.EnumPopup("Pattern Origin", subData.patternOrigin);
                subData.targetingPattern = (TargetingPatternType)EditorGUILayout.EnumPopup("Pattern Type", subData.targetingPattern);
                subData.tileColor = EditorGUILayout.ColorField("Tile Color", subData.tileColor);
                subData.combatantColor = EditorGUILayout.ColorField("Combatant Color", subData.combatantColor);

                // Reflection-based fields
                if (subData.subactionClassType != null)
                {
                    DrawSubactionReflectionFields(subData);
                }

                // Buttons
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply Preset/Clone Defaults"))
                {
                    ApplySourceDefaults(subData);
                }
                if (GUILayout.Button("Remove"))
                {
                    _subactions.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Subaction (Preset: Damage)"))
            {
                AddSubactionFromPreset(SubactionPresetType.Damage);
            }
            if (GUILayout.Button("Add Subaction (Preset: Heal)"))
            {
                AddSubactionFromPreset(SubactionPresetType.RestoreResource);
            }
            if (GUILayout.Button("Add Subaction (Preset: Status)"))
            {
                AddSubactionFromPreset(SubactionPresetType.InflictStatusEffect);
            }
            if (GUILayout.Button("Add Subaction (Custom)"))
            {
                AddCustomSubaction();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("← Back", GUILayout.Height(30)))
        {
            _currentStep = WizardStep.FillActionInfo;
        }

        if (GUILayout.Button("Next →", GUILayout.Height(30)))
        {
            _currentStep = WizardStep.Summary;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void AddSubactionFromPreset(SubactionPresetType presetType)
    {
        var data = new SubactionCreationData
        {
            source = SubactionSource.Preset,
            presetType = presetType
        };

        // By default, guess a subaction class type
        switch (presetType)
        {
            case SubactionPresetType.Damage:
                data.subactionClassType = typeof(Damage);
                break;
            case SubactionPresetType.RestoreResource:
                data.subactionClassType = typeof(AddResource);
                break;
            case SubactionPresetType.InflictStatusEffect:
                data.subactionClassType = typeof(InflictStatusEffect);
                break;
        }
        // Fill reflection-based fields with some defaults
        ApplySourceDefaults(data);

        _subactions.Add(data);
    }

    private void AddCustomSubaction()
    {
        var data = new SubactionCreationData
        {
            source = SubactionSource.Custom,
            subactionTypeIndex = 0
        };
        data.subactionClassType = _allSubactionTypes[0];

        _subactions.Add(data);
    }

    private void ApplySourceDefaults(SubactionCreationData data)
    {
        data.fieldValues.Clear();

        if (data.source == SubactionSource.Preset)
        {
            switch (data.presetType)
            {
                case SubactionPresetType.Damage:
                    data.subactionClassType = typeof(Damage);
                    data.fieldValues["damageToDeal"] = "10";
                    data.tileColor = Color.red;
                    data.combatantColor = Color.red;
                    break;
                case SubactionPresetType.RestoreResource:
                    data.subactionClassType = typeof(AddResource);
                    data.fieldValues["healAmount"] = "12";
                    data.tileColor = Color.green;
                    data.combatantColor = Color.green;
                    break;
                case SubactionPresetType.InflictStatusEffect:
                    data.subactionClassType = typeof(InflictStatusEffect);
                    data.fieldValues["damagePerTurn"] = "2";
                    data.fieldValues["healPerTurn"] = "0";
                    data.fieldValues["durationTurns"] = "3";
                    data.tileColor = Color.magenta;
                    data.combatantColor = Color.magenta;
                    break;
            }
        }
        else if (data.source == SubactionSource.Clone && data.cloneSource != null)
        {
            data.subactionClassType = data.cloneSource.GetType();

            var fields = data.subactionClassType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(TargetingPattern)) continue;
                object val = f.GetValue(data.cloneSource);
                if (val != null)
                {
                    data.fieldValues[f.Name] = val.ToString();
                }
            }

            var prop = data.cloneSource.GetType().GetProperty("TargetingPattern",
                BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.PropertyType == typeof(TargetingPattern))
            {
                var existingPattern = prop.GetValue(data.cloneSource) as TargetingPattern;
                if (existingPattern != null)
                {
                    data.patternOrigin = existingPattern.PatternOrigin;
                    data.tileColor = existingPattern.TargetedTileColor;
                    data.combatantColor = existingPattern.TargetedCombatantColor;
                }
            }
        }
    }

    private void DrawSubactionReflectionFields(SubactionCreationData data)
    {
        if (data.subactionClassType == null) return;

        var fields = data.subactionClassType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var f in fields)
        {
            bool isFieldSerialized = f.IsPublic ||
                                     f.GetCustomAttributes(typeof(SerializeField), true).Length > 0;
            if (!isFieldSerialized) continue;
            if (f.FieldType == typeof(TargetingPattern)) continue;

            string currentVal;
            data.fieldValues.TryGetValue(f.Name, out currentVal);
            if (currentVal == null) currentVal = "";

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(f.Name, GUILayout.Width(120));

            if (f.FieldType == typeof(int))
            {
                int i; int.TryParse(currentVal, out i);
                i = EditorGUILayout.IntField(i);
                data.fieldValues[f.Name] = i.ToString();
            }
            else if (f.FieldType == typeof(float))
            {
                float fl; float.TryParse(currentVal, out fl);
                fl = EditorGUILayout.FloatField(fl);
                data.fieldValues[f.Name] = fl.ToString();
            }
            else if (f.FieldType == typeof(bool))
            {
                bool b; bool.TryParse(currentVal, out b);
                b = EditorGUILayout.Toggle(b);
                data.fieldValues[f.Name] = b.ToString();
            }
            else if (f.FieldType.IsEnum)
            {
                Enum eVal = null;
                try { eVal = (Enum)Enum.Parse(f.FieldType, currentVal); }
                catch { eVal = (Enum)Enum.GetValues(f.FieldType).GetValue(0); }
                var newVal = EditorGUILayout.EnumPopup(eVal);
                data.fieldValues[f.Name] = newVal.ToString();
            }
            else if (f.FieldType == typeof(string))
            {
                string sVal = EditorGUILayout.TextField(currentVal);
                data.fieldValues[f.Name] = sVal;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(f.FieldType))
            {
                // For ScriptableObject fields, we can allow the user to drag-and-drop an asset:
                ScriptableObject soVal = null;
                data.fieldValues.TryGetValue(f.Name, out var soName);

                // We can’t reliably "get" the original SO from name alone. Typically you'd store a GUID/path 
                // if you wanted to restore a reference. For simplicity, we'll just display a field:
                ScriptableObject newSOVal = (ScriptableObject)EditorGUILayout.ObjectField(soVal, f.FieldType, false);

                // We just store the name. (In a production scenario, you’d want to store and re-assign the actual object.)
                data.fieldValues[f.Name] = newSOVal ? newSOVal.name : "";
            }
            else
            {
                EditorGUILayout.LabelField("(Unsupported type)");
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    #endregion

    #region Step 4: Summary + Create

    private void DrawStepSummary()
    {
        EditorGUILayout.LabelField("Step 4: Summary & Creation", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Review your settings. If everything is good, click 'Create Asset'.", MessageType.Info);

        EditorGUILayout.LabelField("Name:", _assetName);
        EditorGUILayout.LabelField("Type:", _assetType.ToString());
        EditorGUILayout.LabelField("Description:", _assetDescription);

        if (_assetType == ActionAssetType.Ability)
        {
            EditorGUILayout.LabelField("AbilityType:", _abilityType.ToString());
            EditorGUILayout.LabelField("Resource Cost:", _resourceCost.ToString());
            EditorGUILayout.LabelField("Cooldown Turns:", _cooldownTurns.ToString());
            EditorGUILayout.LabelField("Enemy Ability?", _isEnemyAbility.ToString());
        }
        else if (_assetType == ActionAssetType.Consumable)
        {
            EditorGUILayout.LabelField("Uses:", _consumableUses.ToString());
            EditorGUILayout.LabelField("Price:", _price.ToString());
        }
        else
        {
            EditorGUILayout.LabelField("Price:", _price.ToString());
        }

        if (_assetType != ActionAssetType.EquipmentMod)
        {
            EditorGUILayout.LabelField("Subactions:", _subactions.Count.ToString());
            foreach (var s in _subactions)
            {
                EditorGUILayout.LabelField($"  - (Class: {s.subactionClassType?.Name}) Pattern: {s.patternName}");
            }
        }

        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("← Back", GUILayout.Height(30)))
        {
            _currentStep = WizardStep.ManageSubactions;
        }

        if (GUILayout.Button("Create Asset", GUILayout.Height(30)))
        {
            CreateFinalAsset();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Creates the final asset (Ability, Consumable, or Equipment Mod), along with
    /// its subactions/patterns if applicable. This version also creates a subfolder
    /// named after _assetName to organize everything.
    /// </summary>
    private void CreateFinalAsset()
    {
        if (_idDatabase == null)
        {
            Debug.LogError("No GlobalIDDatabase assigned. Cannot proceed.");
            return;
        }

        // Get the currently selected folder (or "Assets" fallback).
        string baseFolderPath = GetSelectedPathOrFallback();

        // Create a new subfolder named after the asset.
        // This returns a GUID we then convert back to a path.
        string newFolderGUID = AssetDatabase.CreateFolder(baseFolderPath, _assetName);
        string newFolderPath = AssetDatabase.GUIDToAssetPath(newFolderGUID);

        if (_assetType == ActionAssetType.Ability)
        {
            CreateAbility(newFolderPath);
        }
        else if (_assetType == ActionAssetType.Consumable)
        {
            CreateConsumable(newFolderPath);
        }
        else
        {
            CreateEquipmentMod(newFolderPath);
        }

        _currentStep = WizardStep.ChooseActionType;
        _subactions.Clear();
        Debug.Log("Combat Action Created Successfully!");
    }

    private void CreateAbility(string folderPath)
    {
        NewAbilitySO ability = ScriptableObject.CreateInstance<NewAbilitySO>();
        ability.name = _assetName;
        ability.itemData.Name = _assetName;
        ability.itemData.Description = _assetDescription;
        ability.itemData.Icon = _assetIcon;
        ability.itemData.itemType = ItemType.PhysicalAbility;
        ability.itemData.MinLevel = _minLevel;
        ability.itemData.MaxLevel = _maxLevel;
        ability.isGeneralAbility = _isGeneralAbility;
        ability.isEnemyAbility = _isEnemyAbility;
        ability.Icon = _assetIcon;
        ability.AbilityType = _abilityType;
        ability.ResourceCost = _resourceCost;
        ability.CooldownTurns = _cooldownTurns;
        ability.OverrideController = _animOverride;
        ability.FighterOverrideController = _fighterAnimOverride;
        ability.MageOverrideController = _mageAnimOverride;
        ability.RogueOverrideController = _rogueAnimOverride;
        ability.TankOverrideController = _tankAnimOverride;

        // ID assignment
        if (ability.itemData.ID == 0 && _abilityType == AbilityType.PHYSICAL && !_isEnemyAbility)
        {
            ability.itemData.ID = _idDatabase.nextPhysicalAbilityID++;
        }
        else if (ability.itemData.ID == 0 && _abilityType == AbilityType.MAGICAL && !_isEnemyAbility)
        {
            ability.itemData.ID = _idDatabase.nextMagicalAbilityID++;
        }
        else if (ability.itemData.ID == 0 && _isEnemyAbility)
        {
            ability.itemData.ID = _idDatabase.nextEnemyAbilityID++;
        }

        // Create subactions
        List<CombatSubactionSO> subList = new List<CombatSubactionSO>();
        for (int i = 0; i < _subactions.Count; i++)
        {
            var subData = _subactions[i];
            var sub = CreateSubactionWithPattern(subData, folderPath, i + 1);
            if (sub != null) subList.Add(sub);
        }
        ability.Actions = subList.ToArray();

        string abilityPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, _assetName + ".asset"));
        AssetDatabase.CreateAsset(ability, abilityPath);
        EditorUtility.SetDirty(_idDatabase);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = ability;
    }

    private void CreateConsumable(string folderPath)
    {
        ConsumableSO consumable = ScriptableObject.CreateInstance<ConsumableSO>();
        consumable.name = _assetName;
        consumable.itemData.Name = _assetName;
        consumable.itemData.Description = _assetDescription;
        consumable.itemData.Icon = _assetIcon;
        consumable.itemData.itemType = ItemType.Consumable;
        consumable.itemData.MinLevel = _minLevel;
        consumable.itemData.MaxLevel = _maxLevel;
        consumable.itemData.Price = _price;

        consumable.Icon = _assetIcon;
        consumable.Uses = _consumableUses;
        consumable.OverrideController = _animOverride;
        consumable.FighterOverrideController = _fighterAnimOverride;
        consumable.MageOverrideController = _mageAnimOverride;
        consumable.RogueOverrideController = _rogueAnimOverride;
        consumable.TankOverrideController = _tankAnimOverride;

        if (consumable.itemData.ID == 0)
        {
            consumable.itemData.ID = _idDatabase.nextConsumableID++;
        }

        List<CombatSubactionSO> subList = new List<CombatSubactionSO>();
        for (int i = 0; i < _subactions.Count; i++)
        {
            var subData = _subactions[i];
            var sub = CreateSubactionWithPattern(subData, folderPath, i + 1);
            if (sub != null) subList.Add(sub);
        }
        consumable.Actions = subList.ToArray();

        string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, _assetName + ".asset"));
        AssetDatabase.CreateAsset(consumable, path);
        EditorUtility.SetDirty(_idDatabase);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = consumable;
    }

    private void CreateEquipmentMod(string folderPath)
    {
        EquipmentModSO mod = ScriptableObject.CreateInstance<EquipmentModSO>();
        mod.name = _assetName;
        mod.itemData.Name = _assetName;
        mod.itemData.Description = _assetDescription;
        mod.itemData.Icon = _assetIcon;
        mod.itemData.itemType = ItemType.EquipmentMod;
        mod.itemData.Price = _price;
        mod.itemData.MinLevel = _minLevel;
        mod.itemData.MaxLevel = _maxLevel;

        if (mod.itemData.ID == 0)
        {
            mod.itemData.ID = _idDatabase.nextEquipmentModID++;
        }

        StatSetSO statAsset = ScriptableObject.CreateInstance<StatSetSO>();
        statAsset.name = _assetName + "_Stats";
        statAsset.PhysicalPower = _equipmentModPhysicalPower;
        statAsset.MagicalPower = _equipmentModMagicalPower;
        statAsset.PhysicalSlots = _equipmentModPhysicalSlots;
        statAsset.MagicalSlots = _equipmentModMagicalSlots;
        statAsset.Stamina = _equipmentModStamina;
        statAsset.Mana = _equipmentModMana;
        statAsset.MaxHealth = _equipmentModMaxHealth;
        statAsset.DamageRDX = _equipmentModDamageRDX;
        statAsset.Speed = _equipmentModSpeed;

        string statPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, statAsset.name + ".asset"));
        AssetDatabase.CreateAsset(statAsset, statPath);
        mod.StatBonus = statAsset;

        string modPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, _assetName + ".asset"));
        AssetDatabase.CreateAsset(mod, modPath);
        EditorUtility.SetDirty(_idDatabase);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = mod;
    }

    /// <summary>
    /// Creates and saves a subaction along with its TargetingPattern in the provided folderPath.
    /// The subaction’s name follows the pattern: "<_assetName>_<SubactionClassName>_<Index>"
    /// </summary>
    private CombatSubactionSO CreateSubactionWithPattern(SubactionCreationData data, string folderPath, int subIndex)
    {
        if (data.subactionClassType == null)
        {
            Debug.LogWarning("Invalid subaction class type. Skipping creation.");
            return null;
        }

        
        Type patternType = data.targetingPattern == TargetingPatternType.SingleTile ? SingleTile : AOE;
        if (patternType == null)
        {
            Debug.LogWarning("No TargetingPattern types found. Cannot create pattern.");
            return null;
        }
        var pattern = ScriptableObject.CreateInstance(patternType) as TargetingPattern;
        pattern.name = string.IsNullOrEmpty(data.patternName) ? "Pattern_" + subIndex : data.patternName;
        pattern.PatternOrigin = data.patternOrigin;
        pattern.TargetedTileColor = data.tileColor;
        pattern.TargetedCombatantColor = data.combatantColor;

        string patternPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(folderPath, pattern.name + ".asset"));
        AssetDatabase.CreateAsset(pattern, patternPath);

        // Create the subaction
        var subaction = ScriptableObject.CreateInstance(data.subactionClassType) as CombatSubactionSO;
        subaction.name = _assetName + "_" + data.subactionClassType.Name + "_" + subIndex;

        // Assign pattern
        var prop = data.subactionClassType.GetProperty("TargetingPattern", BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.PropertyType == typeof(TargetingPattern) && prop.CanWrite)
        {
            prop.SetValue(subaction, pattern, null);
        }
        else
        {
            var field = data.subactionClassType.GetField("_targetingPattern", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(TargetingPattern))
            {
                field.SetValue(subaction, pattern);
            }
        }

        // Apply reflection-based fields
        ApplyFieldValues(subaction, data.fieldValues);

        string subactionPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(folderPath, subaction.name + ".asset"));
        AssetDatabase.CreateAsset(subaction, subactionPath);

        return subaction;
    }

    private void ApplyFieldValues(CombatSubactionSO subaction, Dictionary<string, string> values)
    {
        var fields = subaction.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var f in fields)
        {
            bool isFieldSerialized = f.IsPublic ||
                                     f.GetCustomAttributes(typeof(SerializeField), true).Length > 0;
            if (!isFieldSerialized) continue;
            if (f.FieldType == typeof(TargetingPattern)) continue;

            if (!values.ContainsKey(f.Name)) continue;
            string strVal = values[f.Name];

            object parsed = null;
            if (f.FieldType == typeof(int))
            {
                int i; int.TryParse(strVal, out i);
                parsed = i;
            }
            else if (f.FieldType == typeof(float))
            {
                float fl; float.TryParse(strVal, out fl);
                parsed = fl;
            }
            else if (f.FieldType == typeof(bool))
            {
                bool b; bool.TryParse(strVal, out b);
                parsed = b;
            }
            else if (f.FieldType.IsEnum)
            {
                try
                {
                    parsed = Enum.Parse(f.FieldType, strVal);
                }
                catch { }
            }
            else if (f.FieldType == typeof(string))
            {
                parsed = strVal;
            }

            if (parsed != null)
            {
                f.SetValue(subaction, parsed);
            }
        }
    }

    #endregion
}
