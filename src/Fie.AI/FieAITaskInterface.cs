using System;
using System.Collections.Generic;

namespace Fie.AI
{
	public interface FieAITaskInterface
	{
		bool TaskExec(FieAITaskController manager);

		bool Task(FieAITaskController manager);

		Dictionary<Type, int> GetWeight();
	}
}
