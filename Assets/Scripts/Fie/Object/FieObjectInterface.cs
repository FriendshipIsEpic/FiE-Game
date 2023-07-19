namespace Fie.Object
{
	public interface FieObjectInterface
	{
		FieObjectBaseState baseState
		{
			get;
			set;
		}

		FieObjectGroundState groundState
		{
			get;
			set;
		}

		FieObjectFlipState flipState
		{
			get;
			set;
		}
	}
}
