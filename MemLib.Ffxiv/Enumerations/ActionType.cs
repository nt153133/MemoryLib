namespace MemLib.Ffxiv.Enumerations {
    public enum ActionType : byte {
        None,
        Spell,
        Item,
        KeyItem,
        Ability,
        General,
        Companion,
        CraftAction = 9,
        MainCommand,
        PetAction1,
        Mount = 13,
        ChocoboRaceAbility = 16,
        ChocoboRaceItem
    }
}