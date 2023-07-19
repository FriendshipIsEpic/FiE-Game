// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDMConstants

namespace GameDataEditor
{
	public class GDMConstants
	{
		public const string MetaDataFormat = "{0}{1}";

		public const string GdeSignature = "GDE";

		public const string TypePrefix = "_gdeType_";

		public const string IsListPrefix = "_gdeIsList_";

		public const string SchemaPrefix = "_gdeSchema_";

		public const string SchemaKey = "_gdeSchema";

		public const string MetaDataFileName = "gde_meta_data";

		public const string ErrorLoadingValue = "Could not load {0} value from item name:{1}, field name:{2}!";

		public const string ErrorSettingValue = "Could not save {0} value to item name:{1}, field name:{2}!";

		public const string ErrorSettingCustomValue = "Could not save {0} value to item name:{1}, field name:{2}, custom field name:{3}!";

		public const string ErrorResettingValue = "Could not reset item name:{0}, field name:{1}!";

		public const string ErrorResettingCustomValue = "Could not reset item name:{0}, field name:{1}, custom field name:{2}!";

		public const string ErrorTextAssetNull = "GDEInit: TextAsset is null!";

		public const string ErrorCorruptPrefFormat = "Corrupt preference file for: {0}";

		public const string ErrorNotBoolArrayFormat = "{0} is not a boolean array.";

		public const string ErrorNotArrayFormat = "{0} is not a {1} array.";

		public const string ErrorNotInitialized = "GDE Data Manager not initialized!";

		public const string ActionCategory = "Game Data Editor";

		public const string ItemNameTooltip = "Item Name";

		public const string FieldNameTooltip = "Field Name";

		public const string BoolType = "bool";

		public const string ColorType = "Color";

		public const string FloatType = "float";

		public const string IntType = "int";

		public const string StringType = "string";

		public const string Vec2Type = "Vector2";

		public const string Vec3Type = "Vector3";

		public const string CustomType = "Custom";

		public const string InitActionTooltip = "Initializes the Game Data Manager";

		public const string GDEDataFilenameTooltip = "GDE Data File Name";

		public const string EncryptedCheckboxTooltip = "Check if GDE Data has been encrypted.";

		public const string ResetActionTooltip = "Resets a Field to the original value on a GDE Item";

		public const string ResetCustomActionTooltip = "Resets a Field to the original value in a GDE Custom Item";

		public const string ResetFieldNameTooltip = "The name of the field inside the custom item.";

		public const string GetBoolActionTooltip = "Gets a bool from a GDE Item";

		public const string GetColorActionTooltip = "Gets a Color from a GDE Item";

		public const string GetFloatActionTooltip = "Gets a float from a GDE Item";

		public const string GetIntActionTooltip = "Gets an int from a GDE Item";

		public const string GetStringActionTooltip = "Gets a string from a GDE Item";

		public const string GetVec2ActionTooltip = "Gets a Vector2 from a GDE Item";

		public const string GetVec3ActionTooltip = "Gets a Vector3 from a GDE Item";

		public const string GetGOActionTooltip = "Gets a GameObject from a GDE Item";

		public const string GetTexture2DActionTooltip = "Gets a Texture2D from a GDE Item";

		public const string GetMaterialActionTooltip = "Gets a Material from a GDE Item";

		public const string GetAudioClipActionTooltip = "Gets an AudioClip from a GDE Item";

		public const string GetBoolCustomActionTooltip = "Gets a bool from a GDE Custom Item";

		public const string GetColorCustomActionTooltip = "Gets a Color from a GDE Custom Item";

		public const string GetFloatCustomActionTooltip = "Gets a Float from a GDE Custom Item";

		public const string GetIntCustomActionTooltip = "Gets a Int from a GDE Custom Item";

		public const string GetStringCustomActionTooltip = "Gets a string from a GDE Custom Item";

		public const string GetVec2CustomActionTooltip = "Gets a Vector2 from a GDE Custom Item";

		public const string GetVec3CustomActionTooltip = "Gets a Vector3 from a GDE Custom Item";

		public const string GetGOCustomActionTooltip = "Gets a GameObject from a GDE Custom Item";

		public const string GetTexture2DCustomActionTooltip = "Gets a Texture2D from a GDE Custom Item";

		public const string GetMaterialCustomActionTooltip = "Gets a Material from a GDE Custom Item";

		public const string GetAudioClipCustomActionTooltip = "Geta an AudioClip from a GDE Custom Item";

		public const string BoolCustomFieldTooltip = "The field name of the bool inside the custom item.";

		public const string ColorCustomFieldTooltip = "The field name of the Color inside the custom item.";

		public const string FloatCustomFieldTooltip = "The field name of the float inside the custom item.";

		public const string IntCustomFieldTooltip = "The field name of the int inside the custom item.";

		public const string StringCustomFieldTooltip = "The field name of the string inside the custom item.";

		public const string Vec2CustomFieldTooltip = "The field name of the Vector2 inside the custom item.";

		public const string Vec3CustomFieldTooltip = "The field name of the Vector3 inside the custom item.";

		public const string GOCustomFieldTooltip = "The field name of the GameObject inside the custom item.";

		public const string Texture2DCustomFieldTooltip = "The field name of the Texture2D inside the custom item.";

		public const string MaterialCustomFieldTooltip = "The field name of the Material inside the custom item.";

		public const string AudioClipCustomFieldTooltip = "The field name of the AudioClip inside the custom item.";

		public const string SetBoolActionTooltip = "Sets a Bool on a GDE Item";

		public const string SetColorActionTooltip = "Sets a Color on a GDE Item";

		public const string SetFloatActionTooltip = "Sets a float on a GDE Item";

		public const string SetIntActionTooltip = "Sets an int on a GDE Item";

		public const string SetStringActionTooltip = "Sets a string on a GDE Item";

		public const string SetVec2ActionTooltip = "Sets a Vector2 on a GDE Item";

		public const string SetVec3ActionTooltip = "Sets a Vector3 on a GDE Item";

		public const string SetCustomActionTooltip = "Sets a Custom on a GDE Item";

		public const string SetCustomBoolActionTooltip = "Sets a bool on a GDE Custom Item";

		public const string SetCustomColorActionTooltip = "Sets a Color on a GDE Custom Item";

		public const string SetCustomFloatActionTooltip = "Sets a float on a GDE Custom Item";

		public const string SetCustomIntActionTooltip = "Sets an int on a GDE Custom Item";

		public const string SetCustomStringActionTooltip = "Sets a string on a GDE Custom Item";

		public const string SetCustomVec2ActionTooltip = "Sets a Vector2 on a GDE Custom Item";

		public const string SetCustomVec3ActionTooltip = "Sets a Vector3 on a GDE Custom Item";
	}
}