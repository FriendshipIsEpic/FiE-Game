using Fie.Manager;
using GameDataEditor;

public class FieLocalizeUtility
{
	public const string META_TAG_VALUE1 = "___Value1___";

	public const string META_TAG_VALUE2 = "___Value2___";

	public const string META_TAG_VALUE3 = "___Value3___";

	public const string META_TAG_VALUE4 = "___Value5___";

	public const string META_TAG_VALUE5 = "___Value5___";

	public static string GetConstantText(string key)
	{
		GDEConstantTextListData constantTextData;
		return GetConstantText(key, out constantTextData);
	}

	public static string GetConstantText(string key, out GDEConstantTextListData constantTextData)
	{
		string result = string.Empty;
		constantTextData = FieMasterData<GDEConstantTextListData>.I.GetMasterData(key);
		if (constantTextData == null)
		{
			return string.Empty;
		}
		switch (FieManagerBehaviour<FieEnvironmentManager>.I.currentLanguage)
		{
		case FieEnvironmentManager.Language.English:
			result = constantTextData.English;
			break;
		case FieEnvironmentManager.Language.Japanese:
			result = constantTextData.Japanese;
			break;
		case FieEnvironmentManager.Language.French:
			result = constantTextData.French;
			break;
		}
		return result;
	}

	public static string GetWordScriptText(string key)
	{
		GDEWordScriptsListData constantTextData;
		return GetWordScriptText(key, out constantTextData);
	}

	public static string GetWordScriptText(string key, out GDEWordScriptsListData constantTextData)
	{
		string result = string.Empty;
		constantTextData = FieMasterData<GDEWordScriptsListData>.I.GetMasterData(key);
		if (constantTextData == null)
		{
			return string.Empty;
		}
		switch (FieManagerBehaviour<FieEnvironmentManager>.I.currentLanguage)
		{
		case FieEnvironmentManager.Language.English:
			result = constantTextData.EnglishCaption;
			break;
		case FieEnvironmentManager.Language.Japanese:
			result = constantTextData.JapaneseCaption;
			break;
		case FieEnvironmentManager.Language.French:
			result = constantTextData.FrenchCaption;
			break;
		}
		return result;
	}
}
