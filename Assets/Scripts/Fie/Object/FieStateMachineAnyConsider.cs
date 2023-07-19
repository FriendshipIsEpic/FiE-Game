using System;
using System.Collections.Generic;

namespace Fie.Object
{
	public abstract class FieStateMachineAnyConsider : FieStateMachineInterface
	{
		event StateChangeDelegate FieStateMachineInterface.stateChangeEvent
		{
			add
			{
				stateChangeEvent += value;
			}
			remove
			{
				stateChangeEvent -= value;
			}
		}

		private event StateChangeDelegate stateChangeEvent;

		public abstract void terminateAndCallStateChangeEvent(FieGameCharacter gameCharacter, Type fromState, Type toState);

		public abstract void updateState<T>(ref T gameCharacter) where T : FieGameCharacter;

		public abstract void initialize(FieGameCharacter gameCharacter);

		public abstract void terminate(FieGameCharacter gameCharacter);

		public abstract bool isEnd();

		public abstract bool isFinished();

		public abstract float getDelay();

		public abstract bool isNotNetworkSync();

		public abstract Type getNextState();

		public abstract List<Type> getIgnoreStateList();

		public abstract List<Type> getAllowedStateList();
	}
}
