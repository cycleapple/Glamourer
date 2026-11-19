using System.Reflection;
using System.Runtime.CompilerServices;
using System.Numerics;
using Glamourer.Gui.Materials;
using Glamourer.Interop.Material;
using Glamourer.State;
using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
void Check(bool ok, string text) { if (!ok) throw new Exception(text); Console.WriteLine("PASS: " + text); }
var category = (EquipSlot)23;
Check(category.ToSlot() == EquipSlot.Body, "Ramza coat inventory slot is Body");
Check(category.ToHumanSlot() == HumanSlot.Body, "Ramza coat render slot is Body");
Check(category.IsEquipmentPiece(), "Ramza coat participates in equipment identification");
var convert = typeof(FullEquipTypeExtensions).GetMethod("ToEquipType", BindingFlags.Static | BindingFlags.Public, null, new[]{typeof(EquipSlot),typeof(WeaponCategory),typeof(bool)}, null)!;
Check((FullEquipType)convert.Invoke(null, new object[]{category, WeaponCategory.Unknown, true})! == FullEquipType.Body, "Ramza coat enters the Body search list");
var state = (ActorState)RuntimeHelpers.GetUninitializedObject(typeof(ActorState));
var materials = new MaterialValueManager<MaterialValueState>();
typeof(ActorState).GetField("Materials")!.SetValue(state, materials);
var popup = (AdvancedDyePopup)RuntimeHelpers.GetUninitializedObject(typeof(AdvancedDyePopup));
typeof(AdvancedDyePopup).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(popup, state);
var create = typeof(AdvancedDyePopup).GetMethod("CreateManualValue", BindingFlags.NonPublic | BindingFlags.Instance)!;
var blue = new ColorRow(new Vector3(0,0,1), Vector3.One, Vector3.Zero, 1,1);
var yellow = new ColorRow(new Vector3(1,1,0), Vector3.One, Vector3.Zero, 1,1);
var gold = new ColorRow(new Vector3(1,0.8f,0), Vector3.One, Vector3.Zero, 1,1);
var gear = new CharacterArmor((PrimaryId)341, (Variant)1, StainIds.None).ToWeapon(0);
foreach(var slot in new[]{EquipSlot.RFinger,EquipSlot.LFinger}) {
 var index=MaterialValueIndex.FromSlot(slot);
 materials.TryAddValue(index, new MaterialValueState(blue,yellow,gear,StateSource.Manual));
 var result=(MaterialValueState)create.Invoke(popup,new object[]{index,gold})!;
 Check(result.Game.NearEqual(blue) && result.Model.NearEqual(gold), $"{slot}: repeated paste preserves blue baseline and applies new color");
 Check(result.EqualGame(blue,gear), $"{slot}: unchanged equipment and base color survive material reload");
 Check(!result.EqualGame(yellow,gear), $"{slot}: actual base color change is still detected");
}
var pendingIndex=MaterialValueIndex.FromSlot(EquipSlot.LFinger) with {RowIndex=1};
materials.TryAddValue(pendingIndex,new MaterialValueState(blue,yellow,gear,StateSource.Pending));
var pending=(MaterialValueState)create.Invoke(popup,new object[]{pendingIndex,gold})!;
Check(pending.Source == StateSource.Pending, "pending baseline capture survives another edit");

var freshIndex=MaterialValueIndex.FromSlot(EquipSlot.LFinger) with {RowIndex=2};
var fresh=(MaterialValueState)create.Invoke(popup,new object[]{freshIndex,yellow})!;
Check(fresh.Source == StateSource.Pending && fresh.Model.NearEqual(yellow), "first paste with unavailable resource keeps the color pending baseline capture");
Check(!new MaterialValueState(yellow,yellow,gear,StateSource.Manual).EqualGame(blue,gear), "old paste baseline reproduces the reset condition");
