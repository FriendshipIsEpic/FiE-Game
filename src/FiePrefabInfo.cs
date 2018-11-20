using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class FiePrefabInfo : Attribute
{
	public readonly string path;

	public FiePrefabInfo(string path)
	{
		this.path = path;
	}
}
